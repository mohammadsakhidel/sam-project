using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SamKiosk.Code.Utils
{
    public class KioskExceptionManager
    {
        internal static void Handle(Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }
}
