using Business_Logic.Interface;
using Business_Logic.Repository;
using Data_Access.Custom_Models;
using Data_Access.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Net;
using OfficeOpenXml;
using Npgsql.EntityFrameworkCore.PostgreSQL.Query.Expressions.Internal;
using Data_Access.Coustom_Models;
using Rotativa.AspNetCore;

namespace Hallo_Doc.Controllers
{
    public class AdminController : Controller
    {
        private readonly IAdminDashboard _adminDashboard;
        private readonly IProviderDashboard _providerDashboard;
        private readonly ISessionUtils _sessionUtils;

        public AdminController(IAdminDashboard adminDashboard, ISessionUtils sessionUtils, IProviderDashboard providerDashboard)
        {
            _adminDashboard = adminDashboard;
            _sessionUtils = sessionUtils;
            _providerDashboard = providerDashboard;
        }

        #region Dashboard

        [CustomAuthorize("", "Admin")]
        public IActionResult AdminDashboard()
        {
            byte[] byteArray = HttpContext.Session.Get("statusArray");

            if (byteArray == null)
            {
                HttpContext.Session.SetInt32("prevStatus", 1);
            }
            else
            {
                int[] statusArray = new int[byteArray.Length / sizeof(int)];
                Buffer.BlockCopy(byteArray, 0, statusArray, 0, byteArray.Length);

                HttpContext.Session.SetInt32("prevStatus", statusArray[0]);
            }

            return View();

        }

        [CustomAuthorize("Dashboard", "Admin")]
        public IActionResult GetDashboard()
        {
            byte[] byteArray = HttpContext.Session.Get("statusArray");

            if (byteArray == null)
            {
                int[] arr = { 1 };

                HttpContext.Session.SetInt32("prevStatus", 1);

                AdminDashboardCm adminDashboardCm = new AdminDashboardCm()
                {
                    StatusForName = 1,
                    RequestListAdminDash = _adminDashboard.GetRequestData(arr, null, 0),
                    regions = _adminDashboard.GetRegions(),
                    countRequest = _adminDashboard.GetCountRequest(),
                };

                return PartialView("Admin/_Admin_Dashboard", adminDashboardCm);
            }

            else
            {
                int[] statusArray = new int[byteArray.Length / sizeof(int)];
                Buffer.BlockCopy(byteArray, 0, statusArray, 0, byteArray.Length);

                HttpContext.Session.SetInt32("prevStatus", statusArray[0]);

                AdminDashboardCm adminDashboardCm = new AdminDashboardCm()
                {
                    StatusForName = statusArray[0],
                    RequestListAdminDash = _adminDashboard.GetRequestData(statusArray, null, 0),
                    regions = _adminDashboard.GetRegions(),
                    countRequest = _adminDashboard.GetCountRequest(),
                };

                return PartialView("Admin/_Admin_Dashboard", adminDashboardCm);
            }
        }

        [CustomAuthorize("Dashboard", "Admin")]
        [HttpPost]
        public IActionResult TableRecords(int[] status, string requesttypeid, int regionid)
        {
            byte[] byteArray = new byte[status.Length * sizeof(int)];
            Buffer.BlockCopy(status, 0, byteArray, 0, byteArray.Length);

            HttpContext.Session.Set("statusArray", byteArray);

            AdminDashboardCm adminDashboardCm = new AdminDashboardCm()
            {
                StatusForName = status[0],
                countRequest = _adminDashboard.GetCountRequest(),
                RequestListAdminDash = _adminDashboard.GetRequestData(status, requesttypeid, regionid),
                regions = _adminDashboard.GetRegions(),
            };

            return PartialView("Admin/_AdminTable", adminDashboardCm);
        }

        [CustomAuthorize("Dashboard", "Admin")]
        public IActionResult FilterTableRecords(int[] status, string requesttypeid, int regionid)
        {
            AdminDashboardCm adminDashboardCm = new AdminDashboardCm()
            {
                StatusForName = status[0],
                RequestListAdminDash = _adminDashboard.GetRequestData(status, requesttypeid, regionid),
                regions = _adminDashboard.GetRegions(),
                reqTypeId = requesttypeid,
            };

            return PartialView("Admin/_AdminRequestTable", adminDashboardCm);
        }


        #region View Case

        public IActionResult ViewCaseRecords(int status, int requestid, int callId)
        {
            if (requestid != 0)
            {
                var viewCaseData = _adminDashboard.GetViewCaseData(requestid);

                AdminDashboardCm adminDashboardCm = new AdminDashboardCm
                {
                    StatusForName = status,
                    adminViewCaseData = viewCaseData,
                    callId = callId,
                };

                return PartialView("Admin/_Admin_ViewCase", adminDashboardCm);
            }
            return Json(new { Error = "Returned in else" }); ;
        }


        [HttpPost]
        public IActionResult UpdateViewCaseRecords(AdminDashboardCm adminDashboardCm)
        {
            _adminDashboard.SetViewCaseData(adminDashboardCm.adminViewCaseData, adminDashboardCm.adminViewCaseData.RequestId);

            return PartialView("Admin/_Admin_ViewCase", adminDashboardCm);
        }

        #endregion


        #region View Notes

        public IActionResult ViewNotes(int requestid, int status, int callId)
        {
            AdminDashboardCm adminDashboardCm = new AdminDashboardCm()
            {
                viewNotesData = _adminDashboard.GetViewNotesData(requestid),
                StatusForName = status,
                callId = callId,
            };

            return PartialView("Admin/_Admin_ViewNotes", adminDashboardCm);

        }

        [HttpPost]
        public IActionResult UpdateViewNotes(AdminDashboardCm adminDashboardCm)
        {
            var user = _sessionUtils.GetUser(HttpContext.Session);
            int aspId = user.AspNetUserId;

            _adminDashboard.SetViewNotesData(adminDashboardCm.viewNotesData, adminDashboardCm.viewNotesData.RequestId, aspId);

            return Json(new { reqId = adminDashboardCm.viewNotesData.RequestId, status = adminDashboardCm.StatusForName, callId = adminDashboardCm.callId });
        }

        #endregion


        #region Cancel Case

        public IActionResult CancelModal(int requestid, int status)
        {

            ModalCm modalCm = new ModalCm()
            {
                cancelCaseModal = _adminDashboard.GetCancelCaseData(requestid),
                casetags = _adminDashboard.GetCasetags(),
                StatusForName = status,
            };

            return PartialView("Admin/_Admin_CancelCase", modalCm);

        }

        [HttpPost]
        public IActionResult CancelCase(ModalCm modalCm)
        {
            if (modalCm.cancelCaseModal.RequestId != 0)
            {
                _adminDashboard.SetCancelCaseData(modalCm.cancelCaseModal);
                modalCm.StatusForName = modalCm.StatusForName;

                return Ok(modalCm.StatusForName);
            }
            return Json(new { Error = "Returned in else" });
        }

        #endregion


        #region Assign Case

        public IActionResult AsignModal(int requestid, int modalid)
        {
            ModalCm modalCm = new ModalCm()
            {
                ModalId = modalid,
                asignCaseModal = _adminDashboard.GetAsignCaseData(requestid),
                regions = _adminDashboard.GetRegions(),
                physicians = _adminDashboard.GetPhysicians(0),
            };

            return PartialView("Admin/_Admin_AsignCase", modalCm);

        }

        public IActionResult FilterAsignModal(int regionid)
        {
            ModalCm modalCm = new ModalCm()
            {
                physicians = _adminDashboard.GetPhysicians(regionid),
            };

            return Json(new { success = modalCm.physicians });

        }

        [HttpPost]
        public IActionResult AsignCase(ModalCm modalCm)
        {
            if (modalCm.asignCaseModal.RequestId != 0)
            {
                _adminDashboard.SetAsignCaseData(modalCm);

                return Ok();
            }
            return Json(new { Error = "Returned in else" });
        }

        #endregion


        #region Block Case

        public IActionResult BlockModal(int requestid)
        {
            ModalCm modalCm = new ModalCm()
            {
                blockCaseModal = _adminDashboard.GetBlockCaseModal(requestid),
            };

            return PartialView("Admin/_Admin_BlockCase", modalCm);
        }

        [HttpPost]
        public IActionResult BlockCase(ModalCm modalCm)
        {
            if (modalCm.blockCaseModal.RequestId != 0)
            {
                _adminDashboard.SetBlockCaseData(modalCm.blockCaseModal);

                return Ok();
            }

            return Json(new { Error = "Returned in else" });
        }

        #endregion


        #region View Documents

        public IActionResult ViewDocuments(int requestid, int status, int callId)
        {
            AdminViewDocumentsCm adminViewDocumentsCm = new AdminViewDocumentsCm();
            adminViewDocumentsCm = _adminDashboard.GetViewDocumentsData(requestid);
            adminViewDocumentsCm.viewDocuments = _adminDashboard.GetViewDocumentsList(requestid);
            adminViewDocumentsCm.statusForName = status;
            adminViewDocumentsCm.callId = callId;

            return PartialView("Admin/_Admin_ViewDocument", adminViewDocumentsCm);
        }

        [HttpPost]
        public IActionResult UploadDocument(AdminViewDocumentsCm adminViewDocumentsCm)
        {
            _adminDashboard.SetViewDocumentData(adminViewDocumentsCm);

            return Ok();
        }

        public IActionResult DeleteFile(int requestwisefileid, int requestid, int status)
        {
            _adminDashboard.DeleteFileData(requestwisefileid);

            return Ok();

        }

        public IActionResult SendFile(int[] requestwisefileid, int requestid, int status)
        {
            var user = _sessionUtils.GetUser(HttpContext.Session);
            int aspId = user.AspNetUserId;

            _adminDashboard.SendEmailWithFile(requestwisefileid, requestid, aspId);

            return Ok();
        }

        #endregion


        #region Send Orders

        public IActionResult GetSendOrderData(int requestid, int status, int callId)
        {
            AdminSendOrderCm adminSendOrderCm = new AdminSendOrderCm();
            adminSendOrderCm.healthprofessionaltypes = _adminDashboard.GetHealthprofessionaltypes();
            adminSendOrderCm.RequestId = requestid;
            adminSendOrderCm.statusForName = status;
            adminSendOrderCm.callId = callId;

            return PartialView("Admin/_Admin_SendOrders", adminSendOrderCm);
        }

        public IActionResult FilterBusinessByProfessions(int professionid)
        {
            AdminSendOrderCm adminSendOrderCm = new AdminSendOrderCm();
            adminSendOrderCm.healthprofessionals = _adminDashboard.GetHealthprofessionals(professionid);

            return Json(new { success = adminSendOrderCm.healthprofessionals });
        }

        public IActionResult BusinessData(int businessid)
        {
            return Json(new { success = _adminDashboard.GetBusinessData(businessid) });
        }

        [HttpPost]
        public IActionResult SendOrderData(AdminSendOrderCm adminSendOrderCm)
        {
            var user = _sessionUtils.GetUser(HttpContext.Session);
            int aspId = user.AspNetUserId;

            _adminDashboard.SetSendOrderData(adminSendOrderCm, aspId);

            return Ok();
        }

        #endregion


        #region Clear Case
        public IActionResult ClearModal(int requestid, int status)
        {
            ModalCm modalCm = new ModalCm();
            modalCm.RequestId = requestid;
            modalCm.StatusForName = status;

            return PartialView("Admin/_Admin_ClearCase", modalCm);
        }

        [HttpPost]
        public IActionResult ClearCase(int requestId)
        {
            _adminDashboard.SetClearCaseData(requestId);

            return Ok();
        }

        #endregion


        #region Send Agreement 

        public IActionResult SendAgreementModal(int requestid, int requesttypeid, int status, int callId)
        {
            ModalCm modalCm = new ModalCm();
            modalCm.sendAgreementModal = _adminDashboard.GetSendAgreementModal(requestid, requesttypeid);
            modalCm.StatusForName = status;
            modalCm.callId = callId;

            return PartialView("Admin/_Admin_SendAgreement", modalCm);
        }

        [HttpPost]
        public IActionResult SendAgreement(ModalCm modalCm)
        {
            var user = _sessionUtils.GetUser(HttpContext.Session);
            int aspId = user.AspNetUserId;

            _adminDashboard.SendAgreementEmail(modalCm.sendAgreementModal, aspId);

            return Ok();
        }

        #endregion


        #region Send Link

        public IActionResult SendLinkModal(int status, int callId)
        {
            AdminDashboardCm adminDashboardCm = new AdminDashboardCm();
            adminDashboardCm.StatusForName = status;
            adminDashboardCm.callId = callId;

            return PartialView("Admin/_Admin_SendLink", adminDashboardCm);
        }

        [HttpPost]
        public IActionResult SendLink(AdminDashboardCm adminDashboardCm)
        {
            var user = _sessionUtils.GetUser(HttpContext.Session);
            int aspId = user.AspNetUserId;

            _adminDashboard.SendSubmitRequestLink(adminDashboardCm.sendLink, aspId);

            return Ok();
        }

