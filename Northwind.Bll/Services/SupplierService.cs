using Northwind.Bll.Abstractions;
using Northwind.Dal.Abstractions;
using Northwind.Domain.Entities;

namespace Northwind.Bll.Services
{
    public class SupplierService : BaseService, ISupplierService
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly IRepository<Supplier> _supplierRepository;

        public SupplierService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _supplierRepository = _unitOfWork.Repository<Supplier>();
        }

        public async Task<IEnumerable<Supplier>> GetSuppliersAsync(CancellationToken cancellationToken)
        {
            var filter = _supplierRepository.Queryable();
            return await GetOrderedResult(
                filter,
                e => e.CompanyName,
                cancellationToken);
        }
    }
}
