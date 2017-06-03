using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamModels.DTOs
{
    public class TemplateCategoryDto
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public int Order { get; set; }

        public string Description { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
