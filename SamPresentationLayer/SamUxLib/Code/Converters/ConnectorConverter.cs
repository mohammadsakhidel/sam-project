using SamUxLib.Code.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SamUxLib.Code.Converters
{
    public class ConnectorConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (values != null && values.Any())
                {
                    var result = values[0];
                    var splitter = parameter != null ? parameter.ToString() : " ";
                    for (int i = 1; i < values.Length; i++)
                    {
                        result += $" {values[i].ToString()}";
                    }
                    return result;
                }
                else
                {
                    return "";
                }
            }
            catch (Exception ex)
            {
                return ExceptionManager.ConverterException<string>(ex);
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
