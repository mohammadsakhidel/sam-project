using SamUxLib.Code.Enums;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transitionals;

namespace SamUxLib.Code.Objects
{
    public class Slide
    {
        public Bitmap Image { get; set; }
        public SlideType Type { get; set; }
        public int DurationSeconds { get; set; }
        public Transition InTransition { get; set; }
        public object DataObject { get; set; }
    }
}
