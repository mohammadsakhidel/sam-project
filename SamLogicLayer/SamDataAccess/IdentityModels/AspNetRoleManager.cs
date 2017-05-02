using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamDataAccess.IdentityModels
{
    public class AspNetRoleManager : RoleManager<AspNetRole>
    {
        public AspNetRoleManager(IRoleStore<AspNetRole, string> store) : base(store)
        {
        }
    }
}
