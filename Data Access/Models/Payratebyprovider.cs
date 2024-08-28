using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Data_Access.Models;

[Table("payratebyprovider")]
public partial class Payratebyprovider
{
    [Key]
    [Column("payrateid")]
    public int Payrateid { get; set; }

    [Column("payratecategoryid")]
    public int Payratecategoryid { get; set; }

    [Column("physicianid")]
    public int Physicianid { get; set; }

    [Column("payrate")]
    [Precision(8, 3)]
    public decimal Payrate { get; set; }

    [Column("createdby")]
    public int Createdby { get; set; }

    [Column("createddate", TypeName = "timestamp without time zone")]
    public DateTime? Createddate { get; set; }

    [Column("modifiedby")]
    public int? Modifiedby { get; set; }

    [Column("modifieddate", TypeName = "timestamp without time zone")]
    public DateTime? Modifieddate { get; set; }

    [ForeignKey("Createdby")]
    [InverseProperty("PayratebyproviderCreatedbyNavigations")]
    public virtual Aspnetuser CreatedbyNavigation { get; set; } = null!;

    [ForeignKey("Modifiedby")]
    [InverseProperty("PayratebyproviderModifiedbyNavigations")]
    public virtual Aspnetuser? ModifiedbyNavigation { get; set; }

    [ForeignKey("Payratecategoryid")]
    [InverseProperty("Payratebyproviders")]
    public virtual Payratecategory Payratecategory { get; set; } = null!;

    [ForeignKey("Physicianid")]
    [InverseProperty("Payratebyproviders")]
    public virtual Physician Physician { get; set; } = null!;
}
