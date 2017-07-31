using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamModels.DTOs
{
    public class ObitHoldingDto
    {
        #region Main Props:
        public int ID { get; set; }

        public int ObitID { get; set; }

        public DateTime BeginTime { get; set; }

        public DateTime EndTime { get; set; }

        public string SaloonID { get; set; }

        public string SaloonName { get; set; }
        #endregion

        #region Navigation Props:
        public ObitDto Obit { get; set; }
        #endregion
    }
}
