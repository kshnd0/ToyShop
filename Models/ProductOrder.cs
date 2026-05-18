using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ToyShop.Models;

[Table("Product_Order")]
[Index("IdProductOrder", Name = "My_Index_Product_Order", IsUnique = true)]
public partial class ProductOrder
{
    [Key]
    [Column("ID_Product_Order")]
    public int IdProductOrder { get; set; }

    [Column("ID_Order")]
    public int IdOrder { get; set; }

    [Column("ID_Product")]
    public int IdProduct { get; set; }

    public int Amount { get; set; }

    [ForeignKey("IdOrder")]
    [InverseProperty("ProductOrders")]
    public virtual Order IdOrderNavigation { get; set; } = null!;

    [ForeignKey("IdProduct")]
    [InverseProperty("ProductOrders")]
    public virtual Product IdProductNavigation { get; set; } = null!;
}
