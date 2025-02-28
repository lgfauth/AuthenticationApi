using Domain.Entities;

namespace Repository.Interfaces
{
    public interface IAuthRepository
    {
        Task<User?> GetUserByUsernameAndPasswordAsync(string username, string password);
    }
}