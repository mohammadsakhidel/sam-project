using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamModels.Entities
{
    public class MosqueBanner : Banner
    {
        public int? MosqueID { get; set; }

        #region Navigation Props:
        public virtual Mosque Mosque { get; set; }
        #endregion
    }
}
