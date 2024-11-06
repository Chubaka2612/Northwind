using System;
using System.Collections.Generic;

namespace Northwind.Domain.Entities;

public partial class Category : Entity
{
    public int CategoryId { get; set; }

    public string CategoryName { get; set; }

    public string Description { get; set; }

    public byte[] Picture { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    public string Base64String
    {
        get
        {
            if (Picture.Length == 10746)
            {
                return Convert.ToBase64String(Picture, 78, Picture.Length - 78);
            }
            else
            {
                return Convert.ToBase64String(Picture);
            }
        }
    }
}
