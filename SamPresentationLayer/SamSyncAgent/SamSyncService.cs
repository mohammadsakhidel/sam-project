using SamClient.Models.Repos;
using SamSyncAgent.Code.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

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

        #region CONSTS:
        const int SYNC_TIMER_INTERVAL = 30000;
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

                #region Check Starting Prereqs:
                var allowStart = true;
                using (var srepo = new ClientSettingRepo())
                {
                    var setting = srepo.Get(1);
                    if (setting == null || setting.MosqueID <= 0 || string.IsNullOrEmpty(setting.SaloonID))
                        allowStart = false;
                }
                #endregion

                #region Start:
                if (allowStart)
                {
                    var sync_timer = new Timer();
                    sync_timer.Elapsed += Sync_timer_Elapsed;
                    sync_timer.Enabled = true;
                    _timers.Add(sync_timer);

                    Log("Sam sync service stoped.");
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
                foreach (var timer in _timers)
                {
                    timer.Dispose();
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
        private void Sync_timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            throw new NotImplementedException();
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
