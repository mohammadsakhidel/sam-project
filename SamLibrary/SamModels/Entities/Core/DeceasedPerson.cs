using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamModels.Entities.Core
{
    public class DeceasedPerson
    {
        public int ID { get; set; }

        [Required]
        public int ObitID { get; set; }

        [Required]
        [MaxLength(32)]
        public string Name { get; set; }

        [Required]
        [MaxLength(32)]
        public string Surname { get; set; }

        [Required]
        public bool Gender { get; set; }

        #region Navigation Props:
        public virtual Obit Obit { get; set; }
        #endregion
    }
}
