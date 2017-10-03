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

        public static string GenerateConsolationTrackingNumber()
        {
            var randomNumber = TextUtils.GetRandomNumbers(10, 100, 1).First();
            var timeStr = DateTimeUtils.Now.ToString("fffmmHHddss");
            return $"{randomNumber.ToString("D2")}{timeStr}";
        }

        public static string GenerateObitTrackingNumber()
        {
            var randomNumber = TextUtils.GetRandomNumbers(10, 100, 1).First();
            var timeStr = DateTimeUtils.Now.ToString("fffHHmmddss");
            return $"{randomNumber.ToString("D2")}{timeStr}";
        }
    }
}
