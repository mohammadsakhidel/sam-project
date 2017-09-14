using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamModels.Entities
{
    public class Display
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

        #region Navigations:
        public virtual Consolation Consolation { get; set; }
        #endregion
    }
}
