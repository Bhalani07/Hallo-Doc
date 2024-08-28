using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Assignment_Data.Models;

[Table("book")]
public partial class Book
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("bookname")]
    [StringLength(256)]
    public string Bookname { get; set; } = null!;

    [Column("author")]
    [StringLength(256)]
    public string? Author { get; set; }

    [Column("borrowerid")]
    public int? Borrowerid { get; set; }

    [Column("borrowername")]
    [StringLength(256)]
    public string? Borrowername { get; set; }

    [Column("dateofissue", TypeName = "timestamp without time zone")]
    public DateTime? Dateofissue { get; set; }

    [Column("city")]
    [StringLength(256)]
    public string? City { get; set; }

    [Column("genere")]
    [StringLength(256)]
    public string? Genere { get; set; }

    [Column("isdeleted")]
    public bool? Isdeleted { get; set; }

    [ForeignKey("Borrowerid")]
    [InverseProperty("Books")]
    public virtual Borrower? Borrower { get; set; }
}
