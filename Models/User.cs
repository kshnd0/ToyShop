using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ToyShop.Models;

[Table("User")]
[Index("IdUser", Name = "My_Index_User", IsUnique = true)]
[Index("Login", Name = "UQ__User__5E55825B3B434C1E", IsUnique = true)]
[Index("Email", Name = "UQ__User__A9D10534892B412F", IsUnique = true)]
public partial class User
{
    [Key]
    [Column("ID_User")]
    public int IdUser { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string Name { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string Lastname { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string? Patronymic { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string Login { get; set; } = null!;

    [StringLength(255)]
    [Unicode(false)]
    public string Password { get; set; } = null!;

    [StringLength(20)]
    [Unicode(false)]
    public string Number { get; set; } = null!;

    [StringLength(100)]
    [Unicode(false)]
    public string Email { get; set; } = null!;

    [StringLength(50)]
    public string? Role { get; set; }

    [InverseProperty("IdUserNavigation")]
    public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();

    [InverseProperty("IdUserNavigation")]
    public virtual Basket? Basket { get; set; }

    [InverseProperty("IdUserNavigation")]
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
