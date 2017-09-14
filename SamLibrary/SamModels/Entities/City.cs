using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamModels.Entities
{
    public class City
    {
        public int ID { get; set; }

        [Required]
        public int ProvinceID { get; set; }

        [Required]
        [MaxLength(32)]
        public string Name { get; set; }

        #region Navigation Props:
        public virtual Province Province { get; set; }
        #endregion
    }
}
