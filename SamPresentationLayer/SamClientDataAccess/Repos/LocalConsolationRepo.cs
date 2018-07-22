using SamClientDataAccess.Contexts;
using SamClientDataAccess.Repos.BaseClasses;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RamancoLibrary.Utilities;
using SamUtils.Enums;
using SamClientDataAccess.ClientModels;

namespace SamClientDataAccess.Repos
{
    public class LocalConsolationRepo : Repo<SamClientDbContext, LocalConsolation>
    {
        #region Ctors:
        public LocalConsolationRepo() : base()
        {

        }
        public LocalConsolationRepo(SamClientDbContext context) : base(context)
        {

        }
        #endregion

        #region Extensions:
        public List<LocalConsolation> GetConsolationsToDisplay()
        {
            #region prereq data:
            var setting = context.ClientSettings.Find(1);
            if (setting == null)
                throw new Exception("Client Settings Not Found!");

            DateTime now = DateTimeUtils.Now;
            var confirmed = ConsolationStatus.confirmed.ToString();
            var displayed = ConsolationStatus.displayed.ToString();
            #endregion

            var saloonCurrentObitIds = (from o in context.Obits
                                        join h in context.ObitHoldings on o.ID equals h.ObitID
                                        where o.MosqueID == setting.MosqueID && h.SaloonID == setting.SaloonID
                                            && (now >= h.BeginTime && now <= h.EndTime)
                                        select o.ID).Distinct().ToList();
            var saloonCurrentObitStringIds = saloonCurrentObitIds.Select(oid => oid.ToString()).ToList();

            var all = from c in context.Consolations
                      where (
                                saloonCurrentObitIds.Contains(c.ObitID) ||
                                saloonCurrentObitStringIds.Any(oid => c.OtherObits.Contains(oid + ",") || c.OtherObits.EndsWith(", " + oid) || c.OtherObits == oid)
                            )
                            && (c.Status == confirmed || c.Status == displayed)
                            && c.ImageBytes != null
                      orderby c.CreationTime ascending
                      select c;

            return all.ToList();
        }
        public void AddOrUpdate(LocalConsolation consolation)
        {
            var exists = set.Where(c => c.ID == consolation.ID).Any();
            if (!exists)
            {
                Add(consolation);
            }
            else
            {
                Update(consolation);
            }
        }
        public void Update(LocalConsolation newConsolation)
        {
            var consolation = Get(newConsolation.ID);
            if (consolation != null)
            {
                consolation.Status = newConsolation.Status;
                consolation.PaymentStatus = newConsolation.PaymentStatus;
                consolation.TrackingNumber = newConsolation.TrackingNumber;
                consolation.LastUpdateTime = newConsolation.LastUpdateTime;
                consolation.ExtraData = newConsolation.ExtraData;
                if (newConsolation.ImageBytes != null && newConsolation.ImageBytes.Any())
                    consolation.ImageBytes = newConsolation.ImageBytes;
            }
        }
        public List<LocalConsolation> GetObsoleteItems()
        {
            var days = 7;
            var now = DateTimeUtils.Now.AddDays(-days);

            var q = from c in context.Consolations.Include(context => context.Obit.ObitHoldings)
                    where !c.Obit.ObitHoldings.Where(h => h.EndTime > now).Any()
                    select c;

            return q.ToList();
        }
        #endregion
    }
}