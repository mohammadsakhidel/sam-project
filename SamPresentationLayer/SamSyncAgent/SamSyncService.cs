using AutoMapper;
using ClientModels.Models;
using RamancoLibrary.Utilities;
using SamClientDataAccess.ClientModels;
using SamClientDataAccess.Repos;
using SamModels.DTOs;
using SamModels.Entities;
using SamModels.Enums;
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
                Mapper.Initialize(MappingUtil.ClientsConfiguration);
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
                    using (var mosqueRepo = new MosqueRepo(settingRepo.Context))
                    using (var obitRepo = new ObitRepo(settingRepo.Context))
                    using (var taskRepo = new DownloadImageTaskRepo(settingRepo.Context))
                    using (var consolationRepo = new ConsolationRepo(settingRepo.Context))
                    using (var bannerRepo = new BannerRepo(settingRepo.Context))
                    {
                        #region update mosque:
                        if (dto.Mosque != null)
                        {
                            var mosque = Mapper.Map<MosqueDto, Mosque>(dto.Mosque);
                            mosqueRepo.AddOrUpdate(mosque);
                        }
                        #endregion
                        #region update obits:
                        if (dto.Obits != null && dto.Obits.Any())
                        {
                            foreach (var obitDto in dto.Obits)
                            {
                                var obit = Mapper.Map<ObitDto, Obit>(obitDto, opts =>
                                {
                                    opts.AfterMap((src, dest) =>
                                    {
                                        dest.Mosque = null;
                                    });
                                });
                                obitRepo.AddOrUpdate(obit);
                            }
                        }
                        #endregion
                        #region update blobs:
                        if (dto.ImageBlobs != null && dto.ImageBlobs.Any())
                        {
                            foreach (var imageBlob in dto.ImageBlobs)
                            {
                                var downloadTask = new DownloadImageTask()
                                {
                                    ImageToDownload = imageBlob,
                                    CreationTime = DateTimeUtils.Now,
                                    Status = DownloadTaskStatus.pending.ToString(),
                                    Type = DownloadTaskType.blob.ToString()
                                };
                                taskRepo.Add(downloadTask);
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

                                var downloadTask = new DownloadImageTask()
                                {
                                    ImageToDownload = consolationDto.ID.ToString(),
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
                                var banner = (Banner)Mapper.Map(hierarchy, typeof(BannerHierarchyDto), hierarchy.GetEntityType());
                                bannerRepo.AddOrUpdate(banner);
                            }
                        }
                        #endregion
                        #region delete removed entities:
                        if (dto.RemovedEntities != null && dto.RemovedEntities.Any())
                        {
                            foreach (var removedEntity in dto.RemovedEntities)
                            {
                                #region remove banner:
                                if (removedEntity.EntityType == typeof(Banner).Name)
                                {
                                    var bannerId = Convert.ToInt32(removedEntity.EntityID);
                                    if (bannerRepo.Exists(bannerId))
                                        bannerRepo.Remove(bannerId);
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

                        var hadUpdates = dto.Mosque != null || !CollUtils.IsNullOrEmpty(dto.Obits) || !CollUtils.IsNullOrEmpty(dto.ImageBlobs)
                            || !CollUtils.IsNullOrEmpty(dto.Templates) || !CollUtils.IsNullOrEmpty(dto.Consolations)
                            || !CollUtils.IsNullOrEmpty(dto.Banners) || !CollUtils.IsNullOrEmpty(dto.RemovedEntities);
                        if (hadUpdates)
                            Log($"Update Done.{Environment.NewLine}{(dto.Mosque != null ? "1" : "0")} Mosque, {CollUtils.Count(dto.Obits)} Obits, " +
                                $"{CollUtils.Count(dto.ImageBlobs)} ImageBlob Download Tasks, {CollUtils.Count(dto.Consolations)} Consolations, " +
                                $"{CollUtils.Count(dto.Banners)} Banners & {dto.RemovedEntities?.Count()} Removed Entities Updated." +
                                $"{Environment.NewLine}Last Update Time became {dto.QueryTime.ToString("HH:mm:ss yyyy-MM-dd")}.");
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
                using (var drepo = new DisplayRepo(srepo.Context))
                using (var hc = HttpUtil.CreateClient())
                {
                    #region get displays to upload:
                    var setting = srepo.Get();
                    var uploadTime = DateTimeUtils.Now;
                    var displays = drepo.GetPendingDisplays(setting.LastDisplaysUploadTime, uploadTime);
                    var dtos = displays.Select(d => Mapper.Map<Display, DisplayDto>(d)).ToArray();
                    #endregion

                    #region upload:
                    var response = await hc.PostAsJsonAsync<DisplayDto[]>(ApiActions.sync_updatedisplays, dtos);
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
                                    imageBytes = hc.GetByteArrayAsync($"{ApiActions.consolations_getpreview}/{task.ImageToDownload}").Result;
                                }
                                else
                                {
                                    imageBytes = hc.GetByteArrayAsync($"{ApiActions.blobs_getimage}/{task.ImageToDownload}").Result;
                                }
                                #endregion
                                #region save image to database and change task status:
                                if (imageBytes != null && imageBytes.Any())
                                {
                                    using (var ts = new TransactionScope())
                                    using (var brepo = new BlobRepo())
                                    using (var crepo = new ConsolationImageRepo())
                                    {
                                        #region add or update consolation image:
                                        if (task.Type == DownloadTaskType.consolation.ToString())
                                        {
                                            var cImage = new ConsolationImage()
                                            {
                                                ConsolationID = Convert.ToInt32(task.ImageToDownload),
                                                Bytes = imageBytes,
                                                CreationTime = DateTime.Now,
                                                LastUpdateTime = DateTime.Now
                                            };
                                            crepo.AddOrUpdate(cImage);
                                            crepo.Save();
                                        }
                                        #endregion
                                        #region add or update image blob:
                                        else
                                        {
                                            var blob = new ImageBlob()
                                            {
                                                ID = task.ImageToDownload,
                                                Bytes = imageBytes,
                                                CreationTime = DateTimeUtils.Now,
                                                LastUpdateTime = DateTimeUtils.Now,
                                            };

                                            brepo.AddOrUpdateImage(blob);
                                            brepo.Save();
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
                using (var crepo = new ConsolationRepo())
                using (var brepo = new BannerRepo())
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
        #endregion
    }
}