using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using WerewolfCircle.Auth;
using WerewolfCircle.Data;
using WerewolfCircle.Utils;

namespace WerewolfCircle.Hubs
{
    [Authorize]
    public class GameHub : Hub<IGameHubClient>
    {
        //private static readonly ConcurrentDictionary<string, IList<string>> s_connectionIds = new();
        private readonly GameDbContext _dbContext;

        public GameHub(GameDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public override async Task OnConnectedAsync()
        {
            if (Context.User?.Identity is not ClaimsIdentity identity || !identity.IsAuthenticated)
            {
                Debug.Fail("User was not authenticated when connecting to hub.");
                return;
            }

            string roomId = identity.FindFirst(JwtConfig.RoomIdClaimType)!.Value;
            bool isAdmin = identity.FindFirst(JwtConfig.RoleClaimType)?.Value == Policies.AdminRole;
            string? playerName = identity.FindFirst(JwtRegisteredClaimNames.GivenName)?.Value;

            if (!isAdmin)
            {
                Game? game = await _dbContext.GetGameByRoomId(roomId, tracking: false);
                if (game is null)
                    throw new HubException($"Game '{roomId}' does not exist.");

                if (!game.Players.Any(p => p.Name == playerName))
                {
                    throw new HubException($"There's no player with the name '{playerName}' in game '{roomId}'.");
                }
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, roomId);

            //IList<string> connectionIds = s_connectionIds.GetOrAdd(Context.UserIdentifier!, new List<string>());
            //lock (connectionIds)
            //{
            //    connectionIds.Add(Context.ConnectionId);
            //}
        }

        //public override Task OnDisconnectedAsync(Exception? exception)
        //{
        //    _logger.LogDebug($"Client disconnected from hub: UserId: {Context.UserIdentifier} ConnId: {Context.ConnectionId}");
        //    IList<string> connectionIds = s_connectionIds[Context.UserIdentifier!];
        //    lock (connectionIds)
        //    {
        //        connectionIds.Remove(Context.ConnectionId);
        //    }

        //    if (!connectionIds.Any())
        //    {
        //        // This isn't perfectly race-condition-safe but a person would have to leave from one device
        //        // and join from another one in a very very small timeframe and when we realize joining from
        //        // multiple devices isn't even supported or implemented yet, I'd say this is perfectly acceptable :)
        //        s_connectionIds.TryRemove(Context.UserIdentifier!, out _);
        //    }

        //    return Task.CompletedTask;
        //}

        /* TODO This doesn't work (and it probably shouldn't either) because
         * methods on the hub are only designed for being called by the clients.
         * You don't have access to the hub methods with IHubContext.
         * So instead we need something like static PopConnectionIds(string playerName, string roomId)
         * which removes and returns all connection ids so you can use them to remove the user
         * from the game-group. For invoking something on all clients of a user, you don't use
         * PopConnectionIds but IHubContext.Clients.User() (something for the summary).
         */
        //internal async Task RemovePlayerFromGame(string playerName, string roomId)
        //{
        //    string userIdentifier = NameUserIdProvider.GetUserId(roomId, playerName);
        //    if (s_connectionIds.TryRemove(userIdentifier, out IList<string>? connectionIds))
        //    {
        //        foreach (string connectionId in connectionIds)
        //        {
        //            await Groups.RemoveFromGroupAsync(connectionId, roomId);
        //        }
        //    }
        //}
    }
}
