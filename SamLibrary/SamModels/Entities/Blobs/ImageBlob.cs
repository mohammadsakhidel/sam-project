using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamModels.Entities.Blobs
{
    public class ImageBlob : Blob
    {
        public int? ImageWidth { get; set; }

        public int? ImageHeight { get; set; }
    }
}
