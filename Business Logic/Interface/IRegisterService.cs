using Data_Access.Custom_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic.Interface
{
    public interface IRegisterService
    {
        public bool RegisterUser(RegisterCm registerCm);

        public void ResetPassword(RegisterCm registerCm);
    }
}
