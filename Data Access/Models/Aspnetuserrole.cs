using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Data_Access.Models;

[Keyless]
[Table("aspnetuserroles")]
public partial class Aspnetuserrole
{
    [Column("userid")]
    public int Userid { get; set; }

    [Column("roleid")]
    public int Roleid { get; set; }

    [ForeignKey("Roleid")]
    public virtual Aspnetrole Role { get; set; } = null!;

    [ForeignKey("Userid")]
    public virtual Aspnetuser User { get; set; } = null!;
}
