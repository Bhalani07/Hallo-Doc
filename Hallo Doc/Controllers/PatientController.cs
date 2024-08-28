using Business_Logic.Interface;
using Business_Logic.Repository;
using Data_Access.Custom_Models;
using Data_Access.Models;
using Hallo_Doc.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Cryptography;

namespace Hallo_Doc.Controllers
{
    public class PatientController : Controller
    {
        private readonly ILoginService _loginService;
        private readonly IPatientRequest _patientRequest;
        private readonly IPatientDashboard _patientDashboard;
        private readonly IRegisterService _registerService;
        private readonly ISessionUtils _sessionUtils;

        public PatientController(ILoginService loginService, IPatientRequest patientRequest, IPatientDashboard patientDashboard, IRegisterService registerService, ISessionUtils sessionUtils)
        {
            _loginService = loginService;
            _patientRequest = patientRequest;
            _patientDashboard = patientDashboard;
            _registerService = registerService;
            _sessionUtils = sessionUtils;
        }


        #region Review Agreement

        public IActionResult ReviewAgreement(string pid)
        {
            int requestId = Int32.Parse(_loginService.Decrypt(pid));

            ReviewAgreementCm reviewAgreementCm = _patientRequest.GetReviewAgreement(requestId);

            if(reviewAgreementCm == null)
            {
                TempData["error"] = "You've Reviewed This Earlier!!";
                return RedirectToAction("Login", "Home");
            }

            return View(reviewAgreementCm);
        }

        public IActionResult Review_Agree(int requestId)
        {
            _patientRequest.AgreeReview(requestId);
            TempData["success"] = "Agreement Agreed!!";

            return RedirectToAction("Login", "Home");
        }

        [HttpPost]
        public IActionResult Review_Cancel(ReviewAgreementCm reviewAgreementCm)
        {
            _patientRequest.CancelReview(reviewAgreementCm);
            TempData["success"] = "Agreement Cancelled!!";

            return RedirectToAction("Login", "Home");
        }

        #endregion


        #region Register

        public IActionResult PatientRegister(string email)
        {
            RegisterCm registerCm = new RegisterCm();
            registerCm.Email = _loginService.Decrypt(email);

            return View(registerCm);
        }

        [HttpPost]
        public IActionResult PatientRegister(RegisterCm registerCm)
        {
            if (_registerService.RegisterUser(registerCm))
            {
                TempData["success"] = "User Registered!!";
                return RedirectToAction("Login", "Home");
            }

            TempData["error"] = "User Already registred!!";
            return RedirectToAction("Login", "Home");

        }

        #endregion


        #region Landing

        public IActionResult PatientLanding()
        {
            return View();
        }

        public IActionResult SubmitRequest()
        {
            return View();
        }

        #endregion


        #region Patient Request

        public IActionResult PatientRequest()
        {
            PatientRequestCm patientRequestCm = new PatientRequestCm();
            patientRequestCm.Regions = _patientRequest.GetRegions();

            return View(patientRequestCm);
        }

        [HttpPost]
        public IActionResult PatientRequest(PatientRequestCm patientRequestCm)
        {
            int ReqTypeId = 1;
            if (_patientRequest.CreatePatientRequest(patientRequestCm, ReqTypeId))
            {
                TempData["success"] = "Request Added";
                return RedirectToAction("PatientLanding");
            }
            else
            {
                TempData["error"] = "Request Doesn't Added";
            }
            return View();
        }

        public IActionResult PatientCheck(string email)
        {
            var aspnetuser = _loginService.CheckAspUser(email);

            if (aspnetuser != null)
            {
                if (aspnetuser.Roleid == 3)
                {
                    return Ok(true);
                }
                return Ok(false);
            }
            return Ok(false);
        }

        public IActionResult IsEmailBlocked(string email)
        {
            var blockedEmail = _loginService.CheckBlockedUser(email, "");

            return Ok(blockedEmail);
        }

        public IActionResult IsPhoneBlocked(string phone)
        {
            var blockedPhone = _loginService.CheckBlockedUser("" , phone);

            return Ok(blockedPhone);
        }

        public IActionResult NotAdminProvider(string email)
        {
            var aspnetuser = _loginService.CheckAspUser(email);

            if (aspnetuser != null)
            {
                if (aspnetuser.Roleid != 3)
                {
                    return Ok(true);
                }
                return Ok(false);
            }
            return Ok(false);
        }

