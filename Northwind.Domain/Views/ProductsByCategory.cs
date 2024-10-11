using System;
using System.Collections.Generic;
using Northwind.Domain.Entities;

namespace Northwind.Domain.Views;

public partial class ProductsByCategory : Entity
{
    public string CategoryName { get; set; }

    public string ProductName { get; set; }

    public string QuantityPerUnit { get; set; }

    public short? UnitsInStock { get; set; }

    public bool Discontinued { get; set; }
}
