using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamModels.Entities
{
    public class TemplateCategory
    {
        public int ID { get; set; }

        [Required]
        [MaxLength(64)]
        public string Name { get; set; }

        [Required]
        public int Order { get; set; }

        [MaxLength(256)]
        public string Description { get; set; }

        [Required]
        public bool Visible { get; set; } = true;

        #region Navigation Props:
        public virtual ICollection<Template> Templates { get; set; }
        #endregion
    }
}
