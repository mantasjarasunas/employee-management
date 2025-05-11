using System;
using System.Data;

namespace Persistence.Infrastructure
{
    public interface IDbContext
    {
        IDbContextState State { get; }
        IDbConnection Connection { get; }
        IDbTransaction Transaction { get; }
        IUnitOfWork UnitOfWork { get; }
        void Commit();
        void Rollback();
    }

    public class DbContext : IDbContext, IDisposable
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private IDbConnection _connection;
        private IDbTransaction _transaction;
        private IUnitOfWork _unitOfWork;

        public DbContext(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public IDbContextState State { get; private set; } = IDbContextState.Closed;

        public IDbConnection Connection =>
            _connection ??= OpenConnection();

        public IDbTransaction Transaction =>
            _transaction ??= Connection.BeginTransaction();

        public IUnitOfWork UnitOfWork =>
            _unitOfWork ??= new UnitOfWork(Transaction);

        public void Commit()
        {
            try
            {
                UnitOfWork.Commit();
                State = IDbContextState.Comitted;
            }
            catch
            {
                Rollback();
                throw;
            }
            finally
            {
                Reset();
            }
        }

        public void Rollback()
        {
            try
            {
                UnitOfWork.Rollback();
                State = IDbContextState.RolledBack;
            }
            finally
            {
                Reset();
            }
        }

        private IDbConnection OpenConnection()
        {
            State = IDbContextState.Open;
            return _connectionFactory.CreateOpenConnection();
        }

        private void Reset()
        {
            Transaction?.Dispose();

            _transaction = null;
            _unitOfWork = new UnitOfWork(Transaction);
        }

        public void Dispose()
        {
            Connection?.Close();
            Connection?.Dispose();

            if (_transaction != null)
            {
                Transaction?.Dispose();
            }

            _connection = null;
            _transaction = null;
            _unitOfWork = null;
        }
    }

    public enum IDbContextState
    {
        Closed,
        Open,
        Comitted,
        RolledBack
    }
}