        #endregion


        #region Family Request

        public IActionResult FamilyRequest()
        {
            FamilyRequestCm familyRequestCm = new FamilyRequestCm();
            familyRequestCm.Regions = _patientRequest.GetRegions();

            return View(familyRequestCm);
        }

        [HttpPost]
        public IActionResult FamilyRequest(FamilyRequestCm familyRequestCm)
        {
            int ReqTypeId = 2;
            if (_patientRequest.CreateFamilyRequest(familyRequestCm, ReqTypeId))
            {
                TempData["success"] = "Request Added";
                return RedirectToAction("PatientLanding");
            }
            else
            {
                TempData["error"] = "Request Doesn't Added";
            }
            return View();
        }

        #endregion


        #region Concierge Request

        public IActionResult ConciergeRequest()
        {
            ConciergeRequestCm conciergeRequestCm = new ConciergeRequestCm();
            conciergeRequestCm.Regions = _patientRequest.GetRegions();

            return View(conciergeRequestCm);
        }

        [HttpPost]
        public IActionResult ConciergeRequest(ConciergeRequestCm conciergeRequestCm)
        {
            int ReqTypeId = 3;
            if (_patientRequest.CreateConciergeRequest(conciergeRequestCm, ReqTypeId))
            {
                TempData["success"] = "Request Added";
                return RedirectToAction("PatientLanding");
            }
            else
            {
                TempData["error"] = "Request Doesn't Added";
            }
            return View();
        }

        #endregion


        #region Business Request

        public IActionResult BusinessRequest()
        {
            BusinessRequestCm businessRequestCm = new BusinessRequestCm();
            businessRequestCm.Regions = _patientRequest.GetRegions();

            return View(businessRequestCm);
        }

        [HttpPost]
        public IActionResult BusinessRequest(BusinessRequestCm businessRequestCm)
        {
            int ReqTypeId = 4;
            if (_patientRequest.CreateBusinessRequest(businessRequestCm, ReqTypeId))
            {
                TempData["success"] = "Request Added!!";
                return RedirectToAction("PatientLanding");
            }
            else
            {
                TempData["error"] = "Request Doesn't Added!!";
            }
            return View();
        }

        #endregion


        #region Patient Dashboard

        [CustomAuthorize("", "Patient")]
        public IActionResult PatientDashboard()
        {
            var user = _sessionUtils.GetUser(HttpContext.Session);
            int userId = user.UserId;

            PatientDashboardCm patientDashboardCm = new PatientDashboardCm();
            patientDashboardCm.dashboardData = _patientDashboard.RequestList(userId);
            patientDashboardCm.profileData = _patientDashboard.GetProfileData(userId);
            patientDashboardCm.profileData.Regions = _patientRequest.GetRegions();

            return View(patientDashboardCm);

        }

        [CustomAuthorize("", "Patient")]
        [HttpPost]
        public IActionResult PatientDashboard(PatientDashboardCm patientDashboardCm)
        {
            var user = _sessionUtils.GetUser(HttpContext.Session);
            int userId = user.UserId;

            if (_patientDashboard.SetProfileData(patientDashboardCm.profileData, userId))
            {
                patientDashboardCm.dashboardData = _patientDashboard.RequestList(userId);
                patientDashboardCm.profileData = _patientDashboard.GetProfileData(userId);
                patientDashboardCm.profileData.Regions = _patientRequest.GetRegions();

                TempData["success"] = "Profile Updated!!";

                return View(patientDashboardCm);

            }
            TempData["error"] = "Something Went Wrong!!";
            return View();

        }

        #region Patient Upload

        [CustomAuthorize("", "Patient")]
        public IActionResult PatientViewDocument(int rid)
        {
            HttpContext.Session.SetInt32("_sessionReqId", rid);
            var user = _sessionUtils.GetUser(HttpContext.Session);
            int userId = user.UserId;

            PatientDashboardCm patientDashboardCm = new PatientDashboardCm();
            patientDashboardCm.documentData = _patientDashboard.DocumentList(rid);
            patientDashboardCm.ConfirmationNumber = _patientDashboard.GetConfimationNumber(rid);
            patientDashboardCm.profileData = _patientDashboard.GetProfileData(userId);
            patientDashboardCm.profileData.Regions = _patientRequest.GetRegions();

            return View(patientDashboardCm);
        }

