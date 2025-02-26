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
            Console.WriteLine("--------------- // (\"---------------");
            builder.Services.Configure<EnvirolmentVariables>(builder.Configuration);
            Console.WriteLine("Realizada configuração de variaveis de ambiente");

            EnvirolmentVariables variables = builder.Configuration.Get<EnvirolmentVariables>()!;
            Console.WriteLine("verificando valores de variaveis de ambiente: " + variables.MongoDbSettings__ConnectionString);

            DependenceInjections.Injections(builder.Services, variables.MongoDbSettings__ConnectionString);
            Console.WriteLine("Finalizadas injeções de dependência, iniciando configurações do swagger");

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

            Console.WriteLine("Finalizada configuração do swagger, preparando build");

            var app = builder.Build();

            //if (app.Environment.IsDevelopment())
            //{
                app.UseSwagger();
                app.UseSwaggerUI();
            //}

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            Console.WriteLine("Finalizado build, inicializando aplicação");
            Console.WriteLine("--------------- // (\"---------------");

            app.Run();
        }
    }
}
