using SamModels.DTOs;
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
    public class ConsolationStatusActionStyleConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var targetElement = values[0] as FrameworkElement;
                var statusStr = values[1] as string;
                ConsolationStatus status;
                var res = Enum.TryParse(statusStr, out status);
                if (!res) return null;

                Style style;
                switch (status)
                {
                    case ConsolationStatus.pending:
                        style = targetElement.FindResource("small_action_button_confirm") as Style;
                        break;
                    case ConsolationStatus.confirmed:
                        style = targetElement.FindResource("small_action_button_reject") as Style;
                        break;
                    case ConsolationStatus.canceled:
                        style = targetElement.FindResource("small_action_button_confirm") as Style;
                        break;
                    case ConsolationStatus.displayed:
                        style = targetElement.FindResource("small_action_button_reject") as Style;
                        break;
                    default:
                        style = null;
                        break;
                }
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
