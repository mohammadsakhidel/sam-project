using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamModels.DTOs
{
    public class TemplateExtraFieldDto
    {
        public int ID { get; set; }

        public int TemplateID { get; set; }

        public string Name { get; set; }

        public string DisplayName { get; set; }

        public double X { get; set; }

        public double Y { get; set; }

        public string FontFamily { get; set; }

        public string FontSize { get; set; }

        public bool? Bold { get; set; }

        public string TextColor { get; set; }

        public string FlowDirection { get; set; }

        public double BoxWidth { get; set; }

        public double BoxHeight { get; set; }

        public string HorizontalContentAlignment { get; set; }

        public string VerticalContentAlignment { get; set; }

        public bool? WrapContent { get; set; }
    }
}
