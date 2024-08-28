using BCrypt.Net;
using Business_Logic.Interface;
using Data_Access.Custom_Models;
using Data_Access.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic.Repository
{
    public class RegisterService : IRegisterService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILoginService _loginService;

        public RegisterService(ApplicationDbContext context, ILoginService loginService)
        {
            _context = context;
            _loginService = loginService;
        }

        public bool RegisterUser(RegisterCm registerCm)
        {
            var userEmail = _loginService.Decrypt(registerCm.Email);

            var aspUser = _context.Aspnetusers.FirstOrDefault(i => i.Email == userEmail);
            var clientData = _context.Requestclients.FirstOrDefault(u => u.Email == userEmail);

            if(aspUser != null)
            {
                return false;
            }

            if (clientData != null)
            {
                var aspData = new Aspnetuser()
                {
                    Username = userEmail.Substring(0, userEmail.IndexOf("@")),
                    Email = userEmail,
                    Passwordhash = BCrypt.Net.BCrypt.HashPassword(registerCm.Password),
                    Roleid = 3,
                    Createddate = DateTime.Now,
                };

                _context.Aspnetusers.Add(aspData);
                _context.SaveChanges();

                var userData = new User()
                {
                    Firstname = clientData.Firstname,
                    Lastname = clientData.Lastname,
                    Email = userEmail,
                    Mobile = clientData.Phonenumber,
                    Street = clientData.Street,
                    City = clientData.City,
                    State = clientData.State,
                    Regionid = clientData.Regionid,
                    Zipcode = clientData.Zipcode,
                    Strmonth = clientData.Strmonth,
                    Intdate = clientData.Intdate,
                    Intyear = clientData.Intyear,
                    Aspnetuserid = aspData.Id,
                    Createdby = aspData.Id,
                };

                _context.Users.Add(userData);
                _context.SaveChanges();

                aspData.Phonenumber = clientData.Phonenumber;
                aspData.Modifieddate = DateTime.Now;
                _context.SaveChanges();

                Request? request = _context.Requests.FirstOrDefault(i => i.Requestid == clientData.Requestid);

                if (request != null)
                {
                    request.Userid = userData.Userid;
                    request.Modifieddate = DateTime.Now;
                    _context.SaveChanges();
                }

                return true;
            }            

            return false;
        }

        public void ResetPassword(RegisterCm registerCm)
        {
            Aspnetuser? aspnetuser = _context.Aspnetusers.FirstOrDefault(i => i.Email == _loginService.Decrypt(registerCm.Email));

            if (aspnetuser != null)
            {
                aspnetuser.Passwordhash = BCrypt.Net.BCrypt.HashPassword(registerCm.Password);
                _context.SaveChanges();
            }

        }
    }
}
