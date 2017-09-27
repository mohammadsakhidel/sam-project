using AutoMapper;
using SamModels.DTOs;
using SamModels.Entities;
using SamModels.Enums;
using SamUtils.Utils;
using SamUxLib.Code.Utils;
using SamUxLib.Resources.Values;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace SamClient
{
    public partial class App : Application
    {
        #region Consts:
        const int PERSIAN_CULTURE_ID = 1065;
        const int ENGLISH_CULTURE_ID = 1033;
        #endregion

        #region OnStart:
        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                #region Culture Setting, Used by Persian Wpf Toolkit:
                Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(PERSIAN_CULTURE_ID);
                Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;
                #endregion

                InitializeCityUtil();
                InitializeMapper();

                base.OnStartup(e);
            }
            catch (Exception ex)
            {
                ExceptionManager.Handle(ex);
            }
        }
        #endregion

        #region Methods:
        private void InitializeMapper()
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
            });
        }
        private void InitializeCityUtil()
        {
            CityUtil.Func_GetXMLContent = () => {
                try
                {
                    using (var stream = typeof(Strings).Assembly.GetManifestResourceStream("SamUxLib.Resources.XML.ir-cities.xml"))
                    {
                        using (var sr = new StreamReader(stream))
                        {
                            return sr.ReadToEnd();
                        }
                    }
                }
                catch (Exception ex)
                {
                    ExceptionManager.Handle(ex);
                    return "";
                }
            };
        }
        #endregion
    }
}
