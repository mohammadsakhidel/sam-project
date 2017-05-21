using SamDataAccess.Contexts;
using SamModels.Entities.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamDataAccess.Repos.Interfaces
{
    public interface ITemplateRepo : IRepo<SamDbContext, Template>
    {
    }
}
