using SamDataAccess.Contexts;
using SamModels.Entities;
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
    }
}
