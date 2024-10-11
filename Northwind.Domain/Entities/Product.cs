using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Northwind.Domain.Entities;

public partial class Product : Entity
{
    public int ProductId { get; set; }

    [Required(ErrorMessage = "Product Name is required")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Product Name can't exceed 100 characters and be less 3 characters")]
    public string ProductName { get; set; }


    [Required(ErrorMessage = "Supplier is required")]
    public int? SupplierId { get; set; }


    [Required(ErrorMessage = "Category is required")]
    public int? CategoryId { get; set; }

    [DataType(DataType.Text)]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Quantity Per Unit can't exceed 50 characters and be less 3 characters")]
    public string QuantityPerUnit { get; set; }


    [Range(0, double.MaxValue, ErrorMessage = "Unit Price must be a positive number")]
    public decimal? UnitPrice { get; set; }


    [Range(0, short.MaxValue, ErrorMessage = "Units In Stock must be a positive number")]
    public short? UnitsInStock { get; set; }


    [Range(0, short.MaxValue, ErrorMessage = "Units On Order must be a positive number")]
    public short? UnitsOnOrder { get; set; }


    [Range(0, short.MaxValue, ErrorMessage = "Reorder Level must be a positive number")]
    public short? ReorderLevel { get; set; }

    public bool Discontinued { get; set; }

    public virtual Category Category { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual Supplier Supplier { get; set; }
}
