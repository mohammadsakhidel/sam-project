using SamUxLib.Code.Utils;
using SamUxLib.Resources;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SamUxLib.Code.Converters
{
    public class ConsolationStatusDisplayNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var statuStr = value as string;
                return ResourceManager.GetValue($"ConsolationStatus_{statuStr}", "Enums");
            }
            catch (Exception ex)
            {
                return ExceptionManager.ConverterException<string>(ex);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
