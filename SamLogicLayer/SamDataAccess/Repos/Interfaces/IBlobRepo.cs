using SamDataAccess.Contexts;
using SamModels.Entities.Blobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamDataAccess.Repos.Interfaces
{
    public interface IBlobRepo : IRepo<SamDbContext, Blob>
    {
    }
}
