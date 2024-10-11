using System;
using System.Collections.Generic;

namespace Northwind.Domain.Entities;

public partial class Shipper : Entity
{
    public int ShipperId { get; set; }

    public string CompanyName { get; set; }

    public string Phone { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
