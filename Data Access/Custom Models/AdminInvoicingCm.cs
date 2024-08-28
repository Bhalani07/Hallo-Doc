using Data_Access.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Access.Custom_Models
{
    public class AdminInvoicingCm
    {
        public List<Physician>? PhysiciansList {  get; set; }

        public List<Timesheet>? TimesheetsList { get; set; }

        public List<Timesheetdetail>? Timesheetdetails { get; set; }

        public List<Timesheetdetailreimbursement>? Timesheetdetailreimbursements{ get; set; }

        public int? PhysicianId { get; set; }
    }

    public class AdminTimeSheetList
    {
        public int? TimeSheetId { get; set; }

        public DateOnly? StartDate { get; set; }

        public DateOnly? EndDate { get; set; }

        public string? Status { get; set; }
    }
}
