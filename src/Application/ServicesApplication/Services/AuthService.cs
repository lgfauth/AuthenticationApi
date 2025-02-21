using Domain.Models;
using Domain.Models.Envelope;
using Domain.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Repository.Interfaces;
using ServicesApplication.Interfaces;
using ServicesApplication.Utils;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace ServicesApplication.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly JwtSettings _jwtSettings;

        public AuthService(IAuthRepository authRepository, IOptions<JwtSettings> jwtSettings)
        {
            _authRepository = authRepository;
            _jwtSettings = jwtSettings.Value;
        }

        public async Task<IResponse<AuthResponse>> LoginAsync(LoginRequest request)
        {
            string encryptedPassword = Encryptor.HashPassword(request.Password);

            var user = await _authRepository.GetUserByUsernameAndPasswordAsync(request.Username, encryptedPassword);

            if (user is null)
                throw new Exception("Invalid username or password.");

            if (!encryptedPassword.Equals(user.PasswordHash))
                throw new Exception("Invalid username or password.");

            var response = Encryptor.GenerateToken(user, _jwtSettings);

            return new ResponseOk<AuthResponse>(response);
        }

        public IResponse<bool> ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey);

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _jwtSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = _jwtSettings.Audience,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                return new ResponseOk<bool>(true);
            }
            catch
            {
                return new ResponseOk<bool>(false);
            }
        }
    }
}