        #endregion


        #region Create Request

        public IActionResult AdminCreateRequest(int status, int callId)
        {
            AdminCreateRequestCm adminCreateRequestCm = new AdminCreateRequestCm();
            adminCreateRequestCm.Regions = _adminDashboard.GetRegions();
            adminCreateRequestCm.StatusForName = status;
            adminCreateRequestCm.callId = callId;

            return PartialView("Admin/_Admin_CreateRequest", adminCreateRequestCm);
        }

        [HttpPost]
        public IActionResult SendAdminCreateRequest(AdminCreateRequestCm adminCreateRequestCm)
        {
            var user = _sessionUtils.GetUser(HttpContext.Session);
            int aspId = user.AspNetUserId;

            _adminDashboard.SendCreateRequestData(adminCreateRequestCm, aspId);

            return Ok();
        }


        #endregion


        #region Export

        [HttpPost]
        public IActionResult Export(int[] arr, string requesttypeid, int regionid)
        {

            var requestData = _adminDashboard.GetRequestData(arr, requesttypeid, regionid);

            // Set LicenseContext to suppress the LicenseException
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            // Create a new Excel package
            using (ExcelPackage package = new ExcelPackage())
            {
                // Add a new worksheet to the Excel package
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("RequestData");

                // Add headers to the worksheet
                worksheet.Cells[1, 1].Value = "Patient Name";
                worksheet.Cells[1, 2].Value = "Patient Phone";
                worksheet.Cells[1, 3].Value = "Patient Email";
                worksheet.Cells[1, 4].Value = "Patient Address";
                worksheet.Cells[1, 5].Value = "Birth Date";
                worksheet.Cells[1, 6].Value = "Request ID";
                worksheet.Cells[1, 7].Value = "Requestor";
                worksheet.Cells[1, 8].Value = "Requestor Phone";
                worksheet.Cells[1, 9].Value = "Request Type ID";
                worksheet.Cells[1, 10].Value = "Requested Date";
                worksheet.Cells[1, 11].Value = "Waiting Time";
                worksheet.Cells[1, 12].Value = "Request Status";
                worksheet.Cells[1, 13].Value = "Notes";
                worksheet.Cells[1, 14].Value = "Physician Name";

                // Populate the worksheet with table data
                for (int i = 0; i < requestData.Count; i++)
                {
                    worksheet.Cells[i + 3, 1].Value = requestData[i].Name;
                    worksheet.Cells[i + 3, 2].Value = requestData[i].Phone;
                    worksheet.Cells[i + 3, 3].Value = requestData[i].Email;
                    worksheet.Cells[i + 3, 4].Value = requestData[i].Address;
                    worksheet.Cells[i + 3, 5].Value = requestData[i].DateOfBirth;
                    worksheet.Cells[i + 3, 6].Value = requestData[i].RequestId;
                    worksheet.Cells[i + 3, 7].Value = requestData[i].Requestor;
                    worksheet.Cells[i + 3, 8].Value = requestData[i].Mobile;
                    worksheet.Cells[i + 3, 9].Value = requestData[i].RequestTypeId;
                    worksheet.Cells[i + 3, 10].Value = requestData[i].RequestDate;
                    worksheet.Cells[i + 3, 11].Value = "( " + requestData[i].Days + " " + requestData[i].Hours + " )";
                    worksheet.Cells[i + 3, 12].Value = requestData[i].Status;
                    worksheet.Cells[i + 3, 13].Value = requestData[i].Notes;
                    worksheet.Cells[i + 3, 14].Value = requestData[i].Physician;
                }

                // Convert the Excel package to a byte array
                byte[] excelBytes = package.GetAsByteArray();

                // Return the Excel file as a download
                return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            }
        }

        #endregion


        #region Request Support

        public IActionResult RequestSupport()
        {
            return PartialView("Admin/_Admin_RequestSupport");
        }

        public IActionResult RequestDTYPost(ModalCm modalCm)
        {
            var user = _sessionUtils.GetUser(HttpContext.Session);
            int adminId = user.UserId;

            if (_adminDashboard.RequestSupportViaEmail(modalCm.RequestSupportMessage, adminId))
            {
                return Ok(true);
            }

            return Ok(false);

        }

        #endregion


        #region Close Case

        public IActionResult CloseCase(int requestid, int status)
        {
            AdminCloseCaseCm adminCloseCaseCm = new AdminCloseCaseCm();
            adminCloseCaseCm = _adminDashboard.GetCloseCaseData(requestid);
            adminCloseCaseCm.file = _adminDashboard.GetCloseCaseDocuments(requestid);
            adminCloseCaseCm.statusForName = status;

            return PartialView("Admin/_Admin_CloseCase", adminCloseCaseCm);
        }

        [HttpPost]
        public IActionResult UpdateCloseCase(AdminCloseCaseCm adminCloseCaseCm)
        {
            _adminDashboard.UpdateCloseCaseData(adminCloseCaseCm);

            return Ok();
        }

        [HttpPost]
        public IActionResult PostCloseCase(int requestId)
        {
            var user = _sessionUtils.GetUser(HttpContext.Session);
            int aspId = user.AspNetUserId;

            _adminDashboard.SetClosedCase(requestId, aspId);

            return Ok();
        }

        #endregion


        #region Admin Encounter

        public IActionResult AdminEncounter(int requestid, int status, int callId)
        {
            EncounterCm encounterCm = new EncounterCm();
            encounterCm = _adminDashboard.GetEncounterData(requestid, status);
            encounterCm.callId = callId;
            encounterCm.statusForName = status;

            return PartialView("Admin/_Admin_Encounter", encounterCm);

        }

        [HttpPost]
        public IActionResult SubmitEncounter(EncounterCm encounterCm)
        {
            _adminDashboard.SetEncounterData(encounterCm);

            return Json(new { reqId = encounterCm.reqid, status = encounterCm.statusForName, callid = encounterCm.callId });
        }

        #endregion


        #endregion


        #region Profile

        [CustomAuthorize("AdminProfile", "Admin")]
        public IActionResult GetAdminProfile(int aspnetId, int callId)
        {
            var user = _sessionUtils.GetUser(HttpContext.Session);
            int aspId = aspnetId == 0 ? user.AspNetUserId : aspnetId;

            AdminProfileCm adminProfileCm = _adminDashboard.GetProfileData(aspId);
            adminProfileCm.Regions = _adminDashboard.GetRegions();
            adminProfileCm.Roles = _adminDashboard.GetRoles(aspId);
            adminProfileCm.AdminRegions = _adminDashboard.GetAdminregions(aspId);
            adminProfileCm.callId = callId;

            return PartialView("Admin/_Admin_Profile", adminProfileCm);
        }

