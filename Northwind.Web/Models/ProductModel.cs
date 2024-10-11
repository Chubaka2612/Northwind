using Northwind.Domain.Entities;

namespace Northwind.Web.Models
{
    public class ProductModel
    {
        public Product Product { get; set; }

        public IEnumerable<Category> Categories { get; set; }

        public IEnumerable<Supplier> Suppliers { get; set; }
    }
}
