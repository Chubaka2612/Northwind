using System;
using System.Collections.Generic;
using Northwind.Domain.Entities;

namespace Northwind.Domain.Views;

public partial class QuarterlyOrder : Entity
{
    public string CustomerId { get; set; }

    public string CompanyName { get; set; }

    public string City { get; set; }

    public string Country { get; set; }
}
