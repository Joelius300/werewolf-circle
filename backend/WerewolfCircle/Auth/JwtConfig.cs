using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;

namespace WerewolfCircle.Auth
{
    public class JwtConfig
    {
        public const string RoleClaimType = "role";
        public const string RoomIdClaimType = "roomId";

        public string SecretKey { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;

        public SymmetricSecurityKey BuildSecretSecurityKey() => new(Encoding.UTF8.GetBytes(SecretKey));
    }
}
