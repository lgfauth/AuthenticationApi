using AuthenticationApi.Controllers;
using Domain.Models;
using Domain.Models.Envelope;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ServicesApplication.Interfaces;
using ServicesApplication.Validators;

namespace AuthenticationTests.Api
{
    public class AuthControllerTests
    {
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _authServiceMock = new Mock<IAuthService>();
            _controller = new AuthController(_authServiceMock.Object);
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

            var validationEx = new ValidationException(new ResponseModel
            {
                Message = "Password field need be filed.",
                Code = "VL001"
            });

            _authServiceMock.Setup(s => s.LoginAsync(It.IsAny<LoginRequest>()))
                            .ThrowsAsync(validationEx);

            // Act
            var result = await _controller.Login(loginRequest);

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(validationEx.Data, badResult.Value);
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
            // Como o controller retorna um ResponseModel em caso de exceção genérica:
            var response = Assert.IsType<ResponseModel>(badResult.Value);
            Assert.Equal("LA654", response.Code);
            Assert.Equal(exMessage, response.Message);
        }

        [Fact]
        public void ValidateToken_ReturnsBadRequest_WhenTokenIsEmpty()
        {
            // Arrange
            string token = "";

            // Act
            var result = _controller.ValidateToken(token);

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<ResponseModel>(badResult.Value);

            Assert.Equal("LA614", response.Code);
            Assert.Equal("Token paramenter can't be null or empty", response.Message);
        }

        [Fact]
        public void ValidateToken_ReturnsOkWithFalse_WhenServiceResponseIsNullOrNotSuccess()
        {
            // Arrange
            string token = "someToken";
            var serviceResponse = new ResponseOk<bool>(false);
            var expect = new ResponseModel { Message = "Not valid", Code = "LA652" };

            _authServiceMock.Setup(s => s.ValidateToken(token)).Returns(serviceResponse);

            // Act
            var result = _controller.ValidateToken(token);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result).Value;

            Assert.Equal(expect.ToString(), okResult!.ToString());

        }

        [Fact]
        public void ValidateToken_ReturnsOkWithTrue_WhenServiceResponseIsSuccess()
        {
            // Arrange
            string token = "someToken";
            var serviceResponse = new ResponseOk<bool>(true);
            var expect = new ResponseModel { Message = "Valid", Code = "LA652" };

            _authServiceMock.Setup(s => s.ValidateToken(token))
                            .Returns(serviceResponse);

            // Act
            var result = _controller.ValidateToken(token);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result).Value;

            Assert.Equal(expect.ToString(), okResult!.ToString());
        }
    }
}