using Business_Logic.Interface;
using Business_Logic.Repository;
using Data_Access.Custom_Models;
using Data_Access.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Net;
using System.Collections;

namespace Business_Logic.Repository
{
    public class PatientRequest : IPatientRequest
    {
        public readonly ApplicationDbContext _Context;
        public readonly ILoginService _loginService;

        public PatientRequest(ApplicationDbContext context, ILoginService loginService)
        {
            _Context = context;
            _loginService = loginService;
        }

        #region Regions

        public List<Region> GetRegions()
        {
            return _Context.Regions.ToList();
        }

        #endregion


        #region Email Log

        public void EmailLogEntry(string subject, string email, int roleId, string message)
        {
            int? requestId = _Context.Requestclients.OrderBy(i => i.Requestclientid).LastOrDefault(i => i.Email == email).Requestid;
            Request? request = _Context.Requests.FirstOrDefault(i => i.Requestid == requestId);

            var logData = new Emaillog()
            {
                Emailtemplate = message,
                Subjectname = subject,
                Emailid = email,
                Roleid = roleId,
                Createdate = DateTime.Now,
                Sentdate = DateTime.Now,
                Isemailsent = new BitArray(1, true),
                Senttries = 1,
            };

            if (request != null)
            {
                logData.Requestid = request.Requestid;
                logData.Confirmationnumber = request.Confirmationnumber;
            }

            _Context.Emaillogs.Add(logData);
            _Context.SaveChanges();
        }

        #endregion


        #region Request for Patient Form

        public bool CreatePatientRequest(PatientRequestCm patientRequestCm, int ReqTypeId)
        {
            Aspnetuser? aspnetuser = _Context.Aspnetusers.FirstOrDefault(i => i.Email == patientRequestCm.Email);
            var user = _Context.Users.FirstOrDefault(i => i.Email == patientRequestCm.Email);

            if (user == null)
            {
                var aspNetData = new Aspnetuser()
                {
                    Username = patientRequestCm.Email.Substring(0, patientRequestCm.Email.IndexOf("@")),
                    Email = patientRequestCm.Email,
                    Passwordhash = BCrypt.Net.BCrypt.HashPassword(patientRequestCm.Passwordhash),
                    Phonenumber = patientRequestCm.Phone,
                    Createddate = DateTime.Now,
                    Roleid = 3,
                };

                _Context.Aspnetusers.Add(aspNetData);
                _Context.SaveChanges();

                AddUserData(patientRequestCm, aspNetData.Id);
                AddRequestData(patientRequestCm, ReqTypeId);

                return true;
            }
            else
            {
                if (!_Context.Users.Any(i => i.Email == aspnetuser.Email))
                {
                    AddUserData(patientRequestCm, aspnetuser.Id);
                }
                AddRequestData(patientRequestCm, ReqTypeId);

                return true;
            }
        }

        public void AddUserData(PatientRequestCm patientRequestCm, int AspnetuserId)
        {
            var userData = new User()
            {
                Aspnetuserid = AspnetuserId,
                Firstname = patientRequestCm.FirstName,
                Lastname = patientRequestCm.LastName,
                Email = patientRequestCm.Email,
                Mobile = patientRequestCm.Phone,
                Street = patientRequestCm.Street,
                City = patientRequestCm.City,
                State = _Context.Regions.Where(x => x.Regionid == patientRequestCm.RegionId).Select(x => x.Name).FirstOrDefault(),
                Regionid = patientRequestCm.RegionId,
                Zipcode = patientRequestCm.Zipcode,
                Strmonth = patientRequestCm.BirthDate.Month.ToString(),
                Intdate = patientRequestCm.BirthDate.Day,
                Intyear = patientRequestCm.BirthDate.Year,
                Createdby = AspnetuserId,
                Createddate = DateTime.Now,
            };

            _Context.Users.Add(userData);
            _Context.SaveChanges();
        }

