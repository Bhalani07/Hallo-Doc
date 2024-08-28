using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Access.Coustom_Models
{
    public class BlockedRequestModel
    {
        public List<blockedRequest>? blockrequestList {  get; set; }

        public string? searchRecordOne { get; set; }

        public DateTime? searchRecordTwo { get; set; }

        public string? searchRecordThree { get; set; }

        public string? searchRecordFour { get; set; }
    }
    public class blockedRequest
    {
        public int? requestid { get; set; }

        public string? patientname { get; set; }

        public string? contact { get; set; }

        public string? email { get; set; }

        public string? createddate  { get; set; }

        public string? notes { get; set; }

        public bool isactive{ get; set; }
    }

}
