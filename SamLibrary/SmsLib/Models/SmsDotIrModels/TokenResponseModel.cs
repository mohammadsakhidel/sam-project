using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmsLib.Models.SmsDotIrModels
{
    public class TokenResponseModel : ApiResponseBase
    {
        public string TokenKey { get; set; }
    }
}
