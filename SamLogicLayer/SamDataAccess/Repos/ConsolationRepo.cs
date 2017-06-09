using SamDataAccess.Contexts;
using SamDataAccess.Repos.BaseClasses;
using SamDataAccess.Repos.Interfaces;
using SamModels.Entities.Core;
using SamUtils.Enums;
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
        public List<Consolation> GetUpdates(int mosqueId, DateTime lastUpdatetime, DateTime queryTime)
        {
            var confirmed = ConsolationStatus.confirmed.ToString();
            var list = from c in context.Consolations
                       join o in context.Obits
                       on c.ObitID equals o.ID
                       where o.MosqueID == mosqueId && 
                             //c.Status == confirmed &&         TEMP
                             c.CreationTime > lastUpdatetime && 
                             c.CreationTime <= queryTime
                       select c;
            return list.ToList();
        }
    }
}
