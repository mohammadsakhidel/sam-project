using SamUxLib.Resources.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamUtils.Enums;
using SamUtils.Objects.Presenters;
using SamUxLib.Code.Enums;

namespace SamUxLib.Code.Constants
{
    public class Collections
    {
        public static List<FontSizePresenter> FontSizes
        {
            get
            {
                return new List<FontSizePresenter>() {
                    new FontSizePresenter { Size = FontSize.tiny, DisplayName = Strings.Tiny },
                    new FontSizePresenter { Size = FontSize.small, DisplayName = Strings.Small },
                    new FontSizePresenter { Size = FontSize.normal, DisplayName = Strings.Normal },
                    new FontSizePresenter { Size = FontSize.large, DisplayName = Strings.Large },
                    new FontSizePresenter { Size = FontSize.huge, DisplayName = Strings.Huge }
                };
            }
        }

        public static List<TextAlignmentPresenter> HorizontalAlignments
        {
            get
            {
                return new List<TextAlignmentPresenter>() {
                    new TextAlignmentPresenter { Alignment = TextAlignment.left, DisplayText = Strings.Left },
                    new TextAlignmentPresenter { Alignment = TextAlignment.center, DisplayText = Strings.Center },
                    new TextAlignmentPresenter { Alignment = TextAlignment.right, DisplayText = Strings.Right }
                };
            }
        }

        public static List<TextAlignmentPresenter> VerticalAlignments
        {
            get
            {
                return new List<TextAlignmentPresenter>() {
                    new TextAlignmentPresenter { Alignment = TextAlignment.top, DisplayText = Strings.Top },
                    new TextAlignmentPresenter { Alignment = TextAlignment.center, DisplayText = Strings.Center },
                    new TextAlignmentPresenter { Alignment = TextAlignment.bottom, DisplayText = Strings.Bottom }
                };
            }
        }
    }
}
