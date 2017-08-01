using System;

namespace SamUtils.Objects.Presenters
{
    public class AspectRatio
    {
        public string ID
        {
            get
            {
                return $"{WidthRatio}:{HeightRatio}";
            }
        }
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
