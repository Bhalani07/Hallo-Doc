using Data_Access.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Access.Custom_Models
{
    public class ProvidersCm
    {
        public int RegionId { get; set; }

        public string Email { get; set; }

        [Required(ErrorMessage = "Message Is Required")]
        [RegularExpression(@"^[a-zA-Z][a-zA-Z\s]{2,150}$", ErrorMessage = "Contact Message Accepts Only Alphabets ( Min. 2 & Max. 150 )")] 
        public string ContactMessage { get; set; }

        public List<Region> Regions { get; set; }

        public List<Provider> Providers { get; set; }
    }

    public class Provider
    {
        public int aspId { get; set; }

        public string Email { get; set; }

        public string Name { get; set; }

        public string Role { get; set; }

        public string CallStatus { get; set; }

        public short Status { get; set; }
    }
}
