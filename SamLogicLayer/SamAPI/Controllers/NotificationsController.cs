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

namespace SamAPI.Controllers
{
    public class NotificationsController : ApiController
    {
        #region Fields:
        ITelegramClient _telegramClient;
        IObitRepo _obitRepo;
        ISystemParameterRepo _systemParameterRepo;
        #endregion

        #region Ctors:
        public NotificationsController(ITelegramClient telegramClient, IObitRepo obitRepo,
            ISystemParameterRepo systemParameterRepo)
        {
            _telegramClient = telegramClient;
            _obitRepo = obitRepo;
            _systemParameterRepo = systemParameterRepo;
        }
        #endregion

        #region POST METHODS:
        [HttpPost]
        public async System.Threading.Tasks.Task<IHttpActionResult> SendObitGifs()
        {
            try
            {
                #region find completed obits:
                var obits = _obitRepo.GetCompletedObitsWhichHaveConsolations();
                #endregion

                #region generate gifs then send:
                foreach (var obit in obits)
                {
                    #region generate conoslation bitmaps:
                    var consolations = obit.Consolations.ToList();
                    var consolationBitmaps = new List<Bitmap>();
                    foreach (var c in consolations)
                    {
                        var consolationsController = UnityConfig.Container.Resolve<ConsolationsController>();
                        var response = consolationsController.GetPreview(c.ID, thumb: false);
                        if (response.IsSuccessStatusCode)
                        {
                            var stream = await response.Content.ReadAsStreamAsync();
                            var bitmap = new Bitmap(stream);
                            consolationBitmaps.Add(bitmap);
                        }
                    }
                    #endregion

                    #region generate obit gif:
                    var gifWidth = consolationBitmaps.Select(b => b.Width).Max();
                    var gifHeight = consolationBitmaps.Select(b => b.Height).Max();
                    var gifBytes = ImageUtils.GenerateGif(consolationBitmaps.ToArray(), gifWidth, gifHeight, delayInSeconds: 5);
                    #endregion

                    #region send to telegram channel:
                    var obitHoldingDateShamsi = DateTimeUtils.ToShamsi(obit.ObitHoldings.OrderByDescending(h => h.EndTime).First().BeginTime);
                    var obitHoldingDateString = $"{obitHoldingDateShamsi.DayName} {obitHoldingDateShamsi.Day} {obitHoldingDateShamsi.MonthName} {obitHoldingDateShamsi.Year}";
                    var channelId = ConfigurationManager.AppSettings["TelegramChannelID"];
                    var caption = SmsMessages.TelegramGifsHashtag + Environment.NewLine + Environment.NewLine +
                        $"🔶 {SmsMessages.TelegramObitTitle}: {obit.Title}" + Environment.NewLine +
                        $"🔶 {SmsMessages.TelegramHoldingTime}: {obitHoldingDateString}" + Environment.NewLine +
                        $"🔶 {SmsMessages.TelegramObitPlace}: {obit.Mosque.Name}" + Environment.NewLine +
                        $"🔶 {SmsMessages.TelegramMessagesCount}: {obit.Consolations.Count()}" + Environment.NewLine + Environment.NewLine +
                        @"@samsys_ir";

                    await _telegramClient.SendGifAsync(channelId, gifBytes, TextUtils.ToArabicDigits(caption));
                    #endregion
                }
                #endregion

                #region update last gif check time:
                var sysParameters = _systemParameterRepo.Get();
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
        #endregion
    }
}