        [CustomAuthorize("AdminProfile", "Admin")]
        [HttpPost]
        public IActionResult AdminResetPassword(AdminProfileCm adminProfileCm)
        {
            if (adminProfileCm.Password != null)
            {
                _adminDashboard.AdminResetPassword(adminProfileCm.Password, adminProfileCm.AspId);
                return Ok(true);
            }
            return Ok(false);
        }

        [CustomAuthorize("AdminProfile", "Admin")]
        [HttpPost]
        public IActionResult AdminAccountEdit(AdminProfileCm adminProfileCm)
        {
            _adminDashboard.AdminAccountUpdate((short)adminProfileCm.Status, (int)adminProfileCm.RoleId, adminProfileCm.AspId);

            return Ok();
        }

        [CustomAuthorize("AdminProfile", "Admin")]
        [HttpPost]
        public IActionResult AdministratorDetail(AdminProfileCm adminProfileCm, List<int> regions)
        {
            if (adminProfileCm != null)
            {
                if (adminProfileCm.Email == adminProfileCm.ConfirmEmail)
                {
                    _adminDashboard.AdministratorDetail(adminProfileCm, regions);
                    return Json(new { success = true });
                }

                return Json(new { success = false });
            }

            return Json(new { Error = "Returned in else" });
        }

        [CustomAuthorize("AdminProfile", "Admin")]
        [HttpPost]
        public IActionResult MailingDetail(AdminProfileCm adminProfileCm)
        {
            if (adminProfileCm != null)
            {
                _adminDashboard.MailingDetail(adminProfileCm);
                return Ok();
            }

            return Json(new { Error = "Returned in else" });
        }

        [CustomAuthorize("AdminProfile", "Admin")]
        [HttpPost]
        public IActionResult DeleteAdminAccount(int adminId)
        {
            _adminDashboard.RemoveAdmin(adminId);

            return Ok();
        }

        #endregion


        #region Provider

        [CustomAuthorize("Provider", "Admin")]
        public IActionResult GetProvider(int regionId)
        {
            ProvidersCm providersCm = new ProvidersCm();
            providersCm.Regions = _adminDashboard.GetRegions();
            providersCm.Providers = _adminDashboard.GetProviders(regionId);

            return PartialView("Admin/Provider/_Providers", providersCm);
        }

        [CustomAuthorize("Provider", "Admin")]
        public IActionResult ContactProvider()
        {
            return Ok();
        }

        [CustomAuthorize("Provider", "Admin")]
        public IActionResult SendEmailToProvider(ProvidersCm providersCm)
        {
            var user = _sessionUtils.GetUser(HttpContext.Session);
            int aspId = user.AspNetUserId;

            _adminDashboard.ContactProvider(providersCm, aspId);

            return Ok();
        }

        [CustomAuthorize("ProviderProfile", "Admin", "Provider")]
        public IActionResult GetEditProvider(int aspId, int callId)
        {
            if (aspId == 0)
            {
                var user = _sessionUtils.GetUser(HttpContext.Session);
                aspId = user.AspNetUserId;
            }

            ProviderProfileCm providerProfileCm = _adminDashboard.GetProviderProfileData(aspId);
            providerProfileCm.Regions = _adminDashboard.GetRegions();
            providerProfileCm.Roles = _adminDashboard.GetRoles(aspId);
            providerProfileCm.PhysicianRegionTables = _adminDashboard.GetPhysicianRegionTables(aspId);
            providerProfileCm.callId = callId;

            return PartialView("Admin/Provider/_Provider_ProfileEdit", providerProfileCm);
        }

        [CustomAuthorize("ProviderProfile", "Admin", "Provider")]
        public IActionResult PhysicianProfileResetPassword(ProviderProfileCm providerProfileCm)
        {
            if (providerProfileCm.Password != null)
            {
                _adminDashboard.PhysicianResetPassword(providerProfileCm.Password, providerProfileCm.AspId);

                return Json(new { Success = true, aspId = providerProfileCm.AspId });
            }

            return Json(new { Success = false });
        }

        [CustomAuthorize("ProviderProfile", "Admin", "Provider")]
        public IActionResult PhysicianAccountEdit(ProviderProfileCm providerProfileCm)
        {
            _adminDashboard.PhysicianAccountUpdate((short)providerProfileCm.Status, (int)providerProfileCm.RoleId, providerProfileCm.AspId);

            return Ok(providerProfileCm.AspId);
        }

        [CustomAuthorize("ProviderProfile", "Admin", "Provider")]
        public IActionResult PhysicianAdministratorEdit(ProviderProfileCm providerProfileCm, List<int> physicianRegions)
        {
            _adminDashboard.PhysicianAdministratorDataUpdate(providerProfileCm, physicianRegions);

            return Ok(providerProfileCm.AspId);
        }

        [CustomAuthorize("ProviderProfile", "Admin", "Provider")]
        public IActionResult PhysicianMailingEdit(ProviderProfileCm providerProfileCm)
        {
            _adminDashboard.PhysicianMailingDataUpdate(providerProfileCm);

            return Ok(providerProfileCm.AspId);
        }

        [CustomAuthorize("ProviderProfile", "Admin", "Provider")]
        public IActionResult PhysicianBusinessInfoEdit(ProviderProfileCm providerProfileCm)
        {
            _adminDashboard.PhysicianBusinessInfoUpdate(providerProfileCm);

            return Ok(providerProfileCm.AspId);
        }

        [CustomAuthorize("ProviderProfile", "Admin", "Provider")]
        public IActionResult UpdateOnBoarding(ProviderProfileCm providerProfileCm)
        {
            _adminDashboard.EditOnBoardingData(providerProfileCm);

            return Ok(providerProfileCm.AspId);
        }

        [CustomAuthorize("ProviderProfile", "Admin", "Provider")]
        public IActionResult DeletePhysicianAccount(int physicianId)
        {
            _adminDashboard.RemovePhysician(physicianId);

            return Ok();
        }

        [CustomAuthorize("ProviderProfile", "Admin", "Provider")]
        public IActionResult CreateProviderAccount(int callId)
        {
            var user = _sessionUtils.GetUser(HttpContext.Session);
            int aspId = user.AspNetUserId;

            ProviderProfileCm providerProfileCm = new ProviderProfileCm();
            providerProfileCm.Roles = _adminDashboard.GetRoles(19);
            providerProfileCm.Regions = _adminDashboard.GetRegions();
            providerProfileCm.AspId = aspId;
            providerProfileCm.callId = callId;

            return PartialView("Admin/Provider/_Provider_CreateProviderAccount", providerProfileCm);
        }

        [CustomAuthorize("ProviderProfile", "Admin", "Provider")]
        public IActionResult CreateProviderAccountPost(ProviderProfileCm providerProfileCm, List<int> physicianRegions)
        {
            if (_adminDashboard.CreatePhysicianAccount(providerProfileCm, physicianRegions))
            {
                return Json(new { Success = true });
            }
            return Json(new { Success = false });
        }

        #endregion


        #region Account Access

        [CustomAuthorize("AccountAccess", "Admin")]
        public IActionResult GetAccountAccess()
        {
            AdminAccountAccessCm adminAccessCm = new AdminAccountAccessCm();
            adminAccessCm.AccountAccess = _adminDashboard.GetAccountAccessData();

            return PartialView("Admin/Access/_Admin_AccountAccess", adminAccessCm);
        }

        [CustomAuthorize("AccountAccess", "Admin")]
        public IActionResult GetCreateAccess()
        {
            AdminAccountAccessCm adminAccessCm = new AdminAccountAccessCm
            {
                Aspnetroles = _adminDashboard.GetAccountType(),
                Menus = _adminDashboard.GetMenu(0),
            };

            return PartialView("Admin/Access/_Admin_AccountAccessCreate", adminAccessCm);
        }

        [CustomAuthorize("AccountAccess", "Admin")]
        public IActionResult FilterRolesMenu(int accounttype)
        {
            var menu = _adminDashboard.GetMenu(accounttype);

            return Json(menu);
        }

        [CustomAuthorize("AccountAccess", "Admin")]
        public IActionResult FilterEditRolesMenu(int accounttypeid, int roleid)
        {
            var menu = _adminDashboard.GetAccountMenu(accounttypeid, roleid);

            return Json(menu);
        }

        [CustomAuthorize("AccountAccess", "Admin")]
        [HttpPost]
        public IActionResult SetCreateAccessAccount(AdminAccountAccessCm adminAccessCm, List<int> AccountMenu)
        {
            _adminDashboard.SetCreateAccessAccount(adminAccessCm.CreateAccountAccess, AccountMenu);

            return Ok();
        }

        [CustomAuthorize("AccountAccess", "Admin")]
        public IActionResult GetEditAccess(int accounttypeid, int roleid)
        {
            var roledata = _adminDashboard.GetEditAccessData(roleid);
            var Accounttype = _adminDashboard.GetAccountType();
            var menu = _adminDashboard.GetAccountMenu(accounttypeid, roleid);
            AdminAccountAccessCm adminAccessCm = new AdminAccountAccessCm
            {
                Aspnetroles = Accounttype,
                AccountMenu = menu,
                CreateAccountAccess = roledata,
            };
            return PartialView("Admin/Access/_Admin_AccountAccessEdit", adminAccessCm);
        }

        [CustomAuthorize("AccountAccess", "Admin")]
        [HttpPost]
        public IActionResult SetEditAccessAccount(AdminAccountAccessCm adminAccessCm, List<int> AccountMenu)
        {
            _adminDashboard.SetEditAccessAccount(adminAccessCm.CreateAccountAccess, AccountMenu);

            return Ok();
        }

        [CustomAuthorize("AccountAccess", "Admin")]
        [HttpPost]
        public IActionResult DeleteAccountAccess(int roleid)
        {
            _adminDashboard.DeleteAccountAccess(roleid);

            return Ok();
        }

        #endregion


        #region User Access

        [CustomAuthorize("UserAccess", "Admin")]
        public IActionResult GetUserAccess(int accountTypeId)
        {
            AdminUserAccessCm adminUserAccessCm = new AdminUserAccessCm();
            adminUserAccessCm.Aspnetroles = _adminDashboard.GetAccountType();
            adminUserAccessCm.UserAccesses = _adminDashboard.GetUserAccessData(accountTypeId);

            return PartialView("Admin/Access/_Admin_UserAccess", adminUserAccessCm);
        }

        [CustomAuthorize("UserAccess", "Admin")]
        public IActionResult GetCreateAdminAccount(int callId)
        {
            var user = _sessionUtils.GetUser(HttpContext.Session);
            int aspId = user.AspNetUserId;

            AdminProfileCm adminProfileCm = new AdminProfileCm();
            adminProfileCm.Roles = _adminDashboard.GetRoles(aspId);
            adminProfileCm.Regions = _adminDashboard.GetRegions();
            adminProfileCm.AspId = aspId;
            adminProfileCm.callId = callId;

            return PartialView("Admin/_Admin_CreateAccount", adminProfileCm);
        }

        [CustomAuthorize("UserAccess", "Admin")]
        public IActionResult CreateAdminAccountPost(AdminProfileCm adminProfileCm, List<int> adminRegions)
        {
            if (_adminDashboard.CreateAdminAccount(adminProfileCm, adminRegions))
            {
                return Json(new { Success = true });
            }
            return Json(new { Success = false });
        }

        #endregion


        #region Scheduling

        [CustomAuthorize("Scheduling", "Admin", "Provider")]
        public IActionResult GetScheduling(int callId)
        {
            SchedulingCm schedulingCm = new SchedulingCm()
            {
                regions = _adminDashboard.GetRegions(),
                callId = callId,
            };
            return PartialView("Admin/Scheduling/_Scheduling", schedulingCm);
        }

        [CustomAuthorize("Scheduling", "Admin", "Provider")]
        public IActionResult CreateNewShift(int callId)
        {
            SchedulingCm schedulingCm = new SchedulingCm();

            if (callId == 1)
            {
                schedulingCm.regions = _adminDashboard.GetRegions();
                schedulingCm.callId = 1;
            }
            else if (callId == 2)
            {
                var user = _sessionUtils.GetUser(HttpContext.Session);
                int physicianId = user == null ? 0 : user.UserId;

                schedulingCm.regions = _adminDashboard.GetPhysicianRegions(physicianId);
                schedulingCm.callId = 2;
                schedulingCm.physicianId = physicianId;
            }

            return PartialView("Admin/Scheduling/_CreateShift", schedulingCm);
        }

        [HttpPost]
        [CustomAuthorize("Scheduling", "Admin", "Provider")]
        public IActionResult CreateShiftPost(SchedulingCm schedulingCm)
        {
            var user = _sessionUtils.GetUser(HttpContext.Session);
            int aspId = user.AspNetUserId;

            if (_adminDashboard.createShift(schedulingCm.ScheduleModel, aspId))
            {
                return Ok(true);
            }

            return Ok(false);
        }

