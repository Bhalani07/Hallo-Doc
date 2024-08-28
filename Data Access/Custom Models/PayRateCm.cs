using Data_Access;
using Data_Access.Models;

namespace Data_Access.Custom_Models
{
    public class PayrateCm
    {
        public int? AspId { get; set;}

        public int? Phyid { get; set; }

        public List<PayRateForProviderCm>? PayrateForProvider { get; set;}
    }

    public class PayRateForProviderCm
    {
        public int? Categoryid { get; set; }

        public string? Categoryname { get; set; }

        public int? PayrateValue { get; set; }
    }
}