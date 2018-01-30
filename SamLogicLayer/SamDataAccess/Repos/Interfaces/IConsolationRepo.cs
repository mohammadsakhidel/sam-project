using SamDataAccess.Contexts;
using SamModels.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamDataAccess.Repos.Interfaces
{
    public interface IConsolationRepo : IRepo<SamDbContext, Consolation>
    {
        Tuple<Mosque, Obit[], Template[], string[], Consolation[], Banner[], RemovedEntity[]> GetUpdates(int mosqueId, string saloonId, DateTime? clientLastUpdatetime, DateTime queryTime);

        List<Consolation> Filter(int cityId, string status, int count);

        bool IsDisplayed(int id);

        List<Consolation> Find(string[] trackingNumbers);

        Consolation Find(string trackingNumber);

        Consolation FindByPaymentID(string pId);

        List<Consolation> FindReversingConsolations();

        int GetNotifiablePendingsCount();

        List<Tuple<Consolation, Mosque>> Query(DateTime beginDate, DateTime endDate, int? provinceId, int? cityId, int? mosqueId);
    }
}