        public void AddRequestData(PatientRequestCm patientRequestCm, int ReqTypeId)
        {
            User? user = _Context.Users.FirstOrDefault(i => i.Email == patientRequestCm.Email);

            if (user != null)
            {
                Region? region = _Context.Regions.FirstOrDefault(i => i.Regionid == patientRequestCm.RegionId);

                int requestCount = _Context.Requests.Count(i => i.Createddate.Date == DateTime.Now.Date) + 1;

                string confirmation = region.Abbreviation + DateTime.Now.Day.ToString("D2") + DateTime.Now.Month.ToString("D2") + DateTime.Now.Year.ToString().Substring(2, 2) + user.Lastname.Remove(2).ToUpper() + user.Firstname.Remove(2).ToUpper() + requestCount.ToString("D4");

                var reqData = new Request()
                {
                    Requesttypeid = ReqTypeId,
                    Userid = user == null ? null : user.Userid,
                    Firstname = patientRequestCm.FirstName,
                    Lastname = patientRequestCm.LastName,
                    Email = patientRequestCm.Email,
                    Phonenumber = patientRequestCm.Phone,
                    Status = 1,
                    Confirmationnumber = confirmation,
                    Createddate = DateTime.Now,
                };

                _Context.Requests.Add(reqData);
                _Context.SaveChanges();

                AddRequestClientData(patientRequestCm, reqData.Requestid);

                if (patientRequestCm.Upload != null)
                {
                    UploadFile(patientRequestCm, reqData.Requestid);
                }
            }
        }

        public void AddRequestClientData(PatientRequestCm patientRequestCm, int requestId)
        {
            var reqClientData = new Requestclient()
            {
                Requestid = requestId,
                Firstname = patientRequestCm.FirstName,
                Lastname = patientRequestCm.LastName,
                Email = patientRequestCm.Email,
                Phonenumber = patientRequestCm.Phone,
                Notes = patientRequestCm.Symptons,
                Intdate = patientRequestCm.BirthDate.Day,
                Strmonth = patientRequestCm.BirthDate.Month.ToString(),
                Intyear = patientRequestCm.BirthDate.Year,
                Street = patientRequestCm.Street,
                City = patientRequestCm.City,
                State = _Context.Regions.Where(x => x.Regionid == patientRequestCm.RegionId).Select(x => x.Name).FirstOrDefault(),
                Regionid = patientRequestCm.RegionId,
                Zipcode = patientRequestCm.Zipcode,
                Location = patientRequestCm.Room,
                Address = patientRequestCm.Room + " " + patientRequestCm.City + " " + _Context.Regions.Where(x => x.Regionid == patientRequestCm.RegionId).Select(x => x.Name).FirstOrDefault() + " " + patientRequestCm.Zipcode,
            };

            _Context.Requestclients.Add(reqClientData);
            _Context.SaveChanges();
        }

        public void UploadFile(PatientRequestCm patientRequestCm, int requestId)
        {
            IFormFile File1 = patientRequestCm.Upload;

            var fileName = Path.GetFileNameWithoutExtension(File1.FileName) + "_" + DateTime.Now.Date.ToString("yyyy-MM-dd") + "_" + DateTime.Now.ToString("HH-mm-ss") + Path.GetExtension(File1.FileName);

            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "content", fileName);

            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                File1.CopyTo(fileStream);
            }

            var reqWiseFileData = new Requestwisefile()
            {
                Requestid = requestId,
                Filename = fileName,
                Createddate = DateTime.Now,
            };

