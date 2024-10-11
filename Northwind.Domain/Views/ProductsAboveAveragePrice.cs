using System;
using System.Collections.Generic;
using Northwind.Domain.Entities;

namespace Northwind.Domain.Views;

public partial class ProductsAboveAveragePrice : Entity
{
    public string ProductName { get; set; }

    public decimal? UnitPrice { get; set; }
}
