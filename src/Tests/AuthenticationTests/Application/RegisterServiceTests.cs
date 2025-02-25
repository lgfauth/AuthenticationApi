using Domain.Entities;
using Domain.Models;
using Moq;
using Repository.Interfaces;
using ServicesApplication.Interfaces;
using ServicesApplication.Services;

namespace AuthenticationTests.Application
{
    public class RegisterServiceTests
    {
        private readonly Mock<IRegisterRepository> _registerRepositoryMock;
        private readonly Mock<IRabbitMqPublisher> _publisherMock;
        private readonly RegisterService _registerService;

        public RegisterServiceTests()
        {
            _registerRepositoryMock = new Mock<IRegisterRepository>();
            _publisherMock = new Mock<IRabbitMqPublisher>();
            _registerService = new RegisterService(_registerRepositoryMock.Object, _publisherMock.Object);
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
            var ex = await Assert.ThrowsAsync<Exception>(() => _registerService.RegisterAsync(subscriptionRequest));
            Assert.Equal("User already exists.", ex.Message);

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
            var ex = await Assert.ThrowsAsync<Exception>(() => _registerService.UnregisterAsync(unsubscribeRequest));
            Assert.Equal("User not exists.", ex.Message);

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
