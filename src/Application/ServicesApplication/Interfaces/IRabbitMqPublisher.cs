using Domain.Models;

namespace ServicesApplication.Interfaces
{
    public interface IRabbitMqPublisher
    {
        Task PublishUserRegistration(SubscriptionRequest message);
    }
}
