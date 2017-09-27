using SamClientDataAccess.ClientModels;
using SamClientDataAccess.Contexts;
using SamClientDataAccess.Repos.BaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamClientDataAccess.Repos
{
    public class DownloadImageTaskRepo : Repo<SamClientDbContext, DownloadImageTask>
    {
        #region Ctors:
        public DownloadImageTaskRepo()
        {

        }

        public DownloadImageTaskRepo(SamClientDbContext context) : base(context)
        {

        }
        #endregion

        #region Extensions:
        public List<DownloadImageTask> GetTasksOfStatus(string status)
        {
            return context.DownloadImageTasks
                .Where(t => t.Status == status)
                .OrderByDescending(t => t.CreationTime)
                .ToList();
        }
        #endregion
    }
}
