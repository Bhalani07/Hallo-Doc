using System.Diagnostics.Eventing.Reader;
using System.Text.Json.Serialization;
using Data_Access.Models;
using Microsoft.AspNetCore.Http;

namespace Data_Access.Custom_Models
{

    public class ProviderInvoicingCm
    {
        public bool? TimesheetsFinalize{ get; set; }

        public List<Timesheetdetail>? Timesheetdetails { get; set; }

        public List<Timesheetdetailreimbursement>? Timesheetdetailreimbursements{ get; set; }

        public List<ProviderTimesheetDetails>? ProviderTimesheetDetails { get; set; }

        public List<AddReceiptsDetails>? AddReceiptsDetails { get; set; }

        public List<Payratebyprovider>? PayrateByProvider { get; set; }

        public int? CallId {  get; set; }

        public string? DateSelected { get; set; }

        public int? PhysicianId { get; set; }

    }

    public class ProviderTimesheetDetails
    {

        public int? TimeSheetId { get; set; }

        public int? TimeSheetDetailId { get; set; }

        public int? Hours { get; set; }

        public bool? IsWeekend { get; set; }

        public int? NoOfHouseCalls { get; set; }

        public int? NoOfConsults { get; set; }

        public DateOnly? ShiftDetailDate { get; set; }
    }

    public class AddReceiptsDetails
    {
        public int? TimeSheetDetailId { get; set; }

        public int? Amount { get; set; }

        public string? Item { get; set; }

        public string? BillValue { get; set; }

        public IFormFile? Bill { get; set; }

        public DateOnly? ShiftDetailDate { get; set; }

    }

}