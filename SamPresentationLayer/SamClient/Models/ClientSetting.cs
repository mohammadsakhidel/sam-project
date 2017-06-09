using SamModels.Entities.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamClient.Models
{
    public class ClientSetting
    {
        public int ID { get; set; }

        [Required]
        public int MosqueID { get; set; }

        public DateTime? LastUpdateTime { get; set; }

        #region Navigation:
        public virtual Mosque Mosque { get; set; }
        #endregion
    }
}