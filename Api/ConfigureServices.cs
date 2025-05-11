using System.Reflection;
using Business.Services;
using FluentMigrator.Runner;
using FluentValidation.AspNetCore;
using Microsoft.OpenApi.Models;
using Persistence.Infrastructure;
using Persistence.Migrations;

namespace Api;

public static class ConfigureServices
{
    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        var serviceProvider = CreateServices(configuration);

        using (var scope = serviceProvider.CreateScope())
        {
            Database.EnsureDatabase(
                configuration.GetConnectionString("PostgresConnection"),
                configuration.GetValue<string>("DataBase:Name")
            );
            UpdateDatabase(scope.ServiceProvider);
        }

        services.AddControllers();

        services.AddFluentValidation(config =>
        {
            config.RegisterValidatorsFromAssemblyContaining<EmployeeService>();
        });

        services.AddTransient<IDbConnectionFactory>(_ =>
        {
            return new DbConnectionFactory(() =>
            {
                var connection = new Npgsql.NpgsqlConnection(configuration.GetConnectionString("PostgresConnection"));
                connection.Open();
                
                return connection;
            });
        });

        RegisterBusinessServices(services);
        RegisterRepositories(services);

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Employee management API",
                Version = "v1",
                Description = "API for managing employees"
            });

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
        });

        return services;
    }
    
    private static void RegisterRepositories(IServiceCollection services)
    {
        services.AddScoped<IDbContext, DbContext>();
        var repositories = Assembly.GetAssembly(typeof(DbRepository))
            ?.GetTypes().Where(t => t.Namespace != null && t.Namespace.Contains("Repositories")).ToList();

        if (repositories != null)
        {
            foreach (var repositoryInterface in repositories.Where(t => t.IsInterface))
            {
                var implementation = repositories.FirstOrDefault(c => c.IsClass && repositoryInterface.Name[1..] == c.Name);

                if (implementation != null)
                {
                    services.AddScoped(repositoryInterface, implementation);
                }
            }
        }
    }

    private static void RegisterBusinessServices(IServiceCollection services)
    {
        var assembly = Assembly.GetAssembly(typeof(IEmployeeService));

        var businessServices = assembly
            .GetTypes()
            .Where(t => t.Namespace != null && t.Namespace.Contains("Services"))
            .ToList();

        foreach (var serviceInterface in businessServices.Where(t => t.IsInterface))
        {
            var implementation = businessServices
                .FirstOrDefault(c => c.IsClass && serviceInterface.Name[1..] == c.Name);

            if (implementation != null)
            {
                services.AddTransient(serviceInterface, implementation);
            }
        }
    }
    
    private static IServiceProvider CreateServices(IConfiguration configuration)
    {
        return new ServiceCollection()
            .AddFluentMigratorCore()
            .ConfigureRunner(rb => rb
                .AddPostgres()
                .WithGlobalConnectionString(configuration.GetConnectionString("PostgresConnection"))
                .ScanIn(typeof(CreateEmployeeTable).Assembly).For.Migrations())
            .AddLogging(lb => lb.AddFluentMigratorConsole())
            .BuildServiceProvider(false);
    }

    private static void UpdateDatabase(IServiceProvider serviceProvider)
    {
        var runner = serviceProvider.GetRequiredService<IMigrationRunner>();
        runner.MigrateUp();
    }
}