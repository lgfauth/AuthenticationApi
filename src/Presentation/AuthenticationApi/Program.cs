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

            builder.Services.AddControllers();            
            builder.Configuration.AddEnvironmentVariables();

            var variables = new EnvirolmentVariables();
            builder.Configuration.Bind(variables);
            builder.Services.AddSingleton(variables);

            Console.WriteLine("--------------- // ---------------");
            Console.WriteLine("MONGODBSETTINGS__CONNECTIONSTRING: " + builder.Configuration["MONGODBSETTINGS__CONNECTIONSTRING"]);
            Console.WriteLine("RABBITMQCONFIGURATION__QUEUENAME: " + builder.Configuration["RABBITMQCONFIGURATION__QUEUENAME"]);
            Console.WriteLine("verificando valores de variaveis de ambiente ConnectionString: " + variables.MONGODBSETTINGS__CONNECTIONSTRING);
            Console.WriteLine("verificando valores de variaveis de ambiente QueueName: " + variables.RABBITMQCONFIGURATION__QUEUENAME);
            Console.WriteLine("--------------- // ---------------");

            DependenceInjections.Injections(builder.Services, variables.MONGODBSETTINGS__CONNECTIONSTRING);

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
