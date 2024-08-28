using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Data_Access.Models;

[Table("timesheetdetailreimbursement")]
public partial class Timesheetdetailreimbursement
{
    [Key]
    [Column("timesheetdetailreimbursementid")]
    public int Timesheetdetailreimbursementid { get; set; }

    [Column("timesheetdetailid")]
    public int Timesheetdetailid { get; set; }

    [Column("itemname")]
    [StringLength(500)]
    public string? Itemname { get; set; }

    [Column("amount")]
    public int? Amount { get; set; }

    [Column("bill")]
    [StringLength(500)]
    public string? Bill { get; set; }

    [Column("isdeleted")]
    public bool? Isdeleted { get; set; }

    [Column("createdby")]
    public int Createdby { get; set; }

    [Column("createddate", TypeName = "timestamp without time zone")]
    public DateTime? Createddate { get; set; }

    [Column("modifiedby")]
    public int? Modifiedby { get; set; }

    [Column("modifieddate", TypeName = "timestamp without time zone")]
    public DateTime? Modifieddate { get; set; }

    [Column("timesheetdate")]
    public DateOnly? Timesheetdate { get; set; }

    [ForeignKey("Createdby")]
    [InverseProperty("TimesheetdetailreimbursementCreatedbyNavigations")]
    public virtual Aspnetuser CreatedbyNavigation { get; set; } = null!;

    [ForeignKey("Modifiedby")]
    [InverseProperty("TimesheetdetailreimbursementModifiedbyNavigations")]
    public virtual Aspnetuser? ModifiedbyNavigation { get; set; }

    [ForeignKey("Timesheetdetailid")]
    [InverseProperty("Timesheetdetailreimbursements")]
    public virtual Timesheetdetail Timesheetdetail { get; set; } = null!;
}
