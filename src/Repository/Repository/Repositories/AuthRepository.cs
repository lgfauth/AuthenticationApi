using Domain.Entities;
using Domain.Settings;
using MongoDB.Driver;
using Repository.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace Repository.Repositories
{
    [ExcludeFromCodeCoverage]
    public class AuthRepository : IAuthRepository
    {
        private readonly IMongoCollection<User> _users;

        public AuthRepository(EnvirolmentVariables envirolmentVariables)
        {
            string connectionString = string.Format(
                envirolmentVariables.MONGODBSETTINGS__CONNECTIONSTRING,
                envirolmentVariables.MONGODBDATA__USER,
                envirolmentVariables.MONGODBDATA__PASSWORD,
                envirolmentVariables.MONGODBDATA__CLUSTER);

            var client = new MongoClient(connectionString);

            var database = client.GetDatabase(envirolmentVariables.MONGODBSETTINGS__DATABASENAME);
            _users = database.GetCollection<User>("Users");
        }
        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            var response = await _users.FindAsync(Builders<User>.Filter.Eq(u => u.Username, username));

            return response.FirstOrDefault();
        }

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
