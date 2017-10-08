using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamModels.Entities
{
    public class Payment
    {
        [Required]
        [MaxLength(32)]
        public string ID { get; set; }

        [Required]
        [MaxLength(1024)]
        public string Token { get; set; }

        [Required]
        public int Amount { get; set; }

        [Required]
        [MaxLength(32)]
        public string Provider { get; set; }

        [Required]
        [MaxLength(16)]
        public string Status { get; set; }

        [MaxLength(64)]
        public string ReferenceCode { get; set; }

        [Required]
        [MaxLength(16)]
        public string Type { get; set; }

        [Required]
        public DateTime CreationTime { get; set; }

        [Required]
        public DateTime LastUpdateTime { get; set; }
    }
}
