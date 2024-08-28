using Data_Access.Custom_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic.Interface
{
    public interface IPatientDashboard
    {
        List<DashboardData> RequestList(int userId);

        List<DocumentData> DocumentList(int reqId);

        string GetConfimationNumber(int reqId);

        bool DashboardUpload(PatientDashboardCm patientDashboardCm, int reqId);

        ProfileData GetProfileData(int userId);

        bool SetProfileData( ProfileData updatedProfileData, int userId);
    }
}
