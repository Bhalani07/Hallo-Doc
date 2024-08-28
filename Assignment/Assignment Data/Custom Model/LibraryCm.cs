using Assignment_Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment_Data.Custom_Model
{
    public class LibraryCm
    {
        public int? BookId { get; set; }

        [Required(ErrorMessage = "BookName Is Required")]
        [RegularExpression(@"^[a-zA-Z][a-zA-Z0-9\s]{1,49}$", ErrorMessage = "Alphabets within Range ( 2-50 )")]
        public string? BookName { get; set; }

        [Required(ErrorMessage = "Author Is Required")]
        [RegularExpression(@"^[a-zA-Z][a-zA-Z0-9\s]{1,49}$", ErrorMessage = "Alphabets within Range ( 2-50 )")]
        public string? Author { get; set; }

        [Required(ErrorMessage = "Borrower Is Required")]
        [RegularExpression(@"^[a-zA-Z][a-zA-Z0-9\s]{1,49}$", ErrorMessage = "Alphabets within Range ( 2-50 )")]
        public string? BorrowerName { get; set; }

        [Required(ErrorMessage = "IssueDate Is Required")]
        public string? IssueDate { get; set; }

        [Required(ErrorMessage = "Genere Is Required")]
        public string? Genere { get; set; }

        [Required(ErrorMessage = "City Is Required")]
        [RegularExpression(@"^([a-zA-Z]+)$", ErrorMessage = "Invalid City Name")]
        public string? City { get; set; }

        public List<Book>? LibraryList { get; set; }
    }

}
