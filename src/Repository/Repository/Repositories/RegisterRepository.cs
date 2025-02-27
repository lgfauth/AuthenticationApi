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

        public RegisterRepository(IOptions<EnvirolmentVariables> envirolmentVariables)
        {
            string connectionString = string.Format(
                envirolmentVariables.Value.MONGODBSETTINGS__CONNECTIONSTRING,
                envirolmentVariables.Value.MONGODBDATA__USER,
                envirolmentVariables.Value.MONGODBDATA__PASSWORD,
                envirolmentVariables.Value.MONGODBDATA__CLUSTER);

            var client = new MongoClient(connectionString);

            var database = client.GetDatabase(envirolmentVariables.Value.MONGODBSETTINGS__DATABASENAME);
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