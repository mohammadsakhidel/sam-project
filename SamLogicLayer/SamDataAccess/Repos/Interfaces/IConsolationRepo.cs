using SamDataAccess.Contexts;
using SamModels.Entities.Blobs;
using SamModels.Entities.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamDataAccess.Repos.Interfaces
{
    public interface IConsolationRepo : IRepo<SamDbContext, Consolation>
    {
        Tuple<Mosque, Obit[], Template[], ImageBlob[], Consolation[]> GetUpdates(int mosqueId, string saloonId, DateTime? clientLastUpdatetime, DateTime queryTime);

        List<Consolation> Filter(int cityId, string status, int count);

        bool IsDisplayed(int id);
    }
}
