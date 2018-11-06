using System;
using System.Collections.Generic;
using System.Configuration;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Telegram.Bot;
using Telegram.Bot.Types;
using System.Net.Http.Formatting;
using SamModels.DTOs;

namespace SamAPI.Code.Telegram
{
    public class TelegramClient : ITelegramClient
    {
        public TelegramClient()
        {
        }

        public async Task SendGifAsync(string chatId, byte[] gifBytes, string caption)
        {
            using (var httpClient = new HttpClient()) {

                httpClient.BaseAddress = new Uri(ConfigurationManager.AppSettings["TelegramApiAddress"]);
                var dto = new SendGifDto {
                    ChatId = chatId,
                    Caption = caption,
                    GifBase64 = Convert.ToBase64String(gifBytes)
                };
                await httpClient.PostAsJsonAsync<object>("sendgif", dto);

            };
        }
    }
}