using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamUtils.Utils
{
    public class EnumUtil
    {
        public TEnum GetEnum<TEnum>(string value)
        {
            return (TEnum)Enum.Parse(typeof(TEnum), value);
        }
    }
}
