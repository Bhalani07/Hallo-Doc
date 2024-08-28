using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Access.Custom_Models
{
    public class EncounterCm
    {

        public int callId {  get; set; }

        public int physicianId { get; set; }

        public int userId { get; set; } 

        public int statusForName {  get; set; }

        public int reqid { get; set; }

        public int callType { get; set; }

        [Required(ErrorMessage = "Please Select Any Care Type")]
        public int? Option {  get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Location { get; set; }

        public DateTime? BirthDate { get; set; }

        public string? Date { get; set; }

        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }

        public bool? isEncounter { get; set; }

        [RegularExpression(@"^[a-zA-Z].{1,150}$", ErrorMessage = "Must be Min. 2 & Max. 150")]
        public string? HistoryIllness { get; set; }

        [RegularExpression(@"^[a-zA-Z].{1,150}$", ErrorMessage = "Must be Min. 2 & Max. 150")]
        public string? MedicalHistory { get; set; }

        [RegularExpression(@"^[a-zA-Z].{1,150}$", ErrorMessage = "Must be Min. 2 & Max. 150")]
        public string? Medications { get; set; }

        [RegularExpression(@"^[a-zA-Z].{1,150}$", ErrorMessage = "Must be Min. 2 & Max. 150")]
        public string? Allergies { get; set; }

        [RegularExpression(@"^\d{1,2}(\.\d{1,3})?$", ErrorMessage = "Temperature can have at most 3 decimal places.")]
        [Range(0, 50, ErrorMessage = "Invalid Temperature")]
        public decimal? Temp { get; set; }

        [RegularExpression(@"^\d{1,3}(\.\d{1,3})?$", ErrorMessage = "HR can have at most 3 decimal places.")]
        [Range(0, 200, ErrorMessage = "Invalid Heart Rate")]
        public decimal? Hr { get; set; }

        [RegularExpression(@"^\d{1,2}(\.\d{1,3})?$", ErrorMessage = "RR can have at most 3 decimal places.")]
        [Range(0, 70, ErrorMessage = "Invalid Respiratory Rate")]
        public decimal? Rr { get; set; }


        [RegularExpression(@"^\d{1,3}(\.\d{1,3})?$", ErrorMessage = "BP can have at most 3 decimal places.")]
        [Range(60, 150, ErrorMessage = "Invalid Blood Pressure")]
        public int? BpS { get; set; }

        [RegularExpression(@"^\d{1,3}(\.\d{1,3})?$", ErrorMessage = "BP can have at most 3 decimal places.")]
        [Range(60, 150, ErrorMessage = "Invalid Blood Pressure")]
        public int? BpD { get; set; }

        [RegularExpression(@"^\d{1,3}(\.\d{1,3})?$", ErrorMessage = "O2 can have at most 3 decimal places.")]
        [Range(0, 100, ErrorMessage = "Invalid O2")]
        public decimal? O2 { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9].{1,150}$", ErrorMessage = "Must be Min. 2 & Max. 150")]
        public string? Pain { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9].{1,150}$", ErrorMessage = "Must be Min. 2 & Max. 150")]
        public string? Heent { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9].{1,150}$", ErrorMessage = "Must be Min. 2 & Max. 150")]
        public string? Cv { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9].{1,150}$", ErrorMessage = "Must be Min. 2 & Max. 150")]
        public string? Chest { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9].{1,150}$", ErrorMessage = "Must be Min. 2 & Max. 150")]
        public string? Abd { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9].{1,150}$", ErrorMessage = "Must be Min. 2 & Max. 150")]
        public string? Extr { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9].{1,150}$", ErrorMessage = "Must be Min. 2 & Max. 150")]
        public string? Skin { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9].{1,150}$", ErrorMessage = "Must be Min. 2 & Max. 150")]
        public string? Neuro { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9].{1,150}$", ErrorMessage = "Must be Min. 2 & Max. 150")]
        public string? Other { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9].{1,150}$", ErrorMessage = "Must be Min. 2 & Max. 150")]
        public string? Diagnosis { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9].{1,150}$", ErrorMessage = "Must be Min. 2 & Max. 150")]
        public string? TreatmentPlan { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9].{1,150}$", ErrorMessage = "Must be Min. 2 & Max. 150")]
        public string? MedicationDispensed { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9].{1,150}$", ErrorMessage = "Must be Min. 2 & Max. 150")]
        public string? Procedures { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9].{1,150}$", ErrorMessage = "Must be Min. 2 & Max. 150")]
        public string? FollowUp { get; set; }
    }
}
