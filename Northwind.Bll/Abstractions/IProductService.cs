using Northwind.Domain.Entities;

namespace Northwind.Bll.Abstractions
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetProductsAsync(CancellationToken cancellationToken, int amount = 0);

        Task<Product> GetProductAsync(int id, CancellationToken cancellationToken);

        Task<int> AddProductAsync(Product product);

        Task<bool> UpdateProductAsync(Product product);

        Task<bool> DeleteProductAsync(Product product);

    }
}