            _Context.Add(reqWiseFileData);
            _Context.SaveChanges();
        }

        #endregion


        #region Request for Family Form

        public bool CreateFamilyRequest(FamilyRequestCm familyRequestCm, int ReqTypeId)
        {
            Aspnetuser? aspnetuser = _Context.Aspnetusers.FirstOrDefault(i => i.Email == familyRequestCm.Email);
            var user = _Context.Users.FirstOrDefault(i => i.Email == familyRequestCm.Email);

            AddFamilyRequestData(familyRequestCm, ReqTypeId);

            if (aspnetuser == null && user == null)
            {
                var receiver = familyRequestCm.Email;
                var subject = "Hallo Doc - Create Account";
                var here = "https://localhost:7077/Patient/PatientRegisterr?email=" + _loginService.Encrypt(familyRequestCm.Email);
                var message = $"Click <a href=\"{here}\">here</a> to create your hallo doc account...";

                EmailSender(receiver, subject, message);

                EmailLogEntry(subject, receiver, 3, message);
            }

            return true;
        }

        public void AddFamilyRequestData(FamilyRequestCm familyRequestCm, int ReqTypeId)
        {
            User? user = _Context.Users.FirstOrDefault(i => i.Email == familyRequestCm.Email);

            Region? region = _Context.Regions.FirstOrDefault(i => i.Regionid == familyRequestCm.RegionId);

            int requestCount = _Context.Requests.Count(i => i.Createddate.Date == DateTime.Now.Date) + 1;

            string lastName = user == null ? familyRequestCm.LastName.Remove(2).ToUpper() : user.Lastname.Remove(2).ToUpper();
            string firstName = user == null ? familyRequestCm.FirstName.Remove(2).ToUpper() : user.Firstname.Remove(2).ToUpper();

            string confirmation = region.Abbreviation + DateTime.Now.Day.ToString("D2") + DateTime.Now.Month.ToString("D2") + DateTime.Now.Year.ToString().Substring(2, 2) + lastName + firstName + requestCount.ToString("D4");

            var reqData = new Request()
            {
                Requesttypeid = ReqTypeId,
                Userid = user == null ? null : user.Userid,
                Firstname = familyRequestCm.FamilyFName,
                Lastname = familyRequestCm.FamilyLName,
                Email = familyRequestCm.FamilyEmail,
                Phonenumber = familyRequestCm.FamilyPhone,
                Status = 1,
                Confirmationnumber = confirmation,
                Createddate = DateTime.Now,
                Relationname = familyRequestCm.FamilyRelation,
            };

            _Context.Requests.Add(reqData);
            _Context.SaveChanges();

            AddFamilyRequestClientData(familyRequestCm, reqData.Requestid);

            if (familyRequestCm.Upload != null)
            {
                FamilyUploadFile(familyRequestCm, reqData.Requestid);
            }
        }

        public void AddFamilyRequestClientData(FamilyRequestCm familyRequestCm, int requestId)
        {
            var reqClientData = new Requestclient()
            {
                Requestid = requestId,
                Firstname = familyRequestCm.FirstName,
                Lastname = familyRequestCm.LastName,
                Email = familyRequestCm.Email,
                Phonenumber = familyRequestCm.Phone,
                Notes = familyRequestCm.Symptons,
                Intdate = familyRequestCm.BirthDate.Day,
                Strmonth = familyRequestCm.BirthDate.Month.ToString(),
                Intyear = familyRequestCm.BirthDate.Year,
                Regionid = familyRequestCm.RegionId,
                Street = familyRequestCm.Street,
                City = familyRequestCm.City,
                State = _Context.Regions.Where(x => x.Regionid == familyRequestCm.RegionId).Select(x => x.Name).FirstOrDefault(),
                Zipcode = familyRequestCm.Zipcode,
                Location = familyRequestCm.Room,
                Address = familyRequestCm.Room + " " + familyRequestCm.City + " " + _Context.Regions.Where(x => x.Regionid == familyRequestCm.RegionId).Select(x => x.Name).FirstOrDefault() + " " + familyRequestCm.Zipcode,

            };

            _Context.Requestclients.Add(reqClientData);
            _Context.SaveChanges();
        }

        public void FamilyUploadFile(FamilyRequestCm familyRequestCm, int requestId)
        {
            IFormFile File1 = familyRequestCm.Upload;

            var fileName = Path.GetFileNameWithoutExtension(File1.FileName) + "_" + DateTime.Now.Date.ToString("yyyy-MM-dd") + "_" + DateTime.Now.ToString("HH-mm-ss") + Path.GetExtension(File1.FileName);

            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "content", fileName);

            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                File1.CopyTo(fileStream);
            }

            var reqWiseFileData = new Requestwisefile()
            {
                Requestid = requestId,
                Filename = fileName,
                Createddate = DateTime.Now,
            };
            _Context.Add(reqWiseFileData);
            _Context.SaveChanges();
        }

        #endregion


        #region Request for Concierge Form

        public bool CreateConciergeRequest(ConciergeRequestCm conciergeRequestCm, int ReqTypeId)
        {
            Aspnetuser? aspnetuser = _Context.Aspnetusers.FirstOrDefault(i => i.Email == conciergeRequestCm.Email);
            var user = _Context.Users.FirstOrDefault(i => i.Email == conciergeRequestCm.Email);

            AddConciergeRequestData(conciergeRequestCm, ReqTypeId);

            if (user == null)
            {
                //Send Create Account Link to Concierge Email Address
                var receiver = conciergeRequestCm.Email;
                var subject = "Create Account - Hallodoc";
                var here = "https://localhost:7077/Patient/PatientRegister?email=" + _loginService.Encrypt(conciergeRequestCm.Email);
                var message = $"Click <a href=\"{here}\">here</a> to create your hallo doc account...";

                EmailSender(receiver, subject, message);

                EmailLogEntry(subject, receiver, 3, message);
            }

            return true;
        }

        public void AddConciergeRequestData(ConciergeRequestCm conciergeRequestCm, int ReqTypeId)
        {
            User? user = _Context.Users.FirstOrDefault(i => i.Email == conciergeRequestCm.Email);

            Region? region = _Context.Regions.FirstOrDefault(i => i.Regionid == conciergeRequestCm.RegionId);

            int requestCount = _Context.Requests.Count(i => i.Createddate.Date == DateTime.Now.Date) + 1;

            string lastName = user == null ? conciergeRequestCm.LastName.Remove(2).ToUpper() : user.Lastname.Remove(2).ToUpper();
            string firstName = user == null ? conciergeRequestCm.FirstName.Remove(2).ToUpper() : user.Firstname.Remove(2).ToUpper();

            string confirmation = region.Abbreviation + DateTime.Now.Day.ToString("D2") + DateTime.Now.Month.ToString("D2") + DateTime.Now.Year.ToString().Substring(2, 2) + lastName + firstName + requestCount.ToString("D4");

            var reqData = new Request()
            {
                Requesttypeid = ReqTypeId,
                Userid = user == null ? null : user.Userid,
                Firstname = conciergeRequestCm.ConciergeFName,
                Lastname = conciergeRequestCm.ConciergeLName,
                Email = conciergeRequestCm.ConciergeEmail,
                Phonenumber = conciergeRequestCm.ConciergePhone,
                Status = 1,
                Confirmationnumber = confirmation,
                Createddate = DateTime.Now,
            };

            _Context.Requests.Add(reqData);
            _Context.SaveChanges();

            AddConciergeRequestClientData(conciergeRequestCm, reqData.Requestid);
        }

        public void AddConciergeRequestClientData(ConciergeRequestCm conciergeRequestCm, int requestId)
        {
            var reqClientData = new Requestclient()
            {
                Requestid = requestId,
                Firstname = conciergeRequestCm.FirstName,
                Lastname = conciergeRequestCm.LastName,
                Email = conciergeRequestCm.Email,
                Phonenumber = conciergeRequestCm.Phone,
                Notes = conciergeRequestCm.Symptons,
                Intdate = conciergeRequestCm.BirthDate.Day,
                Strmonth = conciergeRequestCm.BirthDate.Month.ToString(),
                Intyear = conciergeRequestCm.BirthDate.Year,
                Regionid = conciergeRequestCm.RegionId,
                Street = conciergeRequestCm.ConciergeStreet,
                City = conciergeRequestCm.ConciergeCity,
                State = _Context.Regions.Where(x => x.Regionid == conciergeRequestCm.RegionId).Select(x => x.Name).FirstOrDefault(),
                Zipcode = conciergeRequestCm.ConciergeZipCode,
                Location = conciergeRequestCm.Room,
                Address = conciergeRequestCm.Room + " " + conciergeRequestCm.ConciergeCity + " " + _Context.Regions.Where(x => x.Regionid == conciergeRequestCm.RegionId).Select(x => x.Name).FirstOrDefault() + " " + conciergeRequestCm.ConciergeZipCode,

            };

            _Context.Requestclients.Add(reqClientData);
            _Context.SaveChanges();

            AddConciergeData(conciergeRequestCm);
        }

        public void AddConciergeData(ConciergeRequestCm conciergeRequestCm)
        {
            var conciergeData = new Concierge()
            {
                Conciergename = conciergeRequestCm.ConciergeFName + " " + conciergeRequestCm.ConciergeLName,
                Address = conciergeRequestCm.ConciergeCity + " " + _Context.Regions.Where(x => x.Regionid == conciergeRequestCm.RegionId).Select(x => x.Name).FirstOrDefault() + " " + conciergeRequestCm.ConciergeZipCode,
                State = _Context.Regions.Where(x => x.Regionid == conciergeRequestCm.RegionId).Select(x => x.Name).FirstOrDefault(),
                Street = conciergeRequestCm.ConciergeStreet,
                City = conciergeRequestCm.ConciergeCity,
                Zipcode = conciergeRequestCm.ConciergeZipCode,
                Createddate = DateTime.Now,
                Regionid = conciergeRequestCm.RegionId,
            };

            _Context.Concierges.Add(conciergeData);
            _Context.SaveChanges();

            AddRequestConcierge(conciergeRequestCm.ConciergeEmail, conciergeData.Conciergeid);
        }

        public void AddRequestConcierge(string email, int conciergeId)
        {
            Request? request = _Context.Requests.FirstOrDefault(i => i.Email == email);

            var reqConciergeData = new Requestconcierge()
            {
                Requestid = request.Requestid,
                Conciergeid = conciergeId,
            };

            _Context.Requestconcierges.Add(reqConciergeData);
            _Context.SaveChanges();
        }

        #endregion


        #region Request for Business Form

        public bool CreateBusinessRequest(BusinessRequestCm businessRequestCm, int ReqTypeId)
        {
            Aspnetuser? aspnetuser = _Context.Aspnetusers.FirstOrDefault(i => i.Email == businessRequestCm.Email);
            var user = _Context.Users.FirstOrDefault(i => i.Email == businessRequestCm.Email);

            AddBusinessRequestData(businessRequestCm, ReqTypeId);

            if (user == null)
            {
                //Send Create Account Link to Business Email Address
                var receiver = businessRequestCm.Email;
                var subject = "Create Account - Hallodoc";
                var here = "https://localhost:7077/Patient/PatientRegister?email=" + _loginService.Encrypt(businessRequestCm.Email);
                var message = $"Click <a href=\"{here}\">here</a> to create your hallo doc account...";

                EmailSender(receiver, subject, message);

                EmailLogEntry(subject, receiver, 3, message);
            }

            return true;
        }

        public void AddBusinessRequestData(BusinessRequestCm businessRequestCm, int ReqTypeId)
        {
            User? user = _Context.Users.FirstOrDefault(i => i.Email == businessRequestCm.Email);

            Region? region = _Context.Regions.FirstOrDefault(i => i.Regionid == businessRequestCm.RegionId);

            int requestCount = _Context.Requests.Count(i => i.Createddate.Date == DateTime.Now.Date) + 1;

            string lastName = user == null ? businessRequestCm.LastName.Remove(2).ToUpper() : user.Lastname.Remove(2).ToUpper();
            string firstName = user == null ? businessRequestCm.FirstName.Remove(2).ToUpper() : user.Firstname.Remove(2).ToUpper();

            string confirmation = region.Abbreviation + DateTime.Now.Day.ToString("D2") + DateTime.Now.Month.ToString("D2") + DateTime.Now.Year.ToString().Substring(2, 2) + lastName + firstName + requestCount.ToString("D4");

            var reqData = new Request()
            {
                Requesttypeid = ReqTypeId,
                Userid = user == null ? null : user.Userid,
                Firstname = businessRequestCm.BusinessFName,
                Lastname = businessRequestCm.BusinessLName,
                Email = businessRequestCm.BusinessEmail,
                Phonenumber = businessRequestCm.BusinessPhone,
                Casenumber = businessRequestCm.BusinessCaseNumber,
                Status = 1,
                Confirmationnumber = confirmation,
                Createddate = DateTime.Now,

            };

            _Context.Requests.Add(reqData);
            _Context.SaveChanges();

            AddBusinessRequestClientData(businessRequestCm, reqData.Requestid);

        }

        public void AddBusinessRequestClientData(BusinessRequestCm businessRequestCm, int requestId)
        {
            var reqClientData = new Requestclient()
            {
                Requestid = requestId,
                Firstname = businessRequestCm.FirstName,
                Lastname = businessRequestCm.LastName,
                Email = businessRequestCm.Email,
                Phonenumber = businessRequestCm.Phone,
                Notes = businessRequestCm.Symptons,
                Intdate = businessRequestCm.BirthDate.Day,
                Strmonth = businessRequestCm.BirthDate.Month.ToString(),
                Intyear = businessRequestCm.BirthDate.Year,
                Street = businessRequestCm.Street,
                City = businessRequestCm.City,
                State = _Context.Regions.Where(x => x.Regionid == businessRequestCm.RegionId).Select(x => x.Name).FirstOrDefault(),
                Zipcode = businessRequestCm.Zipcode,
                Location = businessRequestCm.Room,
                Regionid = businessRequestCm.RegionId,
                Address = businessRequestCm.Room + " " + businessRequestCm.City + " " + _Context.Regions.Where(x => x.Regionid == businessRequestCm.RegionId).Select(x => x.Name).FirstOrDefault() + " " + businessRequestCm.Zipcode,

            };

            _Context.Requestclients.Add(reqClientData);
            _Context.SaveChanges();

            AddBusinessData(businessRequestCm);
        }

        public void AddBusinessData(BusinessRequestCm businessRequestCm)
        {
            var businessData = new Business()
            {
                Name = businessRequestCm.BusinessFName + " " + businessRequestCm.BusinessLName,
                Phonenumber = businessRequestCm.BusinessPhone,
            };

            _Context.Businesses.Add(businessData);
            _Context.SaveChanges();

            AddRequestBusiness(businessRequestCm.BusinessEmail, businessData.Businessid);
        }

        public void AddRequestBusiness(string email, int businessId)
        {
            Request? request = _Context.Requests.FirstOrDefault(i => i.Email == email);

            var businessRequestData = new Requestbusiness()
            {
                Requestid = request.Requestid,
                Businessid = businessId,
            };

            _Context.Requestbusinesses.Add(businessRequestData);
            _Context.SaveChanges();
        }

        #endregion


        #region Request for Me Form

        public PatientRequestCm GetMeData(int userId)
        {
            PatientRequestCm patientRequestCm = new PatientRequestCm();

            User? user = _Context.Users.FirstOrDefault(i => i.Userid == userId);

            var BirthDay = Convert.ToInt32(user.Intdate);
            var BirthMonth = Convert.ToInt32(user.Strmonth);
            var BirthYear = Convert.ToInt32(user.Intyear);

            if (user != null)
            {
                patientRequestCm.FirstName = user.Firstname;
                patientRequestCm.LastName = user.Lastname;
                patientRequestCm.Email = user.Email;
                patientRequestCm.PhoneWithoutCode = user.Mobile;
                patientRequestCm.RegionId = (int)user.Regionid;
                patientRequestCm.Street = user.Street;
                patientRequestCm.City = user.City;
                patientRequestCm.Zipcode = user.Zipcode;
                patientRequestCm.BirthDate = new DateTime(BirthYear, BirthMonth, BirthDay);
                patientRequestCm.Regions = GetRegions();

            }

            bool blockedEmail = _Context.Blockrequests.Any(x => x.Email == user.Email && x.Phonenumber == user.Mobile);
            //bool blockedPhone = _Context.Blockrequests.Any(x => x.Phonenumber == user.Mobile);

            if (blockedEmail)
            {
                return null;
            }

            return patientRequestCm;
        }

        public bool CreateForMeData(PatientRequestCm patientRequestCm, int reqTypeId, int userId)
        {
            User? user = _Context.Users.FirstOrDefault(i => i.Userid == userId);

            if (user != null)
            {
                Region? region = _Context.Regions.FirstOrDefault(i => i.Regionid == patientRequestCm.RegionId);

                int requestCount = _Context.Requests.Count(i => i.Createddate.Date == DateTime.Now.Date) + 1;

                string confirmation = region.Abbreviation + DateTime.Now.Day.ToString("D2") + DateTime.Now.Month.ToString("D2") + DateTime.Now.Year.ToString().Substring(2, 2) + user.Lastname.Remove(2).ToUpper() + user.Firstname.Remove(2).ToUpper() + requestCount.ToString("D4");

                var reqData = new Request()
                {
                    Requesttypeid = reqTypeId,
                    Userid = user == null ? null : user.Userid,
                    Firstname = user.Firstname,
                    Lastname = user.Lastname,
                    Phonenumber = user.Mobile,
                    Email = user.Email,
                    Status = 1,
                    Confirmationnumber = confirmation,
                    Createddate = DateTime.Now,

                };

                _Context.Add(reqData);
                _Context.SaveChanges();

                var reqClientData = new Requestclient()
                {
                    Requestid = reqData.Requestid,
                    Firstname = patientRequestCm.FirstName,
                    Lastname = patientRequestCm.LastName,
                    Email = user.Email,
                    Phonenumber = patientRequestCm.Phone,
                    Notes = patientRequestCm.Symptons,
                    Intdate = patientRequestCm.BirthDate.Day,
                    Strmonth = patientRequestCm.BirthDate.Month.ToString(),
                    Intyear = patientRequestCm.BirthDate.Year,
                    Street = patientRequestCm.Street,
                    City = patientRequestCm.City,
                    State = _Context.Regions.Where(x => x.Regionid == patientRequestCm.RegionId).Select(x => x.Name).FirstOrDefault(),
                    Regionid = patientRequestCm.RegionId,
                    Zipcode = patientRequestCm.Zipcode,
                    Location = patientRequestCm.Room,
                    Address = patientRequestCm.Room + " " + patientRequestCm.City + " " + _Context.Regions.Where(x => x.Regionid == patientRequestCm.RegionId).Select(x => x.Name).FirstOrDefault() + " " + patientRequestCm.Zipcode,
                };

                _Context.Requestclients.Add(reqClientData);
                _Context.SaveChanges();

                if (patientRequestCm.Upload != null)
                {
                    UploadFile(patientRequestCm, reqData.Requestid);
                }
                return true;
            }
            return false;
        }

        #endregion


        #region Request for Others Form

        public bool CreateRequestForOther(FamilyRequestCm familyRequestCm, int reqTypeId, int userId)
        {
            Aspnetuser? aspnetuser = _Context.Aspnetusers.FirstOrDefault(i => i.Email == familyRequestCm.Email);
            var user = _Context.Users.FirstOrDefault(i => i.Email == familyRequestCm.Email);

            AddOtherRequestData(familyRequestCm, reqTypeId, userId);

            if (user == null)
            {
                //Send Create Account Link to Family Email Address
                var receiver = familyRequestCm.Email;
                var subject = "Create Account - Hallodoc";
                var here = "https://localhost:7077/Patient/PatientRegister?email=" + _loginService.Encrypt(familyRequestCm.Email);
                var message = $"Click <a href=\"{here}\">here</a> to create your hallo doc account...";

                EmailSender(receiver, subject, message);

                EmailLogEntry(subject, receiver, 3, message);

            }

            return true;
        }

        public void AddOtherRequestData(FamilyRequestCm familyRequestCm, int reqTypeId, int userId)
        {
            User? user = _Context.Users.FirstOrDefault(i => i.Userid == userId);
            Region? region = _Context.Regions.FirstOrDefault(i => i.Regionid == familyRequestCm.RegionId);

            int requestCount = _Context.Requests.Count(i => i.Createddate.Date == DateTime.Now.Date) + 1;

            string lastName = user == null ? familyRequestCm.LastName.Remove(2).ToUpper() : user.Lastname.Remove(2).ToUpper();
            string firstName = user == null ? familyRequestCm.FirstName.Remove(2).ToUpper() : user.Firstname.Remove(2).ToUpper();

            string confirmation = region.Abbreviation + DateTime.Now.Day.ToString("D2") + DateTime.Now.Month.ToString("D2") + DateTime.Now.Year.ToString().Substring(2, 2) + lastName + firstName + requestCount.ToString("D4");

            var reqData = new Request()
            {
                Requesttypeid = reqTypeId,
                Firstname = user.Firstname,
                Lastname = user.Lastname,
                Phonenumber = user.Mobile,
                Email = user.Email,
                Relationname = familyRequestCm.FamilyRelation,
                Status = 1,
                Createddate = DateTime.Now,
                Confirmationnumber = confirmation,
            };

            _Context.Requests.Add(reqData);
            _Context.SaveChanges();

            AddFamilyRequestClientData(familyRequestCm, reqData.Requestid);

            if (familyRequestCm.Upload != null)
            {
                FamilyUploadFile(familyRequestCm, reqData.Requestid);
            }
        }

        #endregion


        #region Send Email

        public Task EmailSender(string email, string subject, string message)
        {
            var mail = "tatva.dotnet.meetbhalani@outlook.com";
            var password = "eaywlzlxxvvbycfs";

            var client = new SmtpClient("smtp.office365.com", 587)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(mail, password)
            };

            MailMessage mailMessage = new MailMessage(from: mail, to: email, subject, message);
            mailMessage.IsBodyHtml = true;

            return client.SendMailAsync(mailMessage);

        }

        #endregion


        #region Review Agreement

        public ReviewAgreementCm GetReviewAgreement(int reqId)
        {
            var request = _Context.Requests.FirstOrDefault(i => i.Requestid == reqId);
            var patient = _Context.Requestclients.FirstOrDefault(i => i.Requestid == reqId);

            var reviewData = new ReviewAgreementCm();

            if(request.Status == 4 || request.Status == 7)
            {
                return null;
            }

            reviewData.RequestId = reqId;
            reviewData.PatientName = patient.Firstname + " " + patient.Lastname;

            return reviewData;
        }

        public void AgreeReview(int reqId)
        {
            var request = _Context.Requests.FirstOrDefault(i => i.Requestid == reqId);

            var agreeData = new Requeststatuslog()
            {
                Requestid = reqId,
                Status = 4,
                Notes = "Review is Agreed",
                Createddate = DateTime.Now,
            };

            request.Status = 4;
            //request.Accepteddate = DateTime.Now;

            _Context.Requeststatuslogs.Add(agreeData);
            _Context.SaveChanges();
        }

        public void CancelReview(ReviewAgreementCm reviewAgreementCm)
        {
            var request = _Context.Requests.FirstOrDefault(i => i.Requestid == reviewAgreementCm.RequestId);

            var cancelData = new Requeststatuslog()
            {
                Requestid = request.Requestid,
                Status = 7,
                Notes = reviewAgreementCm.CancellationNotes,
                Createddate = DateTime.Now,
            };

            request.Status = 7;

            _Context.Requeststatuslogs.Add(cancelData);
            _Context.SaveChanges();
        }

        #endregion
    }
}
