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
using System.Data.Entity;
using RamancoLibrary.Utilities;

namespace SamDataAccess.Repos
{
    public class ConsolationRepo : Repo<SamDbContext, Consolation>, IConsolationRepo
    {
        public Tuple<Mosque, Obit[], Template[], Consolation[]> GetUpdates(int mosqueId, DateTime? clientLastUpdatetime, DateTime queryTime)
        {
            var confirmed = ConsolationStatus.confirmed.ToString();

            #region mosque updates:
            var mosque = (from m in context.Mosques
                          where m.ID == mosqueId &&
                                (clientLastUpdatetime == null
                                || (m.CreationTime <= queryTime && m.CreationTime > clientLastUpdatetime.Value)
                                || (m.LastUpdateTime != null && (m.LastUpdateTime.Value <= queryTime && m.LastUpdateTime.Value > clientLastUpdatetime.Value)))
                          select m).SingleOrDefault();
            #endregion

            #region obits updates:
            var obits = from o in context.Obits.Include(o => o.ObitHoldings)
                        where o.MosqueID == mosqueId &&
                              (clientLastUpdatetime == null
                              || (o.CreationTime <= queryTime && o.CreationTime > clientLastUpdatetime.Value)
                              || (o.LastUpdateTime != null && (o.LastUpdateTime.Value <= queryTime && o.LastUpdateTime.Value > clientLastUpdatetime.Value)))
                              && o.ObitHoldings.Where(h => h.BeginTime > DateTimeUtils.Now).Any()
                        select o;
            #endregion

            #region templates updates:
            var templates = from t in context.Templates.Include(t => t.TemplateFields)
                            where (clientLastUpdatetime == null
                                  || (t.CreationTime <= queryTime && t.CreationTime > clientLastUpdatetime.Value)
                                  || (t.LastUpdateTime != null && (t.LastUpdateTime.Value <= queryTime && t.LastUpdateTime.Value > clientLastUpdatetime.Value)))
                            select t;
            #endregion

            #region consolation updates:
            var consolations = from c in context.Consolations
                               join o in context.Obits
                               on c.ObitID equals o.ID
                               where o.MosqueID == mosqueId &&
                                     c.Status == confirmed &&
                                     (clientLastUpdatetime == null
                                     || (o.CreationTime <= queryTime && o.CreationTime > clientLastUpdatetime.Value)
                                     || (o.LastUpdateTime != null && (o.LastUpdateTime.Value <= queryTime && o.LastUpdateTime.Value > clientLastUpdatetime.Value)))
                               select c;
            #endregion

            return new Tuple<Mosque, Obit[], Template[], Consolation[]>(mosque, obits.ToArray(), templates.ToArray(), consolations.ToArray());
        }
    }
}