        [HttpPost]
        [CustomAuthorize("Scheduling", "Admin", "Provider")]
        public IActionResult Loadshift(string datestring, string sundaystring, string saturdaystring, string shifttype, int regionid)
        {
            DateTime date = DateTime.Parse(datestring);
            DateTime sunday = DateTime.Parse(sundaystring);
            DateTime saturday = DateTime.Parse(saturdaystring);

            var user = _sessionUtils.GetUser(HttpContext.Session);
            int aspId = user == null ? 0 : user.AspNetUserId;

            switch (shifttype)
            {
                case "month":
                    MonthShiftModal monthShift = new MonthShiftModal();

                    var totalDays = DateTime.DaysInMonth(date.Year, date.Month);
                    var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
                    var startDayIndex = (int)firstDayOfMonth.DayOfWeek;
                    var dayceiling = (int)Math.Ceiling((float)(totalDays + startDayIndex) / 7);

                    monthShift.daysLoop = (int)dayceiling * 7;
                    monthShift.daysInMonth = totalDays;
                    monthShift.firstDayOfMonth = firstDayOfMonth;
                    monthShift.startDayIndex = startDayIndex;
                    monthShift.Physicians = _adminDashboard.GetPhysicians(regionid);

                    if (regionid == 0)
                    {
                        monthShift.shiftDetailsmodals = _adminDashboard.ShiftDetailsmodal(date, sunday, saturday, "month", aspId);
                    }
                    else
                    {
                        monthShift.shiftDetailsmodals = _adminDashboard.ShiftDetailsmodal(date, sunday, saturday, "month", aspId).Where(i => i.Regionid == regionid).ToList();
                    }

                    return PartialView("Admin/Scheduling/_MonthWiseShift", monthShift);

                case "week":

                    WeekShiftModal weekShift = new WeekShiftModal();

                    weekShift.Physicians = _adminDashboard.GetPhysicians(regionid);
                    weekShift.shiftDetailsmodals = _adminDashboard.ShiftDetailsmodal(date, sunday, saturday, "week", aspId);

                    List<int> dlist = new List<int>();

                    for (var i = 0; i < 7; i++)
                    {
                        var date12 = sunday.AddDays(i);
                        dlist.Add(date12.Day);
                    }

                    weekShift.datelist = dlist.ToList();
                    weekShift.dayNames = new string[] { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };

                    return PartialView("Admin/Scheduling/_WeekWiseShift", weekShift);

                case "day":

                    DayShiftModal dayShift = new DayShiftModal();
                    dayShift.Physicians = _adminDashboard.GetPhysicians(regionid);
                    dayShift.shiftDetailsmodals = _adminDashboard.ShiftDetailsmodal(date, sunday, saturday, "day", aspId);

                    return PartialView("Admin/Scheduling/_DayWiseShift", dayShift);

                default:
                    return PartialView();
            }

        }

        [CustomAuthorize("Scheduling", "Admin", "Provider")]
        public IActionResult OpenScheduledModal(ViewShiftModal viewShiftModal)
        {
            var user = _sessionUtils.GetUser(HttpContext.Session);
            int aspId = user == null ? 0 : user.AspNetUserId;

            switch (viewShiftModal.actionType)
            {
                case "shiftdetails":
                    ShiftDetailsmodal shift = _adminDashboard.GetShift(viewShiftModal.shiftdetailsid);
                    return PartialView("Admin/Scheduling/_ViewShift", shift);

                case "moreshifts":
                    DateTime date = DateTime.Parse(viewShiftModal.datestring);

                    ShiftDetailsmodal ScheduleModel = new ShiftDetailsmodal();
                    ScheduleModel.ViewAllList = _adminDashboard.ShiftDetailsmodal(date, DateTime.Now, DateTime.Now, "month", aspId).Where(i => i.Shiftdate.Day == viewShiftModal.columnDate.Day).ToList();

                    ViewBag.TotalShift = ScheduleModel.ViewAllList.Count();
                    return PartialView("Admin/Scheduling/_MoreShift", ScheduleModel);


                default:

                    return PartialView();
            }
        }

        [CustomAuthorize("Scheduling", "Admin", "Provider")]
        public IActionResult OpenScheduledModalWeek(string sundaystring, string saturdaystring, string datestring, DateTime shiftdate, int physicianid)
        {
            var user = _sessionUtils.GetUser(HttpContext.Session);
            int aspId = user == null ? 0 : user.AspNetUserId;

            DateTime sunday = DateTime.Parse(sundaystring);
            DateTime saturday = DateTime.Parse(saturdaystring);
            DateTime date1 = DateTime.Parse(datestring);

            ShiftDetailsmodal ScheduleModel = new ShiftDetailsmodal();
            ScheduleModel.ViewAllList = _adminDashboard.ShiftDetailsmodal(date1, sunday, saturday, "week", aspId).Where(i => i.Shiftdate.Day == shiftdate.Day && i.Physicianid == physicianid).ToList();
            ViewBag.TotalShift = ScheduleModel.ViewAllList.Count();

            return PartialView("Admin/Scheduling/_MoreShift", ScheduleModel);


        }

        [CustomAuthorize("Scheduling", "Admin", "Provider")]
        public IActionResult ReturnShift(int status, int shiftdetailid)
        {
            var user = _sessionUtils.GetUser(HttpContext.Session);
            int aspId = user.AspNetUserId;

            _adminDashboard.SetReturnShift(status, shiftdetailid, aspId);
            return Ok();
        }

        [CustomAuthorize("Scheduling", "Admin", "Provider")]
        public IActionResult DeleteShift(int shiftdetailid)
        {
            var user = _sessionUtils.GetUser(HttpContext.Session);
            int aspId = user.AspNetUserId;

            _adminDashboard.SetDeleteShift(shiftdetailid, aspId);
            return Ok();
        }

        [CustomAuthorize("Scheduling", "Admin", "Provider")]
        public IActionResult EditShiftDetails(ShiftDetailsmodal shiftDetailsmodal)
        {
            var user = _sessionUtils.GetUser(HttpContext.Session);
            int aspId = user.AspNetUserId;

            if (_adminDashboard.SetEditShift(shiftDetailsmodal, aspId))
            {
                return Ok(true);
            }
            return Ok(false);
        }

