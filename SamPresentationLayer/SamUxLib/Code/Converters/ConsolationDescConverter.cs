using RamancoLibrary.Utilities;
using SamModels.DTOs;
using SamUtils.Constants;
using SamUxLib.Code.Utils;
using SamUxLib.Resources;
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
    public class ConsolationDescConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var consolation = value as ConsolationDto;
                if (consolation == null)
                    return "";

                var senderDescLine = $"{Strings.Sender}: {consolation.Customer.FullName} ({consolation.Customer.CellPhoneNumber})" +
                    $" - {Strings.SendTime}: {consolation.CreationTime.ToString(StringFormats.datetime_short)}" +
                    $" - {Strings.Obit}: {consolation.Obit.Title} ({ResourceManager.GetValue($"ObitType_{consolation.Obit.ObitType}", "Enums")})";

                var obitDescLine = $"{Strings.ObitHoldingTime}: {TextUtils.Concat(consolation.Obit.ObitHoldings.OrderBy(h => h.BeginTime).Select(h => $"{h.BeginTime.ToString(StringFormats.date_short)} {Strings.From} {h.BeginTime.ToString(StringFormats.time_short)} {Strings.To} {h.EndTime.ToString(StringFormats.time_short)}").ToList(), " - ", false)}";

                return $"{senderDescLine}{Environment.NewLine}{obitDescLine}";
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
