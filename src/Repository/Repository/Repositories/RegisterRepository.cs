using Domain.Entities;
using Domain.Settings;
using MongoDB.Driver;
using Repository.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace Repository.Repositories
{
    [ExcludeFromCodeCoverage]
    public class RegisterRepository : IRegisterRepository
    {
        private readonly IMongoCollection<User> _users;

        public RegisterRepository(EnvirolmentVariables envirolmentVariables)
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