using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamDataAccess.IdentityModels
{
    public class AspNetRole : IdentityRole
    {
        #region CTROS:
        public AspNetRole() : base()
        {
        }

        public AspNetRole(string roleName) : base(roleName)
        {
        }
        #endregion
    }
}
