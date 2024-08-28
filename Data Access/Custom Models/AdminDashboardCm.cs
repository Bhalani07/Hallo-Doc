using Data_Access.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Access.Custom_Models
{
    public class AdminDashboardCm
    {
        public int sessionId {  get; set; }

        public int callId { get; set; }

        public List<RequestListAdminDash> RequestListAdminDash { get; set; }

        public int StatusForName { get; set; }

        public string reqTypeId { get; set; }

        public AdminViewCaseData adminViewCaseData { get; set; }

        public List<Region> regions { get; set; }

        public ViewNotesData viewNotesData { get; set; }

        public CountRequest countRequest { get; set; }


        // *************** Send Agreement ***************

        public SendLink? sendLink { get; set; }
    }

    public class RequestListAdminDash
    {
        public int? UserId { get; set; }

        public int? PatientAspId { get; set; }

        public int? AdminAspId { get; set; }

        public int? PhyAspId { get; set; }

        public int? PhysicianId { get; set; }       

        public string Name { get; set; }

        public string DateOfBirth { get; set; }

        public string Requestor { get; set; }

        public string RequestDate { get; set; } 

        public string Days { get; set; }

        public string Hours { get; set; }

        public string Phone { get; set; }

        public string Mobile {  get; set; }

        public string? Notes { get; set; }

        public string? Address { get; set; }

        public string ChatWith { get; set; } 

        public string Physician { get; set; }

        public DateTime? DateOfService { get; set; }

        public string Region { get; set; }

        public int Status { get; set; }

        public int RequestTypeId { get; set; }

        public string Email { get; set; }

        public int? RequestId { get; set; }

        public int callType { get; set; }

        public bool isFinalized { get; set; }

    }

    public class AdminViewCaseData
    {
        public int UserId { get; set; }

        public string ConfirmationNumber { get; set; }

        public string Symptons { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime DateOfBirth { get; set; }

        public string Mobile { get; set; }

        [Required(ErrorMessage = "PhoneNumber Is Required")]
        public string MobileWithoutCode { get; set; }

        [Required(ErrorMessage = "Email Is Required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        public string Region { get; set; }

        public string BusinessAddress { get; set; }

        public string Room { get; set; }

        public int RequestTypeId { get; set; }

        public int RequestId { get; set; }
    }

    public class ViewNotesData
    {
        public int RequestId { get; set; }

        [Required(ErrorMessage = "AdminNotes Is Required")]
        [RegularExpression(@"^[a-zA-Z\s]{2,150}$", ErrorMessage = "AdminNotes Accepts Only Alphabets ( Min. 2 & Max. 150 )")]
        public string AdminNotes { get; set; }

        public string? PhysicianNotes { get; set; }

        public List<Requeststatuslog> TransferNotes { get; set; }
    }

    public class CountRequest
    {
        public int NewRequest { get; set; }

        public int PendingRequest { get; set; }

        public int ActiveRequest { get; set; }

        public int ConcludeRequest { get; set; }

        public int ToCloseRequest { get; set; }

        public int UnpaidRequest { get; set; }
    }

    public class SendLink
    {
        [Required(ErrorMessage = "First Name Is Required")]
        [RegularExpression(@"^[a-zA-Z][a-zA-Z\s]{1,15}$", ErrorMessage = "Only alphabets max of 16 characters")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name Is Required")]
        [RegularExpression(@"^[a-zA-Z][a-zA-Z\s]{1,15}$", ErrorMessage = "Only alphabets max of 16 characters")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email Is Required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        public string? Phone { get; set; }

        [Required(ErrorMessage = "PhoneNumber Is Required")]
        public string PhoneWithoutCode { get; set; }
    }

}
