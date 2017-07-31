
using SamUtils.Enums;

namespace SamUtils.Objects.Presenters
{
    public class FontSizePresenter
    {
        public string DisplayName { get; set; }
        public FontSize Size { get; set; }

        public override string ToString()
        {
            return DisplayName;
        }
    }
}
