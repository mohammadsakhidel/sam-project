using RamancoLibrary.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SamDesktop.Code.Converters
{
    public class ShortDateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (value == null)
                    return "";

                var date = (DateTime)value;
                var roji = DateTimeUtils.ToShamsi(date);
                return string.Format("{0:D4}/{1:D2}/{2:D2}", roji.Year, roji.Month, roji.Day);
            }
            catch
            {
                return "";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
