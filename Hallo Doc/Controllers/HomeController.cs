using Business_Logic.Interface;
using Business_Logic.Repository;
using Data_Access.Custom_Models;
using Data_Access.Models;
using Hallo_Doc.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Hallo_Doc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILoginService _loginService;
        private readonly IJwtService _jwtService;
        private readonly ISessionUtils _sessionUtils;
        private readonly IPatientRequest _patientRequest;
        private readonly IRegisterService _registerService;

        public HomeController(ILoginService loginService, IJwtService jwtService, ISessionUtils sessionUtils, IPatientRequest patientRequest, IRegisterService registerService)
        {
            _loginService = loginService;
            _jwtService = jwtService;
            _sessionUtils = sessionUtils;
            _patientRequest = patientRequest;
            _registerService = registerService;
        }


        #region Forget Password

        public IActionResult ForgetPassowrd()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ForgetPassowrd(loginCm loginCm)
        {
            if (_loginService.IsLoginValid(loginCm))
            {
                var receiver = loginCm.Email;
                var subject = "Hallo Doc - Reset Password";
                var here = "https://localhost:7077/Home/ResetPassword?email=" + _loginService.Encrypt(loginCm.Email);
                var message = $"Click <a href=\"{here}\">here</a> to reset your password...";

                _patientRequest.EmailSender(receiver, subject, message);

                TempData["success"] = "Email Sent To Reset Password!!";
                return RedirectToAction("Login");
            }

            TempData["error"] = "User Doesn't Exists!!";
            return View();
        }

        #endregion


        #region Reset Password

        public IActionResult ResetPassword(string email)
        {
            RegisterCm registerCm = new RegisterCm();
            registerCm.Email = _loginService.Decrypt(email);

            return View(registerCm);
        }

        [HttpPost]
        public IActionResult ResetPassword(RegisterCm registerCm)
        {

            _registerService.ResetPassword(registerCm);

            TempData["success"] = "Password Updated!!";
            return RedirectToAction("Login");
        }

        #endregion


        #region Login

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(loginCm loginCm)
        {
            if (_loginService.IsLoginValid(loginCm))
            {
                var aspnetuser = _loginService.Login(loginCm);
                _sessionUtils.SetUser(HttpContext.Session, aspnetuser, 0);

                var jwtToken = _jwtService.GenerateJwtToken(aspnetuser);
                Response.Cookies.Append("jwt", jwtToken);

                if (aspnetuser.Roleid == 1)
                {
                    TempData["success"] = "Admin Authenticated!!";
                    return RedirectToAction("AdminDashboard", "Admin");
                }
                else if (aspnetuser.Roleid == 2)
                {
                    TempData["success"] = "Physician Authenticated!!";
                    return RedirectToAction("ProviderDashboard", "Provider");
                }
                else if (aspnetuser.Roleid == 3)
                {
                    TempData["success"] = "User Authenticated!!";
                    return RedirectToAction("PatientDashboard", "Patient");
                }
                return View();
            }
            else
            {
                TempData["error"] = "Invalid Credentials!!";
                return View();
            }

        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            Response.Cookies.Delete("jwt");

            TempData["success"] = "Logged Out Successfully!!";
            return RedirectToAction("Login");
        }

        #endregion


        #region Access

        public IActionResult Access()
        {
            return View();
        }

        public IActionResult AccessDenied()
        {
            return PartialView("_AccessDenied");
        }

        #endregion


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}