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

                #region Payment Reverse Checker Task:
                var reverseTimer = new Timer(ReversePaymentCallback, null, 20000, REVERSE_PAYMENT_INTERVAL);

                _timers = new List<Timer>();
                _timers.Add(reverseTimer);
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
                }
                #endregion
            }
            catch (Exception ex)
            {
                ExceptionManager.Handle(ex, logger, "REVERSE_PAYMENT");
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
