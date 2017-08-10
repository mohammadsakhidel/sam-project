using ClientModels.Models;
using SamClientDataAccess.Contexts;
using SamClientDataAccess.Repos.BaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamClientDataAccess.Repos
{
    public class ClientSettingRepo : Repo<SamClientDbContext, ClientSetting>
    {
        #region Ctors:
        public ClientSettingRepo() : base()
        {

        }
        public ClientSettingRepo(SamClientDbContext context) : base(context)
        {

        }
        #endregion

        public ClientSetting Get()
        {
            return set.SingleOrDefault(s => s.ID == 1);
        }
    }
}
