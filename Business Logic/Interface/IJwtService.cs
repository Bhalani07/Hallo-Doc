using Data_Access.Custom_Models;
using Data_Access.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic.Interface
{
    public interface IJwtService
    {
        string GenerateJwtToken(Aspnetuser aspnetuser);

        bool ValidateToken(string token, out JwtSecurityToken jwtSecurityToken);
    }
}
