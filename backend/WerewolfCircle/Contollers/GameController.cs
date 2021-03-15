using System;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using WerewolfCircle.Auth;
using WerewolfCircle.Contollers.Models;
using WerewolfCircle.Data;
using WerewolfCircle.Hubs;
using WerewolfCircle.Utils;

namespace WerewolfCircle.Contollers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class GameController : ControllerBase
    {
        private readonly GameDbContext _dbContext;
        private readonly IHubContext<GameHub, IGameHubClient> _gameHub;
        private readonly IAuthTokenGenerator _tokenGenerator;
        private readonly IRoomIdGenerator _roomIdGenerator;

        public GameController(GameDbContext dbContext, IHubContext<GameHub, IGameHubClient> gameHub, IAuthTokenGenerator tokenGenerator, IRoomIdGenerator roomIdGenerator)
        {
            _dbContext = dbContext;
            _gameHub = gameHub;
            _tokenGenerator = tokenGenerator;
            _roomIdGenerator = roomIdGenerator;
        }

        [HttpPost]
        public async Task<IActionResult> Create()
        {
            if (User.Identity!.IsAuthenticated)
                return Forbid();

            string roomId = _roomIdGenerator.GenerateRoomId();
            Game game = new() { RoomId = roomId };

            await _dbContext.Games.AddAsync(game);
            await _dbContext.SaveChangesAsync();

            return Ok(new
            {
                token = _tokenGenerator.GenerateAdminToken(roomId),
            });
        }

        [HttpPost]
        public async Task<IActionResult> Join([FromBody] JoinGameRequest joinRequest)
        {
            if (User.Identity!.IsAuthenticated)
                return Forbid();

            Game? game = await _dbContext.GetGameByRoomId(joinRequest.RoomId, tracking: true);

            if (game is null)
                return BadRequest($"Game '{joinRequest.RoomId}' does not exist.");

            if (!PlayerNameValid(joinRequest.PlayerName))
                return BadRequest("The supplied player name is invalid.");

            if (game.Players.Any(p => p.Name.Equals(joinRequest.PlayerName, StringComparison.OrdinalIgnoreCase)))
                return BadRequest("A player with that name is already in this game.");

            Player player = new()
            {
                Name = joinRequest.PlayerName
            };

            game.Players.Add(player);
            await _dbContext.SaveChangesAsync();

            await _gameHub.Clients.Group(game.RoomId).PlayerJoined(player.Name);

            // TODO just return the token (create will too)
            // and then there's a new endpoint which fetches the current state of the game which returns
            // players _and whatever else there will be_. It has to differentiate between admins and non-admins for information like player roles.
            return Ok(new
            {
                token = _tokenGenerator.GeneratePlayerToken(joinRequest.RoomId, joinRequest.PlayerName),
                players = game.Players.Select(p => p.Name)
            });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Leave()
        {
            if (User.Identity is not ClaimsIdentity identity || !identity.IsAuthenticated)
                return Forbid();

            string roomId = identity.FindFirst(JwtConfig.RoomIdClaimType)!.Value;
            bool isAdmin = identity.FindFirst(JwtConfig.RoleClaimType)?.Value == Policies.AdminRole;
            Debug.Assert(!string.IsNullOrWhiteSpace(roomId));

            Task notifyClientsTask;
            if (isAdmin)
            {
                Game? game = await _dbContext.GetGameByRoomId(roomId, tracking: true);
                if (game is null)
                    return BadRequest($"Game '{roomId}' does not exist.");

                _dbContext.Remove(game);
                notifyClientsTask = _gameHub.Clients.Group(roomId).GameDestroyed();
            }
            else
            {
                string playerName = identity.FindFirst(JwtRegisteredClaimNames.GivenName)!.Value;
                Debug.Assert(!string.IsNullOrWhiteSpace(playerName));
                Player? player;

                try
                {
                    player = await _dbContext.GetPlayer(roomId, playerName, tracking: false);
                }
                catch (ArgumentException e)
                {
                    return BadRequest(e.Message);
                }

                if (player is null)
                    return BadRequest($"Player '{playerName}' does not exist in game '{roomId}'.");

                _dbContext.Remove(player);
                notifyClientsTask = _gameHub.Clients.Group(roomId).PlayerLeft(playerName);
                // TODO remove the player from the signalr group on all clients. See RemovePlayerFromGame in GameHub.
                // We don't do this yet because it's only a "security" measure (if the player modifies the client
                // preventing it from terminating the connection they could continue to act as member of the game after being removed).
                // It's not really relevant until multiple devices per player are supported.
            }

            await Task.WhenAll(_dbContext.SaveChangesAsync(), notifyClientsTask);

            return Ok();
        }

        // Abstract to some validator
        private static bool PlayerNameValid(string playerName) => !string.IsNullOrWhiteSpace(playerName) &&
                                                                  Regex.IsMatch(playerName, @"^[\w\-]{2,25}$");
    }
}