        [CustomAuthorize("Scheduling", "Admin", "Provider")]
        public IActionResult ShiftReview(int regionId, int callId)
        {
            SchedulingCm schedulingCm = new SchedulingCm()
            {
                regions = _adminDashboard.GetRegions(),
                ShiftReview = _adminDashboard.GetShiftReview(regionId, callId),
                regionId = regionId,
                callId = callId,
            };

            return PartialView("Admin/Scheduling/_ShiftReview", schedulingCm);
        }

        [CustomAuthorize("Scheduling", "Admin", "Provider")]
        public IActionResult ApproveShift(int[] shiftDetailsId)
        {
            var user = _sessionUtils.GetUser(HttpContext.Session);
            int aspId = user.AspNetUserId;

            _adminDashboard.ApproveSelectedShift(shiftDetailsId, aspId);

            return Ok();
        }

        [CustomAuthorize("Scheduling", "Admin", "Provider")]
        public IActionResult DeleteSelectedShift(int[] shiftDetailsId)
        {
            var user = _sessionUtils.GetUser(HttpContext.Session);
            int aspId = user.AspNetUserId;

            _adminDashboard.DeleteShiftReview(shiftDetailsId, aspId);

            return Ok();
        }

        [CustomAuthorize("Scheduling", "Admin", "Provider")]
        public IActionResult MDOnCall(int regionId)
        {
            var MdsCallModal = _adminDashboard.GetOnCallDetails(regionId);

            return PartialView("Admin/Scheduling/_MDsOnCall", MdsCallModal);
        }

        #endregion


        #region Provider Location

        [CustomAuthorize("ProviderLocation", "Admin")]
        public IActionResult ProviderLocation()
        {
            return PartialView("Admin/ProviderLocation/_Provider_Location");
        }

        [CustomAuthorize("ProviderLocation", "Admin")]
        public IActionResult GetLocations()
        {
            List<Physicianlocation> getLocation = _adminDashboard.GetPhysicianlocations();
            return Ok(getLocation);
        }

        #endregion


        #region Partners

        [CustomAuthorize("Partners", "Admin")]
        public IActionResult Partners(int professionid)
        {
            var Partnersdata = _adminDashboard.GetPartnersdata(professionid);
            PartnersCm partnersCM = new PartnersCm
            {
                Partnersdata = Partnersdata,
                Professions = _adminDashboard.GetProfession(),
            };
            return PartialView("Admin/Partners/_Partners", partnersCM);
        }

        [CustomAuthorize("Partners", "Admin")]
        public IActionResult AddBusiness(int vendorID)
        {
            if (vendorID == 0)
            {
                PartnersCm partnersCM = new PartnersCm
                {
                    Professions = _adminDashboard.GetProfession(),
                    regions = _adminDashboard.GetRegions(),
                    vendorID = vendorID,
                };

                return PartialView("Admin/Partners/_Partners_Create_Edit", partnersCM);
            }
            else
            {
                PartnersCm partnersCM = _adminDashboard.GetEditBusinessData(vendorID);
                partnersCM.Professions = _adminDashboard.GetProfession();
                partnersCM.regions = _adminDashboard.GetRegions();
                partnersCM.vendorID = vendorID;
                return PartialView("Admin/Partners/_Partners_Create_Edit", partnersCM);
            }

        }

        [CustomAuthorize("Partners", "Admin")]
        public IActionResult CreateNewBusiness(PartnersCm partnersCM)
        {
            var user = _sessionUtils.GetUser(HttpContext.Session);
            int aspId = user.AspNetUserId;

            if (_adminDashboard.CreateNewBusiness(partnersCM, aspId))
            {
                return Json(new { Success = true });
            }
            return Json(new { Success = false });
        }

        [CustomAuthorize("Partners", "Admin")]
        public IActionResult UpdateBusiness(PartnersCm partnersCM)
        {
            if (_adminDashboard.UpdateBusiness(partnersCM))
            {
                return Json(new { Success = true, vendorid = partnersCM.vendorID });
            }
            return Json(new { Success = false, vendorid = partnersCM.vendorID });
        }

        [CustomAuthorize("Partners", "Admin")]
        [HttpPost]
        public IActionResult DelPartner(int VendorID)
        {
            _adminDashboard.DelPartner(VendorID);
            return Ok();
        }

        #endregion


        #region Patient Records

        [CustomAuthorize("PatientRecords", "Admin")]
        public IActionResult GetRecordsTab(GetRecordsModel getRecordsModel)
        {
            getRecordsModel = _adminDashboard.GetRecordsTab(0, getRecordsModel);

            return PartialView("Admin/Records/_GetRecordsTab", getRecordsModel);
        }

        [CustomAuthorize("PatientRecords", "Admin")]
        public IActionResult GetPatientRecordExplore(int UserId)
        {
            GetRecordsModel model = new GetRecordsModel();
            model = _adminDashboard.GetRecordsTab(UserId, model);
            return PartialView("Admin/Records/_ExploreRecords", model);
        }

        #endregion


        #region Search Records

        [CustomAuthorize("SearchRecords", "Admin")]
        public IActionResult GetSearchRecords(SearchRecordsModel searchRecordsModel)
        {

            searchRecordsModel = _adminDashboard.GetSearchRecords(searchRecordsModel);
            searchRecordsModel.requestType = _adminDashboard.GetRequesttypes();

            return PartialView("Admin/Records/_SearchRecords", searchRecordsModel);
        }

