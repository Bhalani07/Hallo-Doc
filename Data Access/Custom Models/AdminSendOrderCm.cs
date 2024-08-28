using Data_Access.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Access.Custom_Models
{
    public class AdminSendOrderCm
    {
        public int callId {  get; set; }

        public int statusForName {  get; set; }

        public List<Healthprofessionaltype> healthprofessionaltypes {  get; set; }

        public List<Healthprofessional> healthprofessionals { get; set; }

        [Required(ErrorMessage = "Business is Required")]
        public int VendorId { get; set; }

        public int RequestId { get; set; }

        [Required(ErrorMessage = "Faxnumber is Required")]        
        [RegularExpression(@"\+?[0-9]{1,3}\s?[-]?\(?\d{3}\)?[-]?\s?[0-9]{3}[-]?\s?[0-9]{4}$", ErrorMessage = "Invalid Fax Number")] 
        public string FaxNumber { get; set; }

        [Required(ErrorMessage = "Business Email is Required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Business Contact is Required")]
        public string BussinessContact { get; set; }

        [Required(ErrorMessage = "Prescreption is Required")]
        [RegularExpression(@"^[a-zA-Z0-9].{1,49}$", ErrorMessage = "Minimum 2 & Maximum 50 Characters Allowed")]
        public string Prescreption { get; set; }

        [Required(ErrorMessage = "No of refils is Required")]
        public int NoOfRefils { get; set; }
    }
}
