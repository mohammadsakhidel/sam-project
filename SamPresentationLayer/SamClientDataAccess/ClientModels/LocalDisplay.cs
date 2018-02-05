using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamClientDataAccess.ClientModels
{
    public class LocalDisplay
    {
        #region Props:
        public int ID { get; set; }

        public int ConsolationID { get; set; }

        [Required]
        public DateTime TimeOfDisplay { get; set; }

        [Required]
        public int DurationMilliSeconds { get; set; }

        [MaxLength(16)]
        public string SyncStatus { get; set; }

        [Required]
        public DateTime CreationTime { get; set; }
        #endregion
    }
}
