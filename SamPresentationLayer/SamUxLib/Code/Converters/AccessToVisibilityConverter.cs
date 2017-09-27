using RamancoLibrary.Security.Tokens;
using SamUtils.Enums;
using SamUtils.Utils;
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
    public class AccessToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                /* RULE EXAMPLE -->  section1:cr|section2:d  */

                #region check null:
                if (value == null || parameter == null
                    || string.IsNullOrEmpty(value.ToString())
                    || string.IsNullOrEmpty(parameter.ToString()))
                    return Visibility.Collapsed;
                #endregion

                #region generate token:
                var tokenString = value.ToString();
                var token = new JwtToken(tokenString);
                #endregion

                #region check if is admin:
                if (token.Payload[JwtToken.ARG_USER_TYPE] == RoleType.admin.ToString())
                    return Visibility.Visible;
                #endregion

                #region check access level:
                var accessLevels = AccessUtil.Deserialize(token.Payload[JwtToken.ARG_ACCESS]);

                var rules = parameter.ToString().Split('|');
                var visible = false;
                foreach (var rule in rules)
                {
                    var sectionName = rule.Split(':')[0];
                    var requiredAccess = rule.Split(':')[1];
                    var userAccessToSection = accessLevels.SingleOrDefault(al => al.Name == sectionName);
                    if (userAccessToSection != null)
                    {
                        if (userAccessToSection.Create && requiredAccess.Contains("c"))
                            visible = true;
                        if (userAccessToSection.Read && requiredAccess.Contains("r"))
                            visible = true;
                        if (userAccessToSection.Update && requiredAccess.Contains("u"))
                            visible = true;
                        if (userAccessToSection.Delete && requiredAccess.Contains("d"))
                            visible = true;
                    }
                }
                #endregion

                return visible ? Visibility.Visible : Visibility.Collapsed;
            }
            catch (Exception)
            {
                return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
