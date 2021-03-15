using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace WerewolfCircle.Auth
{
    public class NameUserIdProvider : IUserIdProvider
    {
        /// <summary>
        /// Creates a unique id for a user using the roomId and the playerName.
        /// If the playerName isn't set, the user is the admin of the specified game!
        /// </summary>
        /// <param name="roomId">The roomId the user is in.</param>
        /// <param name="playerName">The name of the player or null if they're the admin.</param>
        /// <returns>The unique id for the specified user.</returns>
        public static string GetUserId(string roomId, string? playerName)
        {
            // Unless the user is the admin of the game, create a unique identified with gameId and player name.
            if (!string.IsNullOrEmpty(playerName))
            {
                return @$"{roomId}\\{playerName}";
            }

            // Since there can only be one admin, just use the roomId.
            // If this collides with group names we need to use something else since we're already using the roomId as group-id.
            return roomId;
        }

        // Ps. This should be the same we'd use for the sub claim since it's unique per user.
        public string? GetUserId(HubConnectionContext connection)
        {
            if (connection.User?.Identity is ClaimsIdentity identity && identity.IsAuthenticated)
            {
                // Every authenticated user is in a game.
                string roomId = identity.FindFirst(JwtConfig.RoomIdClaimType)!.Value;

                // identity.Name will be the playerName via jwt:given_name specified at startup
                return GetUserId(roomId, identity.Name);
            }

            return null;
        }
    }
}
