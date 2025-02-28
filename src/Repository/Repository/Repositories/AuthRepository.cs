using Domain.Entities;
using Domain.Settings;
using MongoDB.Driver;
using Repository.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace Repository.Repositories
{
    [ExcludeFromCodeCoverage]
    public class AuthRepository : RepositoryBase, IAuthRepository
    {
        public AuthRepository(EnvirolmentVariables envirolmentVariables) : base(envirolmentVariables) { }

        public async Task<User?> GetUserByUsernameAndPasswordAsync(string username, string password)
        {
            var filter = Builders<User>.Filter.And(
             Builders<User>.Filter.Eq(u => u.Username, username),
             Builders<User>.Filter.Eq(u => u.PasswordHash, password));

            var response = await _users.FindAsync(filter);

            return response.FirstOrDefault();

        }
    }
}
