using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamModels.Entities
{
    public class Province
    {
        public int ID { get; set; }

        [Required]
        [MaxLength(32)]
        public string Name { get; set; }

        #region Navigation Props:
        public virtual ICollection<City> Cities { get; set; }
        #endregion
    }
}
