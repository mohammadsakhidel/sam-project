using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamModels.Entities
{
    public class ObitBanner : Banner
    {
        public int? ObitID { get; set; }

        #region Navigation Props:
        public virtual Obit Obit { get; set; }
        #endregion
    }
}
