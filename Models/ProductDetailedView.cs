using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ToyShop.Models;

[Keyless]
public partial class ProductDetailedView
{
    [Column("ID товара")]
    public int IdТовара { get; set; }

    [Column("Название товара")]
    [StringLength(200)]
    [Unicode(false)]
    public string НазваниеТовара { get; set; } = null!;

    [Column(TypeName = "money")]
    public decimal Цена { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string? Производитель { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string Категория { get; set; } = null!;

    [Column("Общий остаток на складах")]
    public int ОбщийОстатокНаСкладах { get; set; }
}
