using System;
using System.Collections.Generic;
using Northwind.Domain.Entities;

namespace Northwind.Domain.Views;

public partial class OrderSubtotal : Entity
{
    public int OrderId { get; set; }

    public decimal? Subtotal { get; set; }
}
