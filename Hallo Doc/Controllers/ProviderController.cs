using Business_Logic.Interface;
using Business_Logic.Repository;
using Data_Access.Coustom_Models;
using Data_Access.Custom_Models;
using Data_Access.Models;
using Microsoft.AspNetCore.Mvc;
using Rotativa.AspNetCore;

namespace Hallo_Doc.Controllers
{
    public class ProviderController : Controller
    {
        private readonly IProviderDashboard _providerDashboard;
        private readonly ISessionUtils _sessionUtils;
        private readonly IAdminDashboard _adminDashboard;

        public ProviderController(IProviderDashboard providerDashboard, ISessionUtils sessionUtils, IAdminDashboard adminDashboard)
        {
            _providerDashboard = providerDashboard;
            _sessionUtils = sessionUtils;
            _adminDashboard = adminDashboard;
        }


        #region Dashboard

        [CustomAuthorize("", "Provider")]
        public IActionResult ProviderDashboard()
        {
            var user = _sessionUtils.GetUser(HttpContext.Session);
            int physicianid = user.UserId;

            AdminDashboardCm admindashboardcm = new AdminDashboardCm()
            {
                sessionId = physicianid,
            };

            byte[] byteArray = HttpContext.Session.Get("providerDashStatus");

            if (byteArray == null)
            {
                HttpContext.Session.SetInt32("lastStatus", 1);
            }
            else
            {
                int[] statusArray = new int[byteArray.Length / sizeof(int)];
                Buffer.BlockCopy(byteArray, 0, statusArray, 0, byteArray.Length);

                HttpContext.Session.SetInt32("lastStatus", statusArray[0]);
            }

            return View(admindashboardcm);
        }

        [CustomAuthorize("Dashboard", "Provider")]
        public IActionResult GetProviderDashboard(int physicianId)
        {
            byte[] byteArray = HttpContext.Session.Get("providerDashStatus");

            if (byteArray == null)
            {
                int[] arr = { 1 };

                HttpContext.Session.SetInt32("lastStatus", 1);

                AdminDashboardCm adminDashboardCm = new AdminDashboardCm()
                {
                    StatusForName = 1,
                    RequestListAdminDash = _providerDashboard.GetRequestData(arr, null, physicianId),
                    countRequest = _providerDashboard.GetCountRequest(physicianId),
                    sessionId = physicianId,
                };

                return PartialView("Provider/_Provider_Dashboard", adminDashboardCm);
            }

            else
            {
                int[] statusArray = new int[byteArray.Length / sizeof(int)];
                Buffer.BlockCopy(byteArray, 0, statusArray, 0, byteArray.Length);

                HttpContext.Session.SetInt32("lastStatus", statusArray[0]);

                AdminDashboardCm adminDashboardCm = new AdminDashboardCm()
                {
                    StatusForName = statusArray[0],
                    RequestListAdminDash = _providerDashboard.GetRequestData(statusArray, null, physicianId),
                    countRequest = _providerDashboard.GetCountRequest(physicianId),
                };

                return PartialView("Provider/_Provider_Dashboard", adminDashboardCm);
            }
        }

        [CustomAuthorize("Dashboard", "Provider")]
        [HttpPost]
        public IActionResult ProviderTableRecords(int[] status, string requesttypeid, int physicianId)
        {
            byte[] byteArray = new byte[status.Length * sizeof(int)];
            Buffer.BlockCopy(status, 0, byteArray, 0, byteArray.Length);

            HttpContext.Session.Set("providerDashStatus", byteArray);

            AdminDashboardCm adminDashboardCm = new AdminDashboardCm()
            {
                StatusForName = status[0],
                countRequest = _providerDashboard.GetCountRequest(physicianId),
                RequestListAdminDash = _providerDashboard.GetRequestData(status, requesttypeid, physicianId),
            };

            return PartialView("Provider/_Provider_DashboardContent", adminDashboardCm);
        }

        [CustomAuthorize("Dashboard", "Provider")]
        public IActionResult ProviderFilterTable(int[] status, string requesttypeid, int physicianId)
        {
            AdminDashboardCm adminDashboardCm = new AdminDashboardCm()
            {
                StatusForName = status[0],
                RequestListAdminDash = _providerDashboard.GetRequestData(status, requesttypeid, physicianId),
                reqTypeId = requesttypeid,
            };

            return PartialView("Provider/_Provider_DashboardTable", adminDashboardCm);
        }

