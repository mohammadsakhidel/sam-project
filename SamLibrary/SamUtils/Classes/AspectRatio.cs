using System;

namespace SamUtils.Classes
{
    public class AspectRatio
    {
        public int WidthRatio { get; set; }
        public int HeightRatio { get; set; }
        public AspectRatioOrientation Orientation { get; set; }

        public override string ToString()
        {
            return $"{Orientation.ToString()}, {(Math.Max(WidthRatio, HeightRatio)).ToString()}:{(Math.Min(WidthRatio, HeightRatio)).ToString()}";
        }
    }

    public enum AspectRatioOrientation
    {
        Landscape = 1,
        Portrait = 2
    }
}
