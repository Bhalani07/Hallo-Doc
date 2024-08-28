using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Access.Custom_Models
{
    public class AdminCloseCaseCm
    {
        public int statusForName {  get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string ConfirmationNumber { get; set; }

        public DateTime BirthDate { get; set; }

        public string? Phone { get; set; }

        [Required(ErrorMessage = "PhoneNumber Is Required")]
        public string PhoneWithoutCode { get; set; }


        [Required(ErrorMessage = "Email Is Required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        public int RequestId { get; set; }

        public List<Documents> file { get; set; }
    }

    public class Documents
    {
        public int requestWiseFileId { get; set; }

        public int requestId { get; set; }

        public string documentName { get; set; }

        public string uploadDate { get; set; }
    }
}
