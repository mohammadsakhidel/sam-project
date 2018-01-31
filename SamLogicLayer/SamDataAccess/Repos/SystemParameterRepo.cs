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
    public class SystemParameterRepo : Repo<SamDbContext, SystemParameter>, ISystemParameterRepo
    {
        public SystemParameterRepo()
        {
            var p = Get();
            if (p == null)
            {
                p = new SystemParameter() {
                    ID = 1,
                    LastGifCheckDate = null
                };
                AddWithSave(p);
            }
        }

        public override SystemParameter Get(params object[] id)
        {
            return base.Get(1);
        }

        public override void Add(SystemParameter entity)
        {
            entity.ID = 1;
            base.Add(entity);
        }
    }
}
