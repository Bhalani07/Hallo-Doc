using Data_Access.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Access.Custom_Models
{
    public class AdminAccountAccessCm
    {
        public List<AccountAccess> AccountAccess { get; set; }

        public AccountAccess CreateAccountAccess { get; set; }

        public List<Aspnetrole> Aspnetroles { get; set; }

        public List<Menu> Menus { get; set; }

        public List<AccountMenu> AccountMenu { get; set; }
    }

    public class AccountAccess
    {
        public int roleid { get; set; }

        public int Adminid { get; set; }


        [Required(ErrorMessage = "Role Name Is Required")]
        public string name { get; set; }

        public string accounttype { get; set; }

        [Required(ErrorMessage = "Account Type Is Required")]
        public int accounttypeid { get; set; }

    }

    public class AccountMenu
    {
        public int menuid { get; set; }

        public int roleid { get; set; }

        public string name { get; set; }

        public int accounttype { get; set; }

        public bool ExistsInTable { get; set; }

    }
}
