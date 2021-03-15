using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace WerewolfCircle.Data
{
    public class GameDbContext : DbContext
    {
        public DbSet<Player> Players { get; set; } = null!;
        public DbSet<Game> Games { get; set; } = null!;

        public GameDbContext(DbContextOptions options) : base(options)
        {
        }

        protected GameDbContext()
        {
        }

        /// <summary>
        /// Returns the game with the specified roomId or null if not found.
        /// </summary>
        /// <param name="roomId">The roomId of the game.</param>
        /// <param name="tracking">Specifies if the returned instance (and children) should be tracked by EF.</param>
        /// <returns>The specified game (including all players) or null.</returns>
        public Task<Game?> GetGameByRoomId(string roomId, bool tracking)
        {
            IQueryable<Game> joinedGames = Games.Include(g => g.Players);

            if (!tracking)
                joinedGames = joinedGames.AsNoTracking();

            // apparently FirstOrDefaultAsync has the wrong generic signature?
            return joinedGames.FirstOrDefaultAsync(g => g.RoomId == roomId)!;
        }

        /// <summary>
        /// Returns the player with the specified name in the specified game
        /// or null if the player doesn't exist. If the game doesn't exist, an
        /// <see cref="ArgumentException"/> is raised.
        /// </summary>
        /// <param name="roomId">The roomId of the game.</param>
        /// <param name="name">The name of the player (case-insensitive).</param>
        /// <param name="tracking">Specifies if the returned instance (and children) should be tracked by EF.</param>
        /// <returns>The specified player or null.</returns>
        public async Task<Player?> GetPlayer(string roomId, string name, bool tracking)
        {
            if (string.IsNullOrEmpty(roomId))
                throw new ArgumentNullException(nameof(roomId));

            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            Game? game = await GetGameByRoomId(roomId, tracking);

            if (game is null)
                throw new ArgumentException($"Game '{roomId}' does not exist.", nameof(roomId));

            return game.Players.FirstOrDefault(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }
    }
}
