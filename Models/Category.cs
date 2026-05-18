using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ToyShop.Models;

[Table("Category")]
[Index("IdCategory", Name = "My_Index_Category", IsUnique = true)]
public partial class Category
{
    [Key]
    [Column("ID_Category")]
    public int IdCategory { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string Name { get; set; } = null!;

    [InverseProperty("IdCategoryNavigation")]
    public virtual ICollection<CategoryEvent> CategoryEvents { get; set; } = new List<CategoryEvent>();

    [InverseProperty("IdCategoryNavigation")]
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
