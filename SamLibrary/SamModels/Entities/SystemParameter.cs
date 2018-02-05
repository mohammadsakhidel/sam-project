using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamModels.Entities
{
    public class SystemParameter
    {
        public int ID { get; set; } = 1;
        public DateTime? LastGifCheckDate { get; set; }
        public DateTime? LastDisplayReportDate { get; set; }
    }
}
