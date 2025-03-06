using Application.Interfaces;
using Application.LogModels;
using Application.Utils;
using Domain.Entities;
using Domain.Models;
using Domain.Models.Envelope;
using MicroservicesLogger.Interfaces;
using MicroservicesLogger.Models;
using Repository.Interfaces;

namespace Application.Services
{
    public class RegisterService : IRegisterService
    {
        private readonly IRegisterRepository _registerRepository;
        private readonly IRabbitMqPublisher _publisher;
        private readonly IApiLog<ApiLogModel> _logger;

        public RegisterService(IRegisterRepository registerRepository, IRabbitMqPublisher publisher, IApiLog<ApiLogModel> logger)
        {
            _logger = logger;
            _publisher = publisher;
            _registerRepository = registerRepository;
        }

        public async Task<IResponse<bool>> RegisterAsync(SubscriptionRequest request)
        {
            var baselog = await _logger.GetBaseLogAsync();
            var logCheck = new SubLog();
            await baselog.AddStepAsync("CHECK_ALREADY_REGISTERED_USER", logCheck);

            logCheck.StartCronometer();

            var existingUser = await _registerRepository.GetUserByUsernameAndEmailAsync(request.Username, request.Email);

            if (existingUser is not null)
            {
                var response = new ResponseModel { Message = "User already exists.", Code = "AL022" };

                logCheck.StopCronometer();
                logCheck.Exception = new Exception(response.Message);

                return new ResponseError<bool>(response);
            }

            logCheck.StopCronometer();

            var logRabbit = new SubLog();
            await baselog.AddStepAsync("SENDING_TO_RABBITMQ", logRabbit);
            
            logRabbit.StartCronometer();

            request.Password = Encryptor.HashPassword(request.Password);

            UserQueueRegister userQueueRegister = new UserQueueRegister(request);
            await _publisher.PublishUserRegistrationOnQueueAsync(userQueueRegister);

            logRabbit.StopCronometer();

            return new ResponseOk<bool>(true);
        }

        public async Task<IResponse<bool>> UnregisterAsync(UnsubscribeRequest request)
        {
            var baselog = await _logger.GetBaseLogAsync();
            var logCheck = new SubLog();
            await baselog.AddStepAsync("CHECK_ALREADY_UNREGISTERED_USER", logCheck);

            var existingUser = await _registerRepository.GetUserByUsernameAndEmailAsync(request.Username, request.Email);

            if (existingUser is null)
            {
                var response = new ResponseModel { Message = "User not exists.", Code = "AL021" };

                logCheck.StopCronometer();
                logCheck.Exception = new Exception(response.Message);

                return new ResponseError<bool>(response);
            }

            logCheck.StopCronometer();

            var logRabbit = new SubLog();
            await baselog.AddStepAsync("SENDING_TO_RABBITMQ", logRabbit);

            logRabbit.StartCronometer();

            UserQueueRegister userQueueRegister = new UserQueueRegister(request);
            await _publisher.PublishUserRegistrationOnQueueAsync(userQueueRegister);
            
            logRabbit.StopCronometer();

            return new ResponseOk<bool>(true);
        }
    }
}
