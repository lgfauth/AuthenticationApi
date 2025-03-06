using Application.Injections;
using Application.Utils;
using Domain.Settings;
using Microsoft.OpenApi.Models;
using System.Reflection;

namespace AuthApi
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();

            var variables = builder.Configuration.Get<EnvirolmentVariables>();
            builder.Services.AddSingleton(variables);

            DependenceInjections.Injections(builder.Services, variables.MONGODBSETTINGS_CONNECTIONSTRING);

            var isDevelopment = builder.Environment.IsDevelopment();
            if (!isDevelopment)
            {
                var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
                builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
            }

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.EnableAnnotations();

                var domainXmlPath = Path.Combine(AppContext.BaseDirectory, "Domain.xml");
                var presentationXmlPath = Path.Combine(AppContext.BaseDirectory,
                    $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");

                options.IncludeXmlComments(domainXmlPath);
                options.IncludeXmlComments(presentationXmlPath);

                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Authentication API", Version = "v1" });
            });

            var app = builder.Build();

            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            await HeathChecker.CheckMongoDbConnection(variables);

            app.Run();
        }
    }
}