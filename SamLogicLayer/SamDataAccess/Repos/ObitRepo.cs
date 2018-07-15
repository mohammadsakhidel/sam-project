using RamancoLibrary.Utilities;
using SamDataAccess.Contexts;
using SamDataAccess.Repos.BaseClasses;
using SamDataAccess.Repos.Interfaces;
using SamModels.Entities;
using SamUtils.Enums;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamDataAccess.Repos
{
    public class ObitRepo : Repo<SamDbContext, Obit>, IObitRepo
    {
        public Obit FindByTrackingNumber(string trackingNumber)
        {
            return set.Where(o => o.TrackingNumber == trackingNumber)
                .OrderByDescending(o => o.CreationTime)
                .FirstOrDefault();
        }

        public List<Obit> Get(int mosqueId, DateTime date)
        {
            var obits = from o in set
                        where o.MosqueID == mosqueId &&
                              (from h in o.ObitHoldings
                               where DbFunctions.TruncateTime(h.BeginTime) == DbFunctions.TruncateTime(date) || DbFunctions.TruncateTime(h.EndTime) == DbFunctions.TruncateTime(date)
                               select h).Any()
                        select o;
            return obits.Include(o => o.ObitHoldings).ToList();
        }

        public List<Obit> GetAllFromDate(int mosqueId, DateTime date)
        {
            var obits = from o in set
                        where o.MosqueID == mosqueId &&
                              (from h in o.ObitHoldings
                               where DbFunctions.TruncateTime(h.BeginTime) >= DbFunctions.TruncateTime(date) || DbFunctions.TruncateTime(h.EndTime) >= DbFunctions.TruncateTime(date)
                               select h).Any()
                        select o;
            return obits.Include(o => o.ObitHoldings).ToList();
        }

        public List<Obit> GetCompletedObitsWhichHaveConsolations(DateTime? fromTime, int minConsolationsCount = 2)
        {
            var lastCheckTime = fromTime.HasValue ? fromTime.Value : DateTime.Now.AddDays(-4);
            var now = DateTime.Now;
            var verified = PaymentStatus.verified.ToString();
            var confirmed = ConsolationStatus.confirmed.ToString();
            var displayed = ConsolationStatus.displayed.ToString();

            var q = from o in context.Obits
                    .Include(o => o.ObitHoldings)
                    .Include(o => o.Consolations)
                    where o.ObitHoldings.Any()
                        && o.ObitHoldings.OrderByDescending(h => h.BeginTime).FirstOrDefault().EndTime > lastCheckTime
                        && o.ObitHoldings.OrderByDescending(h => h.BeginTime).FirstOrDefault().EndTime < now
                        && o.Consolations.Where(c => c.PaymentStatus == verified && (c.Status == confirmed || c.Status == displayed)).Count() >= minConsolationsCount
                    select o;

            return q.ToList();
        }

        public List<Obit> GetCompletedObitsWhichHaveDisplayedConsolations(DateTime? fromTime)
        {
            var lastCheckTime = fromTime.HasValue ? fromTime.Value : DateTime.Now.AddDays(-2);
            var now = DateTime.Now;
            var verified = PaymentStatus.verified.ToString();
            var confirmed = ConsolationStatus.confirmed.ToString();
            var displayed = ConsolationStatus.displayed.ToString();

            var q = from o in context.Obits
                        .Include(o => o.ObitHoldings)
                        .Include(o => o.Consolations)
                    where o.ObitHoldings.Any()
                        && o.ObitHoldings.OrderByDescending(h => h.BeginTime).FirstOrDefault().EndTime > lastCheckTime
                        && o.ObitHoldings.OrderByDescending(h => h.BeginTime).FirstOrDefault().EndTime < now
                        && o.Consolations.Where(c => c.PaymentStatus == verified && c.Status == displayed).Any()
                    select o;

            return q.ToList();
        }

        public List<Obit> GetFutureRelatedObits(int obitId)
        {
            var obit = Get(obitId);
            if (obit == null)
                return new List<Obit>();

            var now = DateTime.Now;
            var query = from o in context.Obits
                        join h in context.ObitHoldings on o.ID equals h.ObitID
                        where !string.IsNullOrEmpty(o.DeceasedIdentifier)
                              && o.DeceasedIdentifier == obit.DeceasedIdentifier
                              && o.ID != obit.ID
                              && h.EndTime > now
                        orderby h.BeginTime
                        select o;
            return query.Distinct().ToList();
        }

        public List<Obit> GetHenceForwardObits(int mosqueId)
        {
            var now = DateTimeUtils.Now;
            var obits = from o in set
                        where o.MosqueID == mosqueId &&
                              (from h in o.ObitHoldings
                               where h.EndTime > now
                               select h).Any()
                        select o;
            return obits.Include(o => o.ObitHoldings).ToList();
        }

        public List<ObitHolding> GetHoldings(int mosqueId, DateTime date)
        {
            var holdings = from h in context.ObitHoldings
                           join o in context.Obits
                           on h.ObitID equals o.ID
                           where o.MosqueID == mosqueId &&
                                 (DbFunctions.TruncateTime(h.BeginTime) == date || DbFunctions.TruncateTime(h.EndTime) == date)
                           orderby h.BeginTime
                           select h;
            return holdings.Include(h => h.Obit).ToList();
        }

        public List<Obit> GetLastObitsWithDeceasedId(int days)
        {
            var beginTime = DateTimeUtils.Now.AddDays(-days);
            var recs = from o in context.Obits.Include(ob => ob.ObitHoldings)
                       where o.ObitHoldings.Any(h => h.BeginTime > beginTime)
                             && !string.IsNullOrEmpty(o.DeceasedIdentifier)
                       select o;
            return recs.ToList();
        }

        public List<Obit> GetLatests(int count)
        {
            var maxDate = DateTimeUtils.Now.AddDays(7);
            var q = from o in context.Obits
                    join h in context.ObitHoldings on o.ID equals h.ObitID
                    where h.BeginTime < maxDate
                    orderby h.BeginTime descending
                    select o;
            return q.Take(count).ToList();
        }

        public List<Obit> Search(string query, DateTime date)
        {
            var obits = from o in set
                        where (o.Title.Contains(query) || query.Contains(o.Title)) &&
                              (from h in o.ObitHoldings
                               where DbFunctions.TruncateTime(h.BeginTime) >= DbFunctions.TruncateTime(date) || DbFunctions.TruncateTime(h.EndTime) >= DbFunctions.TruncateTime(date)
                               select h).Any()
                        select o;
            return obits.Include(o => o.ObitHoldings).ToList();
        }

        public void UpdateWithSave(Obit newObit)
        {
            var obit = Get(newObit.ID);
            if (obit != null)
            {
                obit.Title = newObit.Title;
                obit.ObitType = newObit.ObitType;
                obit.DeceasedIdentifier = newObit.DeceasedIdentifier;
                obit.OwnerCellPhone = newObit.OwnerCellPhone;
                obit.LastUpdateTime = DateTimeUtils.Now;

                //remove old holdings:
                if (obit.ObitHoldings != null)
                    context.ObitHoldings.RemoveRange(obit.ObitHoldings);

                foreach (var h in newObit.ObitHoldings)
                {
                    obit.ObitHoldings.Add(h);
                }

                Save();
            }
        }

    }
}
