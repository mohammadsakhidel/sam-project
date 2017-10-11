using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SamUxLib.Code.Objects
{
    public class DisplayParams
    {
        public int X { get; set; } = 0;
        public int Y { get; set; } = 0;
        public int Width { get; set; } = 512;
        public int Height { get; set; } = 384;

        public static DisplayParams Parse(string paramsString)
        {
            var values = paramsString.Replace(" ", "").Split(',');
            return new DisplayParams() {
                X = Convert.ToInt32(values[0]),
                Y = Convert.ToInt32(values[1]),
                Width = Convert.ToInt32(values[2]),
                Height = Convert.ToInt32(values[3])
            };
        }
    }
}
