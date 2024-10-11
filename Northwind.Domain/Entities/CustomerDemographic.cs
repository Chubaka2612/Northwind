using System;
using System.Collections.Generic;

namespace Northwind.Domain.Entities;

public partial class CustomerDemographic : Entity
{
    public string CustomerTypeId { get; set; }

    public string CustomerDesc { get; set; }

    public virtual ICollection<Customer> Customers { get; set; } = new List<Customer>();
}
