using Data_Access.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Access.Custom_Models
{
    public class FamilyRequestCm    
    {
        // *********** Family Details ************

        [Required (ErrorMessage = "Family FirstName Is Required")]
        [RegularExpression(@"^[a-zA-Z][a-zA-Z\s]{1,15}$", ErrorMessage = "Invalid Family First Name ( Min. 2 & Max. 16 )")]
        public string FamilyFName { get; set; }

        [RegularExpression(@"^[a-zA-Z][a-zA-Z\s]{1,15}$", ErrorMessage = "Invalid Family Last Name ( Min. 2 & Max. 16 )")]
        public string? FamilyLName { get; set; }

        public string? FamilyPhone { get; set; }

		[Required(ErrorMessage = "Family Phone Is Required")]
        public string FamilyPhoneWithoutCode { get; set;}

        [Required(ErrorMessage = "Family Email Is Required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string FamilyEmail { get; set; }

        [RegularExpression(@"^[a-zA-Z][a-zA-Z\s]{1,15}$", ErrorMessage = "Family Relation Accepts Only Alphabets ( Min. 2 & Max. 16 )")]
        public string? FamilyRelation { get; set; }

        // *********** Patient Details ************

        [RegularExpression(@"^[a-zA-Z][a-zA-Z\s]{1,150}$", ErrorMessage = "Symptoms Accepts Only Alphabets ( Min. 2 & Max. 150 )")]
        public string? Symptons { get; set; }

        [Required(ErrorMessage = "FirstName Is Required")]
        [RegularExpression(@"^[a-zA-Z][a-zA-Z\s]{1,15}$", ErrorMessage = "Invalid First Name ( Min. 2 & Max. 16 )")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "LastName Is Required")]
        [RegularExpression(@"^[a-zA-Z][a-zA-Z\s]{1,15}$", ErrorMessage = "Invalid Last Name ( Min. 2 & Max. 16 )")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "BirthDate Is Required")]
        public DateTime BirthDate { get; set; }

        [Required(ErrorMessage = "Email Is Required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        public string? Phone { get; set; }

        [Required(ErrorMessage = "Phone Is Required")]
        public string PhoneWithoutCode { get; set; }


        [RegularExpression(@"^[a-zA-Z0-9].{1,49}$", ErrorMessage = "Minimum 2 & Maximum 50 Characters Allowed")]
        public string? Street { get; set; }

        [RegularExpression(@"^([a-zA-Z]+)$", ErrorMessage = "Invalid City Name")]
        public string? City { get; set; }

        [Required(ErrorMessage = "State Is Required")]
        public int RegionId { get; set; }

        [RegularExpression(@"^\d{6}(?:[-\s]\d{4})?$", ErrorMessage = "Invalid Zipcode")]
        public string? Zipcode { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9].{1,15}$", ErrorMessage = "Minimum 2 & Maximum 16 Characters Allowed")]
        public string? Room { get; set; }

        public IFormFile? Upload { get; set; }

        public List<Region> Regions { get; set; }

    }
}
