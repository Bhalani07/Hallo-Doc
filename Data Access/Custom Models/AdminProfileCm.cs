using Data_Access.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Access.Custom_Models
{
    public class AdminProfileCm
    {
        public int callId {  get; set; }

        public List<Role> Roles { get; set; }

        public List<Region> Regions { get; set; }

        public List<AdminregionTable> AdminRegions { get; set; }

        public int AspId { get; set; }

        public int AdminId { get; set; }

        [Required(ErrorMessage = "Username Is Required")]
        public string Username { get; set; }

        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]{6,}$", ErrorMessage = "Minimum six characters, at least one letter, one number and one special character is mandatory")]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Password Is Required")]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]{8,}$", ErrorMessage = "Minimum eight characters and at least one letter, one number and one special character is mandatory")]
        public string? CreateAdminPassword { get; set; }

        public short Status { get; set; }

        [Required(ErrorMessage = "Role Is Required")]
        public int RoleId { get; set; }

        [Required(ErrorMessage = "FirstName Is Required")]
        [RegularExpression(@"^[a-zA-Z][a-zA-Z\s]{1,15}$", ErrorMessage = "First Name Accepts Only Alphabets ( Min. 2 & Max. 16 )")]
        public string Firstname { get; set; }

        [RegularExpression(@"^[a-zA-Z][a-zA-Z\s]{1,15}$", ErrorMessage = "Last Name Accepts Only Alphabets ( Min. 2 & Max. 16 )")]
        public string? Lastname { get; set; }

        [Required(ErrorMessage = "Email Is Required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        [Compare("Email", ErrorMessage = "Email is Not Matched")]
        public string? ConfirmEmail { get; set; }

        public string Phonenumber { get; set; }

        [Required(ErrorMessage = "PhoneNumber Is Required")]
        public string PhoneWithoutCode { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9].{1,49}$", ErrorMessage = "Minimum 2 & Maximum 50 Characters Allowed")]
        public string Address1 { get; set; }

        public string Address2 { get; set; }

        [RegularExpression(@"^([a-zA-Z]+)$", ErrorMessage = "Invalid City Name")]
        public string? City { get; set; }

        [Required(ErrorMessage = "State Is Required")]
        public int RegionId { get; set; }

        [RegularExpression(@"^\d{6}(?:[-\s]\d{4})?$", ErrorMessage = "Invalid Zipcode")]
        public string? Zipcode { get; set; }

        public string Mobile {  get; set; }

        public string MobileWithoutCode {  get; set; }

    }

    public class AdminregionTable
    {
        public int Adminid { get; set; }

        public int Regionid { get; set; }

        public string Name { get; set; }

        public bool ExistsInTable { get; set; }
    }
}
