using SamUtils.Enums;
using SamUxLib.Resources.Values;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SamUxLib.Code.Converters
{
    public class ConsolationStatusActionNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var statusStr = value as string;
            ConsolationStatus status;
            var res = Enum.TryParse(statusStr, out status);
            if (!res) return null;

            switch (status)
            {
                case ConsolationStatus.pending:
                    return Strings.ConfirmToDisplay;
                case ConsolationStatus.confirmed:
                    return Strings.CancelConfirmation;
                case ConsolationStatus.canceled:
                    return Strings.ConfirmToDisplay;
                case ConsolationStatus.displayed:
                    return Strings.CancelDisplay;
                default:
                    return "";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
