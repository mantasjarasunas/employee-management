using System;
using System.Data;

namespace Persistence.Infrastructure
{
    public enum UnitOfWorkState
    {
        Open,
        Committed,
        RolledBack
    }

    public interface IUnitOfWork
    {
        UnitOfWorkState State { get; }
        IDbTransaction Transaction { get; }
        void Commit();
        void Rollback();
    }

    public class UnitOfWork(IDbTransaction transaction) : IUnitOfWork
    {
        public UnitOfWorkState State { get; private set; } = UnitOfWorkState.Open;

        public IDbTransaction Transaction { get; private set; } = transaction;

        public void Commit()
        {
            try
            {
                Transaction.Commit();
                State = UnitOfWorkState.Committed;
            }
            catch (Exception)
            {
                Transaction.Rollback();
                throw;
            }
        }

        public void Rollback()
        {
            Transaction.Rollback();
            State = UnitOfWorkState.RolledBack;
        }
    }
}