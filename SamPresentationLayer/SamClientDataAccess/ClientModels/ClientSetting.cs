﻿using SamModels.Entities.Core;
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

        #region Consts:
        public const int MIN_DOWNLOAD_INTERVAL = 10000;
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

            return true;
        }
        #endregion
    }
}