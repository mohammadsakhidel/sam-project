using Newtonsoft.Json;
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
    public class ConsolationTitleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var consolation = value as ConsolationDto;
                if (consolation == null)
                    return "";

                var title = "";
                if (!string.IsNullOrEmpty(consolation.TemplateInfo))
                {
                    var info = JsonConvert.DeserializeObject<Dictionary<string, string>>(consolation.TemplateInfo);
                    var keys = info.Keys.ToList();
                    for (int i = 0; i < keys.Count(); i++)
                    {
                        var k = keys[i];
                        var tField = consolation.Template.TemplateFields.SingleOrDefault(f => f.Name == k);
                        title += $"{tField?.DisplayName}: {info[k]}.{(i < keys.Count() - 1 ? Environment.NewLine : "")}";
                    }
                }
                return title;
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
