using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ToyShop.Models;

[Table("Provider")]
[Index("IdProvider", Name = "My_Index_Provider", IsUnique = true)]
[Index("Inn", Name = "UQ__Provider__C490CCF55892045B", IsUnique = true)]
public partial class Provider
{
    [Key]
    [Column("ID_Provider")]
    public int IdProvider { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string Name { get; set; } = null!;

    [StringLength(20)]
    [Unicode(false)]
    public string Number { get; set; } = null!;

    [Column("INN")]
    [StringLength(12)]
    [Unicode(false)]
    public string? Inn { get; set; }

    [InverseProperty("IdProviderNavigation")]
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
