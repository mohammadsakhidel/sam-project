using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamModels.DTOs
{
    public class MosqueDto
    {
        #region Props:
        public int ID { get; set; }

        public string Name { get; set; }

        public string ImamName { get; set; }

        public string ImamCellPhone { get; set; }

        public string InterfaceName { get; set; }

        public string InterfaceCellPhone { get; set; }

        public int CityID { get; set; }

        public string Address { get; set; }

        public string Location { get; set; }

        public string PhoneNumber { get; set; }

        public DateTime CreationTime { get; set; }

        public string Creator { get; set; }
        #endregion

        #region Navigations:
        public List<SaloonDto> Saloons { get; set; }
        #endregion

        #region Overrides:
        public override string ToString()
        {
            return Name;
        }
        #endregion
    }
}
