using AutoMapper;
using SamModels.DTOs;
using SamModels.Entities.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using AutoMapper.Configuration;

namespace SamAPI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            #region AutoMapper Configs:
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<Mosque, MosqueDto>();
                cfg.CreateMap<Template, TemplateDto>().ForMember(dest => dest.TemplateCategoryName, opt => opt.MapFrom(src => src.Category.Name));
            });
            #endregion

            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
