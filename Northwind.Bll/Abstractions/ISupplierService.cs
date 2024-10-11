using Northwind.Domain.Entities;

namespace Northwind.Bll.Abstractions
{
    public interface ISupplierService
    {
        Task<IEnumerable<Supplier>> GetSuppliersAsync(CancellationToken cancellationToken);
    }
}
