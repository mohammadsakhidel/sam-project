using SamDataAccess.Contexts;
using SamModels.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamDataAccess.Repos.Interfaces
{
    public interface IDisplayRepo : IRepo<SamDbContext, Display>
    {
        List<Display> GetDisplays(params int[] consolationId);

        Dictionary<int, int> GetDisplayCounts(params int[] consolationId);
    }
}
