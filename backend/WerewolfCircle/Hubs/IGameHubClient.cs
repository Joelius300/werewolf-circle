using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WerewolfCircle.Hubs
{
    public interface IGameHubClient
    {
        /// <summary>
        /// Add the given player to the circle.
        /// </summary>
        /// <param name="playerName"></param>
        Task PlayerJoined(string playerName);

        /// <summary>
        /// Remove the given player from the circle permanently.
        /// </summary>
        /// <param name="playerName"></param>
        Task PlayerLeft(string playerName);

        /// <summary>
        /// Reset the UI and remove any associations with the current game.
        /// </summary>
        Task GameDestroyed();

        /// <summary>
        /// Mark the given player as disconnected. They might rejoin
        /// (in which case <see cref="PlayerJoined(string)"/> will be called).
        /// </summary>
        /// <param name="playerName"></param>
        // Task PlayerDisconnected(string playerName);

        /// <summary>
        /// Mark the given player as dead and remove them from the circle.
        /// <para>
        /// This method will be called for everyone equally.
        /// The client has to handle their own name being <paramref name="playerName"/>.
        /// </para>
        /// </summary>
        /// <param name="playerName"></param>
        // Task PlayerDied(string playerName);

        /// <summary>
        /// Mark the given player as alive again and add them to the circle at the correct index.
        /// </summary>
        /// <param name="playerName"></param>
        // Task PlayerRevived(string playerName);

        /// <summary>
        /// Set the role for the clients player.
        /// </summary>
        /// <param name="role"></param>
        // Task SetRole(Role role);
    }
}
