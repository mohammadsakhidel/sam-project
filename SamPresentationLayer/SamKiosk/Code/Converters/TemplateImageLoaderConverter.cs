using RamancoLibrary.Utilities;
using SamModels.DTOs;
using SamUtils.Constants;
using SamUtils.Utils;
using SamUxLib.Code.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace SamKiosk.Code.Converters
{
    public class TemplateImageLoaderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var template = value as TemplateDto;
                if (template != null)
                {
                    var thumb = parameter != null && (System.Convert.ToBoolean(parameter));
                    var baseUri = new Uri(ConfigurationManager.AppSettings["api_host"]);
                    var url = new Uri(baseUri, $"{ApiActions.blobs_getimage}/{template.BackgroundImageID}?thumb={thumb.ToString()}");
                    return new BitmapImage(url);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return ExceptionManager.ConverterException<BitmapSource>(ex);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
