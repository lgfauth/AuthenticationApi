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

            var isDevelopment = builder.Environment.IsDevelopment();

            if (isDevelopment)
            {
                builder.Services.AddCors(options =>
                {
                    options.AddPolicy("DevelopmentCorsPolicy", policy =>
                    {
                        policy.AllowAnyOrigin()   // Permite qualquer origem
                              .AllowAnyMethod()   // Permite qualquer método (GET, POST, etc.)
                              .AllowAnyHeader();  // Permite qualquer cabeçalho
                    });
                });
            }
            else
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

                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Ruler RPG API", Version = "v1" });
            });

            var app = builder.Build();

#if DEBUG
            app.UseCors("DevelopmentCorsPolicy");
#endif

            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}