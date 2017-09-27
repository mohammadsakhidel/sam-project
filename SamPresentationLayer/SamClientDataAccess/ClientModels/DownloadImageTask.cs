using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamClientDataAccess.ClientModels
{
    public class DownloadImageTask
    {
        public int ID { get; set; }

        [Required]
        [MaxLength(32)]
        public string ImageToDownload { get; set; }

        [Required]
        [MaxLength(16)]
        public string Status { get; set; }

        [Required]
        public DateTime CreationTime { get; set; }

        public DateTime? DownloadCompletiontime { get; set; }
    }
}
