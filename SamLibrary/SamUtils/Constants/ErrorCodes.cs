using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamUtils.Constants
{
    public class ErrorCodes
    {
        // login errors:
        public const string invalid_username = "ER001";
        public const string invalid_password = "ER002";
        public const string account_blocked = "ER003";

        // accounts errors:
        public const string duplicate_username = "ER003";
        public const string sysadmin_username_changing = "ER004";
        public const string sysadmin_user_delete = "ER005";

    }
}
