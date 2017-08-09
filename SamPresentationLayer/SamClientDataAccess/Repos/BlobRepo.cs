using SamClientDataAccess.Contexts;
using SamClientDataAccess.Repos.BaseClasses;
using SamModels.Entities.Blobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamClientDataAccess.Repos
{
    public class BlobRepo : Repo<SamClientDbContext, Blob>
    {
    }
}
