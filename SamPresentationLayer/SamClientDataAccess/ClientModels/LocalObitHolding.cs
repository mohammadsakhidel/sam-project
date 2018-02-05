using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamClientDataAccess.ClientModels
{
    public class LocalObitHolding
    {
        #region Props;
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
        #endregion

        #region Navigation Props:
        [ForeignKey("ObitID")]
        public virtual LocalObit Obit { get; set; }
        #endregion
    }
}
