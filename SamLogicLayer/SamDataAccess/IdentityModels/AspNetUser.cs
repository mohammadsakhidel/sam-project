using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamDataAccess.IdentityModels
{
    public class AspNetUser : IdentityUser
    {
        #region CTORS:
        public AspNetUser() : base()
        {
        }

        public AspNetUser(string userName) : base(userName)
        {
        }
        #endregion

        public bool IsApproved { get; set; }
    }
}
