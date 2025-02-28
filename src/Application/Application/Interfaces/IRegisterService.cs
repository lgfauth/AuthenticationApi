using Domain.Models;
using Domain.Models.Envelope;

namespace Application.Interfaces
{
    public interface IRegisterService
    {
        Task<IResponse<bool>> RegisterAsync(SubscriptionRequest request);
        Task<IResponse<bool>> UnregisterAsync(UnsubscribeRequest request);
    }
}
