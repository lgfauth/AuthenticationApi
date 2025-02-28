using Domain.Models;
using Domain.Models.Envelope;

namespace Application.Interfaces
{
    public interface IAuthService
    {
        Task<IResponse<AuthResponse>> LoginAsync(LoginRequest request);
        Task<IResponse<bool>> ValidateToken(string token);
    }
}
