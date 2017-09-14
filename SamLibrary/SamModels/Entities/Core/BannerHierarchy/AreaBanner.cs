using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamModels.Entities.Core
{
    public class AreaBanner : Banner
    {
        public int? CityID { get; set; }
        public int? ProvinceID { get; set; }
    }
}
