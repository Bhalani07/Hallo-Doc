using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Access.Custom_Models
{
    public class UserInfo
    {
        public int AspNetUserId { get; set; }

        public int UserId { get; set; }

        public string UserName { get; set; }

        public int AccessRoleId { get; set; }
    }
}
