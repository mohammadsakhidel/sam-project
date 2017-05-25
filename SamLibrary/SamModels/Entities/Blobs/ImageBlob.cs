using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamModels.Entities.Blobs
{
    public class ImageBlob : Blob
    {
        [MaxLength(8)]
        public string ImageFormat { get; set; }

        public int? ImageWidth { get; set; }

        public int? ImageHeight { get; set; }
    }
}
