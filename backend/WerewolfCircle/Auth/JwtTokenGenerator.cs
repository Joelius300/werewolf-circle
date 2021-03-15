using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace WerewolfCircle.Auth
{
    public class JwtTokenGenerator : IAuthTokenGenerator
    {
        private readonly JwtConfig _jwtConfig;

        public JwtTokenGenerator(IOptions<JwtConfig> jwtConfig)
        {
            _jwtConfig = jwtConfig.Value;
        }

        public string GenerateAdminToken(string roomId) => GenerateJwtToken(new Claim[]
        {
            new Claim(JwtConfig.RoomIdClaimType, roomId),
            new Claim(JwtConfig.RoleClaimType, Policies.AdminRole)
        });

        public string GeneratePlayerToken(string roomId, string playerName) => GenerateJwtToken(new Claim[]
        {
            new Claim(JwtRegisteredClaimNames.GivenName, playerName),
            new Claim(JwtConfig.RoomIdClaimType, roomId)
        });

        private string GenerateJwtToken(IEnumerable<Claim> claims)
        {
            SymmetricSecurityKey securityKey = _jwtConfig.BuildSecretSecurityKey();
            SigningCredentials credentials = new(securityKey, SecurityAlgorithms.HmacSha256);

            SecurityToken token = new JwtSecurityToken(issuer: _jwtConfig.Issuer,
                                                       audience: _jwtConfig.Audience,
                                                       claims: claims,
                                                       expires: DateTime.UtcNow.AddDays(7),
                                                       signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
