using AutoMapper;
using SamClientDataAccess.Repos;
using SamModels.DTOs;
using SamModels.Entities.Blobs;
using SamModels.Entities.Core;
using SamSyncAgent.Code.Utils;
using SamUtils.Constants;
using SamUtils.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace SamSyncAgent
{
    partial class SamSyncService : ServiceBase
    {
        #region CTORS:
        public SamSyncService()
        {
            InitializeComponent();
        }
        #endregion

        #region VARS:
        List<Timer> _timers;
        int _mosqueId = 0;
        #endregion

        #region CONSTS:
        const int DOWNLOAD_TIMER_INTERVAL = 300000;
        const int DOWNLOAD_TIMER_DELAY = 10000;
        #endregion

        #region START & STOP:
        protected override void OnStart(string[] args)
        {
            try
            {
                #region Init EventLog:
                var logName = "SamLogs";
                var sourceName = "SamClientSyncService";
                logger = new System.Diagnostics.EventLog();
                if (!System.Diagnostics.EventLog.SourceExists(sourceName))
                {
                    System.Diagnostics.EventLog.CreateEventSource(sourceName, logName);
                }
                logger.Source = sourceName;
                logger.Log = logName;
                #endregion

                #region AutoMapper Config:
                InitializeMapper();
                #endregion

                #region Check Starting Prereqs:
                var allowStart = false;
                using (var srepo = new ClientSettingRepo())
                {
                    var setting = srepo.Get();
                    if (setting != null && setting.MosqueID > 0 && !string.IsNullOrEmpty(setting.SaloonID))
                    {
                        allowStart = true;
                        _mosqueId = setting.MosqueID;
                    }
                }
                #endregion

                #region Start:
                if (allowStart)
                {
                    _timers = new List<Timer>();

                    #region Create Timers:
                    var downloadTimer = new Timer(DownloadTimerCallback, null, DOWNLOAD_TIMER_DELAY, DOWNLOAD_TIMER_INTERVAL);
                    _timers.Add(downloadTimer);
                    #endregion

                    Log("Sam sync timers started.");
                }
                else
                {
                    Log("Starting timers failed, please check client settings existance then restart the service.");
                }
                #endregion
            }
            catch (Exception ex)
            {
                ExceptionManager.Handle(ex, logger);
            }
        }
        protected override void OnStop()
        {
            try
            {
                if (_timers != null && _timers.Any())
                {
                    foreach (var timer in _timers)
                    {
                        timer.Dispose();
                    }
                }

                Log("Sam sync service stopped.");
            }
            catch (Exception ex)
            {
                ExceptionManager.Handle(ex, logger);
            }
        }
        #endregion

        #region TIMER Elapsed Handlers:
        private async void DownloadTimerCallback(object state)
        {
            try
            {
                using (var hc = HttpUtil.CreateClient())
                {
                    var response = await hc.GetAsync($"{ApiActions.sync_getupdates}?mosqueid={_mosqueId}");
                    response.EnsureSuccessStatusCode();
                    var dto = await response.Content.ReadAsAsync<ConsolationsUpdatePackDto>();

                    #region update data:
                    using (var scope = new TransactionScope())
                    using (var settingRepo = new ClientSettingRepo())
                    using (var mosqueRepo = new MosqueRepo(settingRepo.Context))
                    using (var obitRepo = new ObitRepo(settingRepo.Context))
                    using (var templateRepo = new TemplateRepo(settingRepo.Context))
                    using (var blobRepo = new BlobRepo(settingRepo.Context))
                    using (var consolationRepo = new ConsolationRepo(settingRepo.Context))
                    {
                        #region update mosque:
                        var mosque = Mapper.Map<MosqueDto, Mosque>(dto.Mosque);
                        mosqueRepo.AddOrUpdate(mosque);
                        #endregion
                        #region update obits:
                        foreach (var obitDto in dto.Obits)
                        {
                            var obit = Mapper.Map<ObitDto, Obit>(obitDto);
                            obitRepo.AddOrUpdate(obit);
                        }
                        #endregion
                        #region update blobs:
                        foreach (var imageBlobDto in dto.ImageBlobs)
                        {
                            var imageBlob = Mapper.Map<ImageBlobDto, ImageBlob>(imageBlobDto);
                            blobRepo.AddOrUpdateImage(imageBlob);
                        }
                        #endregion
                        #region update templates:
                        foreach (var templateDto in dto.Templates)
                        {
                            var template = Mapper.Map<TemplateDto, Template>(templateDto);
                            templateRepo.AddOrUpdate(template);
                        }
                        #endregion
                        #region update consolations:
                        foreach (var consolationDto in dto.Consolations)
                        {
                            var consolation = Mapper.Map<ConsolationDto, Consolation>(consolationDto);
                            consolationRepo.AddOrUpdate(consolation);
                        }
                        #endregion
                        #region update last update time:
                        var setting = settingRepo.Get();
                        setting.LastUpdateTime = dto.QueryTime;
                        settingRepo.Save();
                        #endregion

                        #region Commit:
                        settingRepo.Save(); //because all repos user a same context.
                        scope.Complete();
                        #endregion

                        Log($"Update Done.{Environment.NewLine}{(mosque != null ? "1" : "0")} Mosque, {dto.Obits.Count()} Obits, {dto.ImageBlobs.Count()} ImageBlobs, {dto.Templates.Count()} Templates & {dto.Consolations.Count()} Consolations Updated.{Environment.NewLine}Lat Update Time became {dto.QueryTime.ToString("HH:mm:ss yyyy-MM-dd")}.");
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                ExceptionManager.Handle(ex, logger, "DOWNLOAD ERROR");
            }
        }
        #endregion

        #region Methods:
        private void Log(string message)
        {
            logger.WriteEntry(message);
        }
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
            });
        }
        #endregion
    }
}
