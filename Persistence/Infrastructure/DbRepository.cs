using System.Data;

namespace Persistence.Infrastructure
{
    public class DbRepository
    {
        private readonly IDbContext _dbContext;

        protected IDbConnection Connection => _dbContext.UnitOfWork.Transaction.Connection;
        private IDbTransaction Transaction => _dbContext.UnitOfWork.Transaction;

        protected DbRepository(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
