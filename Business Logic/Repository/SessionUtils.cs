using Business_Logic.Interface;
using Data_Access.Custom_Models;
using Data_Access.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic.Repository
{
    public class SessionUtils : ISessionUtils
    {
        private readonly ApplicationDbContext _context;

        public SessionUtils(ApplicationDbContext context)
        {
            _context = context;
        }

        public UserInfo GetUser(ISession session)
        {
            UserInfo userInfo = null;

            if (!string.IsNullOrEmpty(session.GetInt32("userId").ToString()))
            {
                userInfo = new UserInfo();
                userInfo.AspNetUserId = (int)session.GetInt32("aspNetUserId");
                userInfo.UserId = (int)session.GetInt32("userId");
                userInfo.UserName = session.GetString("userName");
                userInfo.AccessRoleId = (int)session.GetInt32("accessRoleId");
            }
            return userInfo;
        }

        public void SetUser(ISession session, Aspnetuser aspnetuser, int userId)
        {
            if (aspnetuser != null)
            {
                session.SetInt32("aspNetUserId", aspnetuser.Id);
                session.SetString("userName", aspnetuser.Username);

                if (aspnetuser.Roleid == 3)
                {
                    User? user = _context.Users.FirstOrDefault(i => i.Email == aspnetuser.Email);

                    if(user != null)
                    {
                        session.SetInt32("userId", user.Userid);
                        session.SetInt32("accessRoleId", 0);
                    }
                }
                else if (aspnetuser.Roleid == 2)
                {
                    Physician? physician = _context.Physicians.FirstOrDefault(i => i.Email == aspnetuser.Email);

                    if(physician != null)
                    {
                        session.SetInt32("userId", physician.Physicianid);
                        session.SetInt32("accessRoleId", (int)physician.Roleid);
                    }
                    
                }
                else if (aspnetuser.Roleid == 1)
                {
                    Admin? admin = _context.Admins.FirstOrDefault(i => i.Email == aspnetuser.Email);

                    if(admin != null)
                    {
                        session.SetInt32("userId", admin.Adminid);
                        session.SetInt32("accessRoleId", (int)admin.Roleid);
                    }
                    
                }

            }
            if(userId != 0)
            {
                session.SetInt32("userId", userId);
            }
        }
    }
}
