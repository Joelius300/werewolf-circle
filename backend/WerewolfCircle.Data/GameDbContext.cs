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
    }
}
