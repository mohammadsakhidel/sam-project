using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace SamDesktop
{
    public partial class App : Application
    {
        const int PERSIAN_CULTURE_ID = 1065;
        const int ENGLISH_CULTURE_ID = 1033;

        protected override void OnStartup(StartupEventArgs e)
        {

            #region Culture Setting, Used by Persian Wpf Toolkit:
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(PERSIAN_CULTURE_ID);
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;
            #endregion

            base.OnStartup(e);
        }
    }
}
