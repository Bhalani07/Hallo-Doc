using Data_Access.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Access.Coustom_Models
{
    public class GetRecordsModel
    {
        public List<PatientRecordsLists>? users { get; set; }

        public List<PatientExploreList>? requestList { get; set; }

        public List<Physician> physicians { get; set; }

        public string? searchRecordOne { get; set; }

        public string? searchRecordTwo { get; set; }

        public string? searchRecordThree { get; set; }

        public string? searchRecordFour { get; set; }
    }

    public class PatientRecordsLists
    {
        public string Firstname { get; set; }

        public string Lastname { get; set; }

        public string Email { get; set; }

        public string Mobile { get; set; }

        public int Userid { get; set; }

        public string address { get; set; }
    }


    public class PatientExploreList
    {
        public int RequestId { get; set; }

        public string name { get; set; }

        public string? Createddate { get; set; }

        public string Confirmationnumber { get; set; }

        public string ProviderName { get; set; }

        public int Status { get; set; }

        public bool isFinalize { get; set; }
    }

}
