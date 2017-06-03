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
using System.Transactions;
using SamModels.Entities.Blobs;

namespace SamDataAccess.Repos
{
    public class TemplateRepo : Repo<SamDbContext, Template>, ITemplateRepo
    {
        #region Overrides:
        public override List<Template> GetAll()
        {
            return set.Include(t => t.Category)
                .Include(t => t.TemplateFields).ToList();
        }

        public override Template Get(params object[] id)
        {
            var _id = Convert.ToInt32(id[0]);
            return set.Include(t => t.TemplateFields).SingleOrDefault(t => t.ID == _id);
        }
        #endregion

        #region Extensions:
        public void AddWithSave(Template template, ImageBlob backgroundImage)
        {
            using (var ts = new TransactionScope())
            {
                context.Blobs.Add(backgroundImage);
                context.Templates.Add(template);
                Save();
                ts.Complete();
            }
        }

        public void RemoveAllDependencies(int id)
        {
            using (var ts = new TransactionScope())
            {
                var entity = Get(id);
                if (entity != null)
                {
                    // remove backgroudn image:
                    var blob = context.Blobs.Find(entity.BackgroundImageID);
                    if (blob != null)
                        context.Blobs.Remove(blob);

                    //remove template and fields:
                    context.Templates.Remove(entity);

                    Save();
                    ts.Complete();
                }
            }
        }
        #endregion
    }
}
