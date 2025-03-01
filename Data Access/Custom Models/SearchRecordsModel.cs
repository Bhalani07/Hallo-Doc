﻿using Data_Access.Models;
using Npgsql.EntityFrameworkCore.PostgreSQL.Query.Expressions.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Access.Coustom_Models
{
    public class SearchRecordsModel
    {
        public List<requests>? requestList { get; set; }

        public List<Requesttype>? requestType { get; set; }

        public int? searchRecordOne { get; set; }

        public string? searchRecordTwo { get; set; }

        public int? searchRecordThree { get; set; }

        public DateOnly? searchRecordFour { get; set; }

        public DateOnly? searchRecordFive { get; set; }

        public string? searchRecordSix { get; set; }

        public string? searchRecordSeven { get; set; }

        public string? searchRecordEight { get; set; }

    }
    public class requests
    { 
        public string? patientname { get; set; }

        public string? requestor { get; set; }

        public string? dateOfService { get; set; }

        public string? closeCaseDate { get; set; }

        public string? email { get; set; }

        public string? contact { get; set; }

        public string? address { get; set; }

        public string? zip { get; set; }

        public int status { get; set; }

        public string? physician { get; set; }

        public string? physicianNote { get; set; }

        public string? providerNote { get; set; }

        public string? AdminNote { get; set; }

        public string? pateintNote { get; set; }

        public int? requestid { get; set; } 

        public int? userid { get; set; }    

        public int? requestTypeId { get; set; }
    }

}
