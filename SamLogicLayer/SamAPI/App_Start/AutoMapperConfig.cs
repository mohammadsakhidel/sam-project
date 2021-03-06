using AutoMapper;
using SamDataAccess.IdentityModels;
using SamModels.DTOs;
using SamModels.Entities;
using SamModels.Enums;
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

                #region Saloon:
                cfg.CreateMap<Saloon, SaloonDto>()
                    .ForMember(dest => dest.Mosque, opt => opt.Ignore());
                cfg.CreateMap<SaloonDto, Saloon>();
                #endregion

                #region Template:
                cfg.CreateMap<Template, TemplateDto>();
                cfg.CreateMap<TemplateDto, Template>();
                #endregion

                #region TemplateField:
                cfg.CreateMap<TemplateFieldDto, TemplateField>();
                cfg.CreateMap<TemplateField, TemplateFieldDto>();
                #endregion

                #region ImageBlob:
                cfg.CreateMap<ImageBlob, ImageBlobDto>().AfterMap((mdl, dto) =>
                {
                    dto.BytesEncoded = Convert.ToBase64String(mdl.Bytes);
                    dto.ThumbImageBytesEncoded = Convert.ToBase64String(mdl.ThumbImageBytes);
                });
                cfg.CreateMap<ImageBlobDto, ImageBlob>().AfterMap((dto, mdl) =>
                {
                    mdl.Bytes = Convert.FromBase64String(dto.BytesEncoded);
                    mdl.ThumbImageBytes = Convert.FromBase64String(dto.ThumbImageBytesEncoded);
                });
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

                #region Customer:
                cfg.CreateMap<Customer, CustomerDto>();
                cfg.CreateMap<CustomerDto, Customer>();
                #endregion

                #region Consolation:
                cfg.CreateMap<Consolation, ConsolationDto>();
                cfg.CreateMap<ConsolationDto, Consolation>();
                #endregion

                #region Display:
                cfg.CreateMap<Display, DisplayDto>();
                cfg.CreateMap<DisplayDto, Display>();
                #endregion

                #region Banner:
                cfg.CreateMap<Banner, BannerHierarchyDto>();
                cfg.CreateMap<BannerHierarchyDto, Banner>();

                // global banner:
                cfg.CreateMap<GlobalBanner, BannerHierarchyDto>().AfterMap((model, dto) =>
                {
                    dto.Type = BannerType.global.ToString();
                });
                cfg.CreateMap<BannerHierarchyDto, GlobalBanner>();
                // area banner:
                cfg.CreateMap<AreaBanner, BannerHierarchyDto>().AfterMap((model, dto) =>
                {
                    dto.Type = BannerType.area.ToString();
                });
                cfg.CreateMap<BannerHierarchyDto, AreaBanner>();
                // obit banner:
                cfg.CreateMap<ObitBanner, BannerHierarchyDto>().AfterMap((model, dto) =>
                {
                    dto.Type = BannerType.obit.ToString();
                });
                cfg.CreateMap<BannerHierarchyDto, ObitBanner>();
                // mosque banner:
                cfg.CreateMap<MosqueBanner, BannerHierarchyDto>().AfterMap((model, dto) =>
                {
                    dto.Type = BannerType.mosque.ToString();
                });
                cfg.CreateMap<BannerHierarchyDto, MosqueBanner>();
                #endregion

                #region Display:
                cfg.CreateMap<RemovedEntity, RemovedEntityDto>();
                cfg.CreateMap<RemovedEntityDto, RemovedEntity>();
                #endregion

                #region Payment:
                cfg.CreateMap<Payment, PaymentDto>();
                cfg.CreateMap<PaymentDto, Payment>();
                #endregion

                #region AspNetUser:
                cfg.CreateMap<AspNetUser, IdentityUserDto>();
                cfg.CreateMap<IdentityUserDto, AspNetUser>();
                #endregion

                #region AspNetRole:
                cfg.CreateMap<AspNetRole, IdentityRoleDto>();
                cfg.CreateMap<IdentityRoleDto, AspNetRole>();
                #endregion
            });
        }
    }
}