using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamModels.Entities.Core
{
    public class Saloon
    {
        #region Props:
        [Required]
        [MaxLength(16)]
        [Key, Column(Order = 0)]
        public string ID { get; set; }

        [Required]
        [Key, Column(Order = 1)]
        public int MosqueID { get; set; }

        [Required]
        [MaxLength(32)]
        public string Name { get; set; }

        [MaxLength(16)]
        public string EndpointIP { get; set; }
        #endregion

        #region Navigations:
        public virtual Mosque Mosque { get; set; }
        #endregion
    }
}
