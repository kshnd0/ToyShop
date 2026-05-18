using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ToyShop.Models;

[Table("Basket")]
[Index("IdBasket", Name = "My_Index_Basket", IsUnique = true)]
[Index("IdUser", Name = "UQ__Basket__ED4DE4430F4172C4", IsUnique = true)]
public partial class Basket
{
    [Key]
    [Column("ID_Basket")]
    public int IdBasket { get; set; }

    [Column("ID_User")]
    public int IdUser { get; set; }

    [InverseProperty("IdBasketNavigation")]
    public virtual ICollection<BasketProduct> BasketProducts { get; set; } = new List<BasketProduct>();

    [ForeignKey("IdUser")]
    [InverseProperty("Basket")]
    public virtual User IdUserNavigation { get; set; } = null!;
}
