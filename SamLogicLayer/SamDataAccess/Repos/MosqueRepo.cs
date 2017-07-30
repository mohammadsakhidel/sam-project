using SamDataAccess.Contexts;
using SamDataAccess.Repos.BaseClasses;
using SamDataAccess.Repos.Interfaces;
using SamModels.Entities.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Text;
using System.Threading.Tasks;
using RamancoLibrary.Utilities;

namespace SamDataAccess.Repos
{
    public class MosqueRepo : Repo<SamDbContext, Mosque>, IMosqueRepo
    {
        public override Mosque Get(params object[] id)
        {
            var _id = Convert.ToInt32(id[0]);
            return set.Include(m => m.Saloons).SingleOrDefault(m => m.ID == _id);
        }

        public List<Mosque> FindByCity(int cityId)
        {
            return set.Where(m => m.CityID == cityId).ToList();
        }

        public void UpdateWidthSave(Mosque newMosque)
        {
            var mosque = Get(newMosque.ID);
            if (mosque != null)
            {
                mosque.Name = newMosque.Name;
                mosque.ImamName = newMosque.ImamName;
                mosque.ImamCellPhone = newMosque.ImamCellPhone;
                mosque.InterfaceName = newMosque.InterfaceName;
                mosque.InterfaceCellPhone = newMosque.InterfaceCellPhone;
                mosque.CityID = newMosque.CityID;
                mosque.Address = newMosque.Address;
                mosque.Location = newMosque.Location;
                mosque.PhoneNumber = newMosque.PhoneNumber;
                mosque.LastUpdateTime = DateTimeUtils.Now;

                // update saloons:
                if (mosque.Saloons != null)
                    context.Saloons.RemoveRange(mosque.Saloons);

                mosque.Saloons = newMosque.Saloons;
                Save();
            }
        }
    }
}
