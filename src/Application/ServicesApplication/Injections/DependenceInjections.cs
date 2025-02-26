using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Repository.Interfaces;
using Repository.Repositories;
using ServicesApplication.Interfaces;
using ServicesApplication.Messanger;
using ServicesApplication.Services;
using System.Diagnostics.CodeAnalysis;

namespace ServicesApplication.Injections
{
    [ExcludeFromCodeCoverage]
    public class DependenceInjections
    {
        public static void Injections(IServiceCollection services, string mongoDbConnectionString)
        {

            services.AddSingleton<IMongoClient>(sp => new MongoClient(mongoDbConnectionString));

            services.AddScoped<IRegisterService, RegisterService>();
            services.AddScoped<IRegisterRepository, RegisterRepository>();
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IAuthService, AuthService>();

            services.AddSingleton<IRabbitMqPublisher, RabbitMqPublisher>();
        }
    }
}
