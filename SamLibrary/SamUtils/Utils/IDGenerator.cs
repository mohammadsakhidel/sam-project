using RamancoLibrary.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamUtils.Utils
{
    public class IDGenerator
    {
        public static string GenerateImageID()
        {
            return $"img_{RamancoLibrary.Utilities.TextUtils.GetRandomString(12, true)}";
        }

        public static string GenerateTrackingNumber()
        {
            var randomNumber = TextUtils.GetRandomNumbers(0, 10, 1).First();
            var timeStr = DateTimeUtils.Now.ToString("fffmmHHss");
            return $"{timeStr}{randomNumber.ToString("D1")}";
        }
    }
}
