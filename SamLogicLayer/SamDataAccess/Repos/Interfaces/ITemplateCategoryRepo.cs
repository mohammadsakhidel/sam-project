using SamDataAccess.Contexts;
using SamModels.Entities.Core;
using System.Collections.Generic;

namespace SamDataAccess.Repos.Interfaces
{
    public interface ITemplateCategoryRepo : IRepo<SamDbContext, TemplateCategory>
    {
        void UpdateWithSave(TemplateCategory newCategory);

        List<TemplateCategory> GetAll(bool onlyActives);
    }
}