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
    public class PatientDashboardCm
    {
        public List<DashboardData> dashboardData { get; set; }

        public List<DocumentData> documentData { get; set; }

        public IFormFile? Upload {  get; set; }

        public ProfileData? profileData { get; set; }

        public string? ConfirmationNumber { get; set; }
    }

    public class DashboardData
    {
        public int? AdminAspId { get; set; }

        public int? PhyAspId { get; set; }

        public int? PhysicianId { get; set; }

        public int RequestId { get; set; }

        public DateTime CreatedDate { get; set; }

        public int Status { get; set; }

        public int DocumentCount { get; set; }

        public string PhysicianName { get; set; }


    }

    public class DocumentData
    {
        public DateTime CreatedDate { get; set; }

        public string DocumentName { get; set; }

        public string UploderName { get; set; }

    }

    public class ProfileData
    {
        [RegularExpression(@"^[a-zA-Z][a-zA-Z\s]{1,15}$", ErrorMessage = "First Name Accepts Only Alphabets ( Min. 2 & Max. 16 )")]
        public string? FirstName { get; set; }

        [RegularExpression(@"^[a-zA-Z][a-zA-Z\s]{1,15}$", ErrorMessage = "Last Name Accepts Only Alphabets (  Min. 2 & Max. 16 )")]
        public string? LastName { get; set; }

        public DateTime BirthDate { get; set; }

        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string? Email { get; set; }

        public string? Phone { get; set; }

        [Required(ErrorMessage = "PhoneNumber Is Required")]
        public string PhoneWithoutCode { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9].{1,49}$", ErrorMessage = "Minimum 2 & Maximum 50 Characters Allowed")]
        public string? Street { get; set; }

        [RegularExpression(@"^([a-zA-Z]+)$", ErrorMessage = "Invalid City Name")]
        public string? City { get; set; }

        public int? RegionId { get; set; }

        [RegularExpression(@"^\d{6}(?:[-\s]\d{4})?$", ErrorMessage = "Invalid Zipcode")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "Zipcode should be of 6 Numbers")]
        public string? Zipcode { get; set; }

        public List<Region> Regions { get; set; }

    }

}