using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamModels.DTOs
{
    public class TemplateDto
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public int Order { get; set; }

        public int TemplateCategoryID { get; set; }

        public string TemplateCategoryName { get; set; }

        public string BackgroundImageID { get; set; }

        public string BackgroundImageBase64 { get; set; }

        /// <summary>
        /// This field is a desciptive text for background image and extra fields that
        /// will be used in searches and text messages but not in displayed consolation.
        /// The extra fields should be embeded via {fieldName} syntax.
        /// </summary>
        public string Text { get; set; }

        public double Price { get; set; }

        public int WidthRatio { get; set; }

        public int HeightRatio { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreationTime { get; set; }

        public string Creator { get; set; }

        public TemplateFieldDto[] TemplateFields { get; set; }

        public TemplateCategoryDto Category { get; set; }
    }
}
