using SamDataAccess.Contexts;
using SamModels.Entities;
using System.Collections.Generic;

namespace SamDataAccess.Repos.Interfaces
{
    public interface ITemplateCategoryRepo : IRepo<SamDbContext, TemplateCategory>
    {
        void UpdateWithSave(TemplateCategory newCategory);

        List<TemplateCategory> GetAll(bool onlyActives);
    }
}