using SamDataAccess.Contexts;
using SamDataAccess.Repos.BaseClasses;
using SamDataAccess.Repos.Interfaces;
using SamModels.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamDataAccess.Repos
{
    public class DisplayRepo : Repo<SamDbContext, Display>, IDisplayRepo
    {
        public Dictionary<int, int> GetDisplayCounts(params int[] consolationIds)
        {
            return consolationIds != null ?
                   context.Displays.Where(d => consolationIds.Contains(d.ConsolationID))
                        .GroupBy(d => d.ConsolationID)
                        .ToDictionary(g => g.Key, g => g.Count()) 
                   : new Dictionary<int, int>();
        }

        public List<Display> GetDisplays(params int[] consolationIds)
        {
            return consolationIds != null ?
                context.Displays.Where(d => consolationIds.Contains(d.ConsolationID)).ToList() :
                new List<Display>();
        }
    }
}
