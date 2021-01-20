using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using WerewolfCircle.Data;
using WerewolfCircle.Utils;

namespace WerewolfCircle.Hubs
{
    public class GameHub : Hub<IGameHubClient>
    {
        private const int RoomIdLength = 6;
        private readonly GameDbContext _dbContext;

        public GameHub(GameDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<string> CreateGame()
        {
            await EnsurePlayerNotInGame();

            Game game = new Game
            {
                RoomId = KeyGenerator.GetUniqueKey(RoomIdLength)
            };

            await _dbContext.Games.AddAsync(game);
            await _dbContext.SaveChangesAsync();

            // await Groups.AddToGroupAsync(Context.ConnectionId, game.RoomId); This is going to be required for admin stuff

            Console.WriteLine(game.RoomId);

            return game.RoomId;

            // If user is not in game create a new game and return the game-id
        }

        public async Task<IEnumerable<string>> JoinGame(string roomId, string playerName)
        {
            await EnsurePlayerNotInGame();

            Game game = await _dbContext.Games
                                        .Include(g => g.Players)
                                        .FirstOrDefaultAsync(g => g.RoomId == roomId);

            if (game is null)
                throw new HubException($"Game '{roomId}' does not exist.");

            if (!PlayerNameValid(playerName))
                throw new HubException("The supplied player name is invalid.");

            if (game.Players.Any(p => p.Name.Equals(playerName, StringComparison.OrdinalIgnoreCase))) // DOES THIS WORK?
                throw new HubException("A player with that name is already in this game.");

            Player player = new Player
            {
                ConnectionId = Context.ConnectionId,
                Name = playerName
            };

            game.Players.Add(player);
            await _dbContext.SaveChangesAsync();

            await Clients.Group(game.RoomId).PlayerJoined(playerName);

            await Groups.AddToGroupAsync(Context.ConnectionId, game.RoomId);

            return game.Players.Select(p => p.Name);

            // Add user to group for that game if it exists
            // Add player to that game
            // Duplicate player names are not allowed

            // TODO Maybe not required (yet) but we could return a secret id here which they can
            // use later on to rejoin the same game if they closed the tab/browser without explicitly
            // leaving the game.
        }

        public async Task LeaveGame()
        {
            Player player = await _dbContext.Players
                                            .Include(p => p.Game)
                                            .FirstOrDefaultAsync(p => p.ConnectionId == Context.ConnectionId);

            if (player is null)
                throw new HubException("Player is not in a game.");

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, player.Game.RoomId);

            await Clients.Group(player.Game.RoomId).PlayerLeft(player.Name);

            _dbContext.Remove(player);
            await _dbContext.SaveChangesAsync();

            // If player is in a game, leave that game and its group
            // Calls IGameHubClient.PlayerLeft for everyone else
        }

        // If the player is in the DB, they have to be in a game in which case they can't create or join a new one.
        // This doesn't prevent spamming and is probably abusable otherwise but we can worry about that later.
        private async Task EnsurePlayerNotInGame()
        {
            Player player = await _dbContext.Players.FirstOrDefaultAsync(p => p.ConnectionId == Context.ConnectionId);
            
            if (player is not null)
                throw new HubException("Player is in a game.");
        }

        private static bool PlayerNameValid(string playerName) => !string.IsNullOrWhiteSpace(playerName) &&
                                                                  Regex.IsMatch(playerName, @"^[\w\-]{2,25}$");
    }
}
