using Data_Access.Custom_Models;
using Data_Access.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic.Interface
{
    public interface IProviderDashboard
    {

        #region Dashboard Request

        CountRequest GetCountRequest(int physicianId);

        List<RequestListAdminDash> GetRequestData(int[] Status, string requesttypeid, int physicianId);

        #endregion


        #region Accept Case

        bool SetAcceptCaseData(int requestId, int physicianId);

        #endregion


        #region Transfer Case 

        bool TransferCaseData(TransferCaseModal transferCaseModal);

        #endregion


        #region Encounter Case

        bool SetEncounterCareType(EncounterCm encounterCm);

        bool FinalizeEncounterCase(int requestId);

        bool HouseCallConclude(int requestId);

        #endregion


        #region Conclude Case

        AdminViewDocumentsCm GetViewDocumentsData(int requestid);

        List<ViewDocuments> GetViewDocumentsList(int requestid);

        bool SetViewDocumentData(AdminViewDocumentsCm adminViewDocumentsCm);

        bool ConfirmConcludeCare(AdminViewDocumentsCm adminViewDocumentsCm);

        #endregion


        #region Request To Admin

        bool RequestForEdit(ProviderProfileCm providerProfileCm);

        #endregion


        #region Invoicing

        List<Timesheetdetail> GetTimeSheetDetails(int phyid, string dateSelected);

        List<Timesheetdetailreimbursement> GetTimeSheetDetailsReimbursements(int phyid, string dateSelected);

        List<ProviderTimesheetDetails> GetFinalizeTimeSheetDetails(int phyid, string dateSelected);

        bool PostFinalizeTimesheet(List<ProviderTimesheetDetails> providerTimesheetDetails);

        List<AddReceiptsDetails> GetAddReceiptsDetails(int[] timeSheetDetailId, int AspId);

        bool EditReceipt(int aspId, int timeSheetDetailId, string item, int amount, IFormFile file);

        bool DeleteReceipt(int aspId, int timeSheetDetailId);

        bool FinalizeTimeSheet(int timeSheetId);


        #endregion

    }
}
