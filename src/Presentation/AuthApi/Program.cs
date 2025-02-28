using Application.Injections;
using Domain.Settings;
using Microsoft.OpenApi.Models;
using System.Reflection;

namespace AuthApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();

            var variables = builder.Configuration.Get<EnvirolmentVariables>();
            builder.Services.AddSingleton(variables);

            DependenceInjections.Injections(builder.Services, variables.MONGODBSETTINGS_CONNECTIONSTRING);

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.EnableAnnotations();

                var domainXmlPath = Path.Combine(AppContext.BaseDirectory, "Domain.xml");
                var presentationXmlPath = Path.Combine(AppContext.BaseDirectory,
                    $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");

                options.IncludeXmlComments(domainXmlPath);
                options.IncludeXmlComments(presentationXmlPath);

                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Ruler RPG API", Version = "v1" });
            });

            var app = builder.Build();

            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}