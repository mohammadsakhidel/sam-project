using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamModels.Entities.Core
{
    public class Saloon
    {
        #region Props:
        public int ID { get; set; }

        [Required]
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
