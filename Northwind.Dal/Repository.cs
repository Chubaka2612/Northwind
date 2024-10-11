using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Northwind.Dal.Abstractions;
using Northwind.Domain.Entities;

namespace Northwind.Dal
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : Entity
    {
        protected readonly DbSet<TEntity> DataSet;

        public Repository(DbContext dbContext)
        {
            DataSet = dbContext.Set<TEntity>();
        }

        public async Task<TEntity> FindAsync(CancellationToken cancellationToken, params object[] keyValues)
        {
            return await DataSet.FindAsync(keyValues, cancellationToken);
        }

        public virtual void Add(TEntity item)
        {
            DataSet.Add(item);
        }

        public virtual void Update(TEntity item)
        {
            DataSet.Update(item);
        }

        public virtual void Delete(TEntity item)
        {
            DataSet.Remove(item);
        }

        public virtual void DeleteRange(IEnumerable<TEntity> items)
        {
            DataSet.RemoveRange(items);
        }

        public IQueryable<TEntity> Queryable(params Expression<Func<TEntity, object>>[] includeProperties)
        {
            IQueryable<TEntity> query = DataSet;
            if (includeProperties != null)
            {
                query = includeProperties
                    .Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
            }
            return query;
        }

        public IQueryable<TEntity> Queryable(string[] includeProperties)
        {
            IQueryable<TEntity> query = DataSet;
            if (includeProperties != null)
            {
                query = includeProperties
                    .Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
            }
            return query;
        }

    }
}