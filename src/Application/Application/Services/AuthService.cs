using Application.Interfaces;
using Application.LogModels;
using Application.Utils;
using Domain.Models;
using Domain.Models.Envelope;
using Domain.Settings;
using MicroservicesLogger.Interfaces;
using MicroservicesLogger.Models;
using Microsoft.IdentityModel.Tokens;
using Repository.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IApiLog<ApiLogModel> _logger;
        private readonly IAuthRepository _authRepository;
        private readonly EnvirolmentVariables _envorolmentVariables;

        public AuthService(IAuthRepository authRepository, EnvirolmentVariables envorolmentVariables, IApiLog<ApiLogModel> logger)
        {
            _logger = logger;
            _authRepository = authRepository;
            _envorolmentVariables = envorolmentVariables;
        }

        public async Task<IResponse<AuthResponse>> LoginAsync(LoginRequest request)
        {
            var baselog = await _logger.GetBaseLogAsync();
            var log = new SubLog();

            try
            {
                log.StartCronometer();

                string encryptedPassword = Encryptor.HashPassword(request.Password);

                var user = await _authRepository.GetUserByUsernameAndPasswordAsync(request.Username, encryptedPassword);

                if (user is null || !encryptedPassword.Equals(user.PasswordHash))
                {
                    log.StopCronometer();
                    var responseUser = new ResponseModel { Message = "Invalid username or password.", Code = "VL0975" };

                    return new ResponseError<AuthResponse>(responseUser);
                }

                var response = Encryptor.GenerateToken(user, _envorolmentVariables);

                log.StopCronometer();

                return new ResponseOk<AuthResponse>(response);
            }
            finally
            {
                await baselog.AddStepAsync("PROCESS_LOGIN_REQUEST", log);
            }
        }

        public async Task<IResponse<bool>> ValidateToken(string token)
        {
            var baselog = await _logger.GetBaseLogAsync();
            var log = new SubLog();

            try
            {
                log.StartCronometer();
                
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_envorolmentVariables.JWTSETTINGS_SECRETKEY);

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _envorolmentVariables.JWTSETTINGS_ISSUER,
                    ValidateAudience = true,
                    ValidAudience = _envorolmentVariables.JWTSETTINGS_AUDIENCE,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                log.StopCronometer();

                return new ResponseOk<bool>(true);
            }
            catch
            {
                log.StopCronometer();

                return new ResponseOk<bool>(false);
            }
            finally
            {
                await baselog.AddStepAsync("PROCESS_TOKEN_VALIDATION", log);
            }
        }
    }
}
