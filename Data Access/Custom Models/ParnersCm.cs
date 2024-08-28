using Data_Access.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Access.Custom_Models
{
    public class PartnersCm
    {
        public List<Region> regions { get; set; }

        public List<Healthprofessionaltype> Professions { get; set; }

        public List<Partnersdata> Partnersdata { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9].{1,49}$", ErrorMessage = "Minimum 2 & Maximum 50 Characters Allowed")]
        public string? Street { get; set; }

        [Required(ErrorMessage = "City Is Required")]
        [RegularExpression(@"^([a-zA-Z]+)$", ErrorMessage = "Invalid City Name")]
        public string City { get; set; }

        [Required(ErrorMessage = "State Is Required")]
        public int? RegionId { get; set; }

        [Required(ErrorMessage = "Please Select Profession")]
        public int? SelectedhealthprofID { get; set; }

        [Required(ErrorMessage = "Zip Is Required")]
        [RegularExpression(@"^\d{6}(?:[-\s]\d{4})?$", ErrorMessage = "Invalid Zipcode")]
        public string Zip { get; set; }

        [Required(ErrorMessage = "Business Name Is Required")]
        [RegularExpression(@"^[a-zA-Z][a-zA-Z\s]{1,15}$", ErrorMessage = "BusinessName Accepts Only Alphabets ( Min. 2 & Max. 16 )")]
        public string BusinessName { get; set; }

        [Required(ErrorMessage = "Email Is Required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        public string Phonenumber { get; set; }

        [Required(ErrorMessage = "PhoneNumber Is Required")]
        public string PhonenumberWithoutCode { get; set; }

        [Required(ErrorMessage = "Fax Number Is Required")]
        [RegularExpression(@"\+?[0-9]{1,3}\s?[-]?\(?\d{3}\)?[-]?\s?[0-9]{3}[-]?\s?[0-9]{4}$", ErrorMessage = "Invalid Fax Number")]
        public string FAXNumber { get; set; }

        public string? BusinessContact { get; set; }

        [Required(ErrorMessage = "Business Contact Is Required")]
        public string? BusinessContactWithoutCode { get; set; }

        public int? vendorID { get; set; }
    }

    public class Partnersdata
    {
        public string VendorName { get; set; }

        public string ProfessionName { get; set; }

        public int? VendorId { get; set; }

        public string PhoneNo { get; set; }

        public string? FaxNo { get; set; }

        public string? VendorEmail { get; set; }

        public string Businesscontact { get; set; }
    }
}
