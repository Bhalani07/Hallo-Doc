using Data_Access.Coustom_Models;
using Data_Access.Custom_Models;
using Data_Access.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic.Interface
{
    public interface IAdminDashboard
    {
        #region Common Data

        List<string> GetRoleMenuList(int accessRoleId);

        List<Region> GetRegions();

        List<Region> GetPhysicianRegions(int physicianId);

        List<Casetag> GetCasetags();

        List<Physician> GetPhysicians(int regionid);

        List<Role> GetRoles(int aspId);

        List<Aspnetrole> GetAccountType();

        List<Menu> GetMenu(int accounttype);

        List<Healthprofessionaltype> GetProfession();

        List<Requesttype> GetRequesttypes();

        #endregion


        #region Dashboard

        CountRequest GetCountRequest();

        List<RequestListAdminDash> GetRequestData(int[] Status, string requesttypeid, int regionid);


        #region View Case

        AdminViewCaseData GetViewCaseData(int requestid);

        void SetViewCaseData(AdminViewCaseData updatedViewCaseData, int requestid);

        #endregion


        #region View Notes

        ViewNotesData GetViewNotesData(int requestid);

        void SetViewNotesData(ViewNotesData updatedViewNotesData, int requestid, int aspId);

        #endregion


        #region Cancel Case

        CancelCaseModal GetCancelCaseData(int requestid);

        void SetCancelCaseData(CancelCaseModal updatedCancelCaseData);

        #endregion


        #region Assign Case

        AsignCaseModal GetAsignCaseData(int requestid);

        void SetAsignCaseData(ModalCm modalCm);

        #endregion


        #region Block Case

        BlockCaseModal GetBlockCaseModal(int requestid);

        void SetBlockCaseData(BlockCaseModal updatedBlockCaseData);

        #endregion


        #region View Documents

        AdminViewDocumentsCm GetViewDocumentsData(int requestid);

        List<ViewDocuments> GetViewDocumentsList(int requestid);

        void SetViewDocumentData(AdminViewDocumentsCm adminViewDocumentsCm);

        void DeleteFileData(int requestwisefileid);

        void SendEmailWithFile(int[] requestwisefileid, int requestid, int aspId);

        #endregion


        #region Send Orders

        List<Healthprofessionaltype> GetHealthprofessionaltypes();

        List<Healthprofessional> GetHealthprofessionals(int professionid);

        Healthprofessional GetBusinessData(int businessid);

        void SetSendOrderData(AdminSendOrderCm adminSendOrderCm, int aspId);

        #endregion


        #region Clear Case

        void SetClearCaseData(int requestId);

        #endregion


        #region Send Agreement 

        SendAgreementModal GetSendAgreementModal(int requestid, int reuqesttypeid);

        void SendAgreementEmail(SendAgreementModal sendAgreementModal, int aspId);

        #endregion


        #region Send Link 

        void SendSubmitRequestLink(SendLink sendLink, int aspId);

        #endregion


        #region Create Request

        void SendCreateRequestData(AdminCreateRequestCm adminCreateRequestCm, int aspId);

        void InsertRequestData(AdminCreateRequestCm adminCreateRequestCm, int reqTypeId, int aspId);

        void InsertRequestClientData(AdminCreateRequestCm adminCreateRequestCm, int requestId);

        void InsertNotesData(AdminCreateRequestCm adminCreateRequestCm, int requestId, int aspId);

        bool CheckRegion(int region);

        #endregion


        #region Close Case 

        AdminCloseCaseCm GetCloseCaseData(int requestId);

        List<Documents> GetCloseCaseDocuments(int requestId);

        void UpdateCloseCaseData(AdminCloseCaseCm adminCloseCaseCm);

        void SetClosedCase(int requestId, int aspId);

        #endregion


        #region Request DTY

        bool RequestSupportViaEmail(string message, int adminId);

        #endregion


        #region Encounter 

        EncounterCm GetEncounterData(int requestid, int status);

        void SetEncounterData(EncounterCm encounterCm);

        #endregion

        #endregion


        #region Profile 

        AdminProfileCm GetProfileData(int aspId);

        void AdminResetPassword(string password, int aspId);

        void AdminAccountUpdate(short status, int roleId, int aspId);

        void AdministratorDetail(AdminProfileCm adminProfileCm, List<int> regions);

        void MailingDetail(AdminProfileCm adminProfileCm);

        void RemoveAdmin(int adminId);

        #endregion


        #region Provider 

        List<AdminregionTable> GetAdminregions(int aspId);

        List<Provider> GetProviders(int regionId);

        List<PhysicianRegionTable> GetPhysicianRegionTables(int aspId);

        void ContactProvider(ProvidersCm providersCm, int aspId);

        ProviderProfileCm GetProviderProfileData(int aspId);

        void PhysicianResetPassword(string password, int aspId);

        void PhysicianAccountUpdate(short status, int roleId, int aspId);

        void PhysicianAdministratorDataUpdate(ProviderProfileCm providerProfileCm, List<int> physicianRegions);

        void PhysicianMailingDataUpdate(ProviderProfileCm providerProfileCm);

        void PhysicianBusinessInfoUpdate(ProviderProfileCm providerProfileCm);

        void AddProviderBusinessPhotos(IFormFile photo, IFormFile signature, int aspId);

        void EditOnBoardingData(ProviderProfileCm providerProfileCm);

        void RemovePhysician(int physicianId);

        bool CreatePhysicianAccount(ProviderProfileCm providerProfileCm, List<int> physicianRegions);

        void AddProviderDocuments(int Physicianid, IFormFile Photo, IFormFile ContractorAgreement, IFormFile BackgroundCheck, IFormFile HIPAA, IFormFile NonDisclosure);

        #endregion


        #region Account Access 

        List<AccountAccess> GetAccountAccessData();

        List<AccountMenu> GetAccountMenu(int accounttype, int roleid);

        void SetCreateAccessAccount(AccountAccess accountAccess, List<int> AccountMenu);

        void SetEditAccessAccount(AccountAccess accountAccess, List<int> AccountMenu);

        void DeleteAccountAccess(int roleid);

        AccountAccess GetEditAccessData(int roleid);

        #endregion


        #region User Access

        List<UserAccess> GetUserAccessData(int accountTypeId);

        bool CreateAdminAccount(AdminProfileCm adminProfileCm, List<int> adminRegions);

        #endregion


        #region Scheduling 

        public List<ShiftDetailsmodal> ShiftDetailsmodal(DateTime date, DateTime sunday, DateTime saturday, string type, int aspId);

        ShiftDetailsmodal GetShift(int shiftdetailsid);

        bool createShift(ScheduleModel scheduleModel, int Aspid);

        void SetReturnShift(int status, int shiftdetailid, int Aspid);

        public void SetDeleteShift(int shiftdetailid, int Aspid);

        public bool SetEditShift(ShiftDetailsmodal shiftDetailsmodal, int Aspid);

        public List<ShiftReview> GetShiftReview(int regionId, int callId);

        public void ApproveSelectedShift(int[] shiftDetailsId, int Aspid);

        public void DeleteShiftReview(int[] shiftDetailsId, int Aspid);

        public OnCallModal GetOnCallDetails(int regionId);

        #endregion


        #region Provider Location

        public List<Physicianlocation> GetPhysicianlocations();

        #endregion


        #region Partners

        List<Partnersdata> GetPartnersdata(int professionid);

        bool CreateNewBusiness(PartnersCm partnersCM, int LoggerAspnetuserId);

        PartnersCm GetEditBusinessData(int vendorID);

        bool UpdateBusiness(PartnersCm partnersCM);

        void DelPartner(int VendorID);

        #endregion


        #region Patient Records

        GetRecordsModel GetRecordsTab(int UserId, GetRecordsModel model);

        SearchRecordsModel GetSearchRecords(SearchRecordsModel model);

        void DeletRequest(int requestId);

        List<emailSmsRecords> GetEmailSmsLog(EmailSmsLogModel model);

        List<emailSmsRecords> GetSmsLog(EmailSmsLogModel model);

        BlockedRequestModel GetBlockedRequest(BlockedRequestModel model);

        void UnblockRequest(int requestId);

        #endregion


        #region Pay Rate

        List<PayRateForProviderCm> GetPayRateForProvider(int phyid, int adminAspId);

        bool PostPayrate(int category, int payrate, int phyid, int adminAspId);

        #endregion


        #region Invoicing

        List<Timesheet> GetTimeSheetDetail(int phyid, string dateSelected);

        List<Payratebyprovider> GetPayRateForProviderByPhyId(int phyid);

        bool ApproveTimeSheet(int timeSheetId, int bonus, string notes);
        
        #endregion


        ChatCm GetChatData(int Reqid,int Senderid,int ReciverAspid);

        void AddChatHistory(int Reqid,int senderId, int Receiverid, string Message);
    }
}
