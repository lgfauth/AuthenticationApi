using Domain.Settings;
using Microsoft.OpenApi.Models;
using ServicesApplication.Injections;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace AuthenticationApi
{
    /// <summary>
    /// 
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class Program
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.WebHost.UseUrls("http://0.0.0.0:8080");

            builder.Services.AddControllers();
            
            builder.Configuration.AddEnvironmentVariables();

            //builder.Services.Configure<EnvirolmentVariables>(builder.Configuration);
            //EnvirolmentVariables variables = builder.Configuration.Get<EnvirolmentVariables>()!;

            var variables = new EnvirolmentVariables();
            builder.Configuration.Bind(variables);

            // Registra a classe se necessário
            builder.Services.AddSingleton(variables);

            Console.WriteLine("--------------- // ---------------");
            Console.WriteLine("MongoDbSettings__ConnectionString: " + builder.Configuration["MongoDbSettings__ConnectionString"]);
            Console.WriteLine("RabbitMqConfiguration__QueueName: " + builder.Configuration["RabbitMqConfiguration__QueueName"]);
            Console.WriteLine("verificando valores de variaveis de ambiente ConnectionString: " + variables.MongoDbSettings__ConnectionString);
            Console.WriteLine("verificando valores de variaveis de ambiente QueueName: " + variables.RabbitMqConfiguration__QueueName);
            Console.WriteLine("--------------- // ---------------");

            DependenceInjections.Injections(builder.Services, variables.MongoDbSettings__ConnectionString);

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
