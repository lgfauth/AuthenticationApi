using Application.Interfaces;
using AuthApi.Controllers;
using Domain.Models;
using Domain.Models.Envelope;
using Domain.Validation;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace AuthApiTests.Api
{
    public class RegisterControllerTests
    {
        private readonly Mock<IRegisterService> _registerServiceMock;
        private readonly RegisterController _controller;

        public RegisterControllerTests()
        {
            _registerServiceMock = new Mock<IRegisterService>();
            _controller = new RegisterController(_registerServiceMock.Object);
        }

        #region Tests for Register (Subscribe)

        [Fact]
        public async Task Register_Returns201_WhenServiceResponseIsSuccess()
        {
            // Arrange
            var request = new SubscriptionRequest
            {
                Username = "testuser",
                Password = "TestPassword1!",
                Email = "test@domain.com",
                Name = "Test",
                LastName = "User"
            };

            var serviceResponse = new ResponseOk<bool>(true);

            _registerServiceMock.Setup(s => s.RegisterAsync(It.IsAny<SubscriptionRequest>()))
                                .ReturnsAsync(serviceResponse);

            // Act
            var result = await _controller.Register(request);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(201, statusResult.StatusCode);

            var response = Assert.IsType<ResponseModel>(statusResult.Value);
            Assert.Equal("SC201", response.Code);
            Assert.Contains("confirm", response.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task Register_ReturnsBadRequest_WhenValidationExceptionOccurs()
        {
            // Arrange
            var request = new SubscriptionRequest
            {
                Username = "testuser",
                Password = "weak",
                Email = "invalid-email",
                Name = "Test",
                LastName = "User"
            };

            var validationError = new ResponseModel { Code = "VL006", Message = "Email field is not in a valid format." };
            var validationEx = new ValidationException(validationError);

            _registerServiceMock.Setup(s => s.RegisterAsync(It.IsAny<SubscriptionRequest>()))
                                .ThrowsAsync(validationEx);

            // Act
            var result = await _controller.Register(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<ResponseModel>(badRequestResult.Value);

            Assert.Equal("VL006", response.Code);
            Assert.Equal("Email field is not in a valid format.", response.Message);
        }

        [Fact]
        public async Task Register_ReturnsBadRequest_WhenGenericExceptionOccurs()
        {
            // Arrange
            var request = new SubscriptionRequest
            {
                Username = "testuser",
                Password = "TestPassword1!",
                Email = "test@domain.com",
                Name = "Test",
                LastName = "User"
            };

            string exMessage = "Generic error";
            _registerServiceMock.Setup(s => s.RegisterAsync(It.IsAny<SubscriptionRequest>()))
                                .ThrowsAsync(new Exception(exMessage));

            // Act
            var result = await _controller.Register(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<ResponseModel>(badRequestResult.Value);
            Assert.Equal("EX058", response.Code);
            Assert.Equal(exMessage, response.Message);
        }

        #endregion

        #region Tests for Unregister (Unsubscribe)

        [Fact]
        public async Task Unregister_ReturnsOk_WhenServiceResponseIsSuccess()
        {
            // Arrange
            var request = new UnsubscribeRequest
            {
                Username = "testuser",
                Password = "TestPassword1!",
                Email = "test@domain.com"
            };

            var expect = new ResponseModel
            {
                Message = "All done here, see your email to confirm your unsubscription.",
                Code = "UR695"
            };

            var serviceResponse = new ResponseOk<bool>(true);

            _registerServiceMock.Setup(s => s.UnregisterAsync(It.IsAny<UnsubscribeRequest>()))
                                .ReturnsAsync(serviceResponse);

            // Act
            var result = await _controller.Unregister(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result).Value;
            Assert.Equal(expect.ToString(), okResult!.ToString());
        }

        [Fact]
        public async Task Unregister_ReturnsBadRequest_WhenValidationExceptionOccurs_OnUnregister()
        {
            // Arrange
            var request = new UnsubscribeRequest
            {
                Username = "testuser",
                Password = "weak",
                Email = "invalid@domain"
            };

            var validationError = new ResponseModel
            {
                Message = "Email field is not in a valid format.",
                Code = "VL006"
            };

            var validationEx = new ValidationException(validationError);

            _registerServiceMock.Setup(s => s.UnregisterAsync(It.IsAny<UnsubscribeRequest>()))
                                .ThrowsAsync(validationEx);

            // Act
            var result = await _controller.Unregister(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<ResponseModel>(badRequestResult.Value);
            Assert.Equal("VL006", response.Code);
            Assert.Equal("Email field is not in a valid format.", response.Message);
        }

        [Fact]
        public async Task Unregister_ReturnsBadRequest_WhenGenericExceptionOccurs_OnUnregister()
        {
            // Arrange
            var request = new UnsubscribeRequest
            {
                Username = "testuser",
                Password = "TestPassword1!",
                Email = "test@domain.com"
            };

            string exMessage = "Generic error on unsubscribe";
            _registerServiceMock.Setup(s => s.UnregisterAsync(It.IsAny<UnsubscribeRequest>()))
                                .ThrowsAsync(new Exception(exMessage));

            // Act
            var result = await _controller.Unregister(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<ResponseModel>(badRequestResult.Value);

            Assert.Equal("EX059", response.Code);
            Assert.Equal(exMessage, response.Message);
        }

        #endregion
    }
}
