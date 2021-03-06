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
    public class TemplateCategoryRepo : Repo<SamDbContext, TemplateCategory>, ITemplateCategoryRepo
    {
        public List<TemplateCategory> GetAll(bool onlyActives)
        {
            return set.Where(c => !onlyActives || c.Visible).ToList();
        }

        public void UpdateWithSave(TemplateCategory newCategory)
        {
            var categoryToUpdate = Get(newCategory.ID);
            if (categoryToUpdate != null)
            {
                categoryToUpdate.Name = newCategory.Name;
                categoryToUpdate.Description  = newCategory.Description;
                categoryToUpdate.Order = newCategory.Order;
                categoryToUpdate.Visible = newCategory.Visible;

                Save();
            }
        }
    }
}
