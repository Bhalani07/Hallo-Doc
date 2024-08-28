using Business_Logic.Interface;
using Data_Access.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic.Repository
{
    public class AuthManager : Attribute
    {

    }

    [AttributeUsage(AttributeTargets.All)]
    public class CustomAuthorize : Attribute, IAuthorizationFilter
    {
        private readonly string _menu;
        private readonly string[] _roles;

        public CustomAuthorize(string menu, params string[] role)
        {
            _menu = menu;
            _roles = role;
        }

        public void OnAuthorization(AuthorizationFilterContext filterContext)
        {
            var jwtService = filterContext.HttpContext.RequestServices.GetService<IJwtService>();

            var sessionService = filterContext.HttpContext.RequestServices.GetService<ISessionUtils>();
            var user = sessionService.GetUser(filterContext.HttpContext.Session);

            var roleService = filterContext.HttpContext.RequestServices.GetService<IAdminDashboard>();

            //not login
            if (jwtService == null)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Home", action = "Login" }));
                return;
            }

            var request = filterContext.HttpContext.Request;
            var token = request.Cookies["jwt"];

            if (token == null || !jwtService.ValidateToken(token, out JwtSecurityToken jwtSecurityToken))
            {
                if (filterContext.HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    filterContext.Result = new JsonResult(new { code = 401 });
                    return;
                }
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Home", action = "Login" }));
                return;
            }

            var roleClaim = jwtSecurityToken.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Role).Value;

            // redirect to login if not logged in
            if (roleClaim == null)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Home", action = "Login" }));
                return;
            }

            // check menu access
            if (_menu != "")
            {
                int accessRoleId = user.AccessRoleId;

                List<string> roleMenu = roleService.GetRoleMenuList(accessRoleId);
                bool hasAccess = false;
                if (roleMenu.Any(r => r == _menu))
                {
                    hasAccess = true;
                }
                if (hasAccess == false)
                {
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Home", action = "AccessDenied" }));
                }
            }

            // redirect to access denied if role mis match
            if (string.IsNullOrWhiteSpace(roleClaim) || (_roles.Any() && !_roles.Contains(roleClaim)))
            {
                if (filterContext.HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    filterContext.Result = new JsonResult(new { code = 401 });
                    return;
                }

                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Home", action = "Access" }));
            }
        }
    }
}
