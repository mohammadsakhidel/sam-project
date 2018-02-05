using RamancoLibrary.Utilities;
using SamClientDataAccess.ClientModels;
using SamClientDataAccess.Contexts;
using SamClientDataAccess.Repos.BaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamClientDataAccess.Repos
{
    public class LocalObitRepo : Repo<SamClientDbContext, LocalObit>
    {
        #region Ctors:
        public LocalObitRepo() : base()
        {

        }
        public LocalObitRepo(SamClientDbContext context) : base(context)
        {

        }
        #endregion

        #region Extensions:
        public void AddOrUpdate(LocalObit obit)
        {
            var exists = set.Where(o => o.ID == obit.ID).Any();
            if (!exists)
            {
                Add(obit);
            }
            else
            {
                Update(obit);
            }
        }
        public void Update(LocalObit newObit)
        {
            var obit = Get(newObit.ID);
            if (obit != null)
            {
                obit.Title = newObit.Title;
                obit.TrackingNumber = newObit.TrackingNumber;
                obit.LastUpdateTime = newObit.LastUpdateTime;

                //update holdings:
                if (obit.ObitHoldings != null)
                    context.ObitHoldings.RemoveRange(obit.ObitHoldings);

                obit.ObitHoldings = newObit.ObitHoldings;
            }
        }
        public List<LocalObit> GetActiveObits()
        {
            #region preres:
            var setting = context.ClientSettings.Find(1);
            var mosqueId = setting.MosqueID;
            var saloonId = setting.SaloonID;

            var now = DateTimeUtils.Now;
            #endregion

            var recs = from o in context.Obits
                       where (from h in context.ObitHoldings
                              where h.ObitID == o.ID && h.BeginTime <= now && h.EndTime >= now
                              select h).Any()
                       select o;

            return recs.ToList();
        }
        #endregion
    }
}
