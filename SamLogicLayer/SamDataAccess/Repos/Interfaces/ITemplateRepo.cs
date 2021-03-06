using SamDataAccess.Contexts;
using SamModels.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamDataAccess.Repos.Interfaces
{
    public interface ITemplateRepo : IRepo<SamDbContext, Template>
    {
        #region Extensions:
        void AddWithSave(Template template, ImageBlob backgroundImage);
        void UpdateWithSave(Template template, ImageBlob backgroundImage);
        bool HasAnyConsolations(int id);
        List<Template> GetAll(bool onlyActives);
        #endregion
    }
}
