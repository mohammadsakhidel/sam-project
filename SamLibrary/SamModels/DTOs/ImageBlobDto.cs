using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamModels.DTOs
{
    public class ImageBlobDto
    {
        public string ID { get; set; }

        public string BytesEncoded { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime? LastUpdateTime { get; set; }

        public string ThumbImageBytesEncoded { get; set; }

        public string ImageFormat { get; set; }

        public int? ImageWidth { get; set; }

        public int? ImageHeight { get; set; }
    }
}
