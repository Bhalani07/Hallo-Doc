using Data_Access.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Data_Access.Custom_Models
{
    public class ModalCm
    {
        public int callId {  get; set; }

        public int StatusForName { get; set; }

        public int ModalId { get; set; }


        // *************** Provider Transfer Case ***************
        public TransferCaseModal? transferCaseModal { get; set; }


        // *************** Cancel Case ***************
        public List<Casetag> casetags { get; set; }

        public CancelCaseModal? cancelCaseModal {  get; set; }


        // *************** Asign Case ***************
        public AsignCaseModal? asignCaseModal { get; set;}

        public List<Region> regions { get; set; }

        public List<Physician> physicians { get; set; }


        // *************** Block Case ***************
        public BlockCaseModal? blockCaseModal { get; set; }


        // *************** Clear Case ***************
        public int RequestId { get; set; }


        // *************** Send Agreement ***************
        public SendAgreementModal? sendAgreementModal { get; set; }


        // *************** Request DTY Support ***************

        [Required(ErrorMessage = "Message is Required")]
        [RegularExpression(@"^[a-zA-Z][a-zA-Z\s]{2,150}$", ErrorMessage = "Transfer Description Accepts Only Alphabets ( Min. 2 & Max. 150 )")]
        public string? RequestSupportMessage { get; set; }

    }

    public class CancelCaseModal
    {
        public int RequestId { get; set; }

        public string Name { get; set; }

        [Required(ErrorMessage = "Cancellation note is Required")]
        [RegularExpression(@"^[a-zA-Z][a-zA-Z\s]{2,150}$", ErrorMessage = "Cancellation Notes Accepts Only Alphabets ( Min. 2 & Max. 150 )")]
        public string CancellationNotes { get; set; }

        [Required(ErrorMessage = "Cancellation reason is Required")]
        public int CasetagId { get; set; }
    }

    public class AsignCaseModal
    {
        public int ModalId { get; set; }

        public int RequestId { get; set; }

        [Required(ErrorMessage = "Region is Required")]
        public int RegionId { get; set; }

        [Required(ErrorMessage = "Physician is Required")]
        public int PhysicianId { get; set; }

        [Required(ErrorMessage = "Assign note is Required")]
        [RegularExpression(@"^[a-zA-Z][a-zA-Z\s]{2,150}$", ErrorMessage = "AsignNotes Accepts Only Alphabets ( Min. 2 & Max. 150 )")]
        public string AsignNotes { get; set; }

    }

    public class BlockCaseModal
    {
        public int RequestId { get; set; }

        public string Name { get; set; }

        [Required (ErrorMessage = "Block Reason is Required")]
        [RegularExpression(@"^[a-zA-Z][a-zA-Z\s]{2,150}$", ErrorMessage = "Block Reason Accepts Only Alphabets ( Min. 2 & Max. 150 )")]
        public string BlockReason { get; set;}
    }

    public class SendAgreementModal
    {
        public int RequestId { get; set;}

        public int RequestTypeId { get; set;}

        [Required(ErrorMessage = "Phone number is Required")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Email is Required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }
    }

    public class TransferCaseModal
    {
        public int RequestId { get; set; }

        public int PhysicianId { get; set;}

        [Required(ErrorMessage = "Description is Required")]
        [RegularExpression(@"^[a-zA-Z][a-zA-Z\s]{2,150}$", ErrorMessage = "Transfer Description Accepts Only Alphabets ( Min. 2 & Max. 150 )")]
        public string? TransferDescription { get; set; }
    }

}
