using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Access.Custom_Models
{
    public class AdminViewDocumentsCm
    {
        public int callId {  get; set; }

        public int userId { get; set; }

        public int statusForName {  get; set; }

        public int requestId { get; set; }

        public string patientName { get; set; }

        public string ConfirmationNumber { get; set; }

        public List<ViewDocuments> viewDocuments { get; set; }

        public IFormFile document { get; set; }

        [Required(ErrorMessage = "Provider note is Required")]
        [RegularExpression(@"^[a-zA-Z\s]{2,150}$", ErrorMessage = "Provider Notes Accepts Only Alphabets ( Min. 2 & Max. 150 )")]
        public string ProviderNote { get; set; }

    }

    public class ViewDocuments
    {
        public int requestWiseFileId { get; set; }

        public int requestId { get; set; }

        public string documentName { get; set; }

        public DateTime uploadDate { get; set; }
    }
}
