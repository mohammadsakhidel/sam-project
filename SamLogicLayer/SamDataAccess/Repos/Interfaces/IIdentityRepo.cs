using Microsoft.AspNet.Identity;
using SamDataAccess.IdentityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamDataAccess.Repos.Interfaces
{
    public interface IIdentityRepo
    {
        AspNetUserManager GetUserManager();
        RoleManager<AspNetRole> GetRoleManager();
    }
}
