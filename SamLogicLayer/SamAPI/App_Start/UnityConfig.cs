using Microsoft.Practices.Unity;
using SamAPI.Code.Payment;
using SamAPI.Code.Telegram;
using SamDataAccess.Contexts;
using SamDataAccess.Repos;
using SamDataAccess.Repos.BaseClasses;
using SamDataAccess.Repos.Interfaces;
using SmsLib.Objects;
using System.Web.Http;
using Unity.WebApi;

namespace SamAPI
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
            Container = new UnityContainer();

            Container.RegisterType<IMosqueRepo, MosqueRepo>();
            Container.RegisterType<ITemplateRepo, TemplateRepo>();
            Container.RegisterType<ITemplateCategoryRepo, TemplateCategoryRepo>();
            Container.RegisterType<IBlobRepo, BlobRepo>();
            Container.RegisterType<IObitRepo, ObitRepo>();
            Container.RegisterType<IConsolationRepo, ConsolationRepo>();
            Container.RegisterType<ICustomerRepo, CustomerRepo>();
            Container.RegisterType<IDisplayRepo, DisplayRepo>();
            Container.RegisterType<IIdentityRepo, IdentityRepo>();
            Container.RegisterType<IBannerRepo, BannerRepo>();
            Container.RegisterType<IRemovedEntityRepo, RemovedEntityRepo>();
            Container.RegisterType<IPaymentRepo, PaymentRepo>();
            Container.RegisterType<ISystemParameterRepo, SystemParameterRepo>();
            Container.RegisterType<IPaymentService, MabnaPaymentService>();
            Container.RegisterType<ISmsManager, SmsDotIrSmsManager>();
            Container.RegisterType<ITelegramClient, TelegramClient>();

            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(Container);
        }
        public static UnityContainer Container { get; private set; }
    }
}