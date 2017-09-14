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

        List<Obit> Search(string query, DateTime date);

        List<ObitHolding> GetHoldings(int mosqueId, DateTime date);

        void UpdateWithSave(Obit newObit);
    }
}
