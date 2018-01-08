using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SamDataAccess.Contexts;
using SamDataAccess.IdentityModels;
using SamDataAccess.Repos.Interfaces;
using SamUtils.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamDataAccess.Repos
{
    public class IdentityRepo : IIdentityRepo, IDisposable
    {
        #region CTORS:
        public IdentityRepo()
        {
            context = new SamDbContext();
        }
        #endregion

        #region Fields:
        SamDbContext context = null;
        #endregion

        #region PROPS:
        public SamDbContext Context
        {
            get
            {
                return context;
            }
        }
        #endregion

        #region Methods:
        public AspNetUserManager GetUserManager()
        {
            return new AspNetUserManager(new UserStore<AspNetUser>(context));
        }
        public RoleManager<AspNetRole> GetRoleManager()
        {
            return new AspNetRoleManager(new RoleStore<AspNetRole>(context));
        }
        public List<AspNetUser> GetUsersInRole(params string[] roleNames)
        {
            if (roleNames != null && roleNames.Any())
            {
                var roles = context.Roles.Where(r => roleNames.Contains(r.Name));
                if (roles.Any())
                {
                    return (from user in context.Users
                            where user.Roles.Any(r => roles.Select(rl => rl.Id).Contains(r.RoleId))
                            select user).ToList();
                }
            }
            return new List<AspNetUser>();
        }
        #endregion

        #region IDisposable:
        public void Dispose()
        {
            context.Dispose();
        }
        #endregion
    }
}
