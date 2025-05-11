using Dapper;
using Npgsql;

namespace Persistence.Infrastructure
{
    public static class Database
    {
        public static void EnsureDatabase(string connectionString, string name)
        {
            var builder = new NpgsqlConnectionStringBuilder(connectionString)
            {
                Database = "postgres"
            };

            using var connection = new NpgsqlConnection(builder.ConnectionString);
            var parameters = new DynamicParameters();
            parameters.Add("name", name);
            var records = connection.Query("SELECT datname FROM pg_database WHERE datname = @name", parameters);

            if (!records.Any())
            {
                connection.Execute($"CREATE DATABASE {name}");
            }
        }
    }
}