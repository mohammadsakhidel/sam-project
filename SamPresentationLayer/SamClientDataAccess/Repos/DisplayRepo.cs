using SamClientDataAccess.Contexts;
using SamClientDataAccess.Repos.BaseClasses;
using SamModels.Entities.Core;
using SamUtils.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamClientDataAccess.Repos
{
    public class DisplayRepo : Repo<SamClientDbContext, Display>
    {
        #region Ctors:
        public DisplayRepo() : base()
        {

        }
        public DisplayRepo(SamClientDbContext context) : base(context)
        {

        }
        #endregion

        #region Extensions:
        public List<Display> GetPendingDisplays(DateTime? lastUploadTime, DateTime now)
        {
            var pending = DisplaySyncStatus.pending.ToString();

            var query = from d in set
                        where d.SyncStatus == pending
                              && (!lastUploadTime.HasValue || d.CreationTime > lastUploadTime.Value)
                              && d.CreationTime <= now
                        select d;
            return query.ToList();
        }
        #endregion
    }
}
