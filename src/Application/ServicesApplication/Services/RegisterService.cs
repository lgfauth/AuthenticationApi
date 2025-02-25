using Domain.Entities;
using Domain.Models;
using Domain.Models.Envelope;
using Repository.Interfaces;
using ServicesApplication.Interfaces;
using ServicesApplication.Utils;

namespace ServicesApplication.Services
{
    public class RegisterService : IRegisterService
    {
        private readonly IRegisterRepository _registerRepository;
        private readonly IRabbitMqPublisher _publisher;

        public RegisterService(IRegisterRepository registerRepository, IRabbitMqPublisher publisher)
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

            UserQueueRegister userQueueRegister = new UserQueueRegister(request);
            await _publisher.PublishUserRegistrationOnQueueAsync(userQueueRegister);

            return new ResponseOk<bool>(true);
        }

        public async Task<IResponse<bool>> UnregisterAsync(UnsubscribeRequest request)
        {
            var existingUser = await _registerRepository.GetUserByUsernameAndEmailAsync(request.Username, request.Email);

            if (existingUser is null)
                throw new Exception("User not exists.");

            UserQueueRegister userQueueRegister = new UserQueueRegister(request);
            await _publisher.PublishUserRegistrationOnQueueAsync(userQueueRegister);

            return new ResponseOk<bool>(true);
        }
    }
}