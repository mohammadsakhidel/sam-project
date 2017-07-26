using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamUtils.Enums
{
    [Flags]
    public enum RoleType
    {
        admin = 1,
        customer = 2,
        oprator = 4
    }
}
