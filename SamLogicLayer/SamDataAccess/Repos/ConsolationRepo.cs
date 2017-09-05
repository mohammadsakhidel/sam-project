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
using SamModels.Entities.Blobs;

namespace SamDataAccess.Repos
{
    public class ConsolationRepo : Repo<SamDbContext, Consolation>, IConsolationRepo
    {
        public Tuple<Mosque, Obit[], Template[], ImageBlob[], Consolation[]> GetUpdates(int mosqueId, string saloonId, DateTime? clientLastUpdatetime, DateTime queryTime)
        {
            var confirmed = ConsolationStatus.confirmed.ToString();
            var canceled = ConsolationStatus.canceled.ToString();
            var displayed = ConsolationStatus.displayed.ToString();

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
                              o.ObitHoldings.Any(h => h.SaloonID == saloonId) &&
                              (clientLastUpdatetime == null
                              || (o.CreationTime <= queryTime && o.CreationTime > clientLastUpdatetime.Value)
                              || (o.LastUpdateTime != null && (o.LastUpdateTime.Value <= queryTime && o.LastUpdateTime.Value > clientLastUpdatetime.Value)))
                              && o.ObitHoldings.Any(h => h.EndTime > queryTime)
                        select o;
            #endregion

            #region templates updates:
            var templates = from t in context.Templates.Include(t => t.TemplateFields)
                            where (clientLastUpdatetime == null
                                  || (t.CreationTime <= queryTime && t.CreationTime > clientLastUpdatetime.Value)
                                  || (t.LastUpdateTime != null && (t.LastUpdateTime.Value <= queryTime && t.LastUpdateTime.Value > clientLastUpdatetime.Value)))
                            select t;
            #endregion

            #region blobs updates:
            var blobs = from b in context.Blobs.OfType<ImageBlob>()
                        where (clientLastUpdatetime == null
                              || (b.CreationTime <= queryTime && b.CreationTime > clientLastUpdatetime.Value)
                              || (b.LastUpdateTime != null && (b.LastUpdateTime.Value <= queryTime && b.LastUpdateTime.Value > clientLastUpdatetime.Value)))
                        select b;
            #endregion

            #region consolation updates:
            var consolations = from c in context.Consolations
                               join o in context.Obits
                               on c.ObitID equals o.ID
                               join h in context.ObitHoldings
                               on o.ID equals h.ObitID
                               where o.MosqueID == mosqueId && h.SaloonID == saloonId  && h.EndTime > queryTime &&
                                     (c.Status == confirmed || c.Status == canceled || c.Status == displayed) &&
                                     (clientLastUpdatetime == null
                                     || (c.CreationTime <= queryTime && c.CreationTime > clientLastUpdatetime.Value)
                                     || (c.LastUpdateTime != null && (c.LastUpdateTime.Value <= queryTime && c.LastUpdateTime.Value > clientLastUpdatetime.Value)))
                               select c;
            #endregion

            return new Tuple<Mosque, Obit[], Template[], ImageBlob[], Consolation[]>(mosque, obits.Distinct().ToArray(), templates.Distinct().ToArray(), blobs.Distinct().ToArray(), consolations.Distinct().ToArray());
        }

        public List<Consolation> Filter(int cityId, string status, int count)
        {
            var query = from c in context.Consolations.Include(c => c.Obit.Mosque)
                        where (string.IsNullOrEmpty(status) || c.Status == status)
                              && (cityId <= 0 || cityId == c.Obit.Mosque.CityID)
                        orderby c.CreationTime descending
                        select c;
            return query.Take(count).ToList();
        }

        public bool IsDisplayed(int id)
        {
            var query = from d in context.Displays
                        where d.ConsolationID == id
                        select d.ID;
            return query.Any();
        }

        public List<Consolation> Find(string[] trackingNumbers)
        {
            return set.Where(c => trackingNumbers.Contains(c.TrackingNumber)).ToList();
        }
    }
}