        [CustomAuthorize("SearchRecords", "Admin")]
        public IActionResult ExportSearchRecords(SearchRecordsModel searchRecordsModel)
        {
            var requests = _adminDashboard.GetSearchRecords(searchRecordsModel);

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var excelPackage = new ExcelPackage())
            {
                var worksheet = excelPackage.Workbook.Worksheets.Add("SearchRecords");

                worksheet.Cells[1, 1].Value = "Request ID";
                worksheet.Cells[1, 2].Value = "Patient Name";
                worksheet.Cells[1, 3].Value = "Requestor";
                worksheet.Cells[1, 4].Value = "Date of Service";
                worksheet.Cells[1, 5].Value = "Close Case Date";
                worksheet.Cells[1, 6].Value = "Email";
                worksheet.Cells[1, 7].Value = "Contact";
                worksheet.Cells[1, 8].Value = "Address";
                worksheet.Cells[1, 9].Value = "Zip";
                worksheet.Cells[1, 10].Value = "Status";
                worksheet.Cells[1, 11].Value = "Physician";
                worksheet.Cells[1, 12].Value = "Physician Note";
                worksheet.Cells[1, 13].Value = "Provider Note";
                worksheet.Cells[1, 14].Value = "Admin Note";
                worksheet.Cells[1, 15].Value = "Patient Note";
                worksheet.Cells[1, 16].Value = "Request Type ID";
                worksheet.Cells[1, 17].Value = "User ID";

                for (int i = 0; i < requests.requestList.Count; i++)
                {
                    var rowData = requests.requestList[i];
                    worksheet.Cells[i + 2, 1].Value = rowData.requestid;
                    worksheet.Cells[i + 2, 2].Value = rowData.patientname;
                    worksheet.Cells[i + 2, 3].Value = rowData.requestor;
                    worksheet.Cells[i + 2, 4].Value = rowData.dateOfService;
                    worksheet.Cells[i + 2, 5].Value = rowData.closeCaseDate;
                    worksheet.Cells[i + 2, 6].Value = rowData.email;
                    worksheet.Cells[i + 2, 7].Value = rowData.contact;
                    worksheet.Cells[i + 2, 8].Value = rowData.address;
                    worksheet.Cells[i + 2, 9].Value = rowData.zip;
                    worksheet.Cells[i + 2, 10].Value = rowData.status;
                    worksheet.Cells[i + 2, 11].Value = rowData.physician;
                    worksheet.Cells[i + 2, 12].Value = rowData.physicianNote;
                    worksheet.Cells[i + 2, 13].Value = rowData.providerNote;
                    worksheet.Cells[i + 2, 14].Value = rowData.AdminNote;
                    worksheet.Cells[i + 2, 15].Value = rowData.pateintNote;
                    worksheet.Cells[i + 2, 16].Value = rowData.requestTypeId;
                    worksheet.Cells[i + 2, 17].Value = rowData.userid;
                }

                byte[] excelBytes = excelPackage.GetAsByteArray();

                return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            }

        }

        [CustomAuthorize("SearchRecords", "Admin")]
        public IActionResult DeletRequest(int requestId)
        {
            _adminDashboard.DeletRequest(requestId);
            return Ok();
        }

        #endregion


        #region Email & SMS Log

        [CustomAuthorize("EmailLogs", "Admin")]
        public IActionResult GetEmailSmsLog(EmailSmsLogModel emailSmsLogModel)
        {
            EmailSmsLogModel model = new EmailSmsLogModel()
            {
                recordslist = _adminDashboard.GetEmailSmsLog(emailSmsLogModel),
            };

            return PartialView("Admin/Records/_EmailSmsLog", model);
        }

        [CustomAuthorize("SMSLogs", "Admin")]
        public IActionResult GetSmsLog(EmailSmsLogModel emailSmsLogModel)
        {
            EmailSmsLogModel model = new EmailSmsLogModel()
            {
                recordslist = _adminDashboard.GetSmsLog(emailSmsLogModel),
            };

            return PartialView("Admin/Records/_SmsLog", model);
        }

        #endregion


        #region Block History

        [CustomAuthorize("BlockHistory", "Admin")]
        public IActionResult GetBlockedRequest(BlockedRequestModel model)
        {

            model = _adminDashboard.GetBlockedRequest(model);

            return PartialView("Admin/Records/_BlockedHistory", model);
        }

        [CustomAuthorize("BlockHistory", "Admin")]
        public IActionResult UnblockRequest(int requestId)
        {
            _adminDashboard.UnblockRequest(requestId);
            return Ok();
        }

        #endregion


        #region  Pay Rate

        public IActionResult GetEnterPayrate(int aspId, int phyid)
        {
            var adminAspid = _sessionUtils.GetUser(HttpContext.Session).AspNetUserId;

            PayrateCm payrateCm = new()
            {
                AspId = aspId,
                Phyid = phyid,
                PayrateForProvider = _adminDashboard.GetPayRateForProvider(phyid, adminAspid),
            };

            return PartialView("Provider/_Provider_Payrate", payrateCm);
        }

        public IActionResult PostPayrate(int category, int payrate, int phyid)
        {
            var adminAspId = _sessionUtils.GetUser(HttpContext.Session).AspNetUserId;

            if (_adminDashboard.PostPayrate(category, payrate, phyid, adminAspId))
            {
                return Ok(true);
            }
            return Ok(false);
        }

        #endregion


        #region Invoicing

        public IActionResult GetAdminInvoicing()
        {
            var adminInvoicingCm = new AdminInvoicingCm()
            {
                PhysiciansList = _adminDashboard.GetPhysicians(0),
            };

            return PartialView("Admin/Invoicing/_Invoicing", adminInvoicingCm);
        }

        public IActionResult GetTimeSheetDetail(int phyid, string dateSelected)
        {
            var adminInvoicingCm = new AdminInvoicingCm
            {
                PhysiciansList = _adminDashboard.GetPhysicians(0),
                TimesheetsList = _adminDashboard.GetTimeSheetDetail(phyid, dateSelected),
                Timesheetdetails = _providerDashboard.GetTimeSheetDetails(phyid, dateSelected),
                Timesheetdetailreimbursements = _providerDashboard.GetTimeSheetDetailsReimbursements(phyid, dateSelected),
            };

            return PartialView("Admin/Invoicing/_Invoicing", adminInvoicingCm);
        }

        public IActionResult GetAdminFinalizeTimeSheet(string dateSelected, int phyid)
        {
            ProviderInvoicingCm? providerInvoicingCm = new()
            {
                ProviderTimesheetDetails = _providerDashboard.GetFinalizeTimeSheetDetails(phyid, dateSelected),
                PayrateByProvider = _adminDashboard.GetPayRateForProviderByPhyId(phyid),
                CallId = 1,
                DateSelected = dateSelected,
                PhysicianId = phyid,
            };

            return PartialView("Provider/_Provider_FinalizeTimeSheet", providerInvoicingCm);
        }

        public IActionResult ConfirmApproveTimeSheet(int timeSheetId, int bonus, string notes)
        {
            if (_adminDashboard.ApproveTimeSheet(timeSheetId, bonus, notes))
            {
                return Ok(true);
            }
            return Ok(false);
        }

        #endregion


        public IActionResult ChatPersonDetails(int Reqid,int ReciverAspid)
        {
            var senderId = _sessionUtils.GetUser(HttpContext.Session).AspNetUserId;

            ChatCm chatCm = _adminDashboard.GetChatData(Reqid, senderId,ReciverAspid);

            return Ok(chatCm);
        }

        public IActionResult AddChatDetails(int Reqid, int receiverId,string message)
        {
            var senderId = _sessionUtils.GetUser(HttpContext.Session).AspNetUserId;

            _adminDashboard.AddChatHistory(Reqid, senderId, receiverId, message);

            return Ok();
        }

    }

}