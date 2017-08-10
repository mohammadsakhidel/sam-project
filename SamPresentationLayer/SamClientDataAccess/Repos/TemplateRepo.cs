using SamClientDataAccess.Contexts;
using SamClientDataAccess.Repos.BaseClasses;
using SamModels.Entities.Core;
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
            return set.Include(t => t.TemplateFields).SingleOrDefault(t => t.ID == ((int)id[0]));
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

            #region Update Props:
            oldTemplate.TemplateCategoryID = newTemplate.TemplateCategoryID;
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
            if (oldTemplate.TemplateFields == null)
                oldTemplate.TemplateFields = new List<TemplateField>();
            #region deleted removed fields:
            var toBeRemovedFields = oldTemplate.TemplateFields.Where(of => !newTemplate.TemplateFields.Select(nf => nf.ID).Contains(of.ID));
            context.TemplateFields.RemoveRange(toBeRemovedFields);
            #endregion
            #region update fields:
            var toBeUpdatedFields = oldTemplate.TemplateFields.Where(of => newTemplate.TemplateFields.Select(nf => nf.ID).Contains(of.ID));
            foreach (var oldField in toBeUpdatedFields)
            {
                var newField = newTemplate.TemplateFields.Single(f => f.ID == oldField.ID);
                oldField.Name = newField.Name;
                oldField.DisplayName = newField.DisplayName;
                oldField.X = newField.X;
                oldField.Y = newField.Y;
                oldField.FontFamily = newField.FontFamily;
                oldField.FontSize = newField.FontSize;
                oldField.Bold = newField.Bold;
                oldField.FlowDirection = newField.FlowDirection;
                oldField.BoxWidth = newField.BoxWidth;
                oldField.BoxHeight = newField.BoxHeight;
                oldField.HorizontalContentAlignment = newField.HorizontalContentAlignment;
                oldField.VerticalContentAlignment = newField.VerticalContentAlignment;
                oldField.WrapContent = newField.WrapContent;
                oldField.TextColor = newField.TextColor;
                oldField.Description = newField.Description;
            }
            #endregion
            #region add newly created fields:
            foreach (var newField in newTemplate.TemplateFields.Where(f => f.ID <= 0))
            {
                oldTemplate.TemplateFields.Add(newField);
            }
            #endregion
            #endregion
        }
        #endregion
    }
}
