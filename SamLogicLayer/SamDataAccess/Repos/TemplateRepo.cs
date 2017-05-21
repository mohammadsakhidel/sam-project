using SamDataAccess.Contexts;
using SamDataAccess.Repos.BaseClasses;
using SamDataAccess.Repos.Interfaces;
using SamModels.Entities.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace SamDataAccess.Repos
{
    public class TemplateRepo : Repo<SamDbContext, Template>, ITemplateRepo
    {
        public override List<Template> GetAll()
        {
            return set.Include(t => t.Category).ToList();
        }
    }
}
