
using SamUtils.Enums;

namespace SamUtils.Classes
{
    public class TextAlignmentPresenter
    {
        public string DisplayText { get; set; }
        public TextAlignment Alignment { get; set; }

        public override string ToString()
        {
            return DisplayText;
        }
    }
}
