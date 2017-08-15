using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamModels.DTOs
{
    public class DisplayDto
    {
        #region Props:
        public int ID { get; set; }

        public int ConsolationID { get; set; }

        public DateTime TimeOfDisplay { get; set; }

        public int DurationMilliSeconds { get; set; }

        public DateTime CreationTime { get; set; }
        #endregion
    }
}
