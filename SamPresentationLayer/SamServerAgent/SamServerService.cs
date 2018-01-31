using SamServerAgent.Code.Utils;
using SamUtils.Constants;
using SamUtils.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SamServerAgent
{
    partial class SamServerService : ServiceBase
    {
        #region Constants:
        const int REVERSE_PAYMENT_INTERVAL = 20000;
        const int NOTIFY_OPERATOR_INTERVAL = 240000;
        const int TELEGRAM_GIFS_INTERVAL = 300000;
        #endregion

        #region CTORS:
        public SamServerService()
        {
            InitializeComponent();
        }
        #endregion

        #region Fields:
        List<Timer> _timers;
        #endregion

        #region START - STOP:
        protected override void OnStart(string[] args)
        {
            try
            {
                #region Init EventLog:
                var logName = "SamServerAgentLogs";
                var sourceName = "SamServerAgent";
                logger = new System.Diagnostics.EventLog();
                if (!System.Diagnostics.EventLog.SourceExists(sourceName))
                {
                    System.Diagnostics.EventLog.CreateEventSource(sourceName, logName);
                }
                logger.Source = sourceName;
                logger.Log = logName;
                #endregion

                #region Init Timers:
                var reverseTimer = new Timer(ReversePaymentCallback, null, 20000, REVERSE_PAYMENT_INTERVAL);
                var notifyTimer = new Timer(NotifyOperatorsCallback, null, 30000, NOTIFY_OPERATOR_INTERVAL);
                var telegramGifsTimer = new Timer(SendGifsToTelegramCallback, null, 10000, TELEGRAM_GIFS_INTERVAL);

                _timers = new List<Timer>();
                _timers.Add(reverseTimer);
                _timers.Add(notifyTimer);
                _timers.Add(telegramGifsTimer);
                #endregion

                Log("Sam Server Agent Started!");
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

                Log("Sam Server Agent Stopped!");
            }
            catch (Exception ex)
            {
                ExceptionManager.Handle(ex, logger);
            }
        }
        #endregion

        #region Timer Callbacks:
        private void ReversePaymentCallback(object stat)
        {
            try
            {
                #region Call Api To Reverse Needed Payments:
                using (var hc = HttpUtil.CreateClient())
                {
                    var response = hc.PostAsync(ApiActions.payment_reverse, null).Result;
                    response.EnsureSuccessStatusCode();
                    var count = Convert.ToInt32(response.Content.ReadAsStringAsync().Result);
                    if (count > 0)
                    {
                        Log($"{count} Payment{(count > 1 ? "s" : "")} Reversed!");
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                ExceptionManager.Handle(ex, logger, "REVERSE_PAYMENT");
            }
        }
        private void NotifyOperatorsCallback(object stat)
        {
            try
            {
                #region Call Api:
                using (var hc = HttpUtil.CreateClient())
                {
                    var response = hc.PostAsync(ApiActions.consolations_notify, null).Result;
                    response.EnsureSuccessStatusCode();
                }
                #endregion
            }
            catch (Exception ex)
            {
                ExceptionManager.Handle(ex, logger, "NOTIFY_OPERATOR");
            }
        }
        private void SendGifsToTelegramCallback(object stat)
        {
            try
            {
                #region Call Api:
                using (var hc = HttpUtil.CreateClient())
                {
                    var response = hc.PostAsync(ApiActions.notifications_sendobitgifs, null).Result;
                    response.EnsureSuccessStatusCode();
                }
                #endregion
            }
            catch (Exception ex)
            {
                ExceptionManager.Handle(ex, logger, "TELEGRAM_GIFS");
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
