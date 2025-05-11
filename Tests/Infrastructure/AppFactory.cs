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