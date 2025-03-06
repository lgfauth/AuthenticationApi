using Application.Interfaces;
using Application.LogModels;
using AuthApi.Controllers;
using Domain.Models;
using Domain.Models.Envelope;
using Domain.Validation;
using MicroservicesLogger.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace AuthApiTests.Api
{
    public class AuthControllerTests
    {
        private readonly Mock<IApiLog<ApiLogModel>> _loggerMock;
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _authServiceMock = new Mock<IAuthService>();
            _loggerMock = new Mock<IApiLog<ApiLogModel>>();
            _controller = new AuthController(_authServiceMock.Object, _loggerMock.Object);

            _loggerMock.Setup(x => x.CreateBaseLogAsync()).ReturnsAsync(new ApiLogModel());
        }

        [Fact]
        public async Task Login_ReturnsOkResult_WhenServiceSucceeds()
        {
            // Arrange
            DateTime expires = DateTime.UtcNow.AddMinutes(60);

            var loginRequest = new LoginRequest
            {
                Username = "testuser",
                Password = "TestPassword1!"
            };
            var authResponse = new AuthResponse
            {
                Token = "jwtToken",
                ExpiresAt = expires
            };
            var serviceResponse = new ResponseOk<AuthResponse>(new AuthResponse { Token = "jwtToken", ExpiresAt = expires });

            _authServiceMock.Setup(s => s.LoginAsync(It.IsAny<LoginRequest>()))
                .ReturnsAsync(serviceResponse);

            // Act
            var result = await _controller.Login(loginRequest);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(authResponse.ToString(), okResult.Value!.ToString());
        }

        [Fact]
        public async Task Login_ReturnsBadRequest_WhenValidationExceptionOccurs()
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                Username = "testuser",
                Password = ""
            };

            var validation = new ResponseModel
            {
                Message = "Password field need be filed.",
                Code = "VL001"
            };

            // Act
            var result = await _controller.Login(loginRequest);
            
            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            var serialized = JsonConvert.SerializeObject(badResult.Value);

            Assert.Equal(JsonConvert.SerializeObject(validation), serialized);
        }

        [Fact]
        public async Task Login_ReturnsBadRequest_WhenGenericExceptionOccurs()
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                Username = "testuser",
                Password = "TestPassword1!"
            };
            string exMessage = "An error occurred";
            _authServiceMock.Setup(s => s.LoginAsync(It.IsAny<LoginRequest>()))
                            .ThrowsAsync(new Exception(exMessage));

            // Act
            var result = await _controller.Login(loginRequest);

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<ResponseModel>(badResult.Value);
            Assert.Equal("LA654", response.Code);
            Assert.Equal(exMessage, response.Message);
        }

        [Fact]
        public async Task ValidateToken_ReturnsBadRequest_WhenTokenIsEmpty()
        {
            // Arrange
            string token = "";

            // Act
            var result = await _controller.ValidateToken(token);

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<ResponseModel>(badResult.Value);

            Assert.Equal("LA614", response.Code);
            Assert.Equal("Token paramenter can't be null or empty", response.Message);
        }

        [Fact]
        public async Task ValidateToken_ReturnsOkWithFalse_WhenServiceResponseIsNullOrNotSuccess()
        {
            // Arrange
            string token = "someToken";
            var expect = new ResponseModel { Message = "Not valid", Code = "LA652" };

            _authServiceMock.Setup(s => s.ValidateToken(token)).ReturnsAsync(new ResponseOk<bool>(false));

            // Act
            var result = await _controller.ValidateToken(token);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result).Value;

            Assert.Equal(expect.ToString(), okResult!.ToString());

        }

        [Fact]
        public async Task ValidateToken_ReturnsOkWithTrue_WhenServiceResponseIsSuccess()
        {
            // Arrange
            string token = "someToken";
            var expect = new ResponseModel { Message = "Valid", Code = "LA652" };

            _authServiceMock.Setup(s => s.ValidateToken(token))
                            .ReturnsAsync(new ResponseOk<bool>(true));

            // Act
            var result = await _controller.ValidateToken(token);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result).Value;

            Assert.Equal(expect.ToString(), okResult!.ToString());
        }
    }
}