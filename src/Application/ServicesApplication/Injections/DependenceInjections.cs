using Domain.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Repository.Interfaces;
using Repository.Services;
using ServicesApplication.Interfaces;
using ServicesApplication.Services;

namespace ServicesApplication.Injections
{
    public class DependenceInjections
    {
        public static void Injections(IServiceCollection services)
        {

            services.AddSingleton<IMongoClient>(sp =>
            {
                var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
                return new MongoClient(settings.ConnectionString);
            });

            services.AddScoped<IRegisterService, RegisterService>();
            services.AddScoped<IRegisterRepository, RegisterRepository>();
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IAuthService, AuthService>();
        }
    }
}
