using SamDataAccess.Contexts;
using SamDataAccess.Repos.BaseClasses;
using SamDataAccess.Repos.Interfaces;
using SamModels.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Text;
using System.Threading.Tasks;
using RamancoLibrary.Utilities;
using System.Transactions;
using SamUtils.Enums;

namespace SamDataAccess.Repos
{
    public class MosqueRepo : Repo<SamDbContext, Mosque>, IMosqueRepo
    {
        #region Extensions:
        public override Mosque Get(params object[] id)
        {
            var _id = Convert.ToInt32(id[0]);
            return set.Include(m => m.Saloons).SingleOrDefault(m => m.ID == _id);
        }
        public List<Mosque> FindByCity(int cityId)
        {
            return set.Where(m => m.CityID == cityId).ToList();
        }
        public void UpdateWidthSave(Mosque newMosque, ImageBlob image)
        {
            var mosque = Get(newMosque.ID);
            if (mosque != null)
            {
                using (var ts = new TransactionScope())
                {
                    #region update fields:
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
                    #endregion
                    #region update image:
                    #region remove old image if exists:
                    if (!string.IsNullOrEmpty(mosque.ImageID))
                    {
                        var oldBlob = context.Blobs.SingleOrDefault(b => b.ID == mosque.ImageID);
                        context.Blobs.Remove(oldBlob);
                        mosque.ImageID = "";
                    }
                    #endregion
                    if (image != null)
                    {
                        #region add and set new image:
                        context.Blobs.Add(image);
                        mosque.ImageID = image.ID;
                        #endregion
                    }
                    #endregion
                    #region update saloons:
                    if (mosque.Saloons != null)
                        context.Saloons.RemoveRange(mosque.Saloons);

                    mosque.Saloons = newMosque.Saloons;
                    #endregion

                    Save();
                    ts.Complete();
                }
            }
        }
        public Saloon FindSaloon(int mosqueId, string saloonId)
        {
            return context.Saloons.Find(saloonId, mosqueId);
        }
        public void AddWithSave(Mosque mosque, ImageBlob image)
        {
            using (var ts = new TransactionScope())
            {
                if (image != null)
                {
                    context.Blobs.Add(image);
                    mosque.ImageID = image.ID;
                }
                context.Mosques.Add(mosque);

                Save();
                ts.Complete();
            }
        }
        public List<Mosque> Search(int provinceId, int cityId, string name)
        {
            var query = from m in context.Mosques
                        join c in context.Cities on m.CityID equals c.ID
                        join p in context.Provinces on c.ProvinceID equals p.ID
                        where (provinceId <= 0 || p.ID == provinceId) &&
                              (cityId <= 0 || c.ID == cityId) &&
                              (name == "" || m.Name.Contains(name))
                        select m;

            return query.ToList();
        }
        public List<Mosque> GetLatests(int count)
        {
            return set.OrderByDescending(m => m.CreationTime).Take(count).ToList();
        }
        public List<Tuple<Mosque, Obit[], Consolation[]>> GetMosquesTurnover(DateTime beginDate, DateTime endDate, int? provinceId, int? cityId, int? mosqueId)
        {
            var verified = PaymentStatus.verified.ToString();

            var items = (from m in context.Mosques
                           join ct in context.Cities on m.CityID equals ct.ID
                           where (!provinceId.HasValue || provinceId.Value == ct.ProvinceID)
                             && (!cityId.HasValue || cityId.Value == ct.ID)
                             && (!mosqueId.HasValue || mosqueId.Value == m.ID)
                           select new
                           {
                               Mosque = m,
                               Obits = m.Obits.Where(o => o.ObitHoldings.Any(h => DbFunctions.TruncateTime(h.BeginTime) >= DbFunctions.TruncateTime(beginDate) && DbFunctions.TruncateTime(h.BeginTime) <= DbFunctions.TruncateTime(endDate))),
                               Consolations = m.Obits.Where(o => o.ObitHoldings.Any(h => DbFunctions.TruncateTime(h.BeginTime) >= DbFunctions.TruncateTime(beginDate) && DbFunctions.TruncateTime(h.BeginTime) <= DbFunctions.TruncateTime(endDate)))
                                                          .SelectMany(o => o.Consolations.Where(c => c.PaymentStatus == verified))
                           }).ToList();

            return items.Select(i => new Tuple<Mosque, Obit[], Consolation[]>(i.Mosque, i.Obits.ToArray(), i.Consolations.ToArray())).ToList();
        }
        #endregion

        #region Overrides:
        public override void Remove(Mosque entity)
        {
            var blob = context.Blobs.SingleOrDefault(b => b.ID == entity.ImageID);
            if (blob != null)
                context.Blobs.Remove(blob);

            base.Remove(entity);
        }


        #endregion
    }
}
