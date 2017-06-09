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
            return set.Where(c => !currentId.HasValue || c.ID != currentId.Value).OrderByDescending(c => c.CreationTime).FirstOrDefault();
        }
        #endregion
    }
}
