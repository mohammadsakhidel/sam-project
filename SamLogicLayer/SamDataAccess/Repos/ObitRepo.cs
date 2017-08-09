using RamancoLibrary.Utilities;
using SamDataAccess.Contexts;
using SamDataAccess.Repos.BaseClasses;
using SamDataAccess.Repos.Interfaces;
using SamModels.Entities.Core;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamDataAccess.Repos
{
    public class ObitRepo : Repo<SamDbContext, Obit>, IObitRepo
    {
        public List<Obit> Get(int mosqueId, DateTime date)
        {
            var obits = from o in set
                        where o.MosqueID == mosqueId &&
                              (from h in o.ObitHoldings
                               where DbFunctions.TruncateTime(h.BeginTime) == DbFunctions.TruncateTime(date) || DbFunctions.TruncateTime(h.EndTime) == DbFunctions.TruncateTime(date)
                               select h).Any()
                        select o;
            return obits.Include(o => o.ObitHoldings).ToList();
        }

        public List<Obit> GetAllFromDate(int mosqueId, DateTime date)
        {
            var obits = from o in set
                        where o.MosqueID == mosqueId &&
                              (from h in o.ObitHoldings
                               where DbFunctions.TruncateTime(h.BeginTime) >= DbFunctions.TruncateTime(date) || DbFunctions.TruncateTime(h.EndTime) >= DbFunctions.TruncateTime(date)
                               select h).Any()
                        select o;
            return obits.Include(o => o.ObitHoldings).ToList();
        }

        public List<ObitHolding> GetHoldings(int mosqueId, DateTime date)
        {
            var holdings = from h in context.ObitHoldings
                           join o in context.Obits
                           on h.ObitID equals o.ID
                           where o.MosqueID == mosqueId &&
                                 (DbFunctions.TruncateTime(h.BeginTime) == date || DbFunctions.TruncateTime(h.EndTime) == date)
                           orderby h.BeginTime
                           select h;
            return holdings.Include(h => h.Obit).ToList();
        }

        public void UpdateWithSave(Obit newObit)
        {
            var obit = Get(newObit.ID);
            if (obit != null)
            {
                obit.Title = newObit.Title;
                obit.ObitType = newObit.ObitType;
                obit.LastUpdateTime = DateTimeUtils.Now;

                //remove old holdings:
                if (obit.ObitHoldings != null)
                    context.ObitHoldings.RemoveRange(obit.ObitHoldings);

                foreach (var h in newObit.ObitHoldings)
                {
                    obit.ObitHoldings.Add(h);
                }

                Save();
            }
        }
    }
}
