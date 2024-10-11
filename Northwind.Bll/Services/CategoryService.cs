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
    }
}
