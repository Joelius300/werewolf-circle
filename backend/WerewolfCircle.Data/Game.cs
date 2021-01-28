using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace WerewolfCircle.Data
{
    [Index(nameof(RoomId), IsUnique = true)]
    public class Game
    {
        public int Id { get; set; }

        [Required]
        public string RoomId { get; set; } = string.Empty;

        [Required]
        public string AdminConnectionId { get; set; } = string.Empty;

        public ICollection<Player> Players { get; set; } = null!;
    }
}
