using SamClientDataAccess.Contexts;
using SamClientDataAccess.Repos.BaseClasses;
using SamModels.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using RamancoLibrary.Utilities;
using SamUtils.Enums;
using ClientModels.Models;
using SamClientDataAccess.ClientModels;

namespace SamClientDataAccess.Repos
{
    public class ConsolationRepo : Repo<SamClientDbContext, Consolation>
    {
        #region Ctors:
        public ConsolationRepo() : base()
        {

        }
        public ConsolationRepo(SamClientDbContext context) : base(context)
        {

        }
        #endregion

        #region Extensions:
        public List<Consolation> GetAll(DateTime date)
        {
            var setting = context.ClientSettings.Find(1);
            if (setting == null)
                throw new Exception("Client Settings Not Found!");

            var items = set.Where(c => c.Obit.MosqueID == setting.MosqueID && DbFunctions.TruncateTime(c.CreationTime) == DbFunctions.TruncateTime(date))
                .Include(c => c.Obit)
                .Include(c => c.Customer)
                .OrderByDescending(c => c.CreationTime)
                .ToList();
            return items;
        }
        public List<Tuple<Consolation, ConsolationImage>> GetConsolationsToDisplay()
        {
            #region prereq data:
            var setting = context.ClientSettings.Find(1);
            if (setting == null)
                throw new Exception("Client Settings Not Found!");

            DateTime now = DateTimeUtils.Now;
            var confirmed = ConsolationStatus.confirmed.ToString();
            var displayed = ConsolationStatus.displayed.ToString();
            #endregion

            var all = from c in context.Consolations
                      join i in context.ConsolationImages on c.ID equals i.ConsolationID
                      join o in context.Obits on c.ObitID equals o.ID
                      join h in context.ObitHoldings on o.ID equals h.ObitID
                      where o.MosqueID == setting.MosqueID
                            && h.SaloonID == setting.SaloonID
                            && (now >= h.BeginTime && now <= h.EndTime)
                            && (c.Status == confirmed || c.Status == displayed)
                      orderby c.CreationTime ascending
                      select new { Consolation = c, Image = i };

            return all.ToList().Select(o => new Tuple<Consolation, ConsolationImage>(o.Consolation, o.Image)).ToList();
        }
        public void AddOrUpdate(Consolation consolation)
        {
            var exists = set.Where(c => c.ID == consolation.ID).Any();
            if (!exists)
            {
                #region customer:
                if (consolation.Customer != null)
                {
                    var cExists = context.Customers.Any(c => c.ID == consolation.CustomerID);
                    if (cExists)
                        consolation.Customer = null;
                }
                #endregion

                Add(consolation);
            }
            else
            {
                Update(consolation);
            }
        }
        public void Update(Consolation newConsolation)
        {
            var consolation = Get(newConsolation.ID);
            if (consolation != null)
            {
                consolation.TemplateID = newConsolation.TemplateID;
                consolation.TemplateInfo = newConsolation.TemplateInfo;
                consolation.Audience = newConsolation.Audience;
                consolation.From = newConsolation.From;
                consolation.Status = newConsolation.Status;
                consolation.PaymentStatus = newConsolation.PaymentStatus;
                consolation.TrackingNumber = newConsolation.TrackingNumber;
                consolation.AmountToPay = newConsolation.AmountToPay;
                consolation.CreationTime = newConsolation.CreationTime;
                consolation.LastUpdateTime = newConsolation.LastUpdateTime;
            }
        }
        public List<Consolation> GetObsoleteItems()
        {
            var now = DateTimeUtils.Now;
            var q = from c in context.Consolations.Include(context => context.Obit.ObitHoldings)
                    where !c.Obit.ObitHoldings.Where(h => h.EndTime > now).Any()
                    select c;

            return q.ToList();
        }
        #endregion

        #region Overrides:
        public override void Remove(Consolation entity)
        {
            #region remove image:
            var image = context.ConsolationImages.SingleOrDefault(ci => ci.ConsolationID == entity.ID);
            if (image != null)
                context.ConsolationImages.Remove(image);
            #endregion

            base.Remove(entity);
        }
        #endregion
    }
}