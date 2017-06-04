using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SamUxLib.Code.Utils
{
    public class VersatileUtil
    {
        public static void CloseWindowFromUserControl(System.Windows.Controls.ContentControl uc, bool dialogResult)
        {
            Window parentWindow = Window.GetWindow(uc);
            parentWindow.DialogResult = dialogResult;
            parentWindow.Close();
        }
    }
}
