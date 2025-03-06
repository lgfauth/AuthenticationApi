using Application.Interfaces;
using Application.LogModels;
using Application.Services;
using Domain.Entities;
using Domain.Models;
using MicroservicesLogger.Interfaces;
using Moq;
using Repository.Interfaces;
using Xunit;

namespace AuthApiTests.Application
{
    public class RegisterServiceTests
    {
        private readonly Mock<IRegisterRepository> _registerRepositoryMock;
        private readonly Mock<IRabbitMqPublisher> _publisherMock;
        private readonly Mock<IApiLog<ApiLogModel>> _loggerMock;
        private readonly RegisterService _registerService;

        public RegisterServiceTests()
        {
            _registerRepositoryMock = new Mock<IRegisterRepository>();
            _publisherMock = new Mock<IRabbitMqPublisher>();
            _loggerMock = new Mock<IApiLog<ApiLogModel>>();

            _registerService = new RegisterService(_registerRepositoryMock.Object, _publisherMock.Object, _loggerMock.Object);

            _loggerMock.Setup(x => x.CreateBaseLogAsync()).ReturnsAsync(new ApiLogModel());
            _loggerMock.Setup(x => x.GetBaseLogAsync()).ReturnsAsync(new ApiLogModel());
        }

        [Fact]
        public async Task RegisterAsync_ThrowsException_WhenUserAlreadyExists()
        {
            // Arrange
            var subscriptionRequest = new SubscriptionRequest
            {
                Username = "testuser",
                Email = "test@example.com",
                Password = "TestPassword1!",
                Name = "Test",
                LastName = "User"
            };

            _registerRepositoryMock
                .Setup(repo => repo.GetUserByUsernameAndEmailAsync(subscriptionRequest.Username, subscriptionRequest.Email))
                .ReturnsAsync(new User());

            // Act & Assert
            var response = await _registerService.RegisterAsync(subscriptionRequest);

            Assert.False(response.IsSuccess);
            Assert.Equal("User already exists.", response.Error.Message);

            _publisherMock.Verify(p => p.PublishUserRegistrationOnQueueAsync(It.IsAny<UserQueueRegister>()), Times.Never);
        }

        [Fact]
        public async Task RegisterAsync_PublishesMessage_AndReturnsSuccess_WhenUserDoesNotExist()
        {
            // Arrange
            var subscriptionRequest = new SubscriptionRequest
            {
                Username = "testuser",
                Email = "test@example.com",
                Password = "TestPassword1!",
                Name = "Test",
                LastName = "User"
            };

            _registerRepositoryMock
                .Setup(repo => repo.GetUserByUsernameAndEmailAsync(subscriptionRequest.Username, subscriptionRequest.Email))
                .ReturnsAsync((User)null!);

            _publisherMock
                .Setup(p => p.PublishUserRegistrationOnQueueAsync(It.IsAny<UserQueueRegister>()))
                .Returns(Task.CompletedTask);

            // Act
            var response = await _registerService.RegisterAsync(subscriptionRequest);

            // Assert
            Assert.NotNull(response);
            Assert.True(response.IsSuccess);
            Assert.True(response.Data);

            _publisherMock.Verify(p => p.PublishUserRegistrationOnQueueAsync(
                It.Is<UserQueueRegister>(u =>
                    u.Username == subscriptionRequest.Username &&
                    u.Email == subscriptionRequest.Email
                )), Times.Once);
        }

        [Fact]
        public async Task UnregisterAsync_ThrowsException_WhenUserDoesNotExist()
        {
            // Arrange
            var unsubscribeRequest = new UnsubscribeRequest
            {
                Username = "testuser",
                Email = "test@example.com",
                Password = "TestPassword1!"
            };

            _registerRepositoryMock
                .Setup(repo => repo.GetUserByUsernameAndEmailAsync(unsubscribeRequest.Username, unsubscribeRequest.Email))
                .ReturnsAsync((User)null!);

            // Act & Assert
            var response = await _registerService.UnregisterAsync(unsubscribeRequest);

            Assert.False(response.IsSuccess);
            Assert.Equal("User not exists.", response.Error.Message);

            _publisherMock.Verify(p => p.PublishUserRegistrationOnQueueAsync(It.IsAny<UserQueueRegister>()), Times.Never);
        }

        [Fact]
        public async Task UnregisterAsync_PublishesMessage_AndReturnsSuccess_WhenUserExists()
        {
            // Arrange
            var unsubscribeRequest = new UnsubscribeRequest
            {
                Username = "testuser",
                Email = "test@example.com",
                Password = "TestPassword1!"
            };

            _registerRepositoryMock
                .Setup(repo => repo.GetUserByUsernameAndEmailAsync(unsubscribeRequest.Username, unsubscribeRequest.Email))
                .ReturnsAsync(new User());

            _publisherMock
                .Setup(p => p.PublishUserRegistrationOnQueueAsync(It.IsAny<UserQueueRegister>()))
                .Returns(Task.CompletedTask);

            // Act
            var response = await _registerService.UnregisterAsync(unsubscribeRequest);

            // Assert
            Assert.NotNull(response);
            Assert.True(response.IsSuccess);
            Assert.True(response.Data);

            _publisherMock.Verify(p => p.PublishUserRegistrationOnQueueAsync(
                It.Is<UserQueueRegister>(u =>
                    u.Username == unsubscribeRequest.Username &&
                    u.Email == unsubscribeRequest.Email
                )), Times.Once);
        }
    }
}
