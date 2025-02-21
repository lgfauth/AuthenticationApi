using Domain.Entities;

namespace Repository.Interfaces
{
    public interface IRegisterRepository
    {
        Task CreateUserAsync(User user);

        Task<User?> GetUserByUsernameAndEmailAsync(string username, string email);
        Task<bool> DeleteUserByIdAsync(string id);
    }
}