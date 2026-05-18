using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ToyShop.Models;

[Table("Basket_Product")]
[Index("IdBasketProduct", Name = "My_Index_Basket_Product", IsUnique = true)]
public partial class BasketProduct
{
    [Key]
    [Column("ID_Basket_Product")]
    public int IdBasketProduct { get; set; }

    [Column("ID_Basket")]
    public int IdBasket { get; set; }

    [Column("ID_Product")]
    public int IdProduct { get; set; }

    public int Amount { get; set; }

    [ForeignKey("IdBasket")]
    [InverseProperty("BasketProducts")]
    public virtual Basket IdBasketNavigation { get; set; } = null!;

    [ForeignKey("IdProduct")]
    [InverseProperty("BasketProducts")]
    public virtual Product IdProductNavigation { get; set; } = null!;
}
