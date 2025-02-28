using Domain.Entities;
using Domain.Settings;
using MongoDB.Driver;
using Repository.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace Repository.Repositories
{
    [ExcludeFromCodeCoverage]
    public class RegisterRepository : RepositoryBase, IRegisterRepository
    {
        public RegisterRepository(EnvirolmentVariables envirolmentVariables):base(envirolmentVariables) { }

        public async Task<User?> GetUserByUsernameAndEmailAsync(string username, string email)
        {
            var filter = Builders<User>.Filter.And(
                         Builders<User>.Filter.Eq(u => u.Username, username),
                         Builders<User>.Filter.Eq(u => u.Email, email));

            var response = await _users.FindAsync(filter);

            return response.FirstOrDefault();
        }
    }
}