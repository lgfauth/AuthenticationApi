using Domain.Entities;
using Domain.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Repository.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace Repository.Repositories
{
    [ExcludeFromCodeCoverage]
    public class RegisterRepository : IRegisterRepository
    {
        private readonly IMongoCollection<User> _users;

        public RegisterRepository(IOptions<MongoDbSettings> mongoDbSettings, IOptions<MongoDbData> mongoDbData)
        {
            string connectionString = string.Format(
                mongoDbSettings.Value.ConnectionString,
                mongoDbData.Value.user,
                mongoDbData.Value.passsword,
                mongoDbData.Value.cluster);

            var client = new MongoClient(connectionString);

            var database = client.GetDatabase(mongoDbSettings.Value.DatabaseName);
            _users = database.GetCollection<User>("Users");

        }

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