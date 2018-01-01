using SamDataAccess.Contexts;
using SamModels.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamDataAccess.Repos.Interfaces
{
    public interface IBannerRepo : IRepo<SamDbContext, Banner>
    {
        List<Banner> GetLatests(int count);
        void AddWithSave(Banner banner, ImageBlob blob);
        void UpdateWithSave(Banner newBanner, ImageBlob image);
        List<Banner> FindByType(Type type, int count);
    }
}
