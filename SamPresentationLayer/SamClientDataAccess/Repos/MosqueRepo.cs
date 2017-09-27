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
    public class MosqueRepo : Repo<SamClientDbContext, Mosque>
    {
        #region Ctors:
        public MosqueRepo() : base()
        {

        }
        public MosqueRepo(SamClientDbContext context) : base(context)
        {

        }
        #endregion

        #region Extension:
        public void AddOrUpdate(Mosque mosque)
        {
            var exists = set.Where(m => m.ID == mosque.ID).Any();
            if (!exists)
            {
                Add(mosque);
            }
            else
            {
                Update(mosque);
            }
        }
        public void Update(Mosque newMosque)
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
                mosque.ImageID = newMosque.ImageID;
                mosque.LastUpdateTime = newMosque.LastUpdateTime;

                // update saloons:
                if (mosque.Saloons != null)
                    context.Saloons.RemoveRange(mosque.Saloons);

                mosque.Saloons = newMosque.Saloons;
            }
        }
        #endregion

        #region Overrides:
        public override void Remove(Mosque entity)
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