        [CustomAuthorize("", "Patient")]
        [HttpPost]
        public IActionResult PatientViewDocument(PatientDashboardCm patientDashboardCm)
        {
            int reqId = (int)(HttpContext.Session.GetInt32("_sessionReqId"));
            var user = _sessionUtils.GetUser(HttpContext.Session);
            int userId = user.UserId;

            if (_patientDashboard.DashboardUpload(patientDashboardCm, reqId))
            {
                patientDashboardCm.dashboardData = _patientDashboard.RequestList(userId);
                patientDashboardCm.profileData = _patientDashboard.GetProfileData(userId);
                patientDashboardCm.profileData.Regions = _patientRequest.GetRegions();

                TempData["success"] = "Document Uploaded!!";

                return RedirectToAction("PatientDashboard", patientDashboardCm);
            }

            TempData["error"] = "Document can't Uploaded!!";
            return View();
        }

        #endregion


        #region Submit For Me

        [CustomAuthorize("", "Patient")]
        public IActionResult SubmitForMe()
        {
            var user = _sessionUtils.GetUser(HttpContext.Session);
            int userId = user.UserId;

            PatientRequestCm patientRequestCm = new PatientRequestCm();
            patientRequestCm.Regions = _patientRequest.GetRegions();
            patientRequestCm = _patientRequest.GetMeData(userId);

            if (patientRequestCm != null)
            {
                return View(patientRequestCm);
            }

            TempData["error"] = "Your Account is Blocked!!";

            PatientDashboardCm patientDashboardCm = new PatientDashboardCm();
            patientDashboardCm.dashboardData = _patientDashboard.RequestList(userId);
            patientDashboardCm.profileData = _patientDashboard.GetProfileData(userId);
            patientDashboardCm.profileData.Regions = _patientRequest.GetRegions();

            return RedirectToAction("PatientDashboard", patientDashboardCm);
        }

        [CustomAuthorize("", "Patient")]
        [HttpPost]
        public IActionResult SubmitForMe(PatientRequestCm patientRequestCm)
        {
            var user = _sessionUtils.GetUser(HttpContext.Session);
            int userId = user.UserId;

            int reqTypeId = 1;

            if (_patientRequest.CreateForMeData(patientRequestCm, reqTypeId, userId))
            {
                TempData["success"] = "Request Created";

                PatientDashboardCm patientDashboardCm = new PatientDashboardCm();
                patientDashboardCm.dashboardData = _patientDashboard.RequestList(userId);
                patientDashboardCm.profileData = _patientDashboard.GetProfileData(userId);
                patientDashboardCm.profileData.Regions = _patientRequest.GetRegions();

                return RedirectToAction("PatientDashboard", patientDashboardCm);
            }
            else
            {
                TempData["error"] = "Request Doesn't Added";
            }
            return View();
        }

        #endregion


        #region Submit For Else

        [CustomAuthorize("", "Patient")]
        public IActionResult SubmitForOther()
        {
            FamilyRequestCm familyRequestCm = new FamilyRequestCm();
            familyRequestCm.Regions = _patientRequest.GetRegions();

            return View(familyRequestCm);
        }

        [CustomAuthorize("", "Patient")]
        [HttpPost]
        public IActionResult SubmitForOther(FamilyRequestCm familyRequestCm)
        {
            var user = _sessionUtils.GetUser(HttpContext.Session);
            int userId = user.UserId;

            int reqTypeId = 2;

            if (_patientRequest.CreateRequestForOther(familyRequestCm, reqTypeId, userId))
            {
                TempData["success"] = "Request Added";

                PatientDashboardCm patientDashboardCm = new PatientDashboardCm();
                patientDashboardCm.dashboardData = _patientDashboard.RequestList(userId);
                patientDashboardCm.profileData = _patientDashboard.GetProfileData(userId);
                patientDashboardCm.profileData.Regions = _patientRequest.GetRegions();

                return RedirectToAction("PatientDashboard", patientDashboardCm);
            }
            else
            {
                TempData["error"] = "Request Doesn't Added";
            }
            return View();
        }

        #endregion

        #endregion


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


    }
}