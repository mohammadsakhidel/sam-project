using Microsoft.Practices.Unity;
using SamDataAccess.Contexts;
using SamDataAccess.Repos;
using SamDataAccess.Repos.BaseClasses;
using SamDataAccess.Repos.Interfaces;
using System.Web.Http;
using Unity.WebApi;

namespace SamAPI
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();

            container.RegisterType<IMosqueRepo, MosqueRepo>();
            container.RegisterType<ITemplateRepo, TemplateRepo>();
            container.RegisterType<ITemplateCategoryRepo, TemplateCategoryRepo>();

            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}