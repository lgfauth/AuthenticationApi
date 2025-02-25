using Domain.Models;
using Domain.Models.Envelope;

namespace ServicesApplication.Interfaces
{
    public interface IRegisterService
    {
        Task<IResponse<bool>> RegisterAsync(SubscriptionRequest request);
        Task<IResponse<bool>> UnregisterAsync(UnsubscribeRequest request);
    }
}