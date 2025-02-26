using Domain.Entities;
using Domain.Models;
using Domain.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Moq;
using Repository.Interfaces;
using ServicesApplication.Services;
using ServicesApplication.Utils;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationTests.Application
{
    public class AuthServiceTests
    {
        private readonly Mock<IAuthRepository> _authRepositoryMock;
        private readonly EnvirolmentVariables _jwtSettings;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _authRepositoryMock = new Mock<IAuthRepository>();
            _jwtSettings = new EnvirolmentVariables
            {
                SecretKey = "abcdefghijklmnopqrstuvwxyz012345",
                Issuer = "TestIssuer",
                Audience = "TestAudience",
                ExpirationMinutes = 60
            };
            var options = Options.Create(_jwtSettings);
            _authService = new AuthService(_authRepositoryMock.Object, options);
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
            var key = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey);

            tokenHandler.ValidateToken(response.Data.Token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = _jwtSettings.Audience,
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
            var exception = await Assert.ThrowsAsync<Exception>(() => _authService.LoginAsync(loginRequest));
            Assert.Equal("Invalid username or password.", exception.Message);
        }

        [Fact]
        public void ValidateToken_ReturnsTrue_WhenTokenIsValid()
        {
            // Arrange
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new System.Security.Claims.ClaimsIdentity(new[] {
                    new System.Security.Claims.Claim(JwtRegisteredClaimNames.Sub, "1"),
                    new System.Security.Claims.Claim("username", "testuser")
                }),
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            string jwtToken = tokenHandler.WriteToken(token);

            // Act
            var response = _authService.ValidateToken(jwtToken);

            // Assert
            Assert.True(response.IsSuccess);
            Assert.True(response.Data);
        }

        [Fact]
        public void ValidateToken_ReturnsFalse_WhenTokenIsInvalid()
        {
            // Arrange
            string invalidToken = "thisIsNotAValidToken";

            // Act
            var response = _authService.ValidateToken(invalidToken);

            // Assert
            Assert.True(response.IsSuccess);
            Assert.False(response.Data);
        }
    }
}
