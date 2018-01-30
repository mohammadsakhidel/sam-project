using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamReportLib.Queries
{
    public class MosquesTurnoverQuery
    {
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }
        public int? ProvinceID { get; set; }
        public int? CityID { get; set; }
        public int? MosqueID { get; set; }
    }
}
