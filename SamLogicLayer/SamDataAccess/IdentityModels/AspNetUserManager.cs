using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamDataAccess.IdentityModels
{
    public class AspNetUserManager : UserManager<AspNetUser>
    {
        public AspNetUserManager(IUserStore<AspNetUser> store)
            : base(store)
        {
        }
    }
}
