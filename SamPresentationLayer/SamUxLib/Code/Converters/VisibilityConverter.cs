using SamUtils.Constants;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace SamUxLib.Code.Converters
{
    public class VisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null || string.IsNullOrEmpty(value.ToString()) 
                    || string.IsNullOrEmpty(parameter.ToString()))
                return Visibility.Collapsed;

            var paramVals = parameter.ToString().Split('-');
            if (paramVals.Contains(value.ToString()))
                return Visibility.Visible;

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
