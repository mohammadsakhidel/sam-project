using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamUtils.Enums;
using SamUtils.Objects.Presenters;

namespace SamUtils.Constants
{
    public class Collections
    {
        public static Dictionary<string, string> ApiClients
        {
            get
            {
                var dic = new Dictionary<string, string>();
                dic.Add("sroSVqFq9Y", "rLVX7PCdTWnYBzuF5T9qrPCT9meRV2wL"); //Authorization: Basic c3JvU1ZxRnE5WTpyTFZYN1BDZFRXbllCenVGNVQ5cXJQQ1Q5bWVSVjJ3TA==
                return dic;
            }
        }
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
