using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ToyShop.Models;

[Table("Order")]
[Index("IdOrder", Name = "My_Index_Order", IsUnique = true)]
public partial class Order
{
    [Key]
    [Column("ID_Order")]
    public int IdOrder { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? RegistrationDate { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string Status { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string? PayMethod { get; set; }

    [Column("ID_User")]
    public int IdUser { get; set; }

    [Column("ID_Employee")]
    public int IdEmployee { get; set; }

    [ForeignKey("IdEmployee")]
    [InverseProperty("Orders")]
    public virtual Employee IdEmployeeNavigation { get; set; } = null!;

    [ForeignKey("IdUser")]
    [InverseProperty("Orders")]
    public virtual User IdUserNavigation { get; set; } = null!;

    [InverseProperty("IdOrderNavigation")]
    public virtual ICollection<ProductOrder> ProductOrders { get; set; } = new List<ProductOrder>();
}
