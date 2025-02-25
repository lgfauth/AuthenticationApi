using Domain.Entities;

namespace Repository.Interfaces
{
    public interface IRegisterRepository
    {
        Task<User?> GetUserByUsernameAndEmailAsync(string username, string email);
    }
}