using Data_Access.Custom_Models;
using Data_Access.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic.Interface
{
    public interface ISessionUtils
    {
        UserInfo GetUser(ISession session);

        void SetUser(ISession session, Aspnetuser aspnetuser, int userId);


    }
}
