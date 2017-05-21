using SamDataAccess.Contexts;
using SamDataAccess.Repos.BaseClasses;
using SamDataAccess.Repos.Interfaces;
using SamModels.Entities.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamDataAccess.Repos
{
    public class MosqueRepo : Repo<SamDbContext, Mosque>, IMosqueRepo
    {
        public List<Mosque> FindByCity(int cityId)
        {
            return set.Where(m => m.CityID == cityId).ToList();
        }
    }
}
