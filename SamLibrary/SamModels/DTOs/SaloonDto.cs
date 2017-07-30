using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamModels.DTOs
{
    public class SaloonDto
    {
        #region Props:
        public int ID { get; set; }

        public int MosqueID { get; set; }

        public string Name { get; set; }

        public string EndpointIP { get; set; }
        #endregion

        #region Navigations:
        public MosqueDto Mosque { get; set; }
        #endregion
    }
}
