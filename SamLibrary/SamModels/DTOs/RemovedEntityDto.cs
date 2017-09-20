using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamModels.DTOs
{
    public class RemovedEntityDto
    {
        public int ID { get; set; }

        public string EntityType { get; set; }

        public DateTime RemovingTime { get; set; }

        public string metadata { get; set; }
    }
}
