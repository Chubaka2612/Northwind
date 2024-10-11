using System;
using System.Collections.Generic;

namespace Northwind.Domain.Entities;

public partial class Region : Entity
{
    public int RegionId { get; set; }

    public string RegionDescription { get; set; }

    public virtual ICollection<Territory> Territories { get; set; } = new List<Territory>();
}
