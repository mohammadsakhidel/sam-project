using SamUxLib.Code.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WPFTabTip;

namespace SamKiosk
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                TabTipAutomation.BindTo<TextBox>();
                //TabTipAutomation.IgnoreHardwareKeyboard = HardwareKeyboardIgnoreOptions.IgnoreAll;
            }
            catch (Exception ex)
            {
                ExceptionManager.Handle(ex);
            }

            base.OnStartup(e);
        }
    }
}
