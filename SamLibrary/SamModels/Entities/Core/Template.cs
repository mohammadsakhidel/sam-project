using SamModels.Entities.Blobs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamModels.Entities.Core
{
    public class Template
    {
        public int ID { get; set; }

        [Required]
        [MaxLength(32)]
        public string Name { get; set; }

        [Required]
        public int Order { get; set; }

        [Required]
        public int TemplateCategoryID { get; set; }

        [Required]
        [MaxLength(32)]
        public string BackgroundImageID { get; set; }

        /// <summary>
        /// This field is a desciptive text for background image and extra fields that
        /// will be used in searches and text messages but not in displayed consolation.
        /// The extra fields should be embeded via {fieldName} syntax.
        /// </summary>
        [Required]
        [MaxLength(1024)]
        public string Text { get; set; }

        [Required]
        public double Price { get; set; }

        [Required]
        public int WidthRatio { get; set; }

        [Required]
        public int HeightRatio { get; set; }

        [Required]
        public bool IsActive { get; set; }

        [Required]
        public DateTime CreationTime { get; set; }

        [Required]
        [MaxLength(32)]
        public string Creator { get; set; }

        public DateTime? LastUpdateTime { get; set; }

        #region Navigation Props:
        public virtual ICollection<TemplateField> TemplateFields { get; set; }
        public virtual TemplateCategory Category { get; set; }

        [ForeignKey("BackgroundImageID")]
        public virtual Blob BackgroundImage { get; set; }
        #endregion
    }
}
