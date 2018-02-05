using SamClientDataAccess.ClientModels;
using SamClientDataAccess.Contexts;
using SamClientDataAccess.Repos.BaseClasses;
using SamUtils.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamClientDataAccess.Repos
{
    public class LocalDisplayRepo : Repo<SamClientDbContext, LocalDisplay>
    {
        #region Ctors:
        public LocalDisplayRepo() : base()
        {

        }
        public LocalDisplayRepo(SamClientDbContext context) : base(context)
        {

        }
        #endregion

        #region Extensions:
        public List<LocalDisplay> GetPendingDisplays(DateTime? lastUploadTime, DateTime now)
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
