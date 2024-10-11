using Northwind.Domain.Entities;
using System.Threading.Tasks;

namespace Northwind.Dal.Abstractions
{
    public interface IUnitOfWork
    {
        IRepository<TEntity> Repository<TEntity>() where TEntity : Entity;

        Task SaveChangesAsync();

        ITransaction BeginTransaction();
    }
}