using Domain.Models;
using Domain.Models.Envelope;

namespace ServicesApplication.Interfaces
{
    public interface IRegisterService
    {
        Task<IResponse<bool>> RegisterAsync(RegisterRequest request);
        Task<IResponse<bool>> UnregisterAsync(UnregisterRequest request);
    }
}