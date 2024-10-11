using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Northwind.Domain.Entities;

namespace Northwind.Bll.Services
{
    public class BaseService
    {
        protected static async Task<IEnumerable<T>> GetOrderedResult<T>(IQueryable<T> filter, Expression<Func<T, object>> orderBySelector, CancellationToken cancellationToken) where T : Entity
        {
            var items = await filter
                .OrderBy(orderBySelector)
                .ToListAsync(cancellationToken);

            return items;
        }
    }
}
