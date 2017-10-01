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
                .Include(c => c.Template)
                .OrderByDescending(c => c.CreationTime)
                .ToList();
            return items;
        }
        public Tuple<Consolation, int> GetNext(int? currentId)
        {
            #region prereq data:
            var setting = context.ClientSettings.Find(1);
            if (setting == null)
                throw new Exception("Client Settings Not Found!");

            var now = DateTimeUtils.Now;
            var confirmed = ConsolationStatus.confirmed.ToString();

            var all = from o in context.Obits
                      join c in context.Consolations on o.ID equals c.ObitID
                      join h in context.ObitHoldings on o.ID equals h.ObitID
                      where o.MosqueID == setting.MosqueID
                            && h.SaloonID == setting.SaloonID
                            && (now >= h.BeginTime && now <= h.EndTime)
                            && c.Status == confirmed
                      orderby c.CreationTime ascending
                      select c;

            var holding = (from h in context.ObitHoldings
                           join o in context.Obits on h.ObitID equals o.ID
                           where o.MosqueID == setting.MosqueID
                                 && h.SaloonID == setting.SaloonID
                                 && (now >= h.BeginTime && now <= h.EndTime)
                           orderby h.EndTime descending
                           select h).FirstOrDefault();
            #endregion

            #region Find Next Consolation:
            Consolation next = null;
            var current = (currentId.HasValue && currentId.Value > 0 ? Get(currentId.Value) : null);

            if (current == null)
            {
                next = all.FirstOrDefault();
            }
            else
            {
                next = all.Where(c => c.ID != current.ID && c.CreationTime >= current.CreationTime)
                          .OrderBy(c => c.CreationTime)
                          .FirstOrDefault();

                if (next == null)
                    return GetNext(null);
            }
            #endregion

            #region Calc Duration:
            var durationMills = ClientSetting.MIN_SLIDE_DURATION_MILLS;
            if (next != null)
            {
                var remainingCount = all.Where(c => c.CreationTime > next.CreationTime).Count();
                var remainingTime = holding.EndTime - now;
                if (remainingCount > 0)
                    durationMills = (int)(remainingTime.TotalMilliseconds / remainingCount);
                else
                    durationMills = setting.DefaultSlideDurationMilliSeconds;

                if (durationMills < ClientSetting.MIN_SLIDE_DURATION_MILLS)
                    durationMills = ClientSetting.MIN_SLIDE_DURATION_MILLS;

                if (durationMills > setting.DefaultSlideDurationMilliSeconds)
                    durationMills = setting.DefaultSlideDurationMilliSeconds;
            }
            #endregion

            return new Tuple<Consolation, int>(next, durationMills);
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
        #endregion
    }
}
