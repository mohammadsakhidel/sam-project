using SamClient.Models.Repos.BaseClasses;
using SamModels.Entities.Core;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamClient.Models.Repos
{
    public class ConsolationRepo : Repo<SamClientDbContext, Consolation>
    {
        #region Extensions:
        public List<Consolation> GetAll(DateTime date)
        {
            var items = set.Where(c => DbFunctions.TruncateTime(c.CreationTime) == DbFunctions.TruncateTime(date))
                .Include(c => c.Obit)
                .Include(c => c.Customer)
                .Include(c => c.Template)
                .OrderByDescending(c => c.CreationTime)
                .ToList();
            return items;
        }
        public Consolation GetNext(int? currentId)
        {
            Consolation next = null;
            var current = (currentId.HasValue ? Get(currentId.Value) : null);
            
            if (current == null)
            {
                next = set.OrderByDescending(c => c.CreationTime).FirstOrDefault();
            }
            else
            {
                next = set.Where(c => c.ID != current.ID && c.CreationTime < current.CreationTime)
                          .OrderByDescending(c => c.CreationTime).FirstOrDefault();

                if (next == null)
                    next = GetNext(null);
            }

            return next;
        }
        #endregion
    }
}
