using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamClientDataAccess.ClientModels
{
    public class LocalObit
    {
        #region Props:
        public int ID { get; set; }

        [Required]
        [MaxLength(32)]
        public string Title { get; set; }

        [Required]
        public int MosqueID { get; set; }

        [Required]
        public DateTime CreationTime { get; set; }

        public DateTime? LastUpdateTime { get; set; }

        [Required]
        [MaxLength(16)]
        public string TrackingNumber { get; set; }
        #endregion

        #region Navigation Props:
        public virtual ICollection<LocalObitHolding> ObitHoldings { get; set; }
        #endregion
    }
}
