using AutoMapper;
using RamancoLibrary.Utilities;
using SamClientDataAccess.ClientModels;
using SamClientDataAccess.Repos;
using SamModels.DTOs;
using SamModels.Enums;
using SamSyncAgent.Code.Structs;
using SamSyncAgent.Code.Utils;
using SamUtils.Constants;
using SamUtils.Enums;
using SamUtils.Utils;
using SamUxLib.Code.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
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
        #endregion

        #region Constants:
        int DISPLAY_TIMER_DELAY_MILLS = 30000;
        int DISPLAY_TIMER_INTERVAL_MILLS = 60000;
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
                Mapper.Initialize(GetMappingConfig());
                #endregion

                #region Set Date Time From Server If Available:
                Task.Run(() =>
                {
                    try
                    {
                        SetDateTimeFromServer();
                        Log($"Date & time successfully set from server.");
                    }
                    catch (Exception ex)
                    {
                        Log($"SET SERVER TIME ERROR: {(ex.InnerException != null ? ex.InnerException.Message : ex.Message)}");
                    }
                });
                #endregion

                #region Check Starting Prereqs:
                var allowStart = false;
                ClientSetting setting;
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

                    #region Download Timer:
                    var downloadTimer = new Timer(DownloadTimerCallback, null, setting.DownloadDelayMilliSeconds, setting.DownloadIntervalMilliSeconds);
                    _timers.Add(downloadTimer);
                    #endregion

                    #region Update Displays Timer:
                    var displayTimer = new Timer(DisplayTimerCallback, null, DISPLAY_TIMER_DELAY_MILLS, DISPLAY_TIMER_INTERVAL_MILLS);
                    _timers.Add(displayTimer);
                    #endregion

                    #region Images Download Task:
                    Task.Run(() =>
                    {
                        while (true)
                        {
                            DownloadImagesCallback();
                        }
                    });
                    #endregion

                    #region Remove Obsolete Items Task:
                    Task.Run(() =>
                    {
                        while (true)
                        {
                            RemoveObsoleteItemsCallback();
                        }
                    });
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
                using (var settingRepo = new ClientSettingRepo())
                using (var hc = HttpUtil.CreateClient())
                {
                    var setting = settingRepo.Get();
                    var url = $"{ApiActions.sync_getupdates}?mosqueid={setting.MosqueID}&saloonid={setting.SaloonID}{$"&lastupdate={(setting.LastUpdateTime.HasValue ? setting.LastUpdateTime.Value.ToString(StringFormats.url_date_time) : "")}"}";
                    var response = await hc.GetAsync(url);
                    response.EnsureSuccessStatusCode();
                    var dto = await response.Content.ReadAsAsync<ConsolationsUpdatePackDto>();

                    #region update data:
                    using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                    using (var obitRepo = new LocalObitRepo(settingRepo.Context))
                    using (var taskRepo = new DownloadImageTaskRepo(settingRepo.Context))
                    using (var consolationRepo = new LocalConsolationRepo(settingRepo.Context))
                    using (var bannerRepo = new LocalBannerRepo(settingRepo.Context))
                    {
                        #region update obits:
                        if (dto.Obits != null && dto.Obits.Any())
                        {
                            foreach (var obitDto in dto.Obits)
                            {
                                var obit = Mapper.Map<ObitDto, LocalObit>(obitDto);
                                obitRepo.AddOrUpdate(obit);
                            }
                        }
                        #endregion
                        #region update consolations:
                        if (dto.Consolations != null && dto.Consolations.Any())
                        {
                            foreach (var consolationDto in dto.Consolations)
                            {
                                var consolation = Mapper.Map<ConsolationDto, LocalConsolation>(consolationDto);
                                consolationRepo.AddOrUpdate(consolation);

                                var downloadTask = new DownloadImageTask()
                                {
                                    AssociatedObjectID = consolation.ID,
                                    DownloadData = "",
                                    CreationTime = DateTimeUtils.Now,
                                    Status = DownloadTaskStatus.pending.ToString(),
                                    Type = DownloadTaskType.consolation.ToString()
                                };
                                taskRepo.Add(downloadTask);
                            }
                        }
                        #endregion
                        #region update banners:
                        if (dto.Banners != null && dto.Banners.Any())
                        {
                            foreach (var hierarchy in dto.Banners)
                            {
                                var banner = Mapper.Map<BannerHierarchyDto, LocalBanner>(hierarchy);
                                bannerRepo.AddOrUpdate(banner);

                                var downloadTask = new DownloadImageTask()
                                {
                                    AssociatedObjectID = banner.ID,
                                    DownloadData = hierarchy.ImageID,
                                    CreationTime = DateTimeUtils.Now,
                                    Status = DownloadTaskStatus.pending.ToString(),
                                    Type = DownloadTaskType.banner.ToString()
                                };
                                taskRepo.Add(downloadTask);
                            }
                        }
                        #endregion
                        #region delete removed entities:
                        if (dto.RemovedEntities != null && dto.RemovedEntities.Any())
                        {
                            foreach (var removedEntity in dto.RemovedEntities)
                            {
                                #region remove banner:
                                if (removedEntity.EntityType == "Banner")
                                {
                                    var bannerId = Convert.ToInt32(removedEntity.EntityID);
                                    if (bannerRepo.Exists(bannerId))
                                        bannerRepo.Remove(bannerId);
                                }
                                else if (removedEntity.EntityType == "Obit")
                                {
                                    var obitId = Convert.ToInt32(removedEntity.EntityID);
                                    if (obitRepo.Exists(obitId))
                                        obitRepo.Remove(obitId);
                                }
                                #endregion
                            }
                        }
                        #endregion
                        #region update last update time:
                        setting = settingRepo.Get();
                        setting.LastUpdateTime = dto.QueryTime;
                        #endregion

                        #region Commit:
                        settingRepo.Save(); // all repos share a same context
                        scope.Complete();
                        #endregion

                        #region log success message:
                        var hadUpdates = !CollUtils.IsNullOrEmpty(dto.Obits)
                            || !CollUtils.IsNullOrEmpty(dto.Consolations)
                            || !CollUtils.IsNullOrEmpty(dto.Banners)
                            || !CollUtils.IsNullOrEmpty(dto.RemovedEntities);

                        if (hadUpdates)
                            Log($"Update Done.{Environment.NewLine}" +
                                $"{CollUtils.Count(dto.Obits)} Obits, " +
                                $"{CollUtils.Count(dto.Consolations)} Consolations, " +
                                $"{CollUtils.Count(dto.Banners)} Banners & " +
                                $"{CollUtils.Count(dto.RemovedEntities)} Removed Entities Updated.{Environment.NewLine}" +
                                $"Last Update Time became {dto.QueryTime.ToString("HH:mm:ss yyyy-MM-dd")}.");
                        #endregion
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                ExceptionManager.Handle(ex, logger, "DOWNLOAD ERROR");
            }
        }
        private async void DisplayTimerCallback(object state)
        {
            try
            {
                using (var srepo = new ClientSettingRepo())
                using (var drepo = new LocalDisplayRepo(srepo.Context))
                using (var hc = HttpUtil.CreateClient())
                {
                    #region get displays to upload:
                    var setting = srepo.Get();
                    var uploadTime = DateTimeUtils.Now;
                    var displays = drepo.GetPendingDisplays(setting.LastDisplaysUploadTime, uploadTime);
                    var dtos = displays.Select(d => Mapper.Map<LocalDisplay, DisplayDto>(d)).ToArray();
                    #endregion

                    #region upload:
                    var response = await hc.PostAsJsonAsync(ApiActions.sync_updatedisplays, dtos);
                    response.EnsureSuccessStatusCode();
                    #endregion

                    #region update display status to synced:
                    setting.LastDisplaysUploadTime = uploadTime;

                    foreach (var display in displays)
                    {
                        display.SyncStatus = DisplaySyncStatus.synced.ToString();
                    }

                    srepo.Save();
                    #endregion
                }
            }
            catch (Exception ex)
            {
                ExceptionManager.Handle(ex, logger, "DISPLAY UPLOAD ERROR");
            }
        }
        private void DownloadImagesCallback()
        {
            try
            {
                using (var trepo = new DownloadImageTaskRepo())
                {
                    var pendingTasks = trepo.GetTasksOfStatus(DownloadTaskStatus.pending.ToString());
                    if (pendingTasks != null && pendingTasks.Any())
                    {
                        using (var hc = HttpUtil.CreateClient())
                        {
                            foreach (var task in pendingTasks)
                            {
                                #region download image bytes:
                                byte[] imageBytes;
                                if (task.Type == DownloadTaskType.consolation.ToString())
                                {
                                    imageBytes = hc.GetByteArrayAsync($"{ApiActions.consolations_getpreview}/{task.AssociatedObjectID}").Result;
                                }
                                else
                                {
                                    imageBytes = hc.GetByteArrayAsync($"{ApiActions.blobs_getimage}/{task.DownloadData}").Result;
                                }
                                #endregion
                                #region save image to database and change task status:
                                if (imageBytes != null && imageBytes.Any())
                                {
                                    using (var ts = new TransactionScope())
                                    using (var brepo = new LocalBannerRepo())
                                    using (var crepo = new LocalConsolationRepo())
                                    {
                                        #region update consolation image bytes:
                                        if (task.Type == DownloadTaskType.consolation.ToString())
                                        {
                                            var consolation = crepo.Get(task.AssociatedObjectID);
                                            if (consolation != null)
                                            {
                                                consolation.ImageBytes = imageBytes;
                                                crepo.Save();
                                            }
                                        }
                                        #endregion
                                        #region update banner image bytes:
                                        else if (task.Type == DownloadTaskType.banner.ToString())
                                        {
                                            var banner = brepo.Get(task.AssociatedObjectID);
                                            if (banner != null)
                                            {
                                                banner.ImageBytes = imageBytes;
                                                brepo.Save();
                                            }
                                        }
                                        #endregion
                                        #region update task status:
                                        task.Status = DownloadTaskStatus.completed.ToString();
                                        task.DownloadCompletiontime = DateTimeUtils.Now;
                                        trepo.Save();
                                        #endregion

                                        ts.Complete();
                                    }
                                }
                                #endregion
                            }
                        }

                        Log($"{pendingTasks.Count} Images downloaded successfully.");
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionManager.Handle(ex, logger, "IMAGE DOWNLOAD ERROR");
            }
            finally
            {
                Thread.Sleep(2500);
            }
        }
        private void RemoveObsoleteItemsCallback()
        {
            try
            {
                using (var crepo = new LocalConsolationRepo())
                using (var brepo = new LocalBannerRepo())
                {
                    var obsoleteConsolations = crepo.GetObsoleteItems();
                    var obsoleteBanners = brepo.GetObsoleteItems();

                    #region remove banners:
                    if (obsoleteBanners.Any())
                    {
                        foreach (var banner in obsoleteBanners)
                        {
                            brepo.Remove(banner);
                        }
                        brepo.Save();
                    }
                    #endregion
                    #region remove consolations:
                    if (obsoleteConsolations.Any())
                    {
                        foreach (var c in obsoleteConsolations)
                        {
                            crepo.Remove(c);
                        }
                        crepo.Save();
                    }
                    #endregion

                    if (obsoleteBanners.Any() || obsoleteConsolations.Any())
                        Log($"{obsoleteBanners.Count} obsolete banners & {obsoleteConsolations.Count} obsolete consolations removed successfully.");
                }
            }
            catch (Exception ex)
            {
                ExceptionManager.Handle(ex, logger, "OBSOLETE ITEMS REMOVER ERROR");
            }
            finally
            {
                Thread.Sleep(2500);
            }
        }
        #endregion

        #region Methods:
        private void Log(string message)
        {
            logger.WriteEntry(message);
        }
        private void SetDateTimeFromServer()
        {
            using (var hc = HttpUtil.CreateClient())
            {
                var serverDateTimeStr = hc.GetStringAsync(ApiActions.sync_getserverdatetime).Result;
                var serverDateTime = DateTime.ParseExact(serverDateTimeStr, StringFormats.datetime_long, CultureInfo.InvariantCulture);
                var sysTime = new SYSTEMTIME()
                {
                    wYear = (short)serverDateTime.Year,
                    wMonth = (short)serverDateTime.Month,
                    wDay = (short)serverDateTime.Day,
                    wDayOfWeek = (short)serverDateTime.DayOfWeek,
                    wHour = (short)serverDateTime.Hour,
                    wMinute = (short)serverDateTime.Minute,
                    wSecond = (short)serverDateTime.Second,
                    wMilliseconds = (short)serverDateTime.Millisecond
                };
                SetSystemTime(ref sysTime);
            }
        }
        private Action<IMapperConfigurationExpression> GetMappingConfig()
        {
            return cfg =>
            {
                #region Obit:
                cfg.CreateMap<ObitDto, LocalObit>();
                #endregion

                #region ObitHolding:
                cfg.CreateMap<ObitHoldingDto, LocalObitHolding>();
                #endregion

                #region Consolation:
                cfg.CreateMap<ConsolationDto, LocalConsolation>();
                #endregion

                #region Display:
                cfg.CreateMap<LocalDisplay, DisplayDto>();
                #endregion

                #region Banner:
                cfg.CreateMap<BannerHierarchyDto, LocalBanner>().AfterMap((dto, mdl) =>
                {
                    if (!string.IsNullOrEmpty(dto.ImageBase64))
                    {
                        mdl.ImageBytes = Convert.FromBase64String(dto.ImageBase64);
                    }
                });
                #endregion
            };
        }
        #endregion

        #region External Methods:
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetSystemTime(ref SYSTEMTIME st);
        #endregion
    }
}