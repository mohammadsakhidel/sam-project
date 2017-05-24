using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamModels.Entities.Core
{
    public class TemplateExtraField
    {
        public int ID { get; set; }

        [Required]
        public int TemplateID { get; set; }

        [Required]
        [MaxLength(16)]
        public string Name { get; set; }

        [Required]
        [MaxLength(32)]
        public string DisplayName { get; set; }

        [Required]
        public double X { get; set; }

        [Required]
        public double Y { get; set; }

        [MaxLength(64)]
        public string FontFamily { get; set; }

        [MaxLength(10)]
        public string FontSize { get; set; }

        public bool? Bold { get; set; }

        [MaxLength(10)]
        public string TextColor { get; set; }

        [MaxLength(4)]
        public string FlowDirection { get; set; }

        [Required]
        public double BoxWidth { get; set; }

        [Required]
        public double BoxHeight { get; set; }

        [MaxLength(16)]
        public string HorizontalContentAlignment { get; set; }

        [MaxLength(16)]
        public string VerticalContentAlignment { get; set; }

        public bool? WrapContent { get; set; }

        #region Navigation Props:
        public virtual Template Template { get; set; }
        #endregion
    }
}
