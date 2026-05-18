using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ToyShop.Models;

[Table("Event")]
[Index("IdEvent", Name = "My_Index_Event", IsUnique = true)]
public partial class Event
{
    [Key]
    [Column("ID_Event")]
    public int IdEvent { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string? Meaning { get; set; }

    [InverseProperty("IdEventNavigation")]
    public virtual ICollection<CategoryEvent> CategoryEvents { get; set; } = new List<CategoryEvent>();
}
