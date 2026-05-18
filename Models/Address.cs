using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ToyShop.Models;

[Table("Address")]
[Index("IdAddress", Name = "My_Index_Address", IsUnique = true)]
public partial class Address
{
    [Key]
    [Column("ID_Address")]
    public int IdAddress { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string City { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string Street { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string House { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string? Apartment { get; set; }

    [Column("ID_User")]
    public int IdUser { get; set; }

    [ForeignKey("IdUser")]
    [InverseProperty("Addresses")]
    public virtual User IdUserNavigation { get; set; } = null!;
}
