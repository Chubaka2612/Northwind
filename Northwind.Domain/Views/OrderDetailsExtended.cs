using System;
using System.Collections.Generic;
using Northwind.Domain.Entities;

namespace Northwind.Domain.Views;

public partial class OrderDetailsExtended : Entity
{
    public int OrderId { get; set; }

    public int ProductId { get; set; }

    public string ProductName { get; set; }

    public decimal UnitPrice { get; set; }

    public short Quantity { get; set; }

    public float Discount { get; set; }

    public decimal? ExtendedPrice { get; set; }
}
