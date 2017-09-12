using SamDataAccess.Contexts;
using SamModels.Entities.Blobs;
using SamModels.Entities.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamDataAccess.Repos.Interfaces
{
    public interface IMosqueRepo : IRepo<SamDbContext, Mosque>
    {
        List<Mosque> FindByCity(int cityId);
        void UpdateWidthSave(Mosque mosque, ImageBlob image);
        Saloon FindSaloon(int mosqueId, string saloonId);
        void AddWithSave(Mosque mosque, ImageBlob image);
        List<Mosque> Search(int provinceId, int cityId, string name);
        List<Mosque> GetLatests(int count);
    }
}
