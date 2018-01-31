using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamAPI.Code.Telegram
{
    public interface ITelegramClient
    {
        Task SendGifAsync(string userName, byte[] gifBytes, string caption);
    }
}
