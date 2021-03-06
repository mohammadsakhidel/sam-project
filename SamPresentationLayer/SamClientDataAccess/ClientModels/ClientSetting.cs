using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamClientDataAccess.ClientModels
{
    public class ClientSetting
    {
        #region Props:
        public int ID { get; set; }

        [Required]
        public int CityID { get; set; }

        [Required]
        public int MosqueID { get; set; }

        [MaxLength(128)]
        public string MosqueName { get; set; }

        [MaxLength(512)]
        public string MosqueAddress { get; set; }

        [Required]
        [MaxLength(16)]
        public string SaloonID { get; set; }

        [Required]
        public int DownloadIntervalMilliSeconds { get; set; }

        [Required]
        public int DownloadDelayMilliSeconds { get; set; }

        [Required]
        public bool AutoSlideShow { get; set; }

        [Required]
        public int DefaultSlideDurationMilliSeconds { get; set; }

        public DateTime? LastUpdateTime { get; set; }

        public DateTime? LastDisplaysUploadTime { get; set; }
        #endregion

        #region Consts:
        public const int MIN_DOWNLOAD_INTERVAL = 10000;
        public const int MIN_SLIDE_DURATION_MILLS = 8000;
        #endregion

        #region Static Methods:
        public static bool IsSettingValid(ClientSetting setting)
        {
            if (setting == null)
                return false;

            if (setting.MosqueID <= 0)
                return false;

            if (string.IsNullOrEmpty(setting.SaloonID))
                return false;

            if (setting.DownloadIntervalMilliSeconds < MIN_DOWNLOAD_INTERVAL)
                return false;

            if (setting.DefaultSlideDurationMilliSeconds < MIN_SLIDE_DURATION_MILLS)
                return false;

            return true;
        }
        #endregion
    }
}