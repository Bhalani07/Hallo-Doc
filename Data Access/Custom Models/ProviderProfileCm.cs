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
    public class ProviderProfileCm
    {
        public int callId {  get; set; }

        public List<Region> Regions { get; set; }

        public List<Role> Roles { get; set; }

        public List<PhysicianRegionTable> PhysicianRegionTables { get; set; }

        public int AspId { get; set; }

        public int PhysicianId { get; set; }

        [Required(ErrorMessage = "Request Message Is Required")]
        [RegularExpression(@"^[a-zA-Z][a-zA-Z\s]{1,150}$", ErrorMessage = "Request Notes Accepts Only Alphabets ( Min. 2 & Max. 150 )")]
        public string? RequestMessage { get; set; } 

        [Required(ErrorMessage = "Username Is Required")]
        [RegularExpression(@"^[a-zA-Z][a-zA-Z\s]{1,15}$", ErrorMessage = "Username Accepts Only Alphabets ( Min. 2 & Max. 16 )")]
        public string? Username { get; set; }

        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]{8,}$", ErrorMessage = "Minimum eight characters and at least one letter, one number and one special character is mandatory")]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Password Is Required")]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]{8,}$", ErrorMessage = "Minimum eight characters and at least one letter, one number and one special character is mandatory")]
        public string? CreatePhyPassword { get; set; }

        [Required(ErrorMessage = "Role Is Required")]
        public int? RoleId { get; set; }

        public short? Status { get; set; }

        [Required(ErrorMessage = "FirstName Is Required")]
        [RegularExpression(@"^[a-zA-Z]{1,15}$", ErrorMessage = "First Name Accepts Only Alphabets ( Min. 2 & Max. 16 )")]
        public string? FirstName { get; set; }

        [RegularExpression(@"^[a-zA-Z]{1,15}$", ErrorMessage = "Last Name Accepts Only Alphabets ( Min. 2 & Max. 16 )")]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "Email Is Required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string? Email { get; set; }

        public string Phonenumber { get; set; }

        [Required(ErrorMessage = "PhoneNumber Is Required")]
        public string PhoneWithoutCode { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9].{1,15}$", ErrorMessage = "Medical Licence Accepts Only Alphabets ( Min. 2 & Max. 16 )")]
        public string? MedicalLicense { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9].{1,15}$", ErrorMessage = "NPI Number Accepts Only Alphabets ( Min. 2 & Max. 16 )")]
        public string? NPINumber { get; set; }

        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string? SyncEmail { get; set; }

        [Required(ErrorMessage = "Adress Is Required")]
        [RegularExpression(@"^[a-zA-Z0-9].{1,49}$", ErrorMessage = "Minimum 2 & Maximum 50 Characters Allowed")]
        public string? Address1 { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9].{1,49}$", ErrorMessage = "Minimum 2 & Maximum 50 Characters Allowed")]
        public string? Address2 { get; set; }

        [RegularExpression(@"^[a-zA-Z\s]*$", ErrorMessage = "City Accepts Only Text Characters")]
        public string? City { get; set; }

        [Required(ErrorMessage = "State Is Required")]
        public int? RegionId { get; set; }

        [Required(ErrorMessage = "Zipcode Is Required")]
        [RegularExpression(@"^\d{6}(?:[-\s]\d{4})?$", ErrorMessage = "Invalid Zipcode")]
        public string? Zipcode { get; set; }

        public decimal? Latitude { get; set; }

        public decimal? Longitude { get; set; }

        public string? AltPhone { get; set; }

        public string? AltPhoneWithoutCode { get; set; }

        [Required(ErrorMessage = "Business Name Is Required")]
        [RegularExpression(@"^[a-zA-Z][a-zA-Z\s]{1,15}$", ErrorMessage = "BusinessName Accepts Only Alphabets ( Min. 2 & Max. 16 )")]
        public string? BusinessName { get; set; }

        [Required(ErrorMessage = "Business Name Is Required")]
        [RegularExpression(@"^[a-zA-Z].{1,19}$", ErrorMessage = "Website Accepts Only Alphabets ( Min. 2 & Max. 20 )")]
        public string? BusinessWebsite { get; set; }

        public IFormFile? Photo { get; set; }

        public string? PhotoValue { get; set; }

        public IFormFile? Signature { get; set; }

        public string? SignatureValue { get; set; }

        [RegularExpression(@"^[a-zA-Z][a-zA-Z\s]{1,150}$", ErrorMessage = "Admin Notes Accepts Only Alphabets ( Min. 2 & Max. 150 )")]
        public string? AdminNotes { get; set; }

        public IFormFile? ContractorAgreement { get; set; }

        public bool IsContractorAgreement { get; set; }

        public IFormFile? BackgroundCheck { get; set; }

        public bool IsBackgroundCheck { get; set; }

        public IFormFile? HIPAA { get; set; }

        public bool IsHIPAA { get; set; }

        public IFormFile? NonDisclosure { get; set; }

        public bool IsNonDisclosure { get; set; }

        public IFormFile? LicenseDocument { get; set; }

        public bool IsLicenseDocument { get; set; }
    }

    public class PhysicianRegionTable
    {
        public int PhysicianId { get; set; }

        public int Regionid { get; set; }

        public string Name { get; set; }

        public bool ExistsInTable { get; set; }
    }
}
