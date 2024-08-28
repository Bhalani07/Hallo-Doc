using Data_Access.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Access.Custom_Models
{
    public class AdminUserAccessCm
    {
        public List<Aspnetrole> Aspnetroles { get; set; }

        public List<UserAccess> UserAccesses { get; set; }

    }

    public class UserAccess
    {
        public int AspId { get; set; }

        public int AccountTypeId { get; set; }

        public string AccountType { get; set; }

        public string AccountHolderName { get; set; }

        public string AccountPhone { get; set; }

        public short AccountStatus { get; set; }

        public int AccountRequests { get; set; }
    }
}
