using Business_Logic.Interface;
using Data_Access.Custom_Models;
using Data_Access.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic.Repository
{
    public class PatientDashboard : IPatientDashboard
    {
        public readonly ApplicationDbContext _Context;

        public PatientDashboard(ApplicationDbContext context)
        {
            _Context = context;
        }

        public List<DashboardData> RequestList(int userId)
        {
            var request = _Context.Requests.Where(r => r.Userid == userId).AsNoTracking();

            var RequestList = request.Select(r => new DashboardData()
            {
                CreatedDate = r.Createddate,
                Status = r.Status,
                DocumentCount = r.Requestwisefiles.Select(f => f.Filename).Count(),
                RequestId = r.Requestwisefiles.Select(f => f.Requestid).FirstOrDefault(),
                PhysicianName = r.Physician.Firstname + " " + r.Physician.Lastname,
                AdminAspId = 16,
                PhyAspId = r.Physician.Aspnetuserid,
                PhysicianId = r.Physicianid,
            }).ToList();

            return RequestList;
        }

        public List<DocumentData> DocumentList(int reqId)
        {
            var requestWiseFile = _Context.Requestwisefiles.Where(rwf => rwf.Requestid == reqId && rwf.Isdeleted != null).AsNoTracking();

            var request = _Context.Requests.FirstOrDefault(x => x.Requestid == reqId);

            var DocumentList = requestWiseFile.Select(rwf => new DocumentData()
            {
                CreatedDate = rwf.Createddate,
                DocumentName = rwf.Filename,
                UploderName = request.Firstname + " " + request.Lastname,

            }).ToList();

            return DocumentList;    
        }

        public string GetConfimationNumber(int reqId)
        {
            Request? request = _Context.Requests.FirstOrDefault(x => x.Requestid == reqId);

            if(request != null)
            {
                return request.Confirmationnumber;
            }

            return "";
        }

        public bool DashboardUpload(PatientDashboardCm patientDashboardCm, int reqId)
        {
            IFormFile File1 = patientDashboardCm.Upload;

            var fileName = Path.GetFileNameWithoutExtension(File1.FileName) + "_" + DateTime.Now.Date.ToString("yyyy-MM-dd") + "_" + DateTime.Now.ToString("HH-mm-ss") + Path.GetExtension(File1.FileName);

            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "content", fileName);

            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                File1.CopyTo(fileStream);
            }

            var file = new Requestwisefile()
            {
                Requestid = reqId,
                Filename = fileName,
                Createddate = DateTime.Now,
            };

            _Context.Add(file);
            _Context.SaveChanges();

            return true;
        }

        public ProfileData GetProfileData(int userId)
        {
            var userData = _Context.Users.FirstOrDefault(u => u.Userid == userId);

            var BirthDay = Convert.ToInt32(userData.Intdate);
            var BirthMonth = Convert.ToInt32(userData.Strmonth);
            var BirthYear = Convert.ToInt32(userData.Intyear);

            var profileData = new ProfileData()
            {
                FirstName = userData.Firstname,
                LastName = userData.Lastname,
                PhoneWithoutCode = userData.Mobile,
                Email = userData.Email,
                Street = userData.Street,
                City = userData.City,
                RegionId = userData.Regionid,
                Zipcode = userData.Zipcode,
                BirthDate = new DateTime(BirthYear, BirthMonth, BirthDay),
            };

            return profileData;
        }

        public bool SetProfileData(ProfileData updatedProfileData, int userId)
        {
            var profileRecord = _Context.Users.FirstOrDefault(u => u.Userid == userId);

            if (profileRecord != null)
            {
                profileRecord.Firstname = updatedProfileData.FirstName;
                profileRecord.Lastname = updatedProfileData.LastName;
                profileRecord.Email = profileRecord.Email;
                profileRecord.Mobile = updatedProfileData.Phone;
                profileRecord.Street = updatedProfileData.Street;
                profileRecord.City = updatedProfileData.City;
                profileRecord.State = _Context.Regions.Where(x => x.Regionid == updatedProfileData.RegionId).Select(x => x.Name).FirstOrDefault();
                profileRecord.Regionid = updatedProfileData.RegionId;
                profileRecord.Zipcode = updatedProfileData.Zipcode;
                profileRecord.Intdate = updatedProfileData.BirthDate.Day;
                profileRecord.Strmonth = updatedProfileData.BirthDate.Month.ToString();
                profileRecord.Intyear = updatedProfileData.BirthDate.Year;
                profileRecord.Modifiedby = profileRecord.Aspnetuserid;
                profileRecord.Modifieddate = DateTime.Now;

                _Context.SaveChanges();

                var aspUserRecord = _Context.Aspnetusers.FirstOrDefault(x => x.Id == profileRecord.Aspnetuserid);


                if (aspUserRecord != null)
                {
                    aspUserRecord.Phonenumber = updatedProfileData.Phone;
                    aspUserRecord.Modifieddate = DateTime.Now;

                    _Context.SaveChanges();

                }
                return true;
            }

            return false;
        }
    }
}