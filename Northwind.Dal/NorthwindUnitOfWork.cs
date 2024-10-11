using Northwind.Dal.Abstractions;
using Northwind.Domain.Entities;

namespace Northwind.Dal
{
    public class NorthwindUnitOfWork : IUnitOfWork
    {
        private readonly NorthwindDbContext _context;

        public NorthwindUnitOfWork(NorthwindDbContext dbContext)
        {
            _context = dbContext;
        }

        public IRepository<TEntity> Repository<TEntity>() where TEntity : Entity
        {
            return new Repository<TEntity>(_context);
        }

        public ITransaction BeginTransaction()
        {
            return new Transaction(_context.Database.BeginTransaction());
        }

        public Task SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }
    }
}