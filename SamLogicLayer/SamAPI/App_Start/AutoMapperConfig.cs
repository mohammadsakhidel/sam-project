using AutoMapper;
using SamModels.DTOs;
using SamModels.Entities.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SamAPI.App_Start
{
    public class AutoMapperConfig
    {
        public static void InitializeMapper()
        {
            Mapper.Initialize(cfg =>
            {
                #region Mosque:
                cfg.CreateMap<Mosque, MosqueDto>();
                cfg.CreateMap<MosqueDto, Mosque>();
                #endregion

                #region Template:
                cfg.CreateMap<Template, TemplateDto>()
                    .ForMember(dest => dest.TemplateCategoryName, opt => opt.MapFrom(src => src.Category.Name));
                cfg.CreateMap<TemplateDto, Template>();
                #endregion

                #region TemplateField:
                cfg.CreateMap<TemplateFieldDto, TemplateField>();
                cfg.CreateMap<TemplateField, TemplateFieldDto>();
                #endregion

                #region TemplateCategory:
                cfg.CreateMap<TemplateCategory, TemplateCategoryDto>();
                cfg.CreateMap<TemplateCategoryDto, TemplateCategory>();
                #endregion

                #region Obit:
                cfg.CreateMap<Obit, ObitDto>();
                cfg.CreateMap<ObitDto, Obit>();
                #endregion

                #region ObitHolding:
                cfg.CreateMap<ObitHolding, ObitHoldingDto>()
                   .ForMember(dest => dest.Obit, opt => opt.Ignore());
                cfg.CreateMap<ObitHoldingDto, ObitHolding>();
                #endregion
            });
        }
    }
}