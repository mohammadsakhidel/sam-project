using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SamAPI.Code.Telegram
{
    public class TelegramClient : ITelegramClient
    {
        #region Fields:
        string _token;
        TelegramBotClient _botClient;
        #endregion

        public TelegramClient()
        {
            _token = ConfigurationManager.AppSettings["TelegramToken"];
            _botClient = new TelegramBotClient(_token);
        }

        public async Task SendGifAsync(string chatId, byte[] gifBytes, string caption)
        {
            using (var ms = new MemoryStream(gifBytes))
            {
                var randomFileName = RamancoLibrary.Utilities.TextUtils.GetRandomString(8, true);
                var fileToSend = new FileToSend($"{randomFileName}.gif", ms);
                await _botClient.SendDocumentAsync(chatId, fileToSend, caption: caption);
            }
        }
    }
}