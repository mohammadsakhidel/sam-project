using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamModels.DTOs
{
    public class SendGifDto
    {
        public string ChatId { get; set; }
        public string GifBase64 { get; set; }
        public string Caption { get; set; }
    }
}
