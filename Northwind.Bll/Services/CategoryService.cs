using Microsoft.EntityFrameworkCore;
using Northwind.Bll.Abstractions;
using Northwind.Dal.Abstractions;
using Northwind.Domain.Entities;

namespace Northwind.Bll.Services
{
    public class CategoryService : BaseService, ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly IRepository<Category> _categoryRepository;

        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _categoryRepository = _unitOfWork.Repository<Category>();

        }

        public async Task<IEnumerable<Category>> GetCategoriesAsync( CancellationToken cancellationToken)
        {
            var filter = _categoryRepository.Queryable();
            return await GetOrderedResult(
                filter,
                e => e.CategoryName,
                cancellationToken);
        }

        public async Task<Category> GetCategoryAsync(int id, CancellationToken cancellationToken)
        {
            return await _categoryRepository.Queryable().Include(p => p.Products).FirstOrDefaultAsync(c => c.CategoryId == id);
        }

        public async Task<bool> UpdateCategoryImageAsync(int id, byte[] imageBytes, CancellationToken cancellationToken = default)
        {
            var category = await _categoryRepository.FindAsync(cancellationToken, id);
            if (category == null)
            {
                throw new KeyNotFoundException($"Category with ID {id} not found.");
            }

            category.Picture = imageBytes;

            _categoryRepository.Update(category);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
