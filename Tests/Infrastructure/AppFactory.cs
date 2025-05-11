// using Business.Services;
// using Dapper;
// using FluentMigrator.Runner;
// using FluentValidation.AspNetCore;
// using Microsoft.AspNetCore.Hosting;
// using Microsoft.AspNetCore.Mvc.Testing;
// using Microsoft.AspNetCore.TestHost;
// using Microsoft.Extensions.Configuration;
// using Microsoft.Extensions.DependencyInjection;
// using Npgsql;
// using Persistence.Migrations;
//
// namespace Tests.Infrastructure
// {
//     public class AppFactory : WebApplicationFactory<Program>
//     {
//         public IConfiguration Configuration { get; private set; }
//         
//         protected override void ConfigureWebHost(IWebHostBuilder builder)
//         {
//             var projectDir = Directory.GetCurrentDirectory();
//             
//             builder.ConfigureAppConfiguration(config =>
//             {
//                 Configuration = new ConfigurationBuilder()
//                     .SetBasePath(projectDir)
//                     .AddJsonFile("integrationSettings.json")
//                     .Build();
//  
//                 config.AddConfiguration(Configuration);
//             });
//             
//             builder.ConfigureTestServices(services =>
//             {
//                 services.AddMvc();
//                 services.AddFluentValidation(x => { x.RegisterValidatorsFromAssemblyContaining<EmployeeService>(); });
//
//                 services.AddFluentMigratorCore()
//                     .ConfigureRunner(rb => rb
//                         .AddPostgres()
//                         .WithGlobalConnectionString(Configuration.GetConnectionString("PostgresConnection"))
//                         .ScanIn(typeof(CreateEmployeeTable).Assembly).For.Migrations())
//                     .AddLogging(lb => lb.AddFluentMigratorConsole());
//             });
//             
//             builder.ConfigureServices((context, services) =>
//             {
//                 using var scope = services.BuildServiceProvider().CreateScope();
//                 var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
//                 runner.MigrateUp();
//             });
//         }
//         
//         // protected override void Dispose(bool disposing)
//         // {
//         //     DropDataBaseIfExists();
//         //     base.Dispose(disposing);
//         // }
//         //
//         // private void DropDataBaseIfExists()
//         // {
//         //     NpgsqlConnection.ClearAllPools();
//         //     
//         //     var dbName = Configuration.GetValue<string>("Database:Name");
//         //     var parameters = new DynamicParameters();
//         //     parameters.Add("name", dbName);
//         //     using var connection = new NpgsqlConnection(Configuration.GetConnectionString("PostgresConnection"));
//         //     var records = connection.Query("SELECT datname FROM pg_database WHERE datname = @name", parameters);
//         //     if (records.Any())
//         //     {
//         //         connection.Execute($"SELECT pg_terminate_backend(pg_stat_activity.pid) FROM pg_stat_activity WHERE pg_stat_activity.datname = '{dbName}'; "
//         //                            + $"DROP DATABASE {dbName};");
//         //     }
//         // }
//     }
// }

using Dapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Tests.Infrastructure
{
    public class AppFactory : WebApplicationFactory<Program>
    {
        public IConfiguration Configuration { get; private set; }
        
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            var projectDir = Directory.GetCurrentDirectory();

            builder.ConfigureAppConfiguration((context, config) =>
            {
                Configuration = new ConfigurationBuilder()
                    .SetBasePath(projectDir)
                    .AddJsonFile("integrationSettings.json", optional: false, reloadOnChange: true)
                    .Build();

                config.AddConfiguration(Configuration);
            });
            
            builder.UseEnvironment("Integration");
        }

        protected override void Dispose(bool disposing)
        {
            DropDataBaseIfExists();
            base.Dispose(disposing);
        }
        
        private void DropDataBaseIfExists()
        {
            NpgsqlConnection.ClearAllPools();
    
            var dbName = Configuration.GetValue<string>("Database:Name");
            var parameters = new DynamicParameters();
            parameters.Add("name", dbName);

            var connectionString = Configuration.GetConnectionString("PostgresConnection");
            var builder = new NpgsqlConnectionStringBuilder(connectionString)
            {
                Database = "postgres"
            };

            using var connection = new NpgsqlConnection(builder.ConnectionString);
            connection.Open();

            connection.Execute(@"
                SELECT pg_terminate_backend(pg_stat_activity.pid) 
                FROM pg_stat_activity 
                WHERE pg_stat_activity.datname = @name 
                AND pid <> pg_backend_pid();", parameters);

            connection.Execute($"DROP DATABASE IF EXISTS {dbName}");
        }

    }
}