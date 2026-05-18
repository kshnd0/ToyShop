using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ToyShop.Models;

[Table("Employee")]
[Index("IdEmployee", Name = "My_Index_Employee", IsUnique = true)]
[Index("Email", Name = "UQ__Employee__A9D105340F96718A", IsUnique = true)]
public partial class Employee
{
    [Key]
    [Column("ID_Employee")]
    public int IdEmployee { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string Name { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string Lastname { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string? Patronymic { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string Number { get; set; } = null!;

    [StringLength(100)]
    [Unicode(false)]
    public string Email { get; set; } = null!;

    [Column("ID_Warehouse")]
    public int? IdWarehouse { get; set; }

    [ForeignKey("IdWarehouse")]
    [InverseProperty("Employees")]
    public virtual Warehouse? IdWarehouseNavigation { get; set; }

    [InverseProperty("IdEmployeeNavigation")]
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
