using AutoMapper;
using ClientModels.Models;
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
        ClientSetting setting = null;
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
                    setting = srepo.Get();
                    var isValidSetting = ClientSetting.IsSettingValid(setting);
                    if (isValidSetting)
                    {
                        allowStart = true;
                    }
                }
                #endregion

                #region Start Timers:
                if (allowStart)
                {
                    _timers = new List<Timer>();

                    #region Create Timers:
                    var downloadTimer = new Timer(DownloadTimerCallback, null, setting.DownloadDelayMilliSeconds, setting.DownloadIntervalMilliSeconds);
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
                    var url = $"{ApiActions.sync_getupdates}?mosqueid={setting.MosqueID}&saloonid={setting.SaloonID}{$"&lastupdate={(setting.LastUpdateTime.HasValue ? setting.LastUpdateTime.Value.ToString(StringFormats.url_date_time) : "")}"}";
                    var response = await hc.GetAsync(url);
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
                        if (mosque != null)
                        {
                            mosqueRepo.AddOrUpdate(mosque);
                        }
                        #endregion
                        #region update obits:
                        if (dto.Obits != null && dto.Obits.Any())
                        {
                            foreach (var obitDto in dto.Obits)
                            {
                                var obit = Mapper.Map<ObitDto, Obit>(obitDto);
                                obitRepo.AddOrUpdate(obit);
                            }
                        }
                        #endregion
                        #region update blobs:
                        if (dto.ImageBlobs != null && dto.ImageBlobs.Any())
                        {
                            foreach (var imageBlobDto in dto.ImageBlobs)
                            {
                                var imageBlob = Mapper.Map<ImageBlobDto, ImageBlob>(imageBlobDto);
                                blobRepo.AddOrUpdateImage(imageBlob);
                            }
                        }
                        #endregion
                        #region update templates:
                        if (dto.Templates != null && dto.Templates.Any())
                        {
                            foreach (var templateDto in dto.Templates)
                            {
                                var template = Mapper.Map<TemplateDto, Template>(templateDto);
                                templateRepo.AddOrUpdate(template);
                            }
                        }
                        #endregion
                        #region update consolations:
                        if (dto.Consolations != null && dto.Consolations.Any())
                        {
                            foreach (var consolationDto in dto.Consolations)
                            {
                                var consolation = Mapper.Map<ConsolationDto, Consolation>(consolationDto);
                                consolationRepo.AddOrUpdate(consolation);
                            }
                        }
                        #endregion
                        #region update last update time:
                        setting = settingRepo.Get();
                        setting.LastUpdateTime = dto.QueryTime;
                        #endregion

                        #region Commit:
                        settingRepo.Save(); //because all repos user a same context.
                        scope.Complete();
                        #endregion

                        var hadUpdates = mosque != null || dto.Obits.Any() || dto.ImageBlobs.Any() || dto.Templates.Any() || dto.Consolations.Any();
                        if (hadUpdates)
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
