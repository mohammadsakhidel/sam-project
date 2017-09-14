using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamModels.Entities.Core
{
    public class HoldingBanner : Banner
    {
        [Required]
        public int ObitHoldingID { get; set; }
    }
}
