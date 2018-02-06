using RamancoLibrary.Utilities;
using SamClientDataAccess.ClientModels;
using SamClientDataAccess.Code.Enums;
using SamClientDataAccess.Contexts;
using SamClientDataAccess.Repos.BaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamClientDataAccess.Repos
{
    public class LocalBannerRepo : Repo<SamClientDbContext, LocalBanner>
    {
        #region Ctors:
        public LocalBannerRepo() : base()
        {
        }
        public LocalBannerRepo(SamClientDbContext context) : base(context)
        {
        }
        #endregion

        #region Extensions:
        public List<LocalBanner> GetBannersToDisplay()
        {
            #region prereq data:
            var setting = context.ClientSettings.Find(1);
            if (setting == null)
                throw new Exception("Client Settings Not Found!");

            DateTime now = DateTimeUtils.Now;
            int mosqueId = setting.MosqueID;

            var currentObitIds = (from o in context.Obits
                                  where o.ObitHoldings.Where(h => h.BeginTime <= now && h.EndTime > now).Any()
                                  select o.ID).ToList();
            #endregion

            var obitbannertype = LocalBannerTypes.obit.ToString();

            var banners = from b in context.Banners
                          where b.IsActive && b.ImageBytes != null &&
                                (!b.LifeBeginTime.HasValue || b.LifeBeginTime.Value <= now) &&
                                (!b.LifeEndTime.HasValue || b.LifeEndTime.Value > now) &&
                                (!(b.Type == obitbannertype) || currentObitIds.Contains(b.ObitID.Value))
                          orderby b.Priority descending, b.CreationTime descending
                          select b;
            return banners.ToList();
        }
        public void AddOrUpdate(LocalBanner banner)
        {
            var exists = set.Where(b => b.ID == banner.ID).Any();
            if (!exists)
            {
                Add(banner);
            }
            else
            {
                Update(banner);
            }
        }
        public void Update(LocalBanner newBanner)
        {
            var banner = Get(newBanner.ID);
            if (banner != null)
            {
                #region base:
                banner.Title = newBanner.Title;
                banner.LifeBeginTime = newBanner.LifeBeginTime;
                banner.LifeEndTime = newBanner.LifeEndTime;
                banner.IsActive = newBanner.IsActive;
                banner.Priority = newBanner.Priority;
                banner.ShowOnStart = newBanner.ShowOnStart;
                banner.DurationSeconds = newBanner.DurationSeconds;
                banner.Interval = newBanner.Interval;
                banner.LastUpdateTime = newBanner.LastUpdateTime;

                if (newBanner.ImageBytes != null && newBanner.ImageBytes.Any())
                    banner.ImageBytes = newBanner.ImageBytes;
                #endregion

                #region inherited banners:
                var obitbannertype = LocalBannerTypes.obit.ToString();
                if (banner.Type == obitbannertype)
                {
                    banner.ObitID = newBanner.ObitID;
                }
                else
                {
                    banner.ObitID = null;
                }
                #endregion
            }
        }
        public List<LocalBanner> GetObsoleteItems()
        {
            var days = 7;
            var now = DateTimeUtils.Now.AddDays(-days);
            var obitbannertype = LocalBannerTypes.obit.ToString();

            var q = from b in context.Banners
                    where (b.LifeEndTime.HasValue && b.LifeEndTime.Value < now) ||
                          (b.Type == obitbannertype && !(from h in context.ObitHoldings where h.ObitID == b.ObitID.Value select h).Where(h => h.EndTime > now).Any())
                    select b;

            return q.ToList();
        }
        #endregion
    }
}
