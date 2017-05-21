using RamancoLibrary.Utilities;
using SamDesktop.Resources.Values;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SamDesktop.Code.Converters
{
    public class ResourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var regex = new Regex(@"^\w+\.\w+$");
            if (!regex.IsMatch(parameter.ToString()))
                throw new Exception("Invalid converter parameter for ResourceConverter.");

            var pair = parameter.ToString().Split('.');
            var className = pair[0];
            var propName = pair[1];

            var resManager = new System.Resources.ResourceManager($"SamDesktop.Resources.Values.{className}", Assembly.GetExecutingAssembly());

            var val = resManager.GetString(propName, System.Threading.Thread.CurrentThread.CurrentUICulture);
            return val;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
