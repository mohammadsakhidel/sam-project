﻿using SamClient.Models.Repos.BaseClasses;
using SamModels.Entities.Blobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamClient.Models.Repos
{
    public class BlobRepo : Repo<SamClientDbContext, Blob>
    {
    }
}
