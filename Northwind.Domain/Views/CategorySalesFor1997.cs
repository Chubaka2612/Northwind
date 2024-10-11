using System;
using System.Collections.Generic;
using Northwind.Domain.Entities;

namespace Northwind.Domain.Views;

public partial class CategorySalesFor1997 : Entity
{
    public string CategoryName { get; set; }

    public decimal? CategorySales { get; set; }
}
