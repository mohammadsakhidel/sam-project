using RamancoLibrary.Utilities;
using SamDataAccess.Contexts;
using SamDataAccess.Repos.BaseClasses;
using SamDataAccess.Repos.Interfaces;
using SamModels.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace SamDataAccess.Repos
{
    public class BannerRepo : Repo<SamDbContext, Banner>, IBannerRepo
    {
        public void AddWithSave(Banner banner, ImageBlob blob)
        {
            using (var ts = new TransactionScope())
            {
                context.Blobs.Add(blob);
                banner.ImageID = blob.ID;
                context.Banners.Add(banner);
                Save();
                ts.Complete();
            }
        }

        public List<Banner> GetLatests(int count)
        {
            return set.OrderByDescending(b => b.CreationTime)
                .Take(count).ToList();
        }

        public void UpdateWithSave(Banner newBanner, ImageBlob image)
        {
            using (var ts = new TransactionScope())
            {
                var banner = Get(newBanner.ID);

                #region BackgroundImage:
                if (image != null)
                {
                    #region Delete Old Background Image Blob:
                    var oldBlob = context.Blobs.Find(banner.ImageID);
                    if (oldBlob != null)
                        context.Blobs.Remove(oldBlob);
                    #endregion

                    #region Add New Blob:
                    context.Blobs.Add(image);
                    #endregion

                    banner.ImageID = image.ID;
                }
                #endregion

                #region update base fields:
                banner.Title = newBanner.Title;
                banner.LifeBeginTime = newBanner.LifeBeginTime;
                banner.LifeEndTime = newBanner.LifeEndTime;
                banner.IsActive = newBanner.IsActive;
                banner.Priority = newBanner.Priority;
                banner.ShowOnStart = newBanner.ShowOnStart;
                banner.DurationSeconds = newBanner.DurationSeconds;
                banner.Interval = newBanner.Interval;
                banner.LastUpdateTime = DateTimeUtils.Now;
                #endregion
                #region update area specific fields:
                if (banner is AreaBanner)
                {
                    var newAreaBanner = (AreaBanner)newBanner;
                    var areaBanner = (AreaBanner)banner;
                    areaBanner.CityID = newAreaBanner.CityID;
                    areaBanner.ProvinceID = newAreaBanner.ProvinceID;
                }
                #endregion
                #region update mosque specific fields:
                if (banner is MosqueBanner)
                {
                    var newMosqueBanner = (MosqueBanner)newBanner;
                    var mosqueBanner = (MosqueBanner)banner;
                    mosqueBanner.MosqueID = newMosqueBanner.MosqueID;
                }
                #endregion
                #region update obit specific fields:
                if (banner is ObitBanner)
                {
                    var newObitBanner = (ObitBanner)newBanner;
                    var obitBanner = (ObitBanner)banner;
                    obitBanner.ObitID = newObitBanner.ObitID;
                }
                #endregion

                Save();
                ts.Complete();
            }
        }

        public override void Remove(Banner entity)
        {
            var blob = context.Blobs.SingleOrDefault(b => b.ID == entity.ImageID);
            if (blob != null)
                context.Blobs.Remove(blob);

            base.Remove(entity);
        }
    }
}