        #endregion


        #region Accept Case 

        public IActionResult AcceptCaseModal(int requestid)
        {
            ModalCm modalCm = new ModalCm();
            modalCm.RequestId = requestid;

            return PartialView("Provider/_Provider_AcceptCase", modalCm);
        }

        [HttpPost]
        public IActionResult AcceptCase(int requestId, int physicianId)
        {
            if (_providerDashboard.SetAcceptCaseData(requestId, physicianId))
            {
                return Ok();
            }
            return BadRequest();
        }

        #endregion


        #region Transfer Case 

        public IActionResult Transfer(int requestid, int physicianId)
        {
            TransferCaseModal transferCaseModal = new TransferCaseModal()
            {
                RequestId = requestid,
                PhysicianId = physicianId
            };

            return PartialView("Provider/_Provider_TransferCase", transferCaseModal);
        }

        [HttpPost]
        public IActionResult TransferCase(TransferCaseModal transferCaseModal)
        {
            if (transferCaseModal.RequestId != 0)
            {
                if (_providerDashboard.TransferCaseData(transferCaseModal))
                {
                    return Ok();
                }

                return BadRequest();
            }

            return Json(new { Error = "Returned in else" });
        }

        #endregion


        #region Encounter Case

        public IActionResult EncounterModal(int requestid, int physicianId)
        {
            EncounterCm encounterCm = new EncounterCm();
            encounterCm.reqid = requestid;
            encounterCm.physicianId = physicianId;

            return PartialView("Provider/_Provider_EncounterModal", encounterCm);
        }

        [HttpPost]
        public IActionResult PostEncounterCare(EncounterCm encounterCm)
        {
            if (_providerDashboard.SetEncounterCareType(encounterCm))
            {
                if (encounterCm.Option == 1)
                {
                    return Ok(true);
                }

                return Ok(false);
            }

            return BadRequest();

        }

        public IActionResult FinalizeEncounterModal(int requestId)
        {
            EncounterCm encounterCm = new EncounterCm();
            encounterCm.reqid = requestId;

            return PartialView("Provider/_Provider_EncounterModal", encounterCm);
        }

        [HttpPost]
        public IActionResult FinalizeEncounter(int requestId)
        {
            if (_providerDashboard.FinalizeEncounterCase(requestId))
            {
                return Ok();
            }

            return BadRequest();
        }

        public IActionResult DownloadEncounter(int requestId)
        {
            EncounterCm encounterCm = new EncounterCm();
            encounterCm.reqid = requestId;

            return PartialView("Provider/_Provider_EncounterModal", encounterCm);
        }

        public IActionResult GeneratePDF([FromQuery] int requestid)
        {
            var encounterFormView = _adminDashboard.GetEncounterData(requestid, 0);
            if (encounterFormView == null)
            {
                return NotFound();
            }
            return new ViewAsPdf("FinalizeForm", encounterFormView)
            {
                FileName = "EncounterForm.pdf"
            };

        }

        public IActionResult HouseCall(int requestId)
        {
            EncounterCm encounterCm = new EncounterCm();
            encounterCm.reqid = requestId;

            return PartialView("Provider/_Provider_EncounterModal", encounterCm);
        }

        [HttpPost]
        public IActionResult HouseCallPost(int requestId)
        {
            if (_providerDashboard.HouseCallConclude(requestId))
            {
                return Ok();
            }

            return BadRequest();
        }

        #endregion


        #region Conclude Case

        public IActionResult GetConcludeCare(int requestid)
        {
            AdminViewDocumentsCm adminViewDocumentsCm = new AdminViewDocumentsCm();
            adminViewDocumentsCm = _providerDashboard.GetViewDocumentsData(requestid);
            adminViewDocumentsCm.viewDocuments = _providerDashboard.GetViewDocumentsList(requestid);

            return PartialView("Provider/_Provider_ConcludeCare", adminViewDocumentsCm);
        }

