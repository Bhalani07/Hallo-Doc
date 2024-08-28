using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Data_Access.Models;

[Table("aspnetusers")]
public partial class Aspnetuser
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("username")]
    [StringLength(256)]
    public string? Username { get; set; }

    [Column("passwordhash", TypeName = "character varying")]
    public string? Passwordhash { get; set; }

    [Column("email")]
    [StringLength(256)]
    public string Email { get; set; } = null!;

    [Column("phonenumber", TypeName = "character varying")]
    public string? Phonenumber { get; set; }

    [Column("ip")]
    [StringLength(20)]
    public string? Ip { get; set; }

    [Column("createddate", TypeName = "timestamp without time zone")]
    public DateTime Createddate { get; set; }

    [Column("modifieddate", TypeName = "timestamp without time zone")]
    public DateTime? Modifieddate { get; set; }

    [Column("roleid")]
    public int Roleid { get; set; }

    [InverseProperty("Aspnetuser")]
    public virtual ICollection<Admin> AdminAspnetusers { get; set; } = new List<Admin>();

    [InverseProperty("CreatedbyNavigation")]
    public virtual ICollection<Admin> AdminCreatedbyNavigations { get; set; } = new List<Admin>();

    [InverseProperty("ModifiedbyNavigation")]
    public virtual ICollection<Admin> AdminModifiedbyNavigations { get; set; } = new List<Admin>();

    [InverseProperty("CreatedbyNavigation")]
    public virtual ICollection<Business> BusinessCreatedbyNavigations { get; set; } = new List<Business>();

    [InverseProperty("ModifiedbyNavigation")]
    public virtual ICollection<Business> BusinessModifiedbyNavigations { get; set; } = new List<Business>();

    [InverseProperty("CreatedbyNavigation")]
    public virtual ICollection<Payratebyprovider> PayratebyproviderCreatedbyNavigations { get; set; } = new List<Payratebyprovider>();

    [InverseProperty("ModifiedbyNavigation")]
    public virtual ICollection<Payratebyprovider> PayratebyproviderModifiedbyNavigations { get; set; } = new List<Payratebyprovider>();

    [InverseProperty("Aspnetuser")]
    public virtual ICollection<Physician> PhysicianAspnetusers { get; set; } = new List<Physician>();

    [InverseProperty("CreatedbyNavigation")]
    public virtual ICollection<Physician> PhysicianCreatedbyNavigations { get; set; } = new List<Physician>();

    [InverseProperty("ModifiedbyNavigation")]
    public virtual ICollection<Physician> PhysicianModifiedbyNavigations { get; set; } = new List<Physician>();

    [InverseProperty("CreatedbyNavigation")]
    public virtual ICollection<Requestnote> RequestnoteCreatedbyNavigations { get; set; } = new List<Requestnote>();

    [InverseProperty("ModifiedbyNavigation")]
    public virtual ICollection<Requestnote> RequestnoteModifiedbyNavigations { get; set; } = new List<Requestnote>();

    [ForeignKey("Roleid")]
    [InverseProperty("Aspnetusers")]
    public virtual Aspnetrole Role { get; set; } = null!;

    [InverseProperty("ModifiedbyNavigation")]
    public virtual ICollection<Shiftdetail> Shiftdetails { get; set; } = new List<Shiftdetail>();

    [InverseProperty("CreatedbyNavigation")]
    public virtual ICollection<Shift> Shifts { get; set; } = new List<Shift>();

    [InverseProperty("CreatedbyNavigation")]
    public virtual ICollection<Timesheet> TimesheetCreatedbyNavigations { get; set; } = new List<Timesheet>();

    [InverseProperty("ModifiedbyNavigation")]
    public virtual ICollection<Timesheet> TimesheetModifiedbyNavigations { get; set; } = new List<Timesheet>();

    [InverseProperty("CreatedbyNavigation")]
    public virtual ICollection<Timesheetdetailreimbursement> TimesheetdetailreimbursementCreatedbyNavigations { get; set; } = new List<Timesheetdetailreimbursement>();

    [InverseProperty("ModifiedbyNavigation")]
    public virtual ICollection<Timesheetdetailreimbursement> TimesheetdetailreimbursementModifiedbyNavigations { get; set; } = new List<Timesheetdetailreimbursement>();

    [InverseProperty("ModifiedbyNavigation")]
    public virtual ICollection<Timesheetdetail> Timesheetdetails { get; set; } = new List<Timesheetdetail>();

    [InverseProperty("Aspnetuser")]
    public virtual ICollection<User> UserAspnetusers { get; set; } = new List<User>();

    [InverseProperty("CreatedbyNavigation")]
    public virtual ICollection<User> UserCreatedbyNavigations { get; set; } = new List<User>();

    [InverseProperty("ModifiedbyNavigation")]
    public virtual ICollection<User> UserModifiedbyNavigations { get; set; } = new List<User>();

    [ForeignKey("Userid")]
    [InverseProperty("Users")]
    public virtual ICollection<Aspnetrole> Roles { get; set; } = new List<Aspnetrole>();
}
