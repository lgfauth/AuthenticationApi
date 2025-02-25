using Domain.Entities;
using Domain.Models;

namespace ServicesApplication.Interfaces
{
    public interface IRabbitMqPublisher
    {
        Task PublishUserRegistrationOnQueueAsync(UserQueueRegister message);
    }
}
