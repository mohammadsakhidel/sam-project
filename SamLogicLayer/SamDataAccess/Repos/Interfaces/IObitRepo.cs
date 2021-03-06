using SamDataAccess.Contexts;
using SamModels.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamDataAccess.Repos.Interfaces
{
    public interface IObitRepo : IRepo<SamDbContext, Obit>
    {
        List<Obit> Get(int mosqueId, DateTime date);

        List<Obit> GetAllFromDate(int mosqueId, DateTime date);

        List<Obit> GetHenceForwardObits(int mosqueId);

        List<Obit> Search(string query, DateTime date);

        List<ObitHolding> GetHoldings(int mosqueId, DateTime date);

        void UpdateWithSave(Obit newObit);

        Obit FindByTrackingNumber(string trackingNumber);

        List<Obit> GetCompletedObitsWhichHaveConsolations(DateTime? fromTime, int minConsolationsCount = 2);

        List<Obit> GetCompletedObitsWhichHaveDisplayedConsolations(DateTime? fromTime);

        List<Obit> GetLastObitsWithDeceasedId(int days);

        List<Obit> GetFutureRelatedObits(int obitId);

        List<Obit> GetLatests(int count);
    }
}
