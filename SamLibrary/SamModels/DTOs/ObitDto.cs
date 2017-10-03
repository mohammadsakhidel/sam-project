using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamModels.DTOs
{
    public class ObitDto
    {
        #region Main Props:
        public int ID { get; set; }

        public string Title { get; set; }

        public string DeceasedIdentifier { get; set; }

        public string ObitType { get; set; }

        public int MosqueID { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime? LastUpdateTime { get; set; }

        public string OwnerCellPhone { get; set; }

        public string TrackingNumber { get; set; }
        #endregion

        #region Navigation Props:
        public MosqueDto Mosque { get; set; }
        public List<ObitHoldingDto> ObitHoldings { get; set; }
        #endregion

        #region Extensions:
        public string ObitTypeDisplay { get; set; }
        #endregion
    }
}