using Domain.Entities;
using Domain.Models;
using Domain.Settings;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ServicesApplication.Utils
{
    public class Encryptor
    {
        public static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));

            return Convert.ToBase64String(hashedBytes);
        }

        public static AuthResponse GenerateToken(User user, EnvirolmentVariables envorolmentVariables)
        {
            var key = Encoding.UTF8.GetBytes(envorolmentVariables.JWTSETTINGS_SECRETKEY);
            var tokenHandler = new JwtSecurityTokenHandler();

            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim("username", user.Username),
            new Claim("email", user.Email),
        };

            var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(envorolmentVariables.JWTSETTINGS_EXPIRATIONMINUTES),
                Issuer = envorolmentVariables.JWTSETTINGS_ISSUER,
                Audience = envorolmentVariables.JWTSETTINGS_AUDIENCE,
                SigningCredentials = credentials
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return new AuthResponse
            {
                Token = tokenHandler.WriteToken(token),
                ExpiresAt = tokenDescriptor.Expires ?? DateTime.UtcNow
            };
        }
    }
}
