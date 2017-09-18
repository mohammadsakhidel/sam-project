using SamDataAccess.Contexts;
using SamDataAccess.Repos.BaseClasses;
using SamDataAccess.Repos.Interfaces;
using SamModels.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Transactions;
using SamModels.Entities;
using RamancoLibrary.Utilities;

namespace SamDataAccess.Repos
{
    public class TemplateRepo : Repo<SamDbContext, Template>, ITemplateRepo
    {
        #region Overrides:
        public override List<Template> GetAll()
        {
            return set.Include(t => t.TemplateFields).ToList();
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
        public void UpdateWithSave(Template newTemplate, ImageBlob backgroundImage)
        {
            using (var ts = new TransactionScope())
            {
                var oldTemplate = Get(newTemplate.ID);

                #region BackgroundImage:
                if (backgroundImage != null)
                {
                    #region Delete Old Background Image Blob:
                    var oldBlob = context.Blobs.Find(oldTemplate.BackgroundImageID);
                    if (oldBlob != null)
                        context.Blobs.Remove(oldBlob);
                    #endregion

                    #region Add New Blob:
                    context.Blobs.Add(backgroundImage);
                    #endregion

                    oldTemplate.BackgroundImageID = backgroundImage.ID;
                }
                #endregion

                #region Update Props:
                oldTemplate.TemplateCategoryID = newTemplate.TemplateCategoryID;
                oldTemplate.Text = newTemplate.Text;
                oldTemplate.Price = newTemplate.Price;
                oldTemplate.WidthRatio = newTemplate.WidthRatio;
                oldTemplate.HeightRatio = newTemplate.HeightRatio;
                oldTemplate.Name = newTemplate.Name;
                oldTemplate.IsActive = newTemplate.IsActive;
                oldTemplate.Order = newTemplate.Order;
                oldTemplate.LastUpdateTime = DateTimeUtils.Now;
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

                Save();
                ts.Complete();
            }
        }
        public bool HasAnyConsolations(int id)
        {
            return context.Consolations.Where(c => c.TemplateID == id).Any();
        }
        public List<Template> GetAll(bool onlyActives)
        {
            return set.Where(t => !onlyActives || t.IsActive).ToList();
        }
        #endregion
    }
}
