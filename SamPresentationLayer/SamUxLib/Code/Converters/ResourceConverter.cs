using RamancoLibrary.Utilities;
using SamUxLib.Code.Utils;
using SamUxLib.Resources;
using SamUxLib.Resources.Values;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SamUxLib.Code.Converters
{
    public class ResourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if ((value == null || value.GetType() != typeof(string)) && parameter != null && !string.IsNullOrEmpty(parameter.ToString()))
                {
                    var regex = new Regex(@"^\w+\.\w+$");
                    if (!regex.IsMatch(parameter.ToString()))
                        throw new Exception("Invalid converter parameter for ResourceConverter.");

                    var pair = parameter.ToString().Split('.');
                    var className = pair[0];
                    var propName = pair[1];

                    var val = ResourceManager.GetValue(propName, className);
                    return val;
                }
                else if (value != null && !string.IsNullOrEmpty(value.ToString()))
                {
                    var prefix = parameter != null && !string.IsNullOrEmpty(parameter.ToString()) ? parameter.ToString() : "Strings.";
                    var pairStr = $"{prefix}{value.ToString()}";
                    var pair = pairStr.Split('.');
                    var className = pair[0];
                    var propName = pair[1];

                    var val = ResourceManager.GetValue(propName, className);
                    return val;
                }
                else
                {
                    return "";
                }
            }
            catch(Exception ex)
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
