using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WerewolfCircle.Contollers.Models
{
    public class JoinGameRequest
    {
        [Required]
        public string PlayerName { get; set; } = string.Empty;

        [Required]
        public string RoomId { get; set; } = string.Empty;
    }
}
