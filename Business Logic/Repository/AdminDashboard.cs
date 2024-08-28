using Business_Logic.Interface;
using Data_Access.Custom_Models;
using Data_Access.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using static System.Net.WebRequestMethods;
using Microsoft.AspNetCore.DataProtection.XmlEncryption;
using File = System.IO.File;
using Data_Access.Coustom_Models;
using OfficeOpenXml;
using Microsoft.IdentityModel.Tokens;

namespace Business_Logic.Repository
{
    public class AdminDashboard : IAdminDashboard
    {
        private readonly ApplicationDbContext _context;
        private readonly ILoginService _loginService;

        public AdminDashboard(ApplicationDbContext context, ILoginService loginService)
        {
            _context = context;
            _loginService = loginService;
        }

        #region Common Data

        public List<string> GetRoleMenuList(int accessRoleId)
        {
            var roleMenus = _context.Rolemenus.Where(u => u.Roleid == accessRoleId).ToList();

            List<string> menus = new();

            if (roleMenus.Count > 0)
            {
                foreach (var item in roleMenus)
                {
                    var menu = _context.Menus.FirstOrDefault(u => u.Menuid == item.Menuid);

                    if (menu != null)
                    {
                        menus.Add(menu.Name);
                    }
                }
                return menus;
            }
            else
            {
                return menus;
            }
        }

        public List<Region> GetRegions()
        {
            var regions = _context.Regions.ToList();
            return regions;
        }

        public List<Region> GetPhysicianRegions(int physicianId)
        {
            var regions = _context.Regions.Include(p => p.Physicianregions).Where(p => p.Physicianregions.Any(x => x.Physicianid == physicianId)).ToList();
            return regions;
        }

        public List<Casetag> GetCasetags()
        {
            var reasons = _context.Casetags.ToList();
            return reasons;
        }

        public List<Physician> GetPhysicians(int regionid)
        {
            var physicians = _context.Physicians.Where(i => i.Isdeleted == null).ToList();

            if (regionid != 0)
            {
                var physicianForRegion = _context.Physicianregions.Where(i => i.Regionid == regionid).Select(i => i.Physicianid).ToList();
                physicians = physicians.Where(i => physicianForRegion.Contains(i.Physicianid)).ToList();
            }

            return physicians;
        }

        public List<Role> GetRoles(int aspId)
        {
            var aspUser = _context.Aspnetusers.FirstOrDefault(y => y.Id == aspId);
            var roleList = new List<Role>();

            if (aspUser != null)
            {
                roleList = _context.Roles.Where(x => x.Accounttype == aspUser.Roleid).ToList();

            }

            return roleList;
        }

        public List<Aspnetrole> GetAccountType()
        {
            var role = _context.Aspnetroles.ToList();
            return role;
        }

        public List<Menu> GetMenu(int accounttype)
        {
            if (accounttype != 0)
            {
                var menu = _context.Menus.Where(r => r.Accounttype == accounttype).ToList();
                return menu;
            }
            else
            {

                var menu = _context.Menus.ToList();
                return menu;
            }
        }

        public List<Healthprofessionaltype> GetProfession()
        {
            var professions = _context.Healthprofessionaltypes.ToList();
            return professions;
        }

        public List<Requesttype> GetRequesttypes()
        {
            return _context.Requesttypes.ToList();
        }

        #endregion


        #region Dashboard

        public CountRequest GetCountRequest()
        {
            var request = _context.Requests;

            var countData = new CountRequest()
            {
                NewRequest = request.Where(i => i.Status == 1 && i.Isdeleted == null && i.Userid != null).Count(),
                PendingRequest = request.Where(i => i.Status == 2 && i.Isdeleted == null && i.Userid != null).Count(),
                ActiveRequest = request.Where(i => i.Status == 4 || i.Status == 5 && i.Isdeleted == null && i.Userid != null).Count(),
                ConcludeRequest = request.Where(i => i.Status == 6 && i.Isdeleted == null && i.Userid != null).Count(),
                ToCloseRequest = request.Where(i => i.Status == 7 || i.Status == 8 || i.Status == 3 && i.Isdeleted == null && i.Userid != null).Count(),
                UnpaidRequest = request.Where(i => i.Status == 9 && i.Isdeleted == null && i.Userid != null).Count(),
            };

            return countData;
        }

        public List<RequestListAdminDash> GetRequestData(int[] Status, string requesttypeid, int regionid)
        {
            var requestList = _context.Requests.Where(i => Status.Contains(i.Status) && i.Isdeleted == null && i.Userid != null);

            if (requesttypeid != null)
            {
                requestList = requestList.Where((i => i.Requesttypeid.ToString() == requesttypeid));
            }

            if (regionid != 0)
            {
                requestList = requestList.Where(i => i.Requestclients.Select(rc => rc.Regionid.ToString()).Contains(regionid.ToString()));
            }

            var GetRequestData = requestList.Select(r => new RequestListAdminDash()
            {
                //aspnetuseridPatient = _context.Users.FirstOrDefault(i => i.Userid == r.Userid)!.Aspnetuserid,
                //aspnetuseridProvider = r.Physicianid == null ? null : _context.Physicians.FirstOrDefault(i => i.Physicianid == r.Physicianid)!.Aspnetuserid,
                Name = r.Requestclients.Select(x => x.Firstname).FirstOrDefault() + " " + r.Requestclients.Select(x => x.Lastname).FirstOrDefault(),
                Requestor = r.Firstname + " " + r.Lastname,
                RequestDate = r.Createddate.ToString("MMM dd, yyyy"),
                Days = (DateTime.Now - r.Createddate).Days.ToString() + "d",
                Hours = (DateTime.Now - r.Createddate).Hours.ToString() + "h",
                Mobile = r.Phonenumber ?? "-",
                Phone = r.Requestclients.Select(x => x.Phonenumber).FirstOrDefault(),
                Address = r.Requestclients.Select(x => x.Street).FirstOrDefault() + " " + r.Requestclients.Select(x => x.City).FirstOrDefault() + " " + r.Requestclients.Select(x => x.State).FirstOrDefault(),
                Notes = r.Requeststatuslogs.OrderBy(x => x.Requeststatuslogid).LastOrDefault() == null ? "-" : r.Requeststatuslogs.OrderBy(x => x.Requeststatuslogid).LastOrDefault().Notes,
                Status = r.Status,
                Physician = r.Physicianid == null ? "-" : r.Physician.Firstname + " " + r.Physician.Lastname,
                DateOfBirth = r.Requestclients.Select(x => x.Intdate).FirstOrDefault() == null ? null : r.Requestclients.Select(x => x.Intdate).FirstOrDefault() + "/" + r.Requestclients.Select(x => x.Strmonth).FirstOrDefault() + "/" + r.Requestclients.Select(x => x.Intyear).FirstOrDefault(),
                RequestTypeId = r.Requesttypeid,
                Email = r.Requestclients.Select(x => x.Email).FirstOrDefault(),
                RequestId = r.Requestid,
                Region = r.Requestclients.Select(x => x.State) == null ? "-" : r.Requestclients.Select(x => x.State).FirstOrDefault(),
                DateOfService = r.Accepteddate,
                PhysicianId = r.Physician.Physicianid,
                PhyAspId = r.Physician.Aspnetuserid,
                PatientAspId = r.User.Aspnetuserid,
                UserId = r.Userid

            }).ToList();

            return GetRequestData;
        }


        #region View Case

        public AdminViewCaseData GetViewCaseData(int requestid)
        {
            var request = _context.Requests.FirstOrDefault(i => i.Requestid == requestid);
            var casedata = _context.Requestclients.FirstOrDefault(i => i.Requestid == requestid);

            var BirthYear = Convert.ToInt32(casedata.Intyear);
            var BirthMonth = Convert.ToInt32(casedata.Strmonth);
            var BirthDate = Convert.ToInt32(casedata.Intdate);

            var AdminViewCaseModel = new AdminViewCaseData()
            {
                UserId = request == null ? 0 : (int)request.Userid,
                ConfirmationNumber = request.Confirmationnumber,
                Symptons = casedata.Notes,
                FirstName = casedata.Firstname,
                LastName = casedata.Lastname,
                MobileWithoutCode = casedata.Phonenumber,
                Email = casedata.Email,
                Region = casedata.State,
                BusinessAddress = casedata.Street + ", " + casedata.City + ", " + casedata.State,
                Room = casedata.Location,
                RequestTypeId = request.Requesttypeid,
                DateOfBirth = new DateTime(BirthYear, BirthMonth, BirthDate),
                RequestId = requestid,

            };

            return AdminViewCaseModel;
        }

        public void SetViewCaseData(AdminViewCaseData updatedViewCaseData, int requestid)
        {
            var casedata = _context.Requestclients.FirstOrDefault(x => x.Requestid == requestid);

            if (casedata != null)
            {
                casedata.Email = updatedViewCaseData.Email;
                casedata.Phonenumber = updatedViewCaseData.Mobile;

                _context.SaveChanges();
            }

        }

        #endregion


        #region View Notes

        public ViewNotesData GetViewNotesData(int requestid)
        {
            var notedata = _context.Requestnotes.FirstOrDefault(i => i.Requestid == requestid);
            var transferdata = _context.Requeststatuslogs.Where(i => i.Requestid == requestid && (i.Transtophysicianid != null || i.Transtoadmin == new BitArray(1, true))).ToList();

            var ViewNotesModel = new ViewNotesData()
            {
                RequestId = requestid,
                AdminNotes = notedata?.Adminnotes == null ? "-" : notedata.Adminnotes,
                PhysicianNotes = notedata?.Physiciannotes == null ? "-" : notedata.Physiciannotes,
                TransferNotes = transferdata == null ? null : transferdata,
            };

            return ViewNotesModel;
        }

        public void SetViewNotesData(ViewNotesData updatedViewNotesData, int requestid, int aspId)
        {
            var notedata = _context.Requestnotes.FirstOrDefault(i => i.Requestid == requestid);
            var aspData = _context.Aspnetusers.FirstOrDefault(i => i.Id == aspId);

            if (notedata != null && aspData != null)
            {
                notedata.Requestid = requestid;
                notedata.Modifiedby = aspId;
                notedata.Modifieddate = DateTime.Now;

                if (aspData.Roleid == 1)
                {
                    notedata.Adminnotes = updatedViewNotesData.AdminNotes;
                }
                else
                {
                    notedata.Physiciannotes = updatedViewNotesData.AdminNotes;
                }

                _context.SaveChanges();
            }
            else if (aspData != null)
            {
                var newnotedata = new Requestnote()
                {
                    Requestid = requestid,
                    Createdby = aspId,
                    Createddate = DateTime.Now,
                };

                if (aspData.Roleid == 1)
                {
                    newnotedata.Adminnotes = updatedViewNotesData.AdminNotes;
                }
                else
                {
                    newnotedata.Physiciannotes = updatedViewNotesData.AdminNotes;
                }

                _context.Requestnotes.Add(newnotedata);
                _context.SaveChanges();
            }


        }

        #endregion


        #region Cancel Case

        public CancelCaseModal GetCancelCaseData(int requestid)
        {
            var cancelCase = _context.Requestclients.FirstOrDefault(i => i.Requestid == requestid);
            var status = _context.Requests.FirstOrDefault(i => i.Requestid == requestid);

            var cancelData = new CancelCaseModal()
            {
                RequestId = requestid,
                Name = cancelCase.Firstname + " " + cancelCase.Lastname,
            };

            return cancelData;
        }

        public void SetCancelCaseData(CancelCaseModal updatedCancelCaseData)
        {
            var requestdata = _context.Requests.FirstOrDefault(i => i.Requestid == updatedCancelCaseData.RequestId);

            var addCancelData = new Requeststatuslog()
            {
                Requestid = requestdata.Requestid,
                Notes = updatedCancelCaseData.CancellationNotes,
                Status = 3,
                Createddate = DateTime.Now,
            };

            requestdata.Status = 3;
            requestdata.Casetag = updatedCancelCaseData.CasetagId.ToString();

            _context.Requeststatuslogs.Add(addCancelData);
            _context.SaveChanges();

        }

        #endregion


        #region Assign Case

        public AsignCaseModal GetAsignCaseData(int requestid)
        {
            var asignData = new AsignCaseModal()
            {
                RequestId = requestid,
            };

            return asignData;
        }

        public void SetAsignCaseData(ModalCm modalCm)
        {
            var request = _context.Requests.FirstOrDefault(i => i.Requestid == modalCm.asignCaseModal.RequestId);

            if (modalCm.ModalId == 1)
            {
                var physician = _context.Physicians.FirstOrDefault(x => x.Physicianid == modalCm.asignCaseModal.PhysicianId);

                string assignNotes = "Admin assigned to " + physician.Firstname + " " + physician.Lastname + " on " + DateTime.Now.ToString("MMM dd, yyyy") + " at " + DateTime.Now.ToString("hh:mm tt") + " : " + modalCm.asignCaseModal.AsignNotes;

                var addAsignData = new Requeststatuslog()
                {
                    Requestid = request.Requestid,
                    Status = 1,
                    Notes = assignNotes,
                    Createddate = DateTime.Now,
                    Physicianid = modalCm.asignCaseModal.PhysicianId,
                };

                _context.Requeststatuslogs.Add(addAsignData);

                request.Status = 1;
                request.Physicianid = physician.Physicianid;
            }

            else
            {
                var physician = _context.Physicians.FirstOrDefault(x => x.Physicianid == modalCm.asignCaseModal.PhysicianId);

                string transferNotes = "Admin transfered to " + physician.Firstname + " " + physician.Lastname + " on " + DateTime.Now.ToString("MMM dd, yyyy") + " at " + DateTime.Now.ToString("hh:mm tt") + " : " + modalCm.asignCaseModal.AsignNotes;

                var addAsignData = new Requeststatuslog()
                {
                    Requestid = request.Requestid,
                    Status = 1,
                    Notes = transferNotes,
                    Createddate = DateTime.Now,
                    Transtophysicianid = modalCm.asignCaseModal.PhysicianId,
                };

                _context.Requeststatuslogs.Add(addAsignData);

                request.Status = 1;
                request.Physicianid = physician.Physicianid;
            }

            _context.SaveChanges();
        }