        [HttpPost]
        public IActionResult UploadConcludeDocument(AdminViewDocumentsCm adminViewDocumentsCm)
        {
            if (_providerDashboard.SetViewDocumentData(adminViewDocumentsCm))
            {
                return Ok(adminViewDocumentsCm.requestId);
            }

            return BadRequest();
        }

        public IActionResult DeleteConcludeFile(int requestwisefileid)
        {
            _adminDashboard.DeleteFileData(requestwisefileid);

            return Ok();
        }

        [HttpPost]
        public IActionResult ConcludeCare(AdminViewDocumentsCm adminViewDocumentsCm)
        {
            if (_providerDashboard.ConfirmConcludeCare(adminViewDocumentsCm))
            {
                return Ok();
            }

            return BadRequest();
        }

        #endregion


        #region Request To Admin

        [HttpPost]
        public IActionResult SendRequestToAdmin(ProviderProfileCm providerProfileCm)
        {
            if (_providerDashboard.RequestForEdit(providerProfileCm))
            {
                return Ok();
            }

            return BadRequest();
        }

        #endregion


        #region Invoicing

        public IActionResult GetProviderInvoicing(string dateSelected)
        {
            var phyid = _sessionUtils.GetUser(HttpContext.Session).UserId;

            ProviderInvoicingCm providerInvoicingCm = new()
            {
                Timesheetdetails = _providerDashboard.GetTimeSheetDetails(phyid, dateSelected),
                Timesheetdetailreimbursements = _providerDashboard.GetTimeSheetDetailsReimbursements(phyid, dateSelected),                
            };

            providerInvoicingCm.TimesheetsFinalize = providerInvoicingCm.Timesheetdetails.Any(x => x.Timesheet.Isfinalize == true);

            return PartialView("Provider/_Provider_Invoicing", providerInvoicingCm);
        }

        public IActionResult OpenFinalizeTimeSheet(string dateSelected)
        {
            var phyid = _sessionUtils.GetUser(HttpContext.Session).UserId;

            ProviderInvoicingCm providerInvoicingCm = new()
            {
                ProviderTimesheetDetails = _providerDashboard.GetFinalizeTimeSheetDetails(phyid, dateSelected),
                CallId = 2,
                DateSelected = dateSelected,
                PhysicianId = phyid,
            };

            return PartialView("Provider/_Provider_FinalizeTimeSheet", providerInvoicingCm);
        }

        [HttpPost]
        public IActionResult PostFinalizeTimesheet(List<ProviderTimesheetDetails> providerTimesheetDetails)
        {
            if (_providerDashboard.PostFinalizeTimesheet(providerTimesheetDetails))
            {
                return Ok(true);
            };
            return Ok(false);
        }

        public IActionResult GetAddReceipts(int[] timeSheetDetailId, int callId)
        {
            var AspId = _sessionUtils.GetUser(HttpContext.Session).AspNetUserId;

            ProviderInvoicingCm providerInvoicingCm = new()
            {
                AddReceiptsDetails = _providerDashboard.GetAddReceiptsDetails(timeSheetDetailId, AspId),
                CallId = callId,
            };

            return PartialView("Provider/_Provider_AddReceipts", providerInvoicingCm);
        }

        [HttpPost]
        public IActionResult PostAddReceipt(int timeSheetDetailId, string item, int amount, IFormFile file)
        {
            var aspId = _sessionUtils.GetUser(HttpContext.Session).AspNetUserId;

            if (_providerDashboard.EditReceipt(aspId, timeSheetDetailId, item, amount, file))
            {
                return Ok(true);
            }
            return Ok(false);
        }

        [HttpPost]
        public IActionResult DeleteReceipt(int timeSheetDetailId)
        {
            var aspId = _sessionUtils.GetUser(HttpContext.Session).AspNetUserId;

            if (_providerDashboard.DeleteReceipt(aspId, timeSheetDetailId))
            {
                return Ok(true);
            }
            return Ok(false);
        }

        [HttpPost]
        public IActionResult ConfirmFinalizeTimeSheet(int timeSheetId)
        {
            if (_providerDashboard.FinalizeTimeSheet(timeSheetId))
            {
                return Ok(true);
            }
            return Ok(false);
        }

        #endregion


    }
}
