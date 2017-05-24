using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamUtils.Enums;
using SamUtils.Classes;

namespace SamUtils.Constants
{
    public class Collections
    {
        public static List<AspectRatio> AspectRatios
        {
            get
            {
                return new List<AspectRatio> {
                    new AspectRatio { Orientation = AspectRatioOrientation.Landscape, WidthRatio = 16, HeightRatio = 9 },
                    new AspectRatio { Orientation = AspectRatioOrientation.Landscape, WidthRatio = 4, HeightRatio = 3 },
                    new AspectRatio { Orientation = AspectRatioOrientation.Portrait, WidthRatio = 9, HeightRatio = 16 },
                    new AspectRatio { Orientation = AspectRatioOrientation.Portrait, WidthRatio = 3, HeightRatio = 4 },
                };
            }
        }
    }
}
