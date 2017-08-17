using SamUtils.Enums;
using SamUxLib.Code.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace SamUxLib.Code.Converters
{
    public class ConsolationStatusIconStyleConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var targetElement = values[0] as FrameworkElement;
                var statusStr = values[1] as string;
                var style = targetElement.TryFindResource($"item_icon_{statusStr}") as Style;
                return style;
            }
            catch (Exception ex)
            {
                return ExceptionManager.ConverterException<Style>(ex);
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
