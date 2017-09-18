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
    }
}
