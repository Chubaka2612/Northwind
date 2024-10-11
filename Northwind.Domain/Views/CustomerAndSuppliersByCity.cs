using System;
using System.Collections.Generic;
using Northwind.Domain.Entities;

namespace Northwind.Domain.Views;

public partial class CustomerAndSuppliersByCity : Entity
{
    public string City { get; set; }

    public string CompanyName { get; set; }

    public string ContactName { get; set; }

    public string Relationship { get; set; }
}
