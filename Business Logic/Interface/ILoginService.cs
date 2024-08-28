using Business_Logic.Repository;
using Data_Access.Custom_Models;
using Data_Access.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic.Interface
{
    public interface ILoginService
    {
        bool IsLoginValid(loginCm loginCm);

        Aspnetuser Login(loginCm loginCm);

        Aspnetuser CheckAspUser(string email);

        bool CheckBlockedUser(string email, string phone);

        string Encrypt(string clearText);

        string Decrypt(string cipherText);

    }
}
