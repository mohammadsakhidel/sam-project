using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamReportLib.Models
{
    public class MosqueTurnoverRecord
    {
        public int Index { get; set; }
        public string MosqueName { get; set; }
        public int ObitsCount { get; set; }
        public int ConsolationCount { get; set; }
        public double ConsolationAverage { get; set; }
        public int TotalIncome { get; set; }
    }
}
