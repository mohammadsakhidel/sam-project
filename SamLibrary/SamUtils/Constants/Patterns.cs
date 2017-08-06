using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamUtils.Constants
{
    public class Patterns
    {
        public const string cellphone = "^0?9[0-9]{9}$";
        public const string phone_number = "^0?[1-9][0-9]{9}$";
        public const string ip = @"^(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$";
        public const string username = @"^[a-zA-Z0-9_\.@-]{6,}$";
        public const string rolename = @"^[a-zA-Z]{4,}$";
        public const string password = @"^.{6,}$";
    }
}
