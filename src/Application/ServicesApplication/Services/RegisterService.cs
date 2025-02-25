using Domain.Models;
using Domain.Models.Envelope;
using Domain.Settings;
using Microsoft.Extensions.Options;
using Repository.Interfaces;
using ServicesApplication.Interfaces;
using ServicesApplication.Utils;

namespace ServicesApplication.Services
{
    public class RegisterService : IRegisterService
    {
        private readonly IRegisterRepository _registerRepository;
        private readonly IRabbitMqPublisher _publisher;

        public RegisterService(IRegisterRepository registerRepository,IRabbitMqPublisher publisher)
        {
            _publisher = publisher;
            _registerRepository = registerRepository;
        }

        public async Task<IResponse<bool>> RegisterAsync(SubscriptionRequest request)
        {
            var existingUser = await _registerRepository.GetUserByUsernameAndEmailAsync(request.Username, request.Email);

            if (existingUser is not null)
                throw new Exception("User already exists.");

            request.Password = Encryptor.HashPassword(request.Password);

            await _publisher.PublishUserRegistration(request);

            return new ResponseOk<bool>(true);
        }

        public async Task<IResponse<bool>> UnregisterAsync(UnsubscribeRequest request)
        {
            var existingUser = await _registerRepository.GetUserByUsernameAndEmailAsync(request.Username, request.Email);

            if (existingUser is null)
                throw new Exception("User not exists.");

            var response = await _registerRepository.DeleteUserByIdAsync(existingUser.Id);

            return new ResponseOk<bool>(response);
        }
    }
}
