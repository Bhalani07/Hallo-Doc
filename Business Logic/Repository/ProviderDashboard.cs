using Business_Logic.Interface;
using Data_Access.Coustom_Models;
using Data_Access.Custom_Models;
using Data_Access.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Business_Logic.Repository
{
    public class ProviderDashboard : IProviderDashboard
    {
        private readonly ApplicationDbContext _context;

        public ProviderDashboard(ApplicationDbContext context)
        {
            _context = context;
        }

        #region Dashboard Request

        public CountRequest GetCountRequest(int physicianId)
        {
            var request = _context.Requests;

            var countData = new CountRequest()
            {
                NewRequest = request.Where(i => i.Status == 1 && i.Physicianid == physicianId && i.Isdeleted == null && i.Userid != null).Count(),
                PendingRequest = request.Where(i => i.Status == 2 && i.Physicianid == physicianId && i.Isdeleted == null && i.Userid != null).Count(),
                ActiveRequest = request.Where(i => (i.Status == 4 || i.Status == 5) && i.Physicianid == physicianId && i.Isdeleted == null && i.Userid != null).Count(),
                ConcludeRequest = request.Where(i => i.Status == 6 && i.Physicianid == physicianId && i.Isdeleted == null && i.Userid != null).Count()
            };

            return countData;
        }

        public List<RequestListAdminDash> GetRequestData(int[] Status, string requesttypeid, int physicinId)
        {
            var requestList = _context.Requests.Where(i => Status.Contains(i.Status) && i.Physicianid == physicinId && i.Isdeleted == null && i.Userid != null);

            if (requesttypeid != null)
            {
                requestList = requestList.Where((i => i.Requesttypeid.ToString() == requesttypeid));
            }

            var tableData = requestList.Select(r => new RequestListAdminDash()
            {
                Name = r.Requestclients.Select(x => x.Firstname).FirstOrDefault() + " " + r.Requestclients.Select(x => x.Lastname).FirstOrDefault(),
                Email = r.Requestclients.Select(x => x.Email).FirstOrDefault(),
                Mobile = r.Phonenumber == null ? "-" : r.Phonenumber,
                Phone = r.Requestclients.Select(x => x.Phonenumber).FirstOrDefault(),
                Address = r.Requestclients.Select(x => x.Street).FirstOrDefault() + ", " + r.Requestclients.Select(x => x.City).FirstOrDefault() + ", " + r.Requestclients.Select(x => x.State).FirstOrDefault(),
                DateOfBirth = r.Requestclients.Select(x => x.Intdate).FirstOrDefault() == null ? null : r.Requestclients.Select(x => x.Intdate).FirstOrDefault() + "/" + r.Requestclients.Select(x => x.Strmonth).FirstOrDefault() + "/" + r.Requestclients.Select(x => x.Intyear).FirstOrDefault(),
                RequestDate = r.Createddate.ToShortDateString(),
                RequestTypeId = r.Requesttypeid,
                RequestId = r.Requestid,
                callType = r.Calltype == null ? 0 : (int)r.Calltype,
                isFinalized = r.Encounters.Select(x => x.IsFinalized).FirstOrDefault() == new BitArray(1, true),
                PhysicianId = r.Physician.Physicianid,
                PatientAspId = r.User.Aspnetuserid,
                UserId = r.Userid,
                AdminAspId = 16,

            }).ToList();

            return tableData;
        }

        #endregion


        #region Accept Case

        public bool SetAcceptCaseData(int requestId, int physicianId)
        {
            try
            {
                var request = _context.Requests.FirstOrDefault(i => i.Requestid == requestId);

                var physician = _context.Physicians.FirstOrDefault(x => x.Physicianid == physicianId);

                string acceptNote = "Case is Accepted By " + physician.Firstname + " " + physician.Lastname + " on " + DateTime.Now.ToString("MMM dd, yyyy") + " at " + DateTime.Now.ToString("hh:mm tt");

                var acceptCase = new Requeststatuslog()
                {
                    Requestid = requestId,
                    Status = 2,
                    Physicianid = physicianId,
                    Createddate = DateTime.Now,
                    Notes = acceptNote,
                };

                _context.Requeststatuslogs.Add(acceptCase);

                request.Status = 2;
                request.Modifieddate = DateTime.Now;
                request.Accepteddate = DateTime.Now;

                _context.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion


        #region Transfer Case 

        public bool TransferCaseData(TransferCaseModal transferCaseModal)
        {
            try
            {
                var request = _context.Requests.FirstOrDefault(i => i.Requestid == transferCaseModal.RequestId);

                var physician = _context.Physicians.FirstOrDefault(x => x.Physicianid == transferCaseModal.PhysicianId);

                string transferNote = physician.Firstname + " Transferred to Admin on " + DateTime.Now.ToString("MMM dd, yyyy") + " at " + DateTime.Now.ToString("hh:mm tt") + " : " + transferCaseModal.TransferDescription;

                var transferData = new Requeststatuslog()
                {
                    Requestid = transferCaseModal.RequestId,
                    Status = 1,
                    Createddate = DateTime.Now,
                    Transtoadmin = new BitArray(1, true),
                    Notes = transferNote,
                };

                request.Status = 1;
                request.Physicianid = null;

                _context.Requeststatuslogs.Add(transferData);
                _context.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion


        #region Encounter Case

        public bool SetEncounterCareType(EncounterCm encounterCm)
        {
            try
            {
                var request = _context.Requests.FirstOrDefault(i => i.Requestid == encounterCm.reqid);

                if (request != null)
                {
                    var physician = _context.Physicians.FirstOrDefault(x => x.Physicianid == encounterCm.physicianId);

                    var care = "";

                    if (encounterCm.Option == 1)
                    {
                        care = "HOUSE CALL";
                        request.Calltype = 1;
                        request.Status = 5;
                    }
                    else if (encounterCm.Option == 2)
                    {
                        care = "CONSULT";
                        request.Calltype = 2;
                        request.Status = 6;
                    }

                    request.Modifieddate = DateTime.Now;
                    _context.SaveChanges();

                    string encounterNote = physician.Firstname + " Selected " + care + " on " + DateTime.Now.ToString("MMM dd, yyyy") + " at " + DateTime.Now.ToString("hh:mm tt");

                    var transferData = new Requeststatuslog()
                    {
                        Requestid = encounterCm.reqid,
                        Status = request.Status,
                        Createddate = DateTime.Now,
                        Notes = encounterNote,
                        Physicianid = encounterCm.physicianId,
                    };

                    _context.Requeststatuslogs.Add(transferData);
                    _context.SaveChanges();
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool FinalizeEncounterCase(int requestId)
        {
            try
            {
                var encounter = _context.Encounters.FirstOrDefault(i => i.RequestId == requestId);

                if (encounter != null)
                {
                    encounter.IsFinalized = new BitArray(1, true);
                    _context.SaveChanges();
                }

                return true;
            }
            catch
            {
                return false;
            }

        }

        public bool HouseCallConclude(int requestId)
        {
            try
            {
                var requestData = _context.Requests.FirstOrDefault(i => i.Requestid == requestId);

                if (requestData != null)
                {
                    requestData.Status = 6;
                    requestData.Modifieddate = DateTime.Now;
                    _context.SaveChanges();

                    var physician = _context.Physicians.FirstOrDefault(i => i.Physicianid == requestData.Physicianid);

                    string houseCallNote = physician.Firstname + " House Called care on " + DateTime.Now.ToString("MMM dd, yyyy") + " at " + DateTime.Now.ToString("hh:mm tt");

                    var statusData = new Requeststatuslog()
                    {
                        Requestid = requestData.Requestid,
                        Status = 6,
                        Physicianid = requestData.Physicianid,
                        Createddate = DateTime.Now,
                        Notes = houseCallNote,
                    };

                    _context.Requeststatuslogs.Add(statusData);
                    _context.SaveChanges();
                }

                return true;
            }
            catch
            {
                return false;
            }

        }

        #endregion


        #region Conclude Case

        public AdminViewDocumentsCm GetViewDocumentsData(int requestid)
        {
            var patient = _context.Requestclients.FirstOrDefault(i => i.Requestid == requestid);

            string confirmation = _context.Requests.FirstOrDefault(i => i.Requestid == requestid).Confirmationnumber;
            var userId = _context.Requests.FirstOrDefault(i => i.Requestid == requestid).Userid;

            var casedata = _context.Requestclients.FirstOrDefault(i => i.Requestid == requestid);
            var requestList = _context.Requests.Where(i => i.Requestid == requestid);

            var documentData = new AdminViewDocumentsCm()
            {
                userId = userId == null ? 0 : (int)userId,
                requestId = requestid,
                patientName = patient.Firstname + " " + patient.Lastname,
                ConfirmationNumber = confirmation,
            };

            return documentData;
        }

        public List<ViewDocuments> GetViewDocumentsList(int requestid)
        {
            var document = _context.Requestwisefiles.Where(i => i.Requestid == requestid && i.Isdeleted != new BitArray(1, true));

            var viewDocuments = document.Select(r => new ViewDocuments()
            {
                requestWiseFileId = r.Requestwisefileid,
                requestId = requestid,
                documentName = r.Filename,
                uploadDate = r.Createddate,

            }).ToList();

            return viewDocuments;
        }

        public bool SetViewDocumentData(AdminViewDocumentsCm adminViewDocumentsCm)
        {
            try
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

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool ConfirmConcludeCare(AdminViewDocumentsCm adminViewDocumentsCm)
        {
            try
            {
                var request = _context.Requests.FirstOrDefault(i => i.Requestid == adminViewDocumentsCm.requestId);

                if (request != null)
                {
                    request.Status = 8;
                    request.Modifieddate = DateTime.Now;
                    request.Completedbyphysician = new BitArray(1, true);

                    _context.SaveChanges();

                    var physician = _context.Physicians.FirstOrDefault(i => i.Physicianid == request.Physicianid);

                    var noteData = new Requestnote()
                    {
                        Requestid = request.Requestid,
                        Physiciannotes = adminViewDocumentsCm.ProviderNote,
                        Createdby = (int)physician.Aspnetuserid,
                        Createddate = DateTime.Now,
                    };

                    _context.Requestnotes.Add(noteData);
                    _context.SaveChanges();

                    string concludeNote = physician.Firstname + " Concluded care on " + DateTime.Now.ToString("MMM dd, yyyy") + " at " + DateTime.Now.ToString("hh:mm tt");

                    var statusData = new Requeststatuslog()
                    {
                        Requestid = request.Requestid,
                        Status = 8,
                        Physicianid = request.Physicianid,
                        Createddate = DateTime.Now,
                        Notes = concludeNote,
                    };

                    _context.Requeststatuslogs.Add(statusData);
                    _context.SaveChanges();
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion


        #region Request To Admin

        public bool RequestForEdit(ProviderProfileCm providerProfileCm)
        {
            try
            {
                var physician = _context.Physicians.FirstOrDefault(i => i.Physicianid == providerProfileCm.PhysicianId);

                if (physician != null)
                {
                    var aspData = _context.Aspnetusers.FirstOrDefault(i => i.Id == physician.Createdby);

                    if (aspData != null)
                    {
                        var mail = "tatva.dotnet.meetbhalani@outlook.com";
                        var password = "eaywlzlxxvvbycfs";
                        var email = aspData.Email;
                        var subject = "Request For Edit";
                        var message = $"Hey <b>{aspData?.Username}</b>, <br><br> Message : " + providerProfileCm.RequestMessage + "<br> Request For : " + physician.Email;

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
                            Createdate = DateTime.Now,
                            Sentdate = DateTime.Now,
                            Isemailsent = new BitArray(1, true),
                            Senttries = 1,
                            Physicianid = physician.Physicianid
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


        #region Invoicing

        public List<Timesheetdetail> GetTimeSheetDetails(int phyid, string dateSelected)
        {
            var data = _context.Timesheetdetails.Include(i => i.Timesheet).Where(i => i.Timesheet.Physicianid == phyid).OrderBy(i => i.Timesheetdetailid).ToList();

            if (data.Count > 0 && dateSelected != null)
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
                    data = data.Where(i => i.Timesheet.Startdate.ToString() == startDate && i.Timesheet.Enddate.ToString() == endDate).ToList();
                }
                else if (splitedDate[1] == "2")
                {
                    var startDate = 16 + "-" + splitedDate[0] + "-" + currentYear.ToString();
                    var endDate = daysInMonth.ToString() + "-" + splitedDate[0] + "-" + currentYear.ToString();
                    data = data.Where(i => i.Timesheet.Startdate.ToString() == startDate && i.Timesheet.Enddate.ToString() == endDate).ToList();
                }
            }

            return data;
        }

        public List<Timesheetdetailreimbursement> GetTimeSheetDetailsReimbursements(int phyid, string dateSelected)
        {
            var data = _context.Timesheetdetailreimbursements.Include(i => i.Timesheetdetail.Timesheet).Where(i => i.Timesheetdetail.Timesheet.Physicianid == phyid).OrderBy(i => i.Timesheetdetailid).ToList();

            if (data.Count > 0 && dateSelected != null)
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
                    data = data.Where(i => i.Timesheetdetail.Timesheet.Startdate.ToString() == startDate && i.Timesheetdetail.Timesheet.Enddate.ToString() == endDate).ToList();
                }
                else if (splitedDate[1] == "2")
                {
                    var startDate = 16 + "-" + splitedDate[0] + "-" + currentYear.ToString();
                    var endDate = daysInMonth.ToString() + "-" + splitedDate[0] + "-" + currentYear.ToString();
                    data = data.Where(i => i.Timesheetdetail.Timesheet.Startdate.ToString() == startDate && i.Timesheetdetail.Timesheet.Enddate.ToString() == endDate).ToList();
                }
            }

            return data;
        }

        public List<ProviderTimesheetDetails> GetFinalizeTimeSheetDetails(int phyid, string dateSelected)
        {
            var currentYear = DateTime.Now.Year;
            var data = _context.Timesheetdetails
                                .Include(i => i.Timesheet)
                                .Where(i => i.Timesheet.Physicianid == phyid)
                                .OrderBy(i => i.Timesheetdetailid)
                                .ToList();

            if (!string.IsNullOrEmpty(dateSelected))
            {
                var splitedDate = dateSelected.Split('-');
                var month = Convert.ToInt32(splitedDate[0]);
                var daysInMonth = DateTime.DaysInMonth(currentYear, month);
                var startDate = splitedDate[1] == "1" ? new DateOnly(currentYear, month, 1) : new DateOnly(currentYear, month, 16);
                var endDate = splitedDate[1] == "1" ? new DateOnly(currentYear, month, 15) : new DateOnly(currentYear, month, daysInMonth);

                data = data.Where(i => i.Timesheet.Startdate == startDate && i.Timesheet.Enddate == endDate).ToList();

                if (data.Count == 0)
                {
                    var newShift = new Timesheet
                    {
                        Physicianid = phyid,
                        Startdate = startDate,
                        Enddate = endDate,
                        Isfinalize = false,
                        Isapproved = false,
                        Createdby = (int)_context.Physicians.FirstOrDefault(i => i.Physicianid == phyid).Aspnetuserid,
                    };

                    _context.Timesheets.Add(newShift);

                    _context.SaveChanges();

                    for (int i = splitedDate[1] == "1" ? 1 : 16; i <= (splitedDate[1] == "1" ? 15 : daysInMonth); i++)
                    {
                        var newShiftData = new Timesheetdetail
                        {
                            Timesheet = newShift,
                            Timesheetdate = new DateOnly(currentYear, month, i),
                        };

                        _context.Timesheetdetails.Add(newShiftData);
                    }

                    _context.SaveChanges();

                    data = _context.Timesheetdetails
                                   .Include(i => i.Timesheet)
                                   .Where(i => i.Timesheet.Physicianid == phyid &&
                                               i.Timesheet.Startdate == startDate &&
                                               i.Timesheet.Enddate == endDate)
                                   .OrderBy(i => i.Timesheetdetailid)
                                   .ToList();
                }
            }

            var timeSheetData = data.Select(i => new ProviderTimesheetDetails
            {
                TimeSheetId = i.Timesheetid,
                TimeSheetDetailId = i.Timesheetdetailid,
                ShiftDetailDate = i.Timesheetdate,
                Hours = (int?)i.Totalhours,
                NoOfConsults = i.Numberofphonecall,
                NoOfHouseCalls = i.Numberofhousecall,
                IsWeekend = i.Isweekend,
            }).ToList();

            return timeSheetData;
        }

        public bool PostFinalizeTimesheet(List<ProviderTimesheetDetails> providerTimesheetDetails)
        {
            try
            {
                if (providerTimesheetDetails != null && providerTimesheetDetails.Count > 0)
                {
                    foreach (var detail in providerTimesheetDetails)
                    {
                        var data = _context.Timesheetdetails.FirstOrDefault(x => x.Timesheetdetailid == detail.TimeSheetDetailId);

                        if (data != null)
                        {
                            data.Totalhours = detail.Hours;
                            data.Numberofhousecall = detail.NoOfHouseCalls;
                            data.Numberofphonecall = detail.NoOfConsults;
                            data.Isweekend = detail.IsWeekend;
                        }
                    }
                    _context.SaveChanges();
                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        public List<AddReceiptsDetails> GetAddReceiptsDetails(int[] timeSheetDetailId, int AspId)
        {
            var data = _context.Timesheetdetailreimbursements
                                .Where(x => timeSheetDetailId.Contains(x.Timesheetdetailid) && x.Isdeleted == null)
                                .OrderBy(x => x.Timesheetdetailreimbursementid)
                                .ToList();


            if (data.Count == 0)
            {
                for (int i = 0; i < timeSheetDetailId.Length; i++)
                {
                    var newReimbursementsData = new Timesheetdetailreimbursement
                    {
                        Timesheetdetailid = timeSheetDetailId[i],
                        Timesheetdate = _context.Timesheetdetails.FirstOrDefault(x => x.Timesheetdetailid == timeSheetDetailId[i])?.Timesheetdate,
                        Createddate = DateTime.Now,
                        Createdby = AspId,
                    };

                    _context.Timesheetdetailreimbursements.Add(newReimbursementsData);
                }

                _context.SaveChanges();

                data = _context.Timesheetdetailreimbursements
                                .Where(x => timeSheetDetailId.Contains(x.Timesheetdetailid))
                                .ToList();
            }


            var reimbursementsData = data.Select(i => new AddReceiptsDetails
            {
                TimeSheetDetailId = i.Timesheetdetailid,
                ShiftDetailDate = i.Timesheetdate,
                Item = i.Itemname,
                Amount = i.Amount,
                BillValue = i.Bill,
            }).ToList();

            return reimbursementsData;
        }

        public bool EditReceipt(int aspId, int timeSheetDetailId, string item, int amount, IFormFile file)
        {
            try
            {
                var reimbursementsData = _context.Timesheetdetailreimbursements.FirstOrDefault(x => x.Timesheetdetailid == timeSheetDetailId);
                var physician = _context.Physicians.FirstOrDefault(x => x.Aspnetuserid == aspId);

                reimbursementsData.Itemname = item;
                reimbursementsData.Amount = amount;
                reimbursementsData.Modifiedby = aspId;
                reimbursementsData.Modifieddate = DateTime.Now;

                string directory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "content");

                if (file != null)
                {
                    //var fileName = "Receipt_" + reimbursementsData.Timesheetdate;

                    string path = Path.Combine(directory, file.FileName);

                    if (File.Exists(path))
                    {
                        File.Delete(path);
                    }

                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }

                    reimbursementsData.Bill = file.FileName;
                }

                _context.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool DeleteReceipt(int aspId, int timeSheetDetailId)
        {
            try
            {
                var reimbursementsData = _context.Timesheetdetailreimbursements.FirstOrDefault(x => x.Timesheetdetailid == timeSheetDetailId);

                reimbursementsData.Isdeleted = true;
                reimbursementsData.Modifiedby = aspId;
                reimbursementsData.Modifieddate = DateTime.Now;

                _context.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool FinalizeTimeSheet(int timeSheetId)
        {
            try
            {
                var timeSheetData = _context.Timesheets.FirstOrDefault(x => x.Timesheetid == timeSheetId);

                timeSheetData.Isfinalize = true;
                timeSheetData.Modifieddate = DateTime.Now;

                _context.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }


        #endregion

    }
}
