using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ToyShop.Models;

[Table("Product")]
[Index("IdProduct", Name = "My_Index_Product", IsUnique = true)]
public partial class Product
{
    [Key]
    [Column("ID_Product")]
    public int IdProduct { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string Name { get; set; } = null!;

    [Column(TypeName = "money")]
    public decimal Cost { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string? Manufacturer { get; set; }

    [Column("ID_Provider")]
    public int IdProvider { get; set; }

    [Column("ID_Category")]
    public int IdCategory { get; set; }

    [InverseProperty("IdProductNavigation")]
    public virtual ICollection<BasketProduct> BasketProducts { get; set; } = new List<BasketProduct>();

    [ForeignKey("IdCategory")]
    [InverseProperty("Products")]
    public virtual Category IdCategoryNavigation { get; set; } = null!;

    [ForeignKey("IdProvider")]
    [InverseProperty("Products")]
    public virtual Provider IdProviderNavigation { get; set; } = null!;

    [InverseProperty("IdProductNavigation")]
    public virtual ICollection<ProductOrder> ProductOrders { get; set; } = new List<ProductOrder>();

    [InverseProperty("IdProductNavigation")]
    public virtual ICollection<WarehouseProduct> WarehouseProducts { get; set; } = new List<WarehouseProduct>();
}
