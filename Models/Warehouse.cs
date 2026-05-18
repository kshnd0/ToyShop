using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ToyShop.Models;

[Table("Warehouse")]
[Index("IdWarehouse", Name = "My_Index_Warehouse", IsUnique = true)]
public partial class Warehouse
{
    [Key]
    [Column("ID_Warehouse")]
    public int IdWarehouse { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string Name { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string City { get; set; } = null!;

    [StringLength(100)]
    [Unicode(false)]
    public string Street { get; set; } = null!;

    [StringLength(10)]
    [Unicode(false)]
    public string House { get; set; } = null!;

    [InverseProperty("IdWarehouseNavigation")]
    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

    [InverseProperty("IdWarehouseNavigation")]
    public virtual ICollection<WarehouseProduct> WarehouseProducts { get; set; } = new List<WarehouseProduct>();
}
