using SamClientDataAccess.Contexts;
using SamClientDataAccess.Repos.BaseClasses;
using SamModels.Entities.Core;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public Consolation GetNext(int? currentId)
        {
            var setting = context.ClientSettings.Find(1);
            if (setting == null)
                throw new Exception("Client Settings Not Found!");

            Consolation next = null;
            var current = (currentId.HasValue ? Get(currentId.Value) : null);
            
            if (current == null)
            {
                next = set.Where(c => c.Obit.MosqueID == setting.MosqueID).OrderByDescending(c => c.CreationTime).FirstOrDefault();
            }
            else
            {
                next = set.Where(c => c.Obit.MosqueID == setting.MosqueID && c.ID != current.ID && c.CreationTime < current.CreationTime)
                          .OrderByDescending(c => c.CreationTime).FirstOrDefault();

                if (next == null)
                    next = GetNext(null);
            }

            return next;
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
                consolation.CreationTime = newConsolation.CreationTime;
                consolation.LastUpdateTime = newConsolation.LastUpdateTime;
            }
        }
        #endregion
    }
}
