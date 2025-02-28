using Domain.Entities;

namespace Application.Interfaces
{
    public interface IRabbitMqPublisher
    {
        Task PublishUserRegistrationOnQueueAsync(UserQueueRegister message);
    }
}
