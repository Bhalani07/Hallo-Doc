using Business_Logic.Interface;
using Data_Access.Custom_Models;
using Data_Access.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic.Interface
{
    public interface IPatientRequest
    {
        #region Regions

        List<Region> GetRegions();

        #endregion


        #region Email Logs

        void EmailLogEntry(string subject, string email, int roleId, string message);

        #endregion


        #region Request for Patient Form 

        bool CreatePatientRequest(PatientRequestCm patientRequestCm, int ReqTypeId);

        void AddUserData(PatientRequestCm patientRequestCm, int AspnetuserId);

        void AddRequestData(PatientRequestCm patientRequestCm, int ReqTypeId);

        void AddRequestClientData(PatientRequestCm patientRequestCm, int requestId);

        void UploadFile(PatientRequestCm patientRequestCm, int requestId);

        #endregion


        #region Request for Family Form

        bool CreateFamilyRequest(FamilyRequestCm familyRequestCm, int ReqTypeId);

        void AddFamilyRequestData(FamilyRequestCm familyRequestCm, int ReqTypeId);

        void AddFamilyRequestClientData(FamilyRequestCm familyRequestCm, int requestId);

        void FamilyUploadFile(FamilyRequestCm familyRequestCm, int requestId);

        #endregion


        #region Request for Concierge Form

        bool CreateConciergeRequest(ConciergeRequestCm conciergeRequestCm, int ReqTypeId);

        void AddConciergeRequestData(ConciergeRequestCm conciergeRequestCm, int ReqTypeId);

        void AddConciergeRequestClientData(ConciergeRequestCm conciergeRequestCm, int requestId);

        void AddConciergeData(ConciergeRequestCm conciergeRequestCm);

        void AddRequestConcierge(string email, int conciergeId);

        #endregion


        #region Request for Business Form

        bool CreateBusinessRequest(BusinessRequestCm businessRequestCm, int ReqTypeId);

        void AddBusinessRequestData(BusinessRequestCm businessRequestCm, int ReqTypeId);

        void AddBusinessRequestClientData(BusinessRequestCm businessRequestCm, int requestId);

        void AddBusinessData(BusinessRequestCm businessRequestCm);

        void AddRequestBusiness(string email, int businessId);

        #endregion


        #region Request for Me Form

        PatientRequestCm GetMeData(int userId);

        bool CreateForMeData(PatientRequestCm patientRequestCm, int ReqTypeId, int userId);

        #endregion


        #region Request for Others Form

        bool CreateRequestForOther(FamilyRequestCm familyRequestCm, int reqTypeId, int userId);

        void AddOtherRequestData(FamilyRequestCm familyRequestCm, int reqTypeId, int userId);

        #endregion


        #region Send Email

        Task EmailSender(string email, string subject, string message);

        #endregion


        #region Review Agreement

        ReviewAgreementCm GetReviewAgreement(int reqId);

        void AgreeReview(int reqId);

        void CancelReview(ReviewAgreementCm reviewAgreementCm);

        #endregion
    }
}
