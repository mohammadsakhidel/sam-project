using SamModels.Entities.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientModels.Models
{
    public class ClientSetting
    {
        #region Props:
        public int ID { get; set; }

        [Required]
        public int MosqueID { get; set; }

        [Required]
        [MaxLength(16)]
        public string SaloonID { get; set; }

        [Required]
        public int DownloadIntervalMilliSeconds { get; set; }

        [Required]
        public int DownloadDelayMilliSeconds { get; set; }

        public DateTime? LastUpdateTime { get; set; }
        #endregion
    }
}