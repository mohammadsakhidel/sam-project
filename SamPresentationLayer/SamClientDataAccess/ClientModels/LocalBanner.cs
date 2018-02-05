using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamClientDataAccess.ClientModels
{
    public class LocalBanner
    {
        #region Porps:
        public int ID { get; set; }

        [Required]
        [MaxLength(64)]
        public string Title { get; set; }

        public byte[] ImageBytes { get; set; }

        public DateTime? LifeBeginTime { get; set; }

        public DateTime? LifeEndTime { get; set; }

        [Required]
        public bool IsActive { get; set; }

        [Required]
        public int Priority { get; set; }

        [Required]
        public bool ShowOnStart { get; set; }

        [Required]
        public int DurationSeconds { get; set; }

        [Required]
        public int Interval { get; set; }

        [Required]
        public DateTime CreationTime { get; set; }

        [Required]
        public DateTime LastUpdateTime { get; set; }
        #endregion

        #region Banner inheritance props:
        [Required]
        [MaxLength(16)]
        public string Type { get; set; }

        public int? ObitID { get; set; }
        #endregion
    }
}
