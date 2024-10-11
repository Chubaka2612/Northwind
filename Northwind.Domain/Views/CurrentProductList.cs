using System;
using System.Collections.Generic;
using Northwind.Domain.Entities;

namespace Northwind.Domain.Views;

public partial class CurrentProductList : Entity
{
    public int ProductId { get; set; }

    public string ProductName { get; set; }
}
