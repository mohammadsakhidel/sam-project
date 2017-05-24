using SamDataAccess.Contexts;
using SamModels.Entities.Core;

namespace SamDataAccess.Repos.Interfaces
{
    public interface ITemplateCategoryRepo : IRepo<SamDbContext, TemplateCategory>
    {
    }
}
