using SamClientDataAccess.Contexts;
using SamClientDataAccess.Repos.BaseClasses;
using SamModels.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace SamClientDataAccess.Repos
{
    public class TemplateRepo : Repo<SamClientDbContext, Template>
    {
        #region Ctors:
        public TemplateRepo() : base()
        {

        }
        public TemplateRepo(SamClientDbContext context) : base(context)
        {

        }
        #endregion

        #region Overrides:
        public override Template Get(params object[] id)
        {
            var _id = Convert.ToInt32(id[0]);
            return set.Include(t => t.TemplateFields).SingleOrDefault(t => t.ID == _id);
        }
        public override void Remove(Template entity)
        {
            #region remove image:
            var blob = context.Blobs.SingleOrDefault(b => b.ID == entity.BackgroundImageID);
            if (blob != null)
                context.Blobs.Remove(blob);
            #endregion

            base.Remove(entity);
        }
        #endregion

        #region Extensions:
        public void AddOrUpdate(Template template)
        {
            var exists = set.Where(t => t.ID == template.ID).Any();
            if (!exists)
            {
                Add(template);
            }
            else
            {
                Update(template);
            }
        }
        public void Update(Template newTemplate)
        {
            var oldTemplate = Get(newTemplate.ID);

            #region Update Category:
            if (oldTemplate.TemplateCategoryID != newTemplate.TemplateCategoryID)
            {
                // insert category if not exists:
                var catExists = context.TemplateCategories.Where(c => c.ID == newTemplate.TemplateCategoryID).Any();
                if (!catExists)
                    context.TemplateCategories.Add(newTemplate.Category);

                oldTemplate.TemplateCategoryID = newTemplate.TemplateCategoryID;
            }
            #endregion

            #region Update Props:
            oldTemplate.Text = newTemplate.Text;
            oldTemplate.Price = newTemplate.Price;
            oldTemplate.BackgroundImageID = newTemplate.BackgroundImageID;
            oldTemplate.WidthRatio = newTemplate.WidthRatio;
            oldTemplate.HeightRatio = newTemplate.HeightRatio;
            oldTemplate.Name = newTemplate.Name;
            oldTemplate.IsActive = newTemplate.IsActive;
            oldTemplate.Order = newTemplate.Order;
            oldTemplate.LastUpdateTime = newTemplate.LastUpdateTime;
            #endregion

            #region Update Template Fields:
            if (oldTemplate.TemplateFields != null)
                context.TemplateFields.RemoveRange(oldTemplate.TemplateFields);

            oldTemplate.TemplateFields = newTemplate.TemplateFields;
            #endregion
        }
        #endregion
    }
}
