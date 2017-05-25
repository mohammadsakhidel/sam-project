using System;

namespace SamUtils.Classes
{
    public class AspectRatio
    {
        public int WidthRatio { get; set; }
        public int HeightRatio { get; set; }
        public AspectRatioOrientation Orientation { get; set; }
        public string DisplayName
        {
            get
            {
                return $"{Orientation.ToString()}, {(Math.Max(WidthRatio, HeightRatio)).ToString()}:{(Math.Min(WidthRatio, HeightRatio)).ToString()}";
            }
        }

        public override string ToString()
        {
            return DisplayName;
        }
    }

    public enum AspectRatioOrientation
    {
        Landscape = 1,
        Portrait = 2
    }
}
