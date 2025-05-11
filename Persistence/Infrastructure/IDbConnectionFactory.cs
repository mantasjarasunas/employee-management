using System.Data;

namespace Persistence.Infrastructure
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateOpenConnection();
    }

    public class DbConnectionFactory(Func<IDbConnection> connectionFactory) : IDbConnectionFactory
    {
        private readonly Func<IDbConnection> _connectionFactoryFn = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));

        public IDbConnection CreateOpenConnection() => _connectionFactoryFn();
    }
}