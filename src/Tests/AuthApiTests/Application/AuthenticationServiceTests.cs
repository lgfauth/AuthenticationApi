using Application.LogModels;
using Application.Services;
using Application.Utils;
using Domain.Entities;
using Domain.Models;
using Domain.Settings;
using MicroservicesLogger.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Moq;
using Repository.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Xunit;

namespace AuthApiTests.Application
{
    public class AuthServiceTests
    {
        private readonly Mock<IAuthRepository> _authRepositoryMock;
        private readonly Mock<IApiLog<ApiLogModel>> _loggerMock;
        private readonly EnvirolmentVariables _variables;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _loggerMock = new Mock<IApiLog<ApiLogModel>>();
            _authRepositoryMock = new Mock<IAuthRepository>();

            _variables = new EnvirolmentVariables
            {
                JWTSETTINGS_SECRETKEY = "abcdefghijklmnopqrstuvwxyz012345",
                JWTSETTINGS_ISSUER = "TestIssuer",
                JWTSETTINGS_AUDIENCE = "TestAudience",
                JWTSETTINGS_EXPIRATIONMINUTES = 60
            };
            
            var options = Options.Create(_variables);
            _authService = new AuthService(_authRepositoryMock.Object, options.Value, _loggerMock.Object);

            _loggerMock.Setup(x => x.CreateBaseLogAsync()).ReturnsAsync(new ApiLogModel());
            _loggerMock.Setup(x => x.GetBaseLogAsync()).ReturnsAsync(new ApiLogModel());
        }

        [Fact]
        public async Task LoginAsync_ReturnsAuthResponse_WhenUserIsValid()
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                Username = "testuser",
                Password = "TestPassword1!"
            };
            // Simula o hash da senha (suponha que Encryptor.HashPassword seja determinístico)
            string hashedPassword = Encryptor.HashPassword(loginRequest.Password);

            var user = new User
            {
                Id = "1",
                Username = loginRequest.Username,
                PasswordHash = hashedPassword,
                Email = "test@example.com",
                Name = "Test",
                LastName = "User"
            };

            _authRepositoryMock
                .Setup(repo => repo.GetUserByUsernameAndPasswordAsync(loginRequest.Username, hashedPassword))
                .ReturnsAsync(user);

            // Act
            var response = await _authService.LoginAsync(loginRequest);

            // Assert
            Assert.True(response.IsSuccess);
            Assert.NotNull(response.Data);
            Assert.False(string.IsNullOrEmpty(response.Data.Token));

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_variables.JWTSETTINGS_SECRETKEY);

            tokenHandler.ValidateToken(response.Data.Token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _variables.JWTSETTINGS_ISSUER,
                ValidateAudience = true,
                ValidAudience = _variables.JWTSETTINGS_AUDIENCE,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);
        }

        [Fact]
        public async Task LoginAsync_ThrowsException_WhenUserIsInvalid()
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                Username = "invaliduser",
                Password = "WrongPassword!"
            };
            string hashedPassword = Encryptor.HashPassword(loginRequest.Password);
            _authRepositoryMock
                .Setup(repo => repo.GetUserByUsernameAndPasswordAsync(loginRequest.Username, hashedPassword))
                .ReturnsAsync((User)null!);

            // Act & Assert
            var response = await _authService.LoginAsync(loginRequest);
            Assert.Equal("Invalid username or password.", response.Error.Message);
        }

        [Fact]
        public async Task ValidateToken_ReturnsTrue_WhenTokenIsValid()
        {
            // Arrange
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_variables.JWTSETTINGS_SECRETKEY);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new System.Security.Claims.ClaimsIdentity(new[] {
                    new System.Security.Claims.Claim(JwtRegisteredClaimNames.Sub, "1"),
                    new System.Security.Claims.Claim("username", "testuser")
                }),
                Expires = DateTime.UtcNow.AddMinutes(_variables.JWTSETTINGS_EXPIRATIONMINUTES),
                Issuer = _variables.JWTSETTINGS_ISSUER,
                Audience = _variables.JWTSETTINGS_AUDIENCE,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            string jwtToken = tokenHandler.WriteToken(token);

            // Act
            var response = await _authService.ValidateToken(jwtToken);

            // Assert
            Assert.True(response.IsSuccess);
            Assert.True(response.Data);
        }

        [Fact]
        public async Task ValidateToken_ReturnsFalse_WhenTokenIsInvalid()
        {
            // Arrange
            string invalidToken = "thisIsNotAValidToken";

            // Act
            var response = await _authService.ValidateToken(invalidToken);

            // Assert
            Assert.True(response.IsSuccess);
            Assert.False(response.Data);
        }
    }
}
