using System;
using System.ComponentModel.DataAnnotations;

namespace WerewolfCircle.Data
{
    public class Player
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public Game Game { get; set; } = null!;
    }
}
