using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ToyShop.Models;

[Table("Category_Event")]
[Index("IdCategoryEvent", Name = "My_Index_Category_Event", IsUnique = true)]
public partial class CategoryEvent
{
    [Key]
    [Column("ID_Category_Event")]
    public int IdCategoryEvent { get; set; }

    [Column("ID_Category")]
    public int IdCategory { get; set; }

    [Column("ID_Event")]
    public int IdEvent { get; set; }

    [Column(TypeName = "decimal(5, 2)")]
    public decimal DiscountPercent { get; set; }

    [ForeignKey("IdCategory")]
    [InverseProperty("CategoryEvents")]
    public virtual Category IdCategoryNavigation { get; set; } = null!;

    [ForeignKey("IdEvent")]
    [InverseProperty("CategoryEvents")]
    public virtual Event IdEventNavigation { get; set; } = null!;
}
