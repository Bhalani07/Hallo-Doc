﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Access.Coustom_Models
{
    public class EmailSmsLogModel
    {
        public List<emailSmsRecords>? recordslist { get; set; }

        public int? searchRecordOne { get; set; }

        public string? searchRecordTwo { get; set; }

        public string? searchRecordThree { get; set; }

        public DateTime? searchRecordFour { get; set; }

        public DateTime? searchRecordFive { get; set; }       

    }
    public class emailSmsRecords
    {
        public int? requestid { get; set; }

        public string? recipient { get; set; }

        public string? action { get; set; }

        public string? rolename { get; set; }

        public string? email { get; set; }

        public string? contact { get; set; }

        public string? createddate { get; set; }

        public string? sentdate { get; set; }

        public string? sent { get; set; }

        public int? senttries { get; set; }

        public string? confirmationNumber { get; set; }

        public int? roleid { get; set; }    
        
    }

}
