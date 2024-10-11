using Northwind.Bll.Abstractions;
using Microsoft.EntityFrameworkCore;
using Northwind.Dal.Abstractions;
using Northwind.Domain.Entities;
using Microsoft.Extensions.Logging;


namespace Northwind.Bll.Services
{
    public class ProductService : BaseService, IProductService
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly IRepository<Product> _productRepository;


        public ProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _productRepository = _unitOfWork.Repository<Product>();

        }

        public async Task<int> AddProductAsync(Product product)
        {
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product));
            }

            _productRepository.Add(product);
            await _unitOfWork.SaveChangesAsync();
            return product.ProductId;
        }


        public async Task<bool> UpdateProductAsync(Product product)
        {
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product));
            }

            var dbProduct = _productRepository.Queryable()
                .AsNoTracking()
                .Include(p => p.Supplier).Include(p => p.Category).FirstOrDefaultAsync(c => c.ProductId == product.ProductId);
            if (dbProduct == null)
            {
                return false;
            }

            _productRepository.Update(product);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<Product> GetProductAsync(int id, CancellationToken cancellationToken)
        {
            return await _productRepository.Queryable().Include(p => p.Supplier).Include(p => p.Category).FirstOrDefaultAsync(c => c.ProductId == id);
        }

        public async Task<IEnumerable<Product>> GetProductsAsync(CancellationToken cancellationToken, int amount = 0)
        {
            var products = amount == 0
                ? await _productRepository.Queryable(new[]
                { nameof(Product.Supplier), nameof(Product.Category) }).ToArrayAsync()
                : await _productRepository.Queryable().Take(amount).Include(p => p.Supplier).Include(p => p.Category).ToArrayAsync();

            return products;
        }
    }
}
