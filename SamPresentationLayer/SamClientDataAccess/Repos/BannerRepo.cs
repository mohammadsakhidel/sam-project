using RamancoLibrary.Utilities;
using SamClientDataAccess.Contexts;
using SamClientDataAccess.Repos.BaseClasses;
using SamModels.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamClientDataAccess.Repos
{
    public class BannerRepo : Repo<SamClientDbContext, Banner>
    {
        #region Ctors:
        public BannerRepo() : base()
        {
        }
        public BannerRepo(SamClientDbContext context) : base(context)
        {
        }
        #endregion

        #region Extensions:
        public List<Tuple<Banner, Blob>> GetBannersToDisplay()
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

            var banners = from b in context.Banners
                          join i in context.Blobs on b.ImageID equals i.ID
                          where b.IsActive &&
                                ((!b.LifeBeginTime.HasValue || b.LifeBeginTime.Value <= now) && (!b.LifeEndTime.HasValue || b.LifeEndTime.Value > now)) &&
                                (!(b is MosqueBanner) || (b as MosqueBanner).MosqueID == mosqueId) &&
                                (!(b is ObitBanner) || currentObitIds.Contains((b as ObitBanner).ObitID.Value))
                          orderby b.Priority ascending, b.CreationTime ascending
                          select new { Banner = b, Blob = i };
            return banners.ToList().Select(o => new Tuple<Banner, Blob>(o.Banner, o.Blob)).ToList();
        }
        public void AddOrUpdate(Banner banner)
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
        public void Update(Banner newBanner)
        {
            var banner = Get(newBanner.ID);
            if (banner != null)
            {
                #region base:
                banner.Title = newBanner.Title;
                banner.ImageID = newBanner.ImageID;
                banner.LifeBeginTime = newBanner.LifeBeginTime;
                banner.LifeEndTime = newBanner.LifeEndTime;
                banner.IsActive = newBanner.IsActive;
                banner.Priority = newBanner.Priority;
                banner.ShowOnStart = newBanner.ShowOnStart;
                banner.DurationSeconds = newBanner.DurationSeconds;
                banner.Interval = newBanner.Interval;
                banner.LastUpdateTime = newBanner.LastUpdateTime;
                #endregion
                #region area:
                if (banner is AreaBanner)
                {
                    var areaBanner = (AreaBanner)banner;
                    var newAreaBanner = (AreaBanner)newBanner;
                    areaBanner.CityID = newAreaBanner.CityID;
                    areaBanner.ProvinceID = newAreaBanner.ProvinceID;
                }
                #endregion
                #region mosque:
                else if (banner is MosqueBanner)
                {
                    var mosqueBanner = (MosqueBanner)banner;
                    var newMosqueBanner = (MosqueBanner)newBanner;
                    mosqueBanner.MosqueID = newMosqueBanner.MosqueID;
                }
                #endregion
                #region obit:
                else if (banner is ObitBanner)
                {
                    var obitBanner = (ObitBanner)banner;
                    var newObitBanner = (ObitBanner)newBanner;
                    obitBanner.ObitID = newObitBanner.ObitID;
                }
                #endregion
            }
        }
        public List<Banner> GetObsoleteItems()
        {
            var now = DateTimeUtils.Now;
            var q = from b in context.Banners
                    where (b.LifeEndTime.HasValue && b.LifeEndTime.Value < now) ||
                          (b is ObitBanner && !(b as ObitBanner).Obit.ObitHoldings.Where(h => h.EndTime > now).Any())
                    select b;

            return q.ToList();
        }
        #endregion

        #region Overrides:
        public override void Remove(Banner entity)
        {
            #region remove image:
            var blob = context.Blobs.SingleOrDefault(b => b.ID == entity.ImageID);
            if (blob != null)
                context.Blobs.Remove(blob);
            #endregion

            base.Remove(entity);
        }
        #endregion
    }
}
