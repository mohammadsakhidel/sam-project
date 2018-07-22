using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamClientDataAccess.ClientModels
{
    public class LocalConsolation
    {
        #region Props:
        public int ID { get; set; }

        [Required]
        public int ObitID { get; set; }

        [Required]
        [MaxLength(16)]
        public string Status { get; set; }

        [Required]
        [MaxLength(16)]
        public string PaymentStatus { get; set; }

        [Required]
        public DateTime CreationTime { get; set; }

        public DateTime? LastUpdateTime { get; set; }

        [Required]
        [MaxLength(16)]
        public string TrackingNumber { get; set; }

        [MaxLength(512)]
        public string ExtraData { get; set; }        

        public byte[] ImageBytes { get; set; }

        [MaxLength(128)]
        public string OtherObits { get; set; }
        #endregion

        #region Navigation Props:
        [ForeignKey("ObitID")]
        public virtual LocalObit Obit { get; set; }
        #endregion
    }
}
