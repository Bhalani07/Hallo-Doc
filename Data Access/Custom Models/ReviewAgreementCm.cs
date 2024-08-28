using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Access.Custom_Models
{
    public class ReviewAgreementCm
    {
        public int RequestId { get; set; }

        public string PatientName { get; set; }

        [Required(ErrorMessage = "Cancelation Reason is Required")]
        [RegularExpression(@"^[a-zA-Z][a-zA-Z\s]{2,150}$", ErrorMessage = "Cancellation Notes Accepts Only Alphabets ( Min. 2 & Max. 150 )")]
        public string CancellationNotes { get; set; }
    }
}
