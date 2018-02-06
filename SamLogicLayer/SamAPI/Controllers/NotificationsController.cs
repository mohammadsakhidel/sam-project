using SamAPI.Code.Telegram;
using SamAPI.Code.Utils;
using SamDataAccess.Repos.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.Practices.Unity;
using System.Drawing;
using RamancoLibrary.Utilities;
using System.Configuration;
using SamAPI.Resources;
using SamUtils.Enums;
using System.Text.RegularExpressions;
using SamUtils.Constants;

namespace SamAPI.Controllers
{
    public class NotificationsController : ApiController
    {
        #region Fields:
        ITelegramClient _telegramClient;
        IObitRepo _obitRepo;
        ISystemParameterRepo _systemParameterRepo;
        IDisplayRepo _displayRepo;
        IConsolationRepo _consolationRepo;
        IIdentityRepo _identityRepo;
        #endregion

        #region Ctors:
        public NotificationsController(ITelegramClient telegramClient, IObitRepo obitRepo,
            ISystemParameterRepo systemParameterRepo, IDisplayRepo displayRepo, 
            IConsolationRepo consolationRepo, IIdentityRepo identityRepo)
        {
            _telegramClient = telegramClient;
            _obitRepo = obitRepo;
            _systemParameterRepo = systemParameterRepo;
            _displayRepo = displayRepo;
            _consolationRepo = consolationRepo;
            _identityRepo = identityRepo;
        }
        #endregion

        #region POST METHODS:
        [HttpPost]
        public async System.Threading.Tasks.Task<IHttpActionResult> SendObitGifs()
        {
            try
            {
                #region find completed obits:
                var sysParameters = _systemParameterRepo.Get();
                var obits = _obitRepo.GetCompletedObitsWhichHaveConsolations(sysParameters != null ? sysParameters.LastGifCheckDate : null, 2);
                #endregion

                #region generate gifs then send:
                foreach (var obit in obits)
                {
                    #region get git from obits controller:
                    byte[] gifBytes = null;
                    var obitsController = UnityConfig.Container.Resolve<ObitsController>();
                    var response = await obitsController.GetGif(obit.ID);
                    if (response.IsSuccessStatusCode)
                    {
                        gifBytes = await response.Content.ReadAsByteArrayAsync();
                    }
                    #endregion

                    #region send to telegram channel:
                    if (gifBytes != null)
                    {
                        var obitHoldingDateShamsi = DateTimeUtils.ToShamsi(obit.ObitHoldings.OrderByDescending(h => h.EndTime).First().BeginTime);
                        var obitHoldingDateString = $"{obitHoldingDateShamsi.DayName} {obitHoldingDateShamsi.Day} {obitHoldingDateShamsi.MonthName} {obitHoldingDateShamsi.Year}";
                        var channelId = ConfigurationManager.AppSettings["TelegramChannelID"];
                        var verified = PaymentStatus.verified.ToString();
                        var confirmed = ConsolationStatus.confirmed.ToString();
                        var displayed = ConsolationStatus.displayed.ToString();
                        var consolationsCount = obit.Consolations.Where(c => c.PaymentStatus == verified && (c.Status == confirmed || c.Status == displayed)).Count();

                        var caption = SmsMessages.TelegramGifsHashtag + Environment.NewLine + Environment.NewLine +
                            $"🔶 {SmsMessages.TelegramObitTitle}: {obit.Title}" + Environment.NewLine +
                            $"🔶 {SmsMessages.TelegramHoldingTime}: {obitHoldingDateString}" + Environment.NewLine +
                            $"🔶 {SmsMessages.TelegramObitPlace}: {obit.Mosque.Name}" + Environment.NewLine +
                            $"🔶 {SmsMessages.TelegramMessagesCount}: {consolationsCount}" + Environment.NewLine + Environment.NewLine +
                            @"@samsys_ir";

                        await _telegramClient.SendGifAsync(channelId, gifBytes, TextUtils.ToArabicDigits(caption));
                    }
                    #endregion
                }
                #endregion

                #region update last gif check time:
                sysParameters.LastGifCheckDate = DateTime.Now;
                _systemParameterRepo.Save();
                #endregion

                return Ok();
            }
            catch (Exception ex)
            {
                return ResponseMessage(ExceptionManager.GetExceptionResponse(this, ex));
            }
        }

        [HttpPost]
        public IHttpActionResult SendDisplayReports()
        {
            try
            {
                #region find completed obits:
                var sysParameters = _systemParameterRepo.Get();
                var obits = _obitRepo.GetCompletedObitsWhichHaveDisplayedConsolations(sysParameters != null ? sysParameters.LastDisplayReportDate : null);
                #endregion

                #region send sms messages:
                var verified = PaymentStatus.verified.ToString();
                var displayed = ConsolationStatus.displayed.ToString();

                foreach (var obit in obits)
                {
                    var consolations = obit.Consolations.Where(c => c.PaymentStatus == verified && c.Status == displayed);
                    if (consolations.Any())
                    {
                        var displayCounts = _displayRepo.GetDisplayCounts(consolations.Select(c => c.ID).ToArray());
                        foreach (var consolation in consolations)
                        {
                            #region Send SMS:
                            if (Regex.IsMatch(consolation.Customer.CellPhoneNumber, Patterns.cellphone))
                            {
                                var messageText = String.Format(SmsMessages.DisplayReportSms,
                                    obit.Title,
                                    TextUtils.ToArabicDigits(displayCounts[consolation.ID].ToString()),
                                    consolation.TrackingNumber);
                                SmsUtil.Send(messageText, consolation.Customer.CellPhoneNumber);
                            }
                            #endregion
                        }
                    }
                }
                #endregion

                #region update last gif check time:
                sysParameters.LastDisplayReportDate = DateTime.Now;
                _systemParameterRepo.Save();
                #endregion

                return Ok();
            }
            catch (Exception ex)
            {
                return ResponseMessage(ExceptionManager.GetExceptionResponse(this, ex));
            }
        }

        [HttpPost]
        public IHttpActionResult NotifyOperators()
        {
            try
            {
                #region check for pending consolations:
                var notifiableCount = _consolationRepo.GetNotifiablePendingsCount();
                #endregion

                if (notifiableCount > 0)
                {
                    #region find associated users:
                    var oprator = RoleType.oprator.ToString();
                    var rm = _identityRepo.GetRoleManager();
                    var um = _identityRepo.GetUserManager();

                    var operatorRoles = rm.Roles
                        .Where(r => r.Type == oprator)
                        .ToList();
                    var operators = _identityRepo.GetUsersInRole(operatorRoles.Select(r => r.Name).ToArray());
                    #endregion

                    #region send message:
                    if (operators.Any())
                    {
                        foreach (var op in operators)
                        {
                            #region Send SMS:
                            if (Regex.IsMatch(op.PhoneNumber, Patterns.cellphone))
                            {
                                string messageText = string.Format(SmsMessages.OperatorNotificationMessage, TextUtils.ToArabicDigits(notifiableCount.ToString()));
                                SmsUtil.Send(messageText, op.PhoneNumber);
                            }
                            #endregion
                        }
                    }
                    #endregion
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return ResponseMessage(ExceptionManager.GetExceptionResponse(this, ex));
            }
        }
        #endregion
    }
}
