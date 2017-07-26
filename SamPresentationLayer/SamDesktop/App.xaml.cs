using SamUxLib.Code.Utils;
using SamUtils.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Reflection;
using SamUxLib.Resources.Values;
using RamancoLibrary.Security.Tokens;

namespace SamDesktop
{
    public partial class App : Application
    {
        #region Consts:
        const int PERSIAN_CULTURE_ID = 1065;
        const int ENGLISH_CULTURE_ID = 1033;
        #endregion

        #region Overrides:
        protected override void OnStartup(StartupEventArgs e)
        {
            #region Culture Setting, Used by Persian Wpf Toolkit:
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(PERSIAN_CULTURE_ID);
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;
            #endregion

            #region CityUtil GetXmlFunc:
            CityUtil.Func_GetXMLContent = () => {
                try
                {
                    using (var stream = typeof(Strings).Assembly.GetManifestResourceStream("SamUxLib.Resources.XML.ir-cities.xml"))
                    {
                        using (var sr = new StreamReader(stream))
                        {
                            return sr.ReadToEnd();
                        }
                    }
                }
                catch (Exception ex)
                {
                    ExceptionManager.Handle(ex);
                    return "";
                }
            };
            #endregion

            base.OnStartup(e);
        }
        #endregion

        #region Static Props:
        public static JwtToken UserToken { get; set; }
        #endregion
    }
}
