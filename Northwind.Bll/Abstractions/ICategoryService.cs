using Northwind.Domain.Entities;

namespace Northwind.Bll.Abstractions
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetCategoriesAsync(CancellationToken cancellationToken);
    }
}
