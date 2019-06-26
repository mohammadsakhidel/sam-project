using SamModels.DTOs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SamSocialNetsAPI.Controllers
{
    public class TelegramController : ApiController
    {
        [HttpPost]
        public async Task<IHttpActionResult> SendGif([FromBody]SendGifDto dto)
        {
            try
            {
                var gifBytes = Convert.FromBase64String(dto.GifBase64);

                using (var ms = new MemoryStream(gifBytes))
                {
                    var randomFileName = RamancoLibrary.Utilities.TextUtils.GetRandomString(8, true);
                    var fileToSend = new FileToSend($"{randomFileName}.gif", ms);

                    var token = ConfigurationManager.AppSettings["TelegramToken"];
                    var botClient = new TelegramBotClient(token);
                    await botClient.SendDocumentAsync(dto.ChatId, fileToSend, caption: dto.Caption);

                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        public IHttpActionResult Test()
        {
            return Ok(123);
        }
    }
}