        #endregion


        #region Block Case

        public BlockCaseModal GetBlockCaseModal(int requestid)
        {
            var blockCase = _context.Requests.FirstOrDefault(i => i.Requestid == requestid);

            var blockData = new BlockCaseModal()
            {
                RequestId = requestid,
                Name = blockCase.Firstname + " " + blockCase.Lastname,
            };

            return blockData;
        }

        public void SetBlockCaseData(BlockCaseModal updatedBlockCaseData)
        {
            var request = _context.Requests.FirstOrDefault(i => i.Requestid == updatedBlockCaseData.RequestId);
            var requestClient = _context.Requestclients.FirstOrDefault(i => i.Requestid == updatedBlockCaseData.RequestId);

            var blockData = new Blockrequest()
            {
                Phonenumber = requestClient.Phonenumber,
                Email = requestClient.Email,
                Reason = updatedBlockCaseData.BlockReason,
                Requestid = request.Requestid,
                Createddate = DateTime.Now,
                Modifieddate = DateTime.Now,
                Isactive = new BitArray(1, true),
            };

            request.Status = 11;

            _context.Blockrequests.Add(blockData);
            _context.SaveChanges();
        }

        #endregion


        #region View Documents

        public AdminViewDocumentsCm GetViewDocumentsData(int requestid)
        {
            var patient = _context.Requestclients.FirstOrDefault(i => i.Requestid == requestid);

            string confirmation = _context.Requests.FirstOrDefault(i => i.Requestid == requestid).Confirmationnumber;
            var userId = (int)_context.Requests.FirstOrDefault(i => i.Requestid == requestid).Userid;

            var casedata = _context.Requestclients.FirstOrDefault(i => i.Requestid == requestid);
            var requestList = _context.Requests.Where(i => i.Requestid == requestid);

            var documentData = new AdminViewDocumentsCm()
            {
                userId = userId == null ? 0 : userId,
                requestId = requestid,
                patientName = patient.Firstname + " " + patient.Lastname,
                ConfirmationNumber = confirmation,
            };

            return documentData;
        }

        public List<ViewDocuments> GetViewDocumentsList(int requestid)
        {
            var document = _context.Requestwisefiles.Where(i => i.Requestid == requestid && i.Isdeleted == null);

            var viewDocuments = document.Select(r => new ViewDocuments()
            {
                requestWiseFileId = r.Requestwisefileid,
                requestId = requestid,
                documentName = r.Filename,
                uploadDate = r.Createddate,
            }).ToList();

            return viewDocuments;
        }

        public void SetViewDocumentData(AdminViewDocumentsCm adminViewDocumentsCm)
        {
            IFormFile File1 = adminViewDocumentsCm.document;

            var fileName = Path.GetFileNameWithoutExtension(File1.FileName) + "_" + DateTime.Now.Date.ToString("yyyy-MM-dd") + "_" + DateTime.Now.ToString("HH-mm-ss") + Path.GetExtension(File1.FileName);

            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "content", fileName);

            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                File1.CopyTo(fileStream);
            }

            var fileData = new Requestwisefile()
            {
                Requestid = adminViewDocumentsCm.requestId,
                Filename = fileName,
                Createddate = DateTime.Now,
            };

