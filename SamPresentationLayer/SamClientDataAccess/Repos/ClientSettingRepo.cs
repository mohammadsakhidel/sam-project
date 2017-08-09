using ClientModels.Models;
using SamClientDataAccess.Contexts;
using SamClientDataAccess.Repos.BaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamClient.Models.Repos
{
    public class ClientSettingRepo : Repo<SamClientDbContext, ClientSetting>
    {
    }
}
