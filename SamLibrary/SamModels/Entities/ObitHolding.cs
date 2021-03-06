using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamModels.Entities
{
    public class ObitHolding
    {
        public int ID { get; set; }

        [Required]
        public int ObitID { get; set; }

        [Required]
        public DateTime BeginTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        [Required]
        [MaxLength(16)]
        public string SaloonID { get; set; }

        #region Navigation Props:
        public virtual Obit Obit { get; set; }
        #endregion
    }
}
