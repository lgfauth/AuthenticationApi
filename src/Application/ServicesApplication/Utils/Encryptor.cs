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

        public static AuthResponse GenerateToken(User user, JwtSettings jwtSettings)
        {
            var key = Encoding.UTF8.GetBytes(jwtSettings.SecretKey);
            var tokenHandler = new JwtSecurityTokenHandler();

            // Claims que podem identificar o usuário e suas permissões
            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim("username", user.Username),
            new Claim("email", user.Email),
            // Você pode adicionar roles ou outras claims conforme a necessidade
        };

            var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(jwtSettings.ExpirationMinutes),
                Issuer = jwtSettings.Issuer,
                Audience = jwtSettings.Audience,
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
