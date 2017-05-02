using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamModels.Entities.Blobs
{
    public abstract class Blob
    {
        [MaxLength(32)]
        public string ID { get; set; }

        [Required]
        [MaxLength(64)]
        public string Name { get; set; }

        [Required]
        [MaxLength(8)]
        public string Extension { get; set; }

        [Required]
        public byte[] Bytes { get; set; }

        [Required]
        public DateTime CreationTime { get; set; }
    }
}
