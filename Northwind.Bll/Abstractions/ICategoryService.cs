using Northwind.Domain.Entities;

namespace Northwind.Bll.Abstractions
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetCategoriesAsync(CancellationToken cancellationToken);

        Task<Category> GetCategoryAsync(int id, CancellationToken cancellationToken);

        Task<bool> UpdateCategoryImageAsync(int id, byte[] imageBytes, CancellationToken cancellationToken = default);
    }
}