            _context.Add(fileData);
            _context.SaveChanges();
        }

        public void DeleteFileData(int requestwisefileid)
        {
            var file = _context.Requestwisefiles.FirstOrDefault(i => i.Requestwisefileid == requestwisefileid);

            if (file != null)
            {
                file.Isdeleted = new BitArray(1);
                file.Isdeleted[0] = true;
            }
            _context.SaveChanges();
        }

        public void SendEmailWithFile(int[] requestwisefileid, int requestid, int aspId)
        {
            var mail = "tatva.dotnet.meetbhalani@outlook.com";
            var password = "eaywlzlxxvvbycfs";
            var email = _context.Requestclients.FirstOrDefault(i => i.Requestid == requestid).Email;
            var subject = "View Document";
            var message = "Check Below Documents";

            SmtpClient smtpClient = new SmtpClient("smtp.office365.com", 587)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(mail, password)
            };

            MailMessage mailMessage = new MailMessage(from: mail, to: email, subject, message);

            foreach (var obj in requestwisefileid)
            {

                var file = _context.Requestwisefiles.FirstOrDefault(i => i.Requestwisefileid == obj);
                string filepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "content", file.Filename);

                mailMessage.Attachments.Add(new Attachment(filepath));
            }


            smtpClient.SendMailAsync(mailMessage);

            var aspData = _context.Aspnetusers.FirstOrDefault(i => i.Id == aspId);
            var requestData = _context.Requests.FirstOrDefault(i => i.Requestid == requestid);

            var emailLog = new Emaillog()
            {
                Requestid = requestid,
                Confirmationnumber = requestData.Confirmationnumber,
                Subjectname = subject,
                Emailid = email,
                Roleid = 3,
                Createdate = DateTime.Now,
                Sentdate = DateTime.Now,
                Isemailsent = new BitArray(1, true),
                Senttries = 1,
            };

            if (aspData != null && aspData.Roleid == 2)
            {
                var physician = _context.Physicians.FirstOrDefault(i => i.Aspnetuserid == aspId);

                emailLog.Physicianid = physician.Physicianid;
            }

            if (aspData != null && aspData.Roleid == 1)
            {
                var admin = _context.Admins.FirstOrDefault(i => i.Aspnetuserid == aspId);

                emailLog.Adminid = admin.Adminid;
            }

            _context.Emaillogs.Add(emailLog);
            _context.SaveChanges();
        }

        #endregion


        #region Send Orders

        public List<Healthprofessionaltype> GetHealthprofessionaltypes()
        {
            var healthProfessionals = _context.Healthprofessionaltypes.ToList();

            return healthProfessionals;
        }

        public List<Healthprofessional> GetHealthprofessionals(int professionid)
        {
            var business = _context.Healthprofessionals.Where(i => i.Profession == professionid).ToList();

            return business;
        }

        public Healthprofessional GetBusinessData(int businessid)
        {
            var businessData = _context.Healthprofessionals.FirstOrDefault(i => i.Vendorid == businessid);

            return businessData;
        }

        public void SetSendOrderData(AdminSendOrderCm adminSendOrderCm, int aspId)
        {
            var orderData = new Orderdetail()
            {
                Vendorid = adminSendOrderCm.VendorId,
                Requestid = adminSendOrderCm.RequestId,
                Faxnumber = adminSendOrderCm.FaxNumber,
                Email = adminSendOrderCm.Email,
                Businesscontact = adminSendOrderCm.BussinessContact,
                Prescription = adminSendOrderCm.Prescreption,
                Noofrefill = adminSendOrderCm.NoOfRefils,
                Createddate = DateTime.Now,
                Createdby = aspId.ToString(),
            };

            _context.Orderdetails.Add(orderData);
            _context.SaveChanges();
        }

        #endregion


        #region Clear Case

        public void SetClearCaseData(int requestId)
        {
            var request = _context.Requests.FirstOrDefault(i => i.Requestid == requestId);

            var clearCase = new Requeststatuslog()
            {
                Requestid = requestId,
                Status = 10,
                //Physicianid = request.Physicianid,
                Createddate = DateTime.Now,
                Notes = "Case is Cleared",
            };

            _context.Requeststatuslogs.Add(clearCase);

            request.Status = 10;

            _context.SaveChanges();
        }

        #endregion


        #region Send Agreement

        public SendAgreementModal GetSendAgreementModal(int requestid, int reuqesttypeid)
        {
            var request = _context.Requestclients.FirstOrDefault(i => i.Requestid == requestid);

            var agreementData = new SendAgreementModal()
            {
                RequestId = requestid,
                RequestTypeId = reuqesttypeid,
                Phone = request.Phonenumber,
                Email = request.Email,
            };

            return agreementData;
        }

        public void SendAgreementEmail(SendAgreementModal sendAgreementModal, int aspId)
        {
            var mail = "tatva.dotnet.meetbhalani@outlook.com";
            var password = "eaywlzlxxvvbycfs";
            var email = sendAgreementModal.Email;
            var subject = "Review Agreement";
            var here = "https://localhost:7077/Patient/ReviewAgreement?pid=" + _loginService.Encrypt(sendAgreementModal.RequestId.ToString());
            var message = $"First you have to login to your account and then click <a href=\"{here}\">here</a> to review your agreement";

            SmtpClient smtpClient = new SmtpClient("smtp.office365.com", 587)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(mail, password)
            };

            MailMessage mailMessage = new MailMessage(from: mail, to: email, subject, message);
            mailMessage.IsBodyHtml = true;

            smtpClient.SendMailAsync(mailMessage);

            var aspData = _context.Aspnetusers.FirstOrDefault(i => i.Id == aspId);
            var requestData = _context.Requests.FirstOrDefault(i => i.Requestid == sendAgreementModal.RequestId);


            var emailLog = new Emaillog()
            {
                Requestid = sendAgreementModal.RequestId,
                Confirmationnumber = requestData.Confirmationnumber,
                Subjectname = subject,
                Emailid = email,
                Roleid = 3,
                Createdate = DateTime.Now,
                Sentdate = DateTime.Now,
                Isemailsent = new BitArray(1, true),
                Senttries = 1,
            };

            if (aspData != null && aspData.Roleid == 2)
            {
                var physician = _context.Physicians.FirstOrDefault(i => i.Aspnetuserid == aspId);

                emailLog.Physicianid = physician.Physicianid;
            }

            if (aspData != null && aspData.Roleid == 1)
            {
                var admin = _context.Admins.FirstOrDefault(i => i.Aspnetuserid == aspId);

                emailLog.Adminid = admin.Adminid;
            }

            _context.Emaillogs.Add(emailLog);
            _context.SaveChanges();
        }

        #endregion


        #region Send Link

        public void SendSubmitRequestLink(SendLink sendLink, int aspId)
        {
            var mail = "tatva.dotnet.meetbhalani@outlook.com";
            var password = "eaywlzlxxvvbycfs";
            var email = sendLink.Email;
            var subject = "Submit Request";
            var link = "https://localhost:7077/Patient/SubmitRequest";
            var message = $"Hey <b>{sendLink.FirstName}</b>, <br>" +
                $"Check below your details : <br>" +
                $"Firstname : {sendLink.FirstName} <br>" +
                $"Lastname : {sendLink.LastName} <br>" +
                $"Phonenumber : {sendLink.Phone} <br><br><br>" +
                $"Click <a href=\"{link}\">here</a> to submit request";

            SmtpClient smtpClient = new SmtpClient("smtp.office365.com", 587)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(mail, password)
            };

            MailMessage mailMessage = new MailMessage(from: mail, to: email, subject, message);
            mailMessage.IsBodyHtml = true;

            smtpClient.SendMailAsync(mailMessage);

            var aspData = _context.Aspnetusers.FirstOrDefault(i => i.Id == aspId);

            var emailLog = new Emaillog()
            {
                Subjectname = subject,
                Emailid = email,
                Roleid = 3,
                Createdate = DateTime.Now,
                Sentdate = DateTime.Now,
                Isemailsent = new BitArray(1, true),
                Senttries = 1,
            };

            if (aspData != null && aspData.Roleid == 2)
            {
                var physician = _context.Physicians.FirstOrDefault(i => i.Aspnetuserid == aspId);

                emailLog.Physicianid = physician.Physicianid;
            }

            if (aspData != null && aspData.Roleid == 1)
            {
                var admin = _context.Admins.FirstOrDefault(i => i.Aspnetuserid == aspId);

                emailLog.Adminid = admin.Adminid;
            }

            _context.Emaillogs.Add(emailLog);
            _context.SaveChanges();
        }

        #endregion


        #region Create Request

        public void SendCreateRequestData(AdminCreateRequestCm adminCreateRequestCm, int aspId)
        {
            Aspnetuser? aspnetuser = _context.Aspnetusers.FirstOrDefault(x => x.Email == adminCreateRequestCm.Email);
            var user = _context.Users.FirstOrDefault(i => i.Email == adminCreateRequestCm.Email);

            if (user == null)
            {
                var mail = "tatva.dotnet.meetbhalani@outlook.com";
                var password = "eaywlzlxxvvbycfs";
                var email = adminCreateRequestCm.Email;
                var subject = "Create Account";
                var link = "https://localhost:7077/Patient/PatientRegister?email=" + _loginService.Encrypt(adminCreateRequestCm.Email);
                var message = $"Hey <b>{adminCreateRequestCm.FirstName}</b>, <br>" +
                    $"Your request is created, click <a href=\"{link}\">here</a> to register and access it...";

                SmtpClient smtpClient = new SmtpClient("smtp.office365.com", 587)
                {
                    EnableSsl = true,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(mail, password)
                };

                MailMessage mailMessage = new MailMessage(from: mail, to: email, subject, message);
                mailMessage.IsBodyHtml = true;

                smtpClient.SendMailAsync(mailMessage);

                var aspData = _context.Aspnetusers.FirstOrDefault(i => i.Id == aspId);

                var emailLog = new Emaillog()
                {
                    Subjectname = subject,
                    Emailid = email,
                    Roleid = 3,
                    Createdate = DateTime.Now,
                    Sentdate = DateTime.Now,
                    Isemailsent = new BitArray(1, true),
                    Senttries = 1,
                };

                if (aspData != null && aspData.Roleid == 2)
                {
                    var physician = _context.Physicians.FirstOrDefault(i => i.Aspnetuserid == aspId);

                    emailLog.Physicianid = physician.Physicianid;
                }

                if (aspData != null && aspData.Roleid == 1)
                {
                    var admin = _context.Admins.FirstOrDefault(i => i.Aspnetuserid == aspId);

                    emailLog.Adminid = admin.Adminid;
                }

                _context.Emaillogs.Add(emailLog);
                _context.SaveChanges();
            }

            InsertRequestData(adminCreateRequestCm, 1, aspId);
        }

        public void InsertRequestData(AdminCreateRequestCm adminCreateRequestCm, int reqTypeId, int aspId)
        {
            User? user = _context.Users.FirstOrDefault(i => i.Email == adminCreateRequestCm.Email);
            var physician = _context.Physicians.FirstOrDefault(i => i.Aspnetuserid == aspId);

            var requestData = new Request()
            {
                Userid = user == null ? null : user.Userid,
                Requesttypeid = reqTypeId,
                Firstname = adminCreateRequestCm.FirstName,
                Lastname = adminCreateRequestCm.LastName,
                Email = adminCreateRequestCm.Email,
                Phonenumber = adminCreateRequestCm.Phone,
                Createddate = DateTime.Now,
            };

            if (physician != null)
            {
                requestData.Status = 2;
                requestData.Physicianid = physician.Physicianid;
            }
            else
            {
                requestData.Status = 1;
            }

            _context.Requests.Add(requestData);
            _context.SaveChanges();

            int requestId = requestData.Requestid;

            InsertRequestClientData(adminCreateRequestCm, requestId);

            InsertNotesData(adminCreateRequestCm, requestId, aspId);
        }

        public void InsertRequestClientData(AdminCreateRequestCm adminCreateRequestCm, int requestId)
        {
            var clientData = new Requestclient()
            {
                Requestid = requestId,
                Firstname = adminCreateRequestCm.FirstName,
                Lastname = adminCreateRequestCm.LastName,
                Email = adminCreateRequestCm.Email,
                Phonenumber = adminCreateRequestCm.Phone,
                Street = adminCreateRequestCm.Street,
                City = adminCreateRequestCm.City,
                State = _context.Regions.Where(x => x.Regionid == adminCreateRequestCm.RegionId).Select(x => x.Name).FirstOrDefault(),
                Regionid = adminCreateRequestCm.RegionId,
                Zipcode = adminCreateRequestCm.Zipcode,
                Intyear = adminCreateRequestCm.DateOfBirth?.Year,
                Intdate = adminCreateRequestCm.DateOfBirth?.Day,
                Strmonth = adminCreateRequestCm.DateOfBirth?.Month.ToString(),
            };

            _context.Requestclients.Add(clientData);
            _context.SaveChanges();
        }

        public void InsertNotesData(AdminCreateRequestCm adminCreateRequestCm, int requestId, int aspId)
        {
            var aspData = _context.Aspnetusers.FirstOrDefault(i => i.Id == aspId);

            var notesData = new Requestnote()
            {
                Requestid = requestId,
                Createdby = aspId,
                Createddate = DateTime.Now,
            };

            if (aspData != null && aspData.Roleid == 2)
            {
                notesData.Physiciannotes = adminCreateRequestCm.AdminNotes;
            }

            if (aspData != null && aspData.Roleid == 1)
            {
                notesData.Adminnotes = adminCreateRequestCm.AdminNotes;
            }

            _context.Requestnotes.Add(notesData);
            _context.SaveChanges();
        }

        public bool CheckRegion(int region)
        {
            if (region == 0)
            {
                return false;
            }
            var regionName = _context.Regions.FirstOrDefault(x => x.Regionid == region).Name;
            bool isAvailable = _context.Regions.Any(x => x.Name == regionName);

            return isAvailable;
        }

        #endregion


        #region Close Case

        public AdminCloseCaseCm GetCloseCaseData(int requestId)
        {
            Request? request = _context.Requests.FirstOrDefault(x => x.Requestid == requestId);
            Requestclient? client = _context.Requestclients.FirstOrDefault(x => x.Requestid == requestId);

            var BirthDay = Convert.ToInt32(client.Intdate);
            var BirthMonth = Convert.ToInt32(client.Strmonth);
            var BirthYear = Convert.ToInt32(client.Intyear);

            var caseData = new AdminCloseCaseCm()
            {
                FirstName = client.Firstname,
                LastName = client.Lastname,
                ConfirmationNumber = request.Confirmationnumber,
                BirthDate = new DateTime(BirthYear, BirthMonth, BirthDay),
                PhoneWithoutCode = client.Phonenumber,
                Email = client.Email,
                RequestId = requestId,
            };

            return caseData;
        }

        public List<Documents> GetCloseCaseDocuments(int requestId)
        {
            var files = _context.Requestwisefiles.Where(x => x.Requestid == requestId);

            var documentList = files.Select(x => new Documents()
            {
                requestWiseFileId = x.Requestwisefileid,
                requestId = x.Requestid,
                documentName = x.Filename,
                uploadDate = x.Createddate.ToShortDateString(),

            }).ToList();

            return documentList;
        }

        public void UpdateCloseCaseData(AdminCloseCaseCm adminCloseCaseCm)
        {
            var client = _context.Requestclients.FirstOrDefault(x => x.Requestid == adminCloseCaseCm.RequestId);

            client.Email = adminCloseCaseCm.Email;
            client.Phonenumber = adminCloseCaseCm.Phone;

            _context.SaveChanges();
        }


        public void SetClosedCase(int requestId, int aspId)
        {
            var request = _context.Requests.FirstOrDefault(x => x.Requestid == requestId);
            var user = _context.Aspnetusers.FirstOrDefault(x => x.Id == aspId);

            string closeNotes = user.Username + " closed request on " + DateTime.Now.ToString("MMM dd, yyyy") + " at " + DateTime.Now.ToString("hh:mm tt");

            var logData = new Requeststatuslog()
            {
                Requestid = requestId,
                Status = 9,
                Notes = closeNotes,
                Createddate = DateTime.Now,
            };

            request.Status = 9;
            request.Lastreservationdate = DateTime.Now;

            _context.Requeststatuslogs.Add(logData);
            _context.SaveChanges();

        }

        #endregion


        #region Request DTY

        public bool RequestSupportViaEmail(string message, int adminId)
        {
            try
            {
                if (message != null)
                {
                    var currentTime = new TimeOnly(DateTime.Now.Hour, DateTime.Now.Minute);
                    BitArray deletedBit = new BitArray(new[] { false });

                    var offDutyQuery = _context.Physicians
                        .Where(p => !_context.Shiftdetails.Any(sd => sd.Shift.Physicianid == p.Physicianid &&
                                                                       sd.Shiftdate.Date == DateTime.Today &&
                                                                       currentTime >= sd.Starttime &&
                                                                       currentTime <= sd.Endtime &&
                                                                       sd.Isdeleted.Equals(deletedBit)) && p.Isdeleted == null).ToList();

                    foreach (var obj in offDutyQuery)
                    {

                        var mail = "tatva.dotnet.meetbhalani@outlook.com";
                        var password = "eaywlzlxxvvbycfs";
                        var email = obj.Email;
                        var subject = "Need Support";

                        SmtpClient smtpClient = new SmtpClient("smtp.office365.com", 587)
                        {
                            EnableSsl = true,
                            UseDefaultCredentials = false,
                            Credentials = new NetworkCredential(mail, password)
                        };

                        MailMessage mailMessage = new MailMessage(from: mail, to: email, subject, message);
                        smtpClient.SendMailAsync(mailMessage);

                        var emailLog = new Emaillog()
                        {
                            Subjectname = subject,
                            Emailid = obj.Email,
                            Roleid = 2,
                            Adminid = adminId,
                            Createdate = DateTime.Now,
                            Sentdate = DateTime.Now,
                            Isemailsent = new BitArray(1, true),
                            Senttries = 1,
                        };

                        _context.Emaillogs.Add(emailLog);
                        _context.SaveChanges();
                    }
                }
                return true;

            }
            catch
            {
                return false;
            }
        }


        #endregion


        #region Encounter

        public EncounterCm GetEncounterData(int requestid, int status)
        {
            var clientdata = _context.Requestclients.FirstOrDefault(i => i.Requestid == requestid);
            var requestData = _context.Requests.FirstOrDefault(i => i.Requestid == requestid);
            var providerdata = _context.Encounters.FirstOrDefault(i => i.RequestId == requestid);
            var userId = _context.Requests.FirstOrDefault(i => i.Requestid == requestid).Userid;

            var birthDate = Convert.ToInt32(clientdata.Intdate);
            var birthMonth = Convert.ToInt32(clientdata.Strmonth);
            var birthYear = Convert.ToInt32(clientdata.Intyear);

            if (providerdata != null)
            {
                var AdminEncounter = new EncounterCm()
                {
                    userId = userId == null ? 0 : (int)userId,
                    statusForName = status,
                    reqid = requestid,
                    FirstName = clientdata.Firstname,
                    LastName = clientdata.Lastname,
                    Location = clientdata.Street + ", " + clientdata.City + ", " + clientdata.State + ", " + clientdata.Zipcode,
                    BirthDate = new DateTime(birthYear, birthMonth, birthDate),
                    Email = clientdata.Email,
                    PhoneNumber = clientdata.Phonenumber,
                    Date = requestData.Accepteddate?.ToString("yyyy-MM-dd"),
                    HistoryIllness = providerdata.HistoryIllness,
                    MedicalHistory = providerdata.MedicalHistory,
                    Medications = providerdata.Medications,
                    Allergies = providerdata.Allergies,
                    Temp = providerdata.Temp,
                    Hr = providerdata.Hr,
                    Rr = providerdata.Rr,
                    BpD = providerdata.BpD,
                    BpS = providerdata.BpS,
                    O2 = providerdata.O2,
                    Pain = providerdata.Pain,
                    Heent = providerdata.Heent,
                    Cv = providerdata.Cv,
                    Chest = providerdata.Chest,
                    Abd = providerdata.Abd,
                    Extr = providerdata.Extr,
                    Skin = providerdata.Skin,
                    Neuro = providerdata.Neuro,
                    Other = providerdata.Other,
                    Diagnosis = providerdata.Diagnosis,
                    TreatmentPlan = providerdata.TreatmentPlan,
                    MedicationDispensed = providerdata.MedicationDispensed,
                    Procedures = providerdata.Procedures,
                    FollowUp = providerdata.FollowUp,
                    isEncounter = true,
                };

                return AdminEncounter;
            }
            else
            {
                var AdminEncounter = new EncounterCm()
                {
                    userId = userId == null ? 0 : (int)userId,
                    statusForName = status,
                    reqid = requestid,
                    FirstName = clientdata.Firstname,
                    LastName = clientdata.Lastname,
                    Location = clientdata.Street + ", " + clientdata.City + ", " + clientdata.State + ", " + clientdata.Zipcode,
                    BirthDate = new DateTime(birthYear, birthMonth, birthDate),
                    Date = requestData.Accepteddate?.ToString("yyyy-MM-dd"),
                    Email = clientdata.Email,
                    PhoneNumber = clientdata.Phonenumber,
                    isEncounter = false,
                };

                return AdminEncounter;
            }

        }

        public void SetEncounterData(EncounterCm encounterCm)
        {
            var patientdata = _context.Encounters.FirstOrDefault(i => i.RequestId == encounterCm.reqid);

            if (patientdata != null)
            {
                patientdata.HistoryIllness = encounterCm.HistoryIllness;
                patientdata.MedicalHistory = encounterCm.MedicalHistory;
                patientdata.Medications = encounterCm.Medications;
                patientdata.Allergies = encounterCm.Allergies;
                patientdata.Temp = encounterCm.Temp;
                patientdata.Hr = encounterCm.Hr;
                patientdata.Rr = encounterCm.Rr;
                patientdata.BpD = encounterCm.BpD;
                patientdata.BpS = encounterCm.BpS;
                patientdata.O2 = encounterCm.O2;
                patientdata.Pain = encounterCm.Pain;
                patientdata.Heent = encounterCm.Heent;
                patientdata.Cv = encounterCm.Cv;
                patientdata.Chest = encounterCm.Chest;
                patientdata.Abd = encounterCm.Abd;
                patientdata.Extr = encounterCm.Extr;
                patientdata.Skin = encounterCm.Skin;
                patientdata.Neuro = encounterCm.Neuro;
                patientdata.Other = encounterCm.Other;
                patientdata.Diagnosis = encounterCm.Diagnosis;
                patientdata.TreatmentPlan = encounterCm.TreatmentPlan;
                patientdata.MedicationDispensed = encounterCm.MedicationDispensed;
                patientdata.Procedures = encounterCm.Procedures;
                patientdata.FollowUp = encounterCm.FollowUp;

                _context.SaveChanges();
            }
            else
            {
                var encounterData = new Encounter()
                {
                    RequestId = encounterCm.reqid,
                    HistoryIllness = encounterCm.HistoryIllness,
                    MedicalHistory = encounterCm.MedicalHistory,
                    Medications = encounterCm.Medications,
                    Allergies = encounterCm.Allergies,
                    Temp = encounterCm.Temp,
                    Hr = encounterCm.Hr,
                    Rr = encounterCm.Rr,
                    BpD = encounterCm.BpD,
                    BpS = encounterCm.BpS,
                    O2 = encounterCm.O2,
                    Pain = encounterCm.Pain,
                    Heent = encounterCm.Heent,
                    Cv = encounterCm.Cv,
                    Chest = encounterCm.Chest,
                    Abd = encounterCm.Abd,
                    Extr = encounterCm.Extr,
                    Skin = encounterCm.Skin,
                    Neuro = encounterCm.Neuro,
                    Other = encounterCm.Other,
                    Diagnosis = encounterCm.Diagnosis,
                    TreatmentPlan = encounterCm.TreatmentPlan,
                    MedicationDispensed = encounterCm.MedicationDispensed,
                    Procedures = encounterCm.Procedures,
                    FollowUp = encounterCm.FollowUp,
                };
                _context.Encounters.Add(encounterData);
                _context.SaveChanges();
            }

        }

        #endregion


        #endregion


        #region Profile

        public List<AdminregionTable> GetAdminregions(int aspId)
        {
            var regions = _context.Regions.ToList();
            var adminRegion = _context.Adminregions.ToList();
            var adminId = _context.Admins.FirstOrDefault(x => x.Aspnetuserid == aspId).Adminid;

            var CheckdRegion = regions.Select(r1 => new AdminregionTable
            {
                Regionid = r1.Regionid,
                Name = r1.Name,
                ExistsInTable = adminRegion.Any(r2 => r2.Regionid == r1.Regionid && r2.Adminid == adminId)

            }).ToList();

            return CheckdRegion;
        }

        public AdminProfileCm GetProfileData(int aspId)
        {
            var aspData = _context.Aspnetusers.FirstOrDefault(i => i.Id == aspId);
            var adminData = _context.Admins.FirstOrDefault(i => i.Aspnetuserid == aspId);

            var adminProfileData = new AdminProfileCm()
            {
                AspId = aspId,
                AdminId = adminData.Adminid,
                Username = aspData.Username,
                Status = (short)adminData.Status,
                RoleId = (int)adminData.Roleid,
                Firstname = adminData.Firstname,
                Lastname = adminData.Lastname,
                Email = adminData.Email,
                ConfirmEmail = adminData.Email,
                PhoneWithoutCode = adminData.Mobile,
                MobileWithoutCode = adminData.Altphone,
                Address1 = adminData.Address1,
                Address2 = adminData.Address2,
                City = adminData.City,
                RegionId = (int)adminData.Regionid,
                Zipcode = adminData.Zip,

            };

            return adminProfileData;

        }

        public void AdminResetPassword(string password, int aspId)
        {
            var Aspnetuser = _context.Aspnetusers.FirstOrDefault(i => i.Id == aspId);

            Aspnetuser.Passwordhash = BCrypt.Net.BCrypt.HashPassword(password);
            _context.SaveChanges();
        }

        public void AdminAccountUpdate(short status, int roleId, int aspId)
        {
            var admin = _context.Admins.FirstOrDefault(x => x.Aspnetuserid == aspId);

            if (admin != null)
            {
                admin.Status = status;
                admin.Roleid = roleId;
                admin.Modifiedby = aspId;
                admin.Modifieddate = DateTime.Now;

                _context.SaveChanges();
            }
        }

        public void AdministratorDetail(AdminProfileCm adminProfileCm, List<int> regions)
        {
            var aspData = _context.Aspnetusers.FirstOrDefault(i => i.Id == adminProfileCm.AspId);
            var adminData = _context.Admins.FirstOrDefault(i => i.Adminid == adminProfileCm.AdminId);

            if (aspData != null)
            {
                aspData.Email = adminProfileCm.Email;
                aspData.Phonenumber = adminProfileCm.Phonenumber;
                aspData.Modifieddate = DateTime.Now;
            }

            if (adminData.Adminid != null)
            {
                var Adminregion = _context.Adminregions.Where(i => i.Adminid == adminProfileCm.AdminId).ToList();
                _context.Adminregions.RemoveRange(Adminregion);
            }

            if (adminData != null)
            {
                adminData.Firstname = adminProfileCm.Firstname;
                adminData.Lastname = adminProfileCm.Lastname;
                adminData.Email = adminProfileCm.Email;
                adminData.Mobile = adminProfileCm.Phonenumber;
                adminData.Modifiedby = adminProfileCm.AspId;
                adminData.Modifieddate = DateTime.Now;
                _context.SaveChanges();
            }


            foreach (int regionid in regions)
            {
                Region? region = _context.Regions.FirstOrDefault(r => r.Regionid == regionid);

                _context.Adminregions.Add(new Adminregion
                {
                    Adminid = adminProfileCm.AdminId,
                    Regionid = regionid,
                });

            }
            _context.SaveChanges();

        }

        public void MailingDetail(AdminProfileCm adminProfileCm)
        {
            var Admindata = _context.Admins.FirstOrDefault(i => i.Adminid == adminProfileCm.AdminId);

            if (Admindata != null)
            {

                Admindata.Address1 = adminProfileCm.Address1;
                Admindata.Address2 = adminProfileCm.Address2;
                Admindata.City = adminProfileCm.City;
                Admindata.Regionid = adminProfileCm.RegionId;
                Admindata.Altphone = adminProfileCm.Mobile;
                Admindata.Zip = adminProfileCm.Zipcode;
                Admindata.Modifiedby = adminProfileCm.AspId;
                Admindata.Modifieddate = DateTime.Now;
                _context.SaveChanges();
            }
        }

        public void RemoveAdmin(int adminId)
        {
            var admin = _context.Admins.FirstOrDefault(x => x.Adminid == adminId);

            admin.Isdeleted = true;
            _context.SaveChanges();
        }

        #endregion


        #region Provider

        public List<Provider> GetProviders(int regionId)
        {
            var physicians = _context.Physicians.Where(x => x.Isdeleted == null).ToList();

            if (regionId != 0)
            {
                physicians = _context.Physicians.Where(x => x.Regionid == regionId).ToList();
            }

            var currentTime = new TimeOnly(DateTime.Now.Hour, DateTime.Now.Minute);
            BitArray deletedBit = new BitArray(new[] { false });

            var offDutyQuery = _context.Physicians
                .Include(p => p.Physicianregions)
                .Where(p => (regionId == 0 || p.Physicianregions.Any(pr => pr.Regionid == regionId)) &&
                            !_context.Shiftdetails.Any(sd => sd.Shift.Physicianid == p.Physicianid &&
                                                               sd.Shiftdate.Date == DateTime.Today &&
                                                               currentTime >= sd.Starttime &&
                                                               currentTime <= sd.Endtime &&
                                                               sd.Isdeleted.Equals(deletedBit)) && p.Isdeleted == null)
                .ToList();

            var providerList = physicians.Select(x => new Provider()
            {
                Email = x.Email,
                aspId = (int)x.Aspnetuserid,
                Name = x.Firstname + " " + x.Lastname,
                Role = _context.Roles.FirstOrDefault(i => i.Roleid == x.Roleid).Name,
                CallStatus = offDutyQuery.Where(i => i.Physicianid == x.Physicianid).Count() == 0 ? "Available" : "Unavailable",
                Status = (short)x.Status,
            }).ToList();


            return providerList;
        }

        public List<PhysicianRegionTable> GetPhysicianRegionTables(int aspId)
        {
            var region = _context.Regions.ToList();
            var physicianRegion = _context.Physicianregions.ToList();
            var phycisianId = _context.Physicians.FirstOrDefault(x => x.Aspnetuserid == aspId).Physicianid;

            var checkedRegion = region.Select(r1 => new PhysicianRegionTable
            {
                Regionid = r1.Regionid,
                Name = r1.Name,
                ExistsInTable = physicianRegion.Any(r2 => r2.Regionid == r1.Regionid && r2.Physicianid == phycisianId)

            }).ToList();

            return checkedRegion;
        }

        public void ContactProvider(ProvidersCm providersCm, int aspId)
        {
            var admin = _context.Admins.FirstOrDefault(x => x.Aspnetuserid == aspId);
            var physician = _context.Physicians.FirstOrDefault(x => x.Email == providersCm.Email);

            var mail = "tatva.dotnet.meetbhalani@outlook.com";
            var password = "eaywlzlxxvvbycfs";
            var email = providersCm.Email;
            var subject = "Message From Admin";
            var message = $"Hey, {providersCm.Email.Substring(0, providersCm.Email.IndexOf('@'))} <br><br>" +
                $"Here is the message from {admin.Firstname} {admin.Lastname} : <br>" +
                $"{providersCm.ContactMessage}";

            SmtpClient smtpClient = new SmtpClient("smtp.office365.com", 587)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(mail, password)
            };

            MailMessage mailMessage = new MailMessage(from: mail, to: email, subject, message);
            mailMessage.IsBodyHtml = true;

            smtpClient.SendMailAsync(mailMessage);

            var emailLog = new Emaillog()
            {
                Subjectname = subject,
                Emailid = email,
                Roleid = 1,
                Adminid = admin.Adminid,
                Physicianid = physician.Physicianid,
                Createdate = DateTime.Now,
                Sentdate = DateTime.Now,
                Isemailsent = new BitArray(1, true),
                Senttries = 1,
            };

            _context.Emaillogs.Add(emailLog);
            _context.SaveChanges();
        }

        public ProviderProfileCm GetProviderProfileData(int aspId)
        {
            var aspData = _context.Aspnetusers.FirstOrDefault(x => x.Id == aspId);

            if (aspData != null)
            {
                var provider = _context.Physicians.FirstOrDefault(x => x.Aspnetuserid == aspId);

                var providerData = new ProviderProfileCm()
                {
                    AspId = (int)provider.Aspnetuserid,
                    PhysicianId = provider.Physicianid,
                    Username = aspData.Username,
                    Status = provider.Status,
                    RoleId = provider.Roleid,
                    FirstName = provider.Firstname,
                    LastName = provider.Lastname,
                    Email = provider.Email,
                    PhoneWithoutCode = provider.Mobile,
                    MedicalLicense = provider.Medicallicense,
                    NPINumber = provider.Npinumber,
                    SyncEmail = provider.Syncemailaddress,
                    Address1 = provider.Address1,
                    Address2 = provider.Address2,
                    City = provider.City,
                    RegionId = provider.Regionid,
                    Zipcode = provider.Zip,
                    AltPhoneWithoutCode = provider.Altphone,
                    BusinessName = provider.Businessname,
                    BusinessWebsite = provider.Businesswebsite,
                    PhotoValue = provider.Photo,
                    SignatureValue = provider.Signature,
                    AdminNotes = provider.Adminnotes,
                    IsContractorAgreement = provider.Isagreementdoc == null ? false : true,
                    IsBackgroundCheck = provider.Isbackgrounddoc == null ? false : true,
                    IsHIPAA = provider.Istrainingdoc == null ? false : true,
                    IsNonDisclosure = provider.Isnondisclosuredoc == null ? false : true,
                    IsLicenseDocument = provider.Islicensedoc == null ? false : true,
                };

                return providerData;
            }

            return null;
        }

        public void PhysicianResetPassword(string password, int aspId)
        {
            var aspUser = _context.Aspnetusers.FirstOrDefault(x => x.Id == aspId);

            if (aspUser != null)
            {
                aspUser.Passwordhash = BCrypt.Net.BCrypt.HashPassword(password);
                aspUser.Modifieddate = DateTime.Now;

                _context.SaveChanges();
            }
        }

        public void PhysicianAccountUpdate(short status, int roleId, int aspId)
        {
            var physician = _context.Physicians.FirstOrDefault(x => x.Aspnetuserid == aspId);

            if (physician != null)
            {
                physician.Status = status;
                physician.Roleid = roleId;
                physician.Modifieddate = DateTime.Now;
                physician.Modifiedby = aspId;

                _context.SaveChanges();
            }
        }

        public void PhysicianAdministratorDataUpdate(ProviderProfileCm providerProfileCm, List<int> physicianRegions)
        {
            var physician = _context.Physicians.FirstOrDefault(x => x.Aspnetuserid == providerProfileCm.AspId);

            if (physician != null)
            {
                physician.Firstname = providerProfileCm.FirstName;
                physician.Lastname = providerProfileCm.LastName;
                physician.Mobile = providerProfileCm.Phonenumber;
                physician.Medicallicense = providerProfileCm.MedicalLicense;
                physician.Npinumber = providerProfileCm.NPINumber;
                physician.Syncemailaddress = providerProfileCm.SyncEmail;
                physician.Modifiedby = providerProfileCm.AspId;
                physician.Modifieddate = DateTime.Now;

                if (_context.Physicianregions.Any(x => x.Physicianid == physician.Physicianid))
                {
                    var physicianRegion = _context.Physicianregions.Where(x => x.Physicianid == physician.Physicianid).ToList();

                    _context.Physicianregions.RemoveRange(physicianRegion);
                    _context.SaveChanges();
                }

                foreach (int regionId in physicianRegions)
                {
                    var region = _context.Regions.FirstOrDefault(x => x.Regionid == regionId);

                    _context.Physicianregions.Add(new Physicianregion
                    {
                        Physicianid = providerProfileCm.PhysicianId,
                        Regionid = region.Regionid,
                    });
                }
                _context.SaveChanges();

            }
        }

        public void PhysicianMailingDataUpdate(ProviderProfileCm providerProfileCm)
        {
            var physician = _context.Physicians.FirstOrDefault(x => x.Aspnetuserid == providerProfileCm.AspId);
            var phylocation = _context.Physicianlocations.FirstOrDefault(x => x.Physicianid == providerProfileCm.PhysicianId);

            if (physician != null)
            {
                physician.Address1 = providerProfileCm.Address1;
                physician.Address2 = providerProfileCm.Address2;
                physician.City = providerProfileCm.City;
                physician.Regionid = providerProfileCm.RegionId;
                physician.Zip = providerProfileCm.Zipcode;
                physician.Altphone = providerProfileCm.AltPhone;
                physician.Modifiedby = providerProfileCm.AspId;
                physician.Modifieddate = DateTime.Now;

                _context.SaveChanges();
            }

            if (phylocation != null)
            {
                phylocation.Latitude = providerProfileCm.Latitude;
                phylocation.Longitude = providerProfileCm.Longitude;
                phylocation.Address = providerProfileCm.Address1 + ", " + providerProfileCm.City;
                _context.SaveChanges();
            }

            if (phylocation == null)
            {
                var physicianLocation = new Physicianlocation()
                {
                    Physicianid = providerProfileCm.PhysicianId,
                    Latitude = providerProfileCm.Latitude,
                    Longitude = providerProfileCm.Longitude,
                    Createddate = DateTime.Now,
                    Physicianname = _context.Physicians.FirstOrDefault(i => i.Physicianid == providerProfileCm.PhysicianId).Firstname + " " + _context.Physicians.FirstOrDefault(i => i.Physicianid == providerProfileCm.PhysicianId).Lastname,
                    Address = providerProfileCm.Address1 + ", " + providerProfileCm.City,
                };

                _context.Add(physicianLocation);
                _context.SaveChanges();
            }
        }

        public void PhysicianBusinessInfoUpdate(ProviderProfileCm providerProfileCm)
        {
            var physician = _context.Physicians.FirstOrDefault(x => x.Aspnetuserid == providerProfileCm.AspId);

            if (physician != null)
            {
                physician.Businessname = providerProfileCm.BusinessName;
                physician.Businesswebsite = providerProfileCm.BusinessWebsite;
                physician.Adminnotes = providerProfileCm.AdminNotes;
                physician.Modifiedby = providerProfileCm.AspId;
                physician.Modifieddate = DateTime.Now;

                _context.SaveChanges();

                if (providerProfileCm.Photo != null || providerProfileCm.Signature != null)
                {
                    AddProviderBusinessPhotos(providerProfileCm.Photo, providerProfileCm.Signature, providerProfileCm.AspId);
                }
            }
        }

        public void AddProviderBusinessPhotos(IFormFile photo, IFormFile signature, int aspId)
        {
            var physician = _context.Physicians.FirstOrDefault(x => x.Aspnetuserid == aspId);

            string directory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "content", physician.Physicianid.ToString());

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            if (photo != null)
            {
                var fileName = "Profile_" + DateTime.Now.Date.ToString("yyyy-MM-dd") + "_" + DateTime.Now.ToString("HH-mm-ss") + Path.GetExtension(photo.FileName);

                string path = Path.Combine(directory, fileName);

                if (File.Exists(path))
                {
                    File.Delete(path);
                }

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    photo.CopyTo(fileStream);
                }

                physician.Photo = fileName;
            }

            if (signature != null)
            {
                var fileName = "Signature_" + DateTime.Now.Date.ToString("yyyy-MM-dd") + "_" + DateTime.Now.ToString("HH-mm-ss") + Path.GetExtension(signature.FileName);

                string path = Path.Combine(directory, fileName);

                if (File.Exists(path))
                {
                    File.Delete(path);
                }

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    signature.CopyTo(fileStream);
                }

                physician.Signature = fileName;
            }

            _context.SaveChanges();

        }

        public void EditOnBoardingData(ProviderProfileCm providerProfileCm)
        {
            var physicianData = _context.Physicians.FirstOrDefault(x => x.Aspnetuserid == providerProfileCm.AspId);

            string directory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "content", physicianData.Physicianid.ToString());

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            if (providerProfileCm.ContractorAgreement != null)
            {
                string path = Path.Combine(directory, "Independent_Contractor" + Path.GetExtension(providerProfileCm.ContractorAgreement.FileName));

                if (File.Exists(path))
                {
                    File.Delete(path);
                }

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    providerProfileCm.ContractorAgreement.CopyTo(fileStream);
                }

                physicianData.Isagreementdoc = new BitArray(1, true);
            }

            if (providerProfileCm.BackgroundCheck != null)
            {
                string path = Path.Combine(directory, "Background" + Path.GetExtension(providerProfileCm.BackgroundCheck.FileName));

                if (File.Exists(path))
                {
                    File.Delete(path);
                }

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    providerProfileCm.BackgroundCheck.CopyTo(fileStream);
                }

                physicianData.Isbackgrounddoc = new BitArray(1, true);
            }

            if (providerProfileCm.HIPAA != null)
            {
                string path = Path.Combine(directory, "HIPAA" + Path.GetExtension(providerProfileCm.HIPAA.FileName));

                if (File.Exists(path))
                {
                    File.Delete(path);
                }

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    providerProfileCm.HIPAA.CopyTo(fileStream);
                }

                physicianData.Istrainingdoc = new BitArray(1, true);
            }

            if (providerProfileCm.NonDisclosure != null)
            {
                string path = Path.Combine(directory, "Non_Disclosure" + Path.GetExtension(providerProfileCm.NonDisclosure.FileName));

                if (File.Exists(path))
                {
                    File.Delete(path);
                }

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    providerProfileCm.NonDisclosure.CopyTo(fileStream);
                }

                physicianData.Isnondisclosuredoc = new BitArray(1, true);
            }

            if (providerProfileCm.LicenseDocument != null)
            {
                string path = Path.Combine(directory, "Licence" + Path.GetExtension(providerProfileCm.LicenseDocument.FileName));

                if (File.Exists(path))
                {
                    File.Delete(path);
                }

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    providerProfileCm.LicenseDocument.CopyTo(fileStream);
                }

                physicianData.Islicensedoc = new BitArray(1, true);
            }

            _context.SaveChanges();

        }

        public void RemovePhysician(int physicianId)
        {
            var physician = _context.Physicians.FirstOrDefault(x => x.Physicianid == physicianId);

            physician.Isdeleted = new BitArray(1, true);
            _context.SaveChanges();
        }

        public bool CreatePhysicianAccount(ProviderProfileCm providerProfileCm, List<int> physicianRegions)
        {
            if (!_context.Aspnetusers.Any(x => x.Email == providerProfileCm.Email))
            {
                var aspData = new Aspnetuser()
                {
                    Username = providerProfileCm.Username,
                    Passwordhash = BCrypt.Net.BCrypt.HashPassword(providerProfileCm.CreatePhyPassword),
                    Email = providerProfileCm.Email,
                    Phonenumber = providerProfileCm.Phonenumber,
                    Createddate = DateTime.Now,
                    Roleid = 2,
                };
                _context.Aspnetusers.Add(aspData);
                _context.SaveChanges();


                var physicianData = new Physician()
                {
                    Aspnetuserid = aspData.Id,
                    Firstname = providerProfileCm.FirstName,
                    Lastname = providerProfileCm.LastName,
                    Email = providerProfileCm.Email,
                    Mobile = providerProfileCm.Phonenumber,
                    Medicallicense = providerProfileCm.MedicalLicense,
                    Adminnotes = providerProfileCm.AdminNotes,
                    Address1 = providerProfileCm.Address1,
                    Address2 = providerProfileCm.Address2,
                    City = providerProfileCm.City,
                    Regionid = providerProfileCm.RegionId,
                    Zip = providerProfileCm.Zipcode,
                    Altphone = providerProfileCm.AltPhone,
                    Createdby = providerProfileCm.AspId,
                    Createddate = DateTime.Now,
                    Status = 1,
                    Businessname = providerProfileCm.BusinessName,
                    Businesswebsite = providerProfileCm.BusinessWebsite,
                    Roleid = providerProfileCm.RoleId,
                };
                _context.Physicians.Add(physicianData);
                _context.SaveChanges();

                foreach (int regionId in physicianRegions)
                {
                    var region = _context.Regions.FirstOrDefault(x => x.Regionid == regionId);

                    _context.Physicianregions.Add(new Physicianregion
                    {
                        Physicianid = physicianData.Physicianid,
                        Regionid = region.Regionid,
                    });
                }
                _context.SaveChanges();

                var phyLocation = new Physicianlocation()
                {
                    Physicianid = physicianData.Physicianid,
                    Latitude = providerProfileCm.Latitude,
                    Longitude = providerProfileCm.Longitude,
                    Createddate = DateTime.Now,
                    Physicianname = providerProfileCm.FirstName + " " + providerProfileCm.LastName,
                    Address = providerProfileCm.City + "," + _context.Regions.FirstOrDefault(i => i.Regionid == providerProfileCm.RegionId).Name,
                };
                _context.Add(phyLocation);
                _context.SaveChanges();

                AddProviderDocuments(physicianData.Physicianid, providerProfileCm.Photo, providerProfileCm.ContractorAgreement, providerProfileCm.BackgroundCheck, providerProfileCm.HIPAA, providerProfileCm.NonDisclosure);

                return true;
            }

            return false;
        }

        public void AddProviderDocuments(int Physicianid, IFormFile Photo, IFormFile ContractorAgreement, IFormFile BackgroundCheck, IFormFile HIPAA, IFormFile NonDisclosure)
        {
            var physicianData = _context.Physicians.FirstOrDefault(x => x.Physicianid == Physicianid);

            string directory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "content", Physicianid.ToString());

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            if (Photo != null)
            {
                var fileName = "Profile_" + DateTime.Now.Date.ToString("yyyy-MM-dd") + "_" + DateTime.Now.ToString("HH-mm-ss") + Path.GetExtension(Photo.FileName);

                string path = Path.Combine(directory, fileName);

                if (File.Exists(path))
                {
                    File.Delete(path);
                }

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    Photo.CopyTo(fileStream);
                }

                physicianData.Photo = fileName;
            }


            if (ContractorAgreement != null)
            {
                string path = Path.Combine(directory, "Independent_Contractor" + Path.GetExtension(ContractorAgreement.FileName));

                if (File.Exists(path))
                {
                    File.Delete(path);
                }

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    ContractorAgreement.CopyTo(fileStream);
                }

                physicianData.Isagreementdoc = new BitArray(1, true);
            }

            if (BackgroundCheck != null)
            {
                string path = Path.Combine(directory, "Background" + Path.GetExtension(BackgroundCheck.FileName));

                if (File.Exists(path))
                {
                    File.Delete(path);
                }

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    BackgroundCheck.CopyTo(fileStream);
                }

                physicianData.Isbackgrounddoc = new BitArray(1, true);
            }

            if (HIPAA != null)
            {
                string path = Path.Combine(directory, "HIPAA" + Path.GetExtension(HIPAA.FileName));

                if (File.Exists(path))
                {
                    File.Delete(path);
                }

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    HIPAA.CopyTo(fileStream);
                }

                physicianData.Istrainingdoc = new BitArray(1, true);
            }

            if (NonDisclosure != null)
            {
                string path = Path.Combine(directory, "Non_Disclosure" + Path.GetExtension(NonDisclosure.FileName));

                if (File.Exists(path))
                {
                    File.Delete(path);
                }

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    NonDisclosure.CopyTo(fileStream);
                }

                physicianData.Isnondisclosuredoc = new BitArray(1, true);
            }

            _context.SaveChanges();
        }

        #endregion


        #region Account Access

        public List<AccountAccess> GetAccountAccessData()
        {
            BitArray deletedBit = new BitArray(new[] { false });
            var Roles = _context.Roles.Where(i => i.Isdeleted.Equals(deletedBit));

            var Accessdata = Roles.Select(r => new AccountAccess()
            {
                name = r.Name,
                accounttype = _context.Aspnetroles.FirstOrDefault(x => x.Id == r.Accounttype).Name,
                accounttypeid = r.Accounttype,
                roleid = r.Roleid,
            }).ToList();

            return Accessdata;
        }

        public List<AccountMenu> GetAccountMenu(int accounttype, int roleid)
        {

            var menu = _context.Menus.Where(r => r.Accounttype == accounttype).ToList();


            var rolemenu = _context.Rolemenus.ToList();

            var checkedMenu = menu.Select(r1 => new AccountMenu
            {
                menuid = r1.Menuid,
                name = r1.Name,
                ExistsInTable = rolemenu.Any(r2 => r2.Roleid == roleid && r2.Menuid == r1.Menuid),

            }).ToList();

            return checkedMenu;

        }

        public void SetCreateAccessAccount(AccountAccess accountAccess, List<int> AccountMenu)
        {
            if (accountAccess != null)
            {
                var role = new Role()
                {
                    Roleid = 6,
                    Name = accountAccess.name,
                    Accounttype = (short)accountAccess.accounttypeid,
                    Createdby = 1,
                    Createddate = DateTime.Now,
                    Isdeleted = new BitArray(1, false),

                };
                _context.Add(role);
                _context.SaveChanges();

                if (AccountMenu != null)
                {
                    foreach (int menuid in AccountMenu)
                    {


                        _context.Rolemenus.Add(new Rolemenu
                        {
                            Roleid = role.Roleid,
                            Menuid = menuid,
                        });
                    }
                    _context.SaveChanges();

                }
            }
        }

        public AccountAccess GetEditAccessData(int roleid)
        {
            var role = _context.Roles.FirstOrDefault(i => i.Roleid == roleid);
            if (role != null)
            {
                var roledata = new AccountAccess()
                {
                    name = role.Name,
                    roleid = roleid,
                    accounttypeid = role.Accounttype,
                };
                return roledata;
            }
            return null;
        }

        public void SetEditAccessAccount(AccountAccess accountAccess, List<int> AccountMenu)
        {
            var role = _context.Roles.FirstOrDefault(x => x.Roleid == accountAccess.roleid);
            if (role != null)
            {

                role.Name = accountAccess.name;
                role.Accounttype = (short)accountAccess.accounttypeid;
                role.Createdby = 1;
                role.Modifieddate = DateTime.Now;

                _context.SaveChanges();

                var rolemenu = _context.Rolemenus.Where(i => i.Roleid == accountAccess.roleid).ToList();
                if (rolemenu != null)
                {
                    _context.Rolemenus.RemoveRange(rolemenu);
                }

                if (AccountMenu != null)
                {
                    foreach (int menuid in AccountMenu)
                    {
                        _context.Rolemenus.Add(new Rolemenu
                        {
                            Roleid = role.Roleid,
                            Menuid = menuid,
                        });
                    }
                    _context.SaveChanges();

                }
            }
        }

        public void DeleteAccountAccess(int roleid)
        {
            var role = _context.Roles.FirstOrDefault(x => x.Roleid == roleid);
            if (role != null)
            {
                role.Isdeleted = new BitArray(1, true);
                _context.SaveChanges();
            }


            var rolemenu = _context.Rolemenus.Where(i => i.Roleid == roleid).ToList();
            if (rolemenu != null)
            {
                _context.Rolemenus.RemoveRange(rolemenu);
            }

            _context.SaveChanges();
        }

        #endregion


        #region User Access 

        public List<UserAccess> GetUserAccessData(int accountTypeId)
        {
            var admin = _context.Admins.Where(i => i.Isdeleted == null).ToList();
            var physician = _context.Physicians.Where(i => i.Isdeleted == null).ToList();

            var adminList = admin.Select(x => new UserAccess
            {
                AspId = (int)x.Aspnetuserid,
                AccountTypeId = 1,
                AccountType = "Admin",
                AccountHolderName = x.Firstname + " " + x.Lastname,
                AccountPhone = x.Mobile,
                AccountStatus = (short)x.Status,
                AccountRequests = _context.Requests.Where(y => y.Status != 10 || y.Status != 11).Count(),
            }).ToList();

            var physicianList = physician.Select(x => new UserAccess
            {
                AspId = (int)x.Aspnetuserid,
                AccountTypeId = 2,
                AccountType = "Provider",
                AccountHolderName = x.Firstname + " " + x.Lastname,
                AccountPhone = x.Mobile,
                AccountStatus = (short)x.Status,
                AccountRequests = _context.Requests.Where(y => y.Physicianid == x.Physicianid && (y.Status == 2 || y.Status == 4 || y.Status == 5 || y.Status == 6)).Count(),
            }).ToList();

            var finalList = adminList.Concat(physicianList).ToList();

            if (accountTypeId == 1)
            {
                return adminList;
            }

            else if (accountTypeId == 2)
            {
                return physicianList;
            }

            else if (accountTypeId == 3)
            {
                return null;
            }

            return finalList;
        }

        public bool CreateAdminAccount(AdminProfileCm adminProfileCm, List<int> adminRegions)
        {
            if (!_context.Aspnetusers.Any(x => x.Email == adminProfileCm.Email))
            {
                var aspData = new Aspnetuser()
                {
                    Username = adminProfileCm.Username,
                    Passwordhash = BCrypt.Net.BCrypt.HashPassword(adminProfileCm.CreateAdminPassword),
                    Email = adminProfileCm.Email,
                    Phonenumber = adminProfileCm.Phonenumber,
                    Createddate = DateTime.Now,
                    Roleid = 1,
                };
                _context.Aspnetusers.Add(aspData);
                _context.SaveChanges();


                var adminData = new Admin()
                {
                    Aspnetuserid = aspData.Id,
                    Firstname = adminProfileCm.Firstname,
                    Lastname = adminProfileCm.Lastname,
                    Email = adminProfileCm.Email,
                    Mobile = adminProfileCm.Phonenumber,
                    Address1 = adminProfileCm.Address1,
                    Address2 = adminProfileCm.Address2,
                    City = adminProfileCm.City,
                    Regionid = adminProfileCm.RegionId,
                    Zip = adminProfileCm.Zipcode,
                    Altphone = adminProfileCm.Mobile,
                    Createdby = adminProfileCm.AspId,
                    Createddate = DateTime.Now,
                    Status = 1,
                    Roleid = adminProfileCm.RoleId,
                };
                _context.Admins.Add(adminData);
                _context.SaveChanges();

                foreach (int regionId in adminRegions)
                {
                    var region = _context.Regions.FirstOrDefault(x => x.Regionid == regionId);

                    _context.Physicianregions.Add(new Physicianregion
                    {
                        Physicianid = adminData.Adminid,
                        Regionid = region.Regionid,
                    });
                }
                _context.SaveChanges();

                return true;
            }

            return false;
        }

        #endregion


        #region Scheduling

        public List<ShiftDetailsmodal> ShiftDetailsmodal(DateTime date, DateTime sunday, DateTime saturday, string type, int aspId)
        {
            var physician = _context.Physicians.FirstOrDefault(i => i.Aspnetuserid == aspId);

            var shiftdetails = _context.Shiftdetails.Where(u => u.Shiftdate.Month == date.Month && u.Shiftdate.Year == date.Year);

            BitArray deletedBit = new BitArray(new[] { true });

            switch (type)
            {
                case "month":
                    shiftdetails = _context.Shiftdetails.Where(u => u.Shiftdate.Month == date.Month && u.Shiftdate.Year == date.Year && !u.Isdeleted.Equals(deletedBit));
                    break;

                case "week":
                    shiftdetails = _context.Shiftdetails.Where(u => u.Shiftdate >= sunday.Date && u.Shiftdate <= saturday && !u.Isdeleted.Equals(deletedBit));
                    break;

                case "day":
                    shiftdetails = _context.Shiftdetails.Where(u => u.Shiftdate.Month == date.Month && u.Shiftdate.Year == date.Year && u.Shiftdate.Day == date.Day && !u.Isdeleted.Equals(deletedBit));
                    break;
            }


            var list = shiftdetails.Where(s => !_context.Physicians.Any(p => p.Physicianid == s.Shift.Physicianid && p.Isdeleted != null)).Select(s => new ShiftDetailsmodal
            {
                Shiftid = s.Shiftid,
                Shiftdetailid = s.Shiftdetailid,
                Shiftdate = s.Shiftdate,
                Startdate = s.Shift.Startdate,
                Starttime = s.Starttime,
                Endtime = s.Endtime,
                Physicianid = s.Shift.Physicianid,
                PhysicianName = s.Shift.Physician.Firstname,
                Status = s.Status,
                regionname = _context.Regions.FirstOrDefault(i => i.Regionid == s.Regionid).Name,
                Abberaviation = _context.Regions.FirstOrDefault(i => i.Regionid == s.Regionid).Abbreviation,
                Regionid = s.Regionid,
            });


            if (physician != null)
            {
                list = list.Where(i => i.Physicianid == physician.Physicianid);
            }

            return list.ToList();
        }

        public bool createShift(ScheduleModel scheduleModel, int Aspid)
        {
            if (_context.Shifts.Where(x => x.Physicianid == scheduleModel.Physicianid).Count() >= 1)
            {
                var shiftData = _context.Shifts.Where(i => i.Physicianid == scheduleModel.Physicianid).ToList();
                var shiftDetailData = new List<Shiftdetail>();

                foreach (var obj in shiftData)
                {
                    var details = _context.Shiftdetails.Where(x => x.Shiftid == obj.Shiftid && x.Isdeleted != new BitArray(1, true)).ToList();
                    shiftDetailData.AddRange(details);
                }


                foreach (var obj in shiftDetailData)
                {
                    var shiftDate = new DateTime(scheduleModel.Startdate.Year, scheduleModel.Startdate.Month, scheduleModel.Startdate.Day);

                    if (obj.Shiftdate.Date == shiftDate.Date)
                    {
                        if ((obj.Starttime <= scheduleModel.Starttime && obj.Endtime >= scheduleModel.Starttime) || (obj.Starttime <= scheduleModel.Endtime && obj.Endtime >= scheduleModel.Endtime) || (obj.Starttime >= scheduleModel.Starttime && obj.Endtime <= scheduleModel.Endtime))
                        {
                            return false;
                        }
                    }
                }
            }

            Shift shift = new Shift();
            shift.Physicianid = scheduleModel.Physicianid;
            shift.Repeatupto = scheduleModel.Repeatupto;
            shift.Startdate = scheduleModel.Startdate;
            shift.Createdby = Aspid;
            shift.Createddate = DateTime.Now;
            shift.Isrepeat = scheduleModel.Isrepeat == false ? new BitArray(1, false) : new BitArray(1, true);
            shift.Repeatupto = scheduleModel.Repeatupto;
            _context.Shifts.Add(shift);
            _context.SaveChanges();

            Shiftdetail sd = new Shiftdetail();
            sd.Shiftid = shift.Shiftid;
            sd.Shiftdate = new DateTime(scheduleModel.Startdate.Year, scheduleModel.Startdate.Month, scheduleModel.Startdate.Day);
            sd.Starttime = scheduleModel.Starttime;
            sd.Endtime = scheduleModel.Endtime;
            sd.Regionid = scheduleModel.Regionid;
            sd.Status = 1;
            sd.Isdeleted = new BitArray(1, false);
            _context.Shiftdetails.Add(sd);
            _context.SaveChanges();

            Shiftdetailregion sr = new Shiftdetailregion();
            sr.Shiftdetailid = sd.Shiftdetailid;
            sr.Regionid = scheduleModel.Regionid;
            sr.Isdeleted = new BitArray(1, false);
            _context.Shiftdetailregions.Add(sr);
            _context.SaveChanges();


            if (scheduleModel.Isrepeat != false && scheduleModel.checkWeekday != null)
            {

                List<int> day = scheduleModel.checkWeekday.Split(',').Select(int.Parse).ToList();

                foreach (int d in day)
                {
                    DayOfWeek desiredDayOfWeek = (DayOfWeek)d;
                    DateTime today = DateTime.Today;
                    DateTime nextOccurrence = new DateTime(scheduleModel.Startdate.Year, scheduleModel.Startdate.Month, scheduleModel.Startdate.Day);
                    int occurrencesFound = 0;
                    while (occurrencesFound < scheduleModel.Repeatupto)
                    {
                        if (nextOccurrence.DayOfWeek == desiredDayOfWeek && (nextOccurrence.Day != scheduleModel.Startdate.Day))
                        {

                            Shiftdetail sdd = new Shiftdetail();
                            sdd.Shiftid = shift.Shiftid;
                            sdd.Shiftdate = nextOccurrence;
                            sdd.Starttime = scheduleModel.Starttime;
                            sdd.Endtime = scheduleModel.Endtime;
                            sdd.Regionid = scheduleModel.Regionid;
                            sdd.Status = 1;
                            sdd.Isdeleted = new BitArray(1, false);
                            _context.Shiftdetails.Add(sdd);
                            _context.SaveChanges();

                            Shiftdetailregion srr = new Shiftdetailregion();
                            srr.Shiftdetailid = sdd.Shiftdetailid;
                            srr.Regionid = scheduleModel.Regionid;
                            srr.Isdeleted = new BitArray(1, false);
                            _context.Shiftdetailregions.Add(srr);
                            _context.SaveChanges();
                            occurrencesFound++;
                        }
                        nextOccurrence = nextOccurrence.AddDays(1);
                    }
                }
            }

            return true;
        }

        public ShiftDetailsmodal GetShift(int shiftdetailsid)
        {
            var shiftdetails = _context.Shiftdetails.FirstOrDefault(s => s.Shiftdetailid == shiftdetailsid);
            var shift = _context.Shifts.FirstOrDefault(i => i.Shiftid == shiftdetails.Shiftid);

            ShiftDetailsmodal shiftModal = new ShiftDetailsmodal
            {
                Shiftdetailid = shiftdetailsid,
                Shiftdate = shiftdetails.Shiftdate,
                Shiftid = shiftdetails.Shiftid,
                Starttime = shiftdetails.Starttime,
                Endtime = shiftdetails.Endtime,
                Regionid = shiftdetails.Regionid,
                Abberaviation = _context.Regions.FirstOrDefault(i => i.Regionid == shiftdetails.Regionid).Abbreviation,
                Status = shiftdetails.Status,
                regions = _context.Regions.ToList(),
                Physicianid = shift.Physicianid,
                PhysicianName = _context.Physicians.FirstOrDefault(x => x.Physicianid == shift.Physicianid).Firstname + " " + _context.Physicians.FirstOrDefault(x => x.Physicianid == shift.Physicianid).Lastname,
            };

            return shiftModal;
        }

        public void SetReturnShift(int status, int shiftdetailid, int Aspid)
        {
            var shiftdetails = _context.Shiftdetails.FirstOrDefault(s => s.Shiftdetailid == shiftdetailid);
            if (status == 1)
            {
                shiftdetails.Status = 2;
                shiftdetails.Modifieddate = DateTime.Now;
                shiftdetails.Modifiedby = Aspid;
            }
            else
            {
                shiftdetails.Status = 1;
                shiftdetails.Modifieddate = DateTime.Now;
                shiftdetails.Modifiedby = Aspid;
            }
            _context.SaveChanges();
        }

        public void SetDeleteShift(int shiftdetailid, int Aspid)
        {
            var shiftdetails = _context.Shiftdetails.FirstOrDefault(s => s.Shiftdetailid == shiftdetailid);

            shiftdetails.Isdeleted = new BitArray(1, true);
            shiftdetails.Modifieddate = DateTime.Now;
            shiftdetails.Modifiedby = Aspid;

            _context.SaveChanges();
        }

        public bool SetEditShift(ShiftDetailsmodal shiftDetailsmodal, int Aspid)
        {
            if (_context.Shifts.Where(x => x.Physicianid == shiftDetailsmodal.Physicianid).Count() >= 1)
            {
                var shiftData = _context.Shifts.Where(i => i.Physicianid == shiftDetailsmodal.Physicianid).ToList();
                var shiftDetailData = new List<Shiftdetail>();

                foreach (var obj in shiftData)
                {
                    var details = _context.Shiftdetails.Where(x => x.Shiftid == obj.Shiftid && x.Shiftdetailid != shiftDetailsmodal.Shiftdetailid && x.Isdeleted != new BitArray(1, true)).ToList();
                    shiftDetailData.AddRange(details);
                }


                foreach (var obj in shiftDetailData)
                {
                    var shiftDate = new DateTime(shiftDetailsmodal.Shiftdate.Year, shiftDetailsmodal.Shiftdate.Month, shiftDetailsmodal.Shiftdate.Day);

                    if (obj.Shiftdate.Date == shiftDate.Date)
                    {
                        if ((obj.Starttime <= shiftDetailsmodal.Starttime && obj.Endtime >= shiftDetailsmodal.Starttime) || (obj.Starttime <= shiftDetailsmodal.Endtime && obj.Endtime >= shiftDetailsmodal.Endtime) || (obj.Starttime >= shiftDetailsmodal.Starttime && obj.Endtime <= shiftDetailsmodal.Endtime))
                        {
                            return false;
                        }
                    }
                }
            }

            var shiftdetails = _context.Shiftdetails.FirstOrDefault(s => s.Shiftdetailid == shiftDetailsmodal.Shiftdetailid);

            if (shiftdetails != null)
            {
                shiftdetails.Shiftdate = shiftDetailsmodal.Shiftdate;
                shiftdetails.Starttime = shiftDetailsmodal.Starttime;
                shiftdetails.Endtime = shiftDetailsmodal.Endtime;
                shiftdetails.Modifieddate = DateTime.Now;
                shiftdetails.Modifiedby = Aspid;

                _context.SaveChanges();
            }

            return true;
        }

        public List<ShiftReview> GetShiftReview(int regionId, int callId)
        {
            BitArray deletedBit = new BitArray(new[] { false });

            var shiftDetail = _context.Shiftdetails.Where(i => i.Isdeleted.Equals(deletedBit) && i.Status != 2);

            DateTime currentDate = DateTime.Now;

            if (regionId != 0)
            {
                shiftDetail = shiftDetail.Where(i => i.Regionid == regionId);
            }

            if (callId == 1)
            {
                shiftDetail = shiftDetail.Where(i => i.Shiftdate.Month == currentDate.Month);
            }

            var reviewList = shiftDetail.Where(s => !_context.Physicians.Any(p => p.Physicianid == s.Shift.Physicianid && p.Isdeleted != null)).Select(x => new ShiftReview
            {
                shiftDetailId = x.Shiftdetailid,
                PhysicianName = _context.Physicians.FirstOrDefault(y => y.Physicianid == _context.Shifts.FirstOrDefault(z => z.Shiftid == x.Shiftid).Physicianid).Firstname + ", " + _context.Physicians.FirstOrDefault(y => y.Physicianid == _context.Shifts.FirstOrDefault(z => z.Shiftid == x.Shiftid).Physicianid).Lastname,
                ShiftDate = x.Shiftdate.ToString("MMM dd, yyyy"),
                ShiftTime = x.Starttime.ToString("hh:mm tt") + " - " + x.Endtime.ToString("hh:mm tt"),
                ShiftRegion = _context.Regions.FirstOrDefault(y => y.Regionid == x.Regionid).Name,

            }).ToList();

            return reviewList;
        }

        public void ApproveSelectedShift(int[] shiftDetailsId, int Aspid)
        {
            foreach (var shiftId in shiftDetailsId)
            {
                var shift = _context.Shiftdetails.FirstOrDefault(i => i.Shiftdetailid == shiftId);

                shift.Status = 2;
                shift.Modifieddate = DateTime.Now;
                shift.Modifiedby = Aspid;

            }
            _context.SaveChanges();
        }

        public void DeleteShiftReview(int[] shiftDetailsId, int Aspid)
        {
            foreach (var shiftId in shiftDetailsId)
            {
                var shift = _context.Shiftdetails.FirstOrDefault(i => i.Shiftdetailid == shiftId);

                shift.Isdeleted = new BitArray(1, true);
                shift.Modifieddate = DateTime.Now;
                shift.Modifiedby = Aspid;

            }
            _context.SaveChanges();
        }

        public OnCallModal GetOnCallDetails(int regionId)
        {
            var currentTime = new TimeOnly(DateTime.Now.Hour, DateTime.Now.Minute);
            BitArray deletedBit = new BitArray(new[] { false });

            var onDutyQuery = _context.Shiftdetails
                .Include(sd => sd.Shift.Physician)
                .Where(sd => (regionId == 0 || sd.Shift.Physician.Physicianregions.Any(pr => pr.Regionid == regionId)) &&
                             sd.Shiftdate.Date == DateTime.Today &&
                             currentTime >= sd.Starttime &&
                             currentTime <= sd.Endtime &&
                             sd.Isdeleted.Equals(deletedBit))
                .Select(sd => sd.Shift.Physician)
                .Distinct()
                .ToList();


            var offDutyQuery = _context.Physicians
                .Include(p => p.Physicianregions)
                .Where(p => (regionId == 0 || p.Physicianregions.Any(pr => pr.Regionid == regionId)) &&
                            !_context.Shiftdetails.Any(sd => sd.Shift.Physicianid == p.Physicianid &&
                                                               sd.Shiftdate.Date == DateTime.Today &&
                                                               currentTime >= sd.Starttime &&
                                                               currentTime <= sd.Endtime &&
                                                               sd.Isdeleted.Equals(deletedBit)) && p.Isdeleted == null)
                .ToList();

            var onCallModal = new OnCallModal
            {
                OnCall = onDutyQuery,
                OffDuty = offDutyQuery,
                regions = GetRegions()
            };

            return onCallModal;
        }

        #endregion


        #region Provider Location

        public List<Physicianlocation> GetPhysicianlocations()
        {
            var phyLocation = _context.Physicianlocations.ToList();
            return phyLocation;
        }

        #endregion


        #region Partners

        public List<Partnersdata> GetPartnersdata(int professionid)
        {
            var vendor = _context.Healthprofessionals.Where(r => r.Isdeleted == null).ToList();
            if (professionid != 0)
            {
                vendor = vendor.Where(i => i.Profession == professionid).ToList();
            }
            var Partnersdata = vendor.Select(r => new Partnersdata()
            {
                VendorId = r.Vendorid,
                VendorName = r.Vendorname,
                ProfessionName = _context.Healthprofessionaltypes.FirstOrDefault(i => i.Healthprofessionalid == r.Profession).Professionname,
                VendorEmail = r.Email,
                FaxNo = r.Faxnumber,
                PhoneNo = r.Phonenumber,
                Businesscontact = r.Businesscontact,
            }).ToList();
            return Partnersdata;
        }

        public bool CreateNewBusiness(PartnersCm partnersCM, int LoggerAspnetuserId)
        {
            if (!(_context.Healthprofessionals.Any(x => x.Email == partnersCM.Email)))
            {
                var healthprof = new Healthprofessional()
                {
                    Vendorname = partnersCM.BusinessName,
                    Profession = partnersCM.SelectedhealthprofID,
                    Faxnumber = partnersCM.FAXNumber,
                    Phonenumber = partnersCM.Phonenumber,
                    Email = partnersCM.Email,
                    Businesscontact = partnersCM.BusinessContact,
                    Address = partnersCM.Street,
                    City = partnersCM.City,
                    Regionid = partnersCM.RegionId,
                    Zip = partnersCM.Zip,
                    State = _context.Regions.FirstOrDefault(i => i.Regionid == partnersCM.RegionId).Name,
                };
                _context.Healthprofessionals.Add(healthprof);
                _context.SaveChanges();
                return true;
            }
            return false;
        }

        public PartnersCm GetEditBusinessData(int vendorID)
        {
            var vendorDetails = _context.Healthprofessionals.FirstOrDefault(i => i.Vendorid == vendorID);

            var partnersCM = new PartnersCm()
            {
                BusinessName = vendorDetails.Vendorname,
                SelectedhealthprofID = vendorDetails.Profession,
                FAXNumber = vendorDetails.Faxnumber,
                Phonenumber = vendorDetails.Phonenumber,
                Email = vendorDetails.Email,
                BusinessContact = vendorDetails.Businesscontact,
                Street = vendorDetails.Address,
                City = vendorDetails.City,
                RegionId = vendorDetails.Regionid,
                Zip = vendorDetails.Zip,
            };
            return partnersCM;
        }

        public bool UpdateBusiness(PartnersCm partnersCM)
        {
            var vendorDetails = _context.Healthprofessionals.FirstOrDefault(i => i.Vendorid == partnersCM.vendorID);

            if (vendorDetails != null)
            {
                vendorDetails.Vendorname = partnersCM.BusinessName;
                vendorDetails.Profession = partnersCM.SelectedhealthprofID;
                vendorDetails.Faxnumber = partnersCM.FAXNumber;
                vendorDetails.Email = partnersCM.Email;
                vendorDetails.Phonenumber = partnersCM.Phonenumber;
                vendorDetails.Businesscontact = partnersCM.BusinessContact;
                vendorDetails.Address = partnersCM.Street;
                vendorDetails.City = partnersCM.City;
                vendorDetails.Regionid = partnersCM.RegionId;
                vendorDetails.Zip = partnersCM.Zip;
                vendorDetails.State = _context.Regions.FirstOrDefault(i => i.Regionid == partnersCM.RegionId).Name;
                vendorDetails.Modifieddate = DateTime.Now.Date;
                _context.SaveChanges();
                return true;
            }
            return false;
        }

        public void DelPartner(int VendorID)
        {
            var vendor = _context.Healthprofessionals.FirstOrDefault(x => x.Vendorid == VendorID);
            if (vendor != null)
            {
                vendor.Isdeleted = new BitArray(1, true);
                _context.SaveChanges();
            }
        }

        #endregion


        #region Patient Records

        public GetRecordsModel GetRecordsTab(int UserId, GetRecordsModel model)
        {
            var records = _context.Users.ToList();

            var usersList = records.Select(x => new PatientRecordsLists
            {
                Firstname = x.Firstname,
                Lastname = x.Lastname,
                Email = x.Email,
                Mobile = x.Mobile,
                Userid = x.Userid,
                address = x.Street + " " + x.City + " " + x.State + " " + x.Zipcode,

            }).ToList();

            if (model != null)
            {
                if (model.searchRecordOne != null)
                {
                    usersList = usersList.Where(r => r.Firstname != null && r.Firstname.Trim().ToLower().Contains(model.searchRecordOne.Trim().ToLower())).ToList();
                }

                if (model.searchRecordTwo != null)
                {
                    usersList = usersList.Where(r => r.Lastname != null && r.Lastname.Trim().ToLower().Contains(model.searchRecordTwo.Trim().ToLower())).ToList();
                }

                if (model.searchRecordThree != null)
                {
                    usersList = usersList.Where(r => r.Email != null && r.Email.Trim().ToLower().Contains(model.searchRecordThree.Trim().ToLower())).ToList();
                }

                if (model.searchRecordFour != null)
                {
                    usersList = usersList.Where(r => r.Mobile != null && r.Mobile.Trim().ToLower().Contains(model.searchRecordFour.Trim().ToLower())).ToList();
                }
            }

            model.users = usersList.ToList();

            if (UserId != null && UserId != 0)
            {
                var recordList = _context.Requests.Where(i => i.Userid == UserId);

                var explorelist = recordList.Select(x => new PatientExploreList()
                {
                    RequestId = x.Requestid,
                    name = _context.Requestclients.FirstOrDefault(r => r.Requestid == x.Requestid).Firstname + ' ' + _context.Requestclients.FirstOrDefault(r => r.Requestid == x.Requestid).Lastname,
                    Confirmationnumber = x.Confirmationnumber == null ? "-" : x.Confirmationnumber,
                    ProviderName = x.Physicianid == null ? "-" : _context.Physicians.FirstOrDefault(i => i.Physicianid == x.Physicianid).Firstname + ' ' + _context.Physicians.FirstOrDefault(i => i.Physicianid == x.Physicianid).Lastname,
                    Createddate = x.Createddate.ToString("MMM dd, yyyy"),
                    Status = x.Status,
                    isFinalize = x.Encounters.Select(x => x.IsFinalized).FirstOrDefault() == new BitArray(1, true),

                }).ToList();

                model.requestList = explorelist;

                model.physicians = _context.Physicians.ToList();
            }

            return model;
        }

        public SearchRecordsModel GetSearchRecords(SearchRecordsModel model)
        {
            var requestQuery = _context.Requests.Where(i => i.Isdeleted != new BitArray(1, true)).ToList();
            var recordList = new List<requests>();

            foreach (var request in requestQuery)
            {
                var requestClient = _context.Requestclients.Where(i => i.Requestid == request.Requestid).FirstOrDefault();
                var physician = _context.Physicians.Where(i => i.Physicianid == request.Physicianid).FirstOrDefault();
                var requestNotes = _context.Requestnotes.Where(i => i.Requestid == request.Requestid).FirstOrDefault();

                var newRequest = new requests
                {
                    patientname = (requestClient?.Firstname ?? "") + " " + (requestClient?.Lastname ?? ""),
                    requestor = request.Firstname + " " + request.Lastname,
                    dateOfService = request.Accepteddate?.ToString("MMM dd, yyyy"),
                    closeCaseDate = request.Lastreservationdate?.ToString("MMM dd, yyyy"),
                    email = requestClient?.Email,
                    contact = requestClient?.Phonenumber,
                    address = requestClient?.Address,
                    zip = requestClient?.Zipcode,
                    status = (int)request.Status,
                    physician = (physician?.Firstname ?? "") + " " + (physician?.Lastname ?? ""),
                    physicianNote = requestNotes?.Physiciannotes,
                    providerNote = null,
                    AdminNote = requestNotes?.Adminnotes,
                    pateintNote = requestClient?.Notes,
                    requestid = request.Requestid,
                    userid = request.Userid,
                    requestTypeId = request.Requesttypeid,

                };

                recordList.Add(newRequest);
            }

            if (model != null)
            {
                if (model.searchRecordOne != null && model.searchRecordOne != 0)
                {
                    recordList = recordList.Where(r => r.status == model.searchRecordOne).ToList();
                }

                if (model.searchRecordTwo != null)
                {
                    recordList = recordList.Where(r => r.patientname.Trim().ToLower().Contains(model.searchRecordTwo.Trim().ToLower())).ToList();
                }

                if (model.searchRecordThree != null && model.searchRecordThree != 0)
                {
                    recordList = recordList.Where(r => r.requestTypeId == model.searchRecordThree).ToList();
                }

                if (model.searchRecordFour != null)
                {

                    recordList = recordList.Where(r => r.dateOfService == model.searchRecordFour?.ToString("MMM dd, yyyy")).ToList();
                }

                if (model.searchRecordFive != null)
                {

                    recordList = recordList.Where(r => r.closeCaseDate == model.searchRecordFive?.ToString("MMM dd, yyyy")).ToList();
                }

                if (model.searchRecordSix != null)
                {
                    recordList = recordList.Where(r => r.physician != null && r.physician.Trim().ToLower().Contains(model.searchRecordSix.Trim().ToLower())).ToList();
                }

                if (model.searchRecordSeven != null)
                {

                    recordList = recordList.Where(r => r.email.Trim().ToLower().Contains(model.searchRecordSeven.Trim().ToLower())).ToList();
                }

                if (model.searchRecordEight != null)
                {
                    recordList = recordList.Where(r => r.contact != null && r.contact.Trim().ToLower().Contains(model.searchRecordEight.Trim().ToLower())).ToList();
                }
            }

            model.requestList = recordList;

            return model;
        }

        public void DeletRequest(int requestId)
        {
            var request = _context.Requests.Where(i => i.Requestid == requestId).FirstOrDefault();

            if (request != null)
            {
                request.Isdeleted = new BitArray(1, true);
                request.Modifieddate = DateTime.Now;

                _context.SaveChanges();
            }
        }

        public List<emailSmsRecords> GetEmailSmsLog(EmailSmsLogModel model)
        {
            var records = _context.Emaillogs.ToList();

            var recordList = records.Select(e => new emailSmsRecords()
            {
                email = e.Emailid,
                createddate = e.Createdate.ToString("MMM dd, yyyy hh:mm tt"),
                sentdate = e.Sentdate?.ToString("MMM dd, yyyy"),
                sent = e.Isemailsent[0] ? "Yes" : "No",
                senttries = e.Senttries,
                confirmationNumber = null,
                rolename = _context.Aspnetroles.FirstOrDefault(x => x.Id == e.Roleid)?.Name,
                recipient = e.Emailid.Substring(0, e.Emailid.IndexOf("@")),
                roleid = e.Roleid,
                action = null,

            }).ToList();

            if (model != null)
            {
                if (model.searchRecordOne != null && model.searchRecordOne != 0)
                {
                    recordList = recordList.Where(r => r.roleid == model.searchRecordOne).ToList();
                }

                if (model.searchRecordTwo != null)
                {
                    recordList = recordList.Where(r => r.recipient != null && r.recipient.Trim().ToLower().Contains(model.searchRecordTwo.Trim().ToLower())).ToList();
                }

                if (model.searchRecordThree != null)
                {
                    recordList = recordList.Where(r => r.email.Trim().ToLower().Contains(model.searchRecordThree.Trim().ToLower())).ToList();
                }

                if (model.searchRecordFour != null)
                {
                    recordList = recordList.Where(r => r.createddate.Substring(0, 12) == model.searchRecordFour?.ToString("MMM dd, yyyy")).ToList();
                }

                if (model.searchRecordFive != null)
                {

                    recordList = recordList.Where(r => r.sentdate == model.searchRecordFive?.ToString("MMM dd, yyyy")).ToList();
                }
            }

            return recordList;
        }

        public List<emailSmsRecords> GetSmsLog(EmailSmsLogModel model)
        {
            var records = _context.Smslogs.ToList();

            var recordList = records.Select(e => new emailSmsRecords()
            {
                contact = e.Mobilenumber,
                createddate = e.Createdate.ToString("MMM dd, yyyy hh:mm tt"),
                sentdate = e.Sentdate?.ToString("MMM dd, yyyy"),
                senttries = e.Senttries,
                confirmationNumber = e.Confirmationnumber,
                rolename = _context.Aspnetroles.FirstOrDefault(x => x.Id == e.Roleid)?.Name,
                recipient = _context.Requestclients.FirstOrDefault(x => x.Requestid == e.Requestid)?.Firstname,
                roleid = e.Roleid,
            }).ToList();

            if (model != null)
            {
                if (model.searchRecordOne != null && model.searchRecordOne != 0)
                {
                    recordList = recordList.Where(r => r.roleid == model.searchRecordOne).ToList();
                }

                if (model.searchRecordTwo != null)
                {
                    recordList = recordList.Where(r => r.recipient.Trim().ToLower().Contains(model.searchRecordTwo.Trim().ToLower())).ToList();
                }

                if (model.searchRecordThree != null)
                {
                    recordList = recordList.Where(r => r.contact.Trim().ToLower().Contains(model.searchRecordThree.Trim().ToLower())).ToList();
                }

                if (model.searchRecordFour != null)
                {
                    recordList = recordList.Where(r => r.createddate == model.searchRecordFour.ToString()).ToList();
                }

                if (model.searchRecordFive != null)
                {

                    recordList = recordList.Where(r => r.sentdate == model.searchRecordFive.ToString()).ToList();
                }
            }

            return recordList;
        }

        public BlockedRequestModel GetBlockedRequest(BlockedRequestModel model)
        {
            var blockrequest = _context.Blockrequests.Where(i => i.Isactive == new BitArray(1, true)).ToList();
            var recordList = new List<blockedRequest>();

            foreach (var obj in blockrequest)
            {
                var request = _context.Requestclients.Where(i => i.Requestid == obj.Requestid);
                bool check;

                if (obj.Isactive != null && obj.Isactive.Length > 0)
                {
                    check = obj.Isactive[0];
                }
                else
                {
                    check = false;
                }

                var newRecord = new blockedRequest
                {
                    patientname = request.Select(i => i.Firstname).FirstOrDefault() + " " + request.Select(i => i.Lastname).FirstOrDefault(),
                    contact = obj.Phonenumber,
                    email = obj.Email,
                    requestid = obj.Requestid,
                    createddate = obj.Createddate?.ToString("MMM dd, yyyy"),
                    notes = obj.Reason,
                    isactive = check,
                };
                recordList.Add(newRecord);
            }

            if (model != null)
            {
                if (model.searchRecordOne != null)
                {
                    recordList = recordList.Where(r => r.patientname != null && r.patientname.Trim().ToLower().Contains(model.searchRecordOne.Trim().ToLower())).ToList();
                }

                if (model.searchRecordTwo != null)
                {
                    recordList = recordList.Where(r => r.createddate == model.searchRecordTwo?.ToString("MMM dd, yyyy")).ToList();
                }

                if (model.searchRecordThree != null)
                {
                    recordList = recordList.Where(r => r.email != null && r.patientname.Trim().ToLower().Contains(model.searchRecordThree.Trim().ToLower())).ToList();
                }

                if (model.searchRecordFour != null)
                {
                    recordList = recordList.Where(r => r.contact != null && r.contact.Trim().ToLower().Contains(model.searchRecordFour.Trim().ToLower())).ToList();
                }
            }

            model.blockrequestList = recordList;
            return model;
        }

        public void UnblockRequest(int requestId)
        {
            var blockrequest = _context.Blockrequests.Where(i => i.Requestid == requestId).FirstOrDefault();
            var request = _context.Requests.Where(i => i.Requestid == requestId).FirstOrDefault();

            if (blockrequest != null && request != null)
            {
                request.Status = 1;
                blockrequest.Isactive = new BitArray(1, false);
            }

            _context.SaveChanges();

        }

        #endregion


        #region Pay Rate

        public List<PayRateForProviderCm> GetPayRateForProvider(int phyid, int adminAspId)
        {
            List<PayRateForProviderCm> dataList = new List<PayRateForProviderCm>();

            var payrate = _context.Payratebyproviders.Where(i => i.Physicianid == phyid).OrderBy(i => i.Payratecategoryid).ToList();

            if (payrate.Count() == 0)
            {
                for (int i = 1; i <= 7; i++)
                {
                    var data = new Payratebyprovider()
                    {
                        Payratecategoryid = i,
                        Physicianid = phyid,
                        Payrate = 0,
                        Createdby = adminAspId,
                    };
                    _context.Payratebyproviders.Add(data);

                    dataList.Add(new PayRateForProviderCm
                    {
                        Categoryid = data.Payratecategoryid,
                        PayrateValue = (int?)data.Payrate,
                        Categoryname = _context.Payratecategories.FirstOrDefault(x => x.Payratecategoryid == data.Payratecategoryid)!.Categoryname
                    });
                }
                _context.SaveChanges();
                return dataList;
            }
            else
            {
                var data = payrate.Select(i => new PayRateForProviderCm
                {
                    Categoryid = i.Payratecategoryid,
                    PayrateValue = (int?)i.Payrate,
                    Categoryname = _context.Payratecategories.FirstOrDefault(x => x.Payratecategoryid == i.Payratecategoryid)!.Categoryname,
                }).ToList();

                return data;
            }

        }

        public bool PostPayrate(int category, int payrate, int phyid, int adminAspId)
        {
            try
            {
                var categoryData = _context.Payratebyproviders.FirstOrDefault(i => i.Physicianid == phyid && i.Payratecategoryid == category);

                if (categoryData != null)
                {
                    categoryData.Payrate = payrate;
                }
                else
                {
                    var catData = new Payratebyprovider
                    {
                        Payratecategoryid = category,
                        Physicianid = phyid,
                        Payrate = payrate,
                        Createdby = adminAspId,
                    };

                    _context.Payratebyproviders.Add(catData);
                }

                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion


        #region Invoicing

        public List<Timesheet> GetTimeSheetDetail(int phyid, string dateSelected)
        {
            var timesheetData = _context.Timesheets.ToList();

            if (phyid != null)
            {
                timesheetData = timesheetData.Where(i => i.Physicianid == phyid).ToList();
            }

            if (dateSelected != null)
            {
                var splitedDate = dateSelected.Split('-');
                var currentYear = DateTime.Now.Year;
                var daysInMonth = DateTime.DaysInMonth(currentYear, Convert.ToInt32(splitedDate[0]));
                if (splitedDate[0].Length == 1)
                {
                    splitedDate[0] = "0" + splitedDate[0];
                }
                if (splitedDate[1] == "1")
                {
                    var startDate = "01" + "-" + splitedDate[0] + "-" + currentYear.ToString();
                    var endDate = 15 + "-" + splitedDate[0] + "-" + currentYear.ToString();
                    timesheetData = timesheetData.Where(i => i.Startdate.ToString() == startDate && i.Enddate.ToString() == endDate).ToList();
                }
                else if (splitedDate[1] == "2")
                {
                    var startDate = 16 + "-" + splitedDate[0] + "-" + currentYear.ToString();
                    var endDate = daysInMonth.ToString() + "-" + splitedDate[0] + "-" + currentYear.ToString();
                    timesheetData = timesheetData.Where(i => i.Startdate.ToString() == startDate && i.Enddate.ToString() == endDate).ToList();
                }
            }

            return timesheetData;
        }

        public List<Payratebyprovider> GetPayRateForProviderByPhyId(int phyid)
        {
            return _context.Payratebyproviders.Where(i => i.Physicianid == phyid).ToList();
        }

        public bool ApproveTimeSheet(int timeSheetId, int bonus, string notes)
        {
            try
            {
                var timeSheetData = _context.Timesheets.FirstOrDefault(x => x.Timesheetid == timeSheetId);

                timeSheetData.Isapproved = true;
                timeSheetData.Modifieddate = DateTime.Now;

                if (bonus != null)
                {
                    timeSheetData.Bonusamount = bonus.ToString();
                }
                if (notes != null)
                {
                    timeSheetData.Adminnotes = notes;
                }

                _context.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion


        public ChatCm GetChatData(int Reqid, int Senderid, int ReciverAspid)
        {
            var aspData = _context.Aspnetusers.FirstOrDefault(x => x.Id == ReciverAspid);

            var chathistory = _context.ChatHistories.Where(i => ((i.SenderAspId == Senderid.ToString() && i.ReceiverAspId == ReciverAspid.ToString()) || (i.SenderAspId == ReciverAspid.ToString() && i.ReceiverAspId == Senderid.ToString())) && i.RequestId == Reqid).OrderBy(i => i.Id).ToList();

            if (aspData != null)
            {
                var chatData = new ChatCm()
                {
                    AspId = ReciverAspid,
                    ReceiverName = aspData.Username,
                    chatHistories = chathistory,
                };

                return chatData;
            }

            return null;
        }

        public void AddChatHistory(int Reqid, int senderId, int Receiverid, string Message)
        {
            if (!Message.IsNullOrEmpty())
            {
                var chat = new ChatHistory()
                {
                    RequestId = Reqid,
                    SenderAspId = senderId.ToString(),
                    ReceiverAspId = Receiverid.ToString(),
                    Message = Message,
                    Time = TimeOnly.FromDateTime(DateTime.Now),
                };
                _context.ChatHistories.Add(chat);
                _context.SaveChanges();
            }
            
        }

    }
}
