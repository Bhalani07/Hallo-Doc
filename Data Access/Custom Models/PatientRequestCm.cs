using Data_Access.Custom_Models;
using Data_Access.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Data_Access.Custom_Models
{
	public class PatientRequestCm
    {
        // *********** Patient Details ************

        [RegularExpression(@"^[a-zA-Z][a-zA-Z\s]{1,150}$", ErrorMessage = "Symptoms Accepts Only Alphabets ( Min. 2 & Max. 150 )")]
        public string? Symptons { get; set; }

		[Required (ErrorMessage = "FirstName Is Required")]
        [RegularExpression(@"^[a-zA-Z][a-zA-Z\s]{1,15}$", ErrorMessage = "First Name Accepts Only Alphabets ( Min. 2 & Max. 16 )")]
		public string FirstName { get; set; }

        [RegularExpression(@"^[a-zA-Z][a-zA-Z\s]{1,15}$", ErrorMessage = "Last Name Accepts Only Alphabets ( Min. 2 & Max. 16 )")]
		public string? LastName { get; set; }

        [Required(ErrorMessage = "BirthDate Is Required")]
        public DateTime BirthDate { get; set; }

		[Required (ErrorMessage = "Email Is Required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]{6,}$", ErrorMessage = "Minimum six characters, at least one letter, one number and one special character is mandatory")]
        public string? Passwordhash { get; set; }

        [Compare("Passwordhash", ErrorMessage = "Password is Not Matched")]
        public string? ConfirmPassword { get; set; }

        public string? Phone { get; set; }

        [Required(ErrorMessage = "PhoneNumber Is Required")]
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
