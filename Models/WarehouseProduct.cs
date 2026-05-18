using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ToyShop.Models;

[Table("Warehouse_Product")]
[Index("IdWarehouseProduct", Name = "My_Index_Warehouse_Product", IsUnique = true)]
public partial class WarehouseProduct
{
    [Key]
    [Column("ID_Warehouse_Product")]
    public int IdWarehouseProduct { get; set; }

    [Column("ID_Warehouse")]
    public int IdWarehouse { get; set; }

    [Column("ID_Product")]
    public int IdProduct { get; set; }

    public int Amount { get; set; }

    [ForeignKey("IdProduct")]
    [InverseProperty("WarehouseProducts")]
    public virtual Product IdProductNavigation { get; set; } = null!;

    [ForeignKey("IdWarehouse")]
    [InverseProperty("WarehouseProducts")]
    public virtual Warehouse IdWarehouseNavigation { get; set; } = null!;
}
