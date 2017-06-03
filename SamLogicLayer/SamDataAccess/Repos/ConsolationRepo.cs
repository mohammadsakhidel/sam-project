using SamDataAccess.Contexts;
using SamDataAccess.Repos.BaseClasses;
using SamDataAccess.Repos.Interfaces;
using SamModels.Entities.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace SamDataAccess.Repos
{
    public class ConsolationRepo : Repo<SamDbContext, Consolation>, IConsolationRepo
    {
    }
}
