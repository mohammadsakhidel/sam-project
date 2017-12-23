using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

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

        public static ServiceControllerStatus GetWindowsServiceStatus(string serviceName)
        {
            ServiceController sc = new ServiceController(serviceName);
            return sc.Status;
        }

        public static Task StartServiceAsync(string serviceName)
        {
            var task = Task.Run(() =>
            {
                ServiceController sc = new ServiceController(serviceName);
                if (sc.Status != ServiceControllerStatus.Running)
                    sc.Start();

                while (GetWindowsServiceStatus(serviceName) != ServiceControllerStatus.Running)
                {
                }
            });

            return task;
        }

        public static Task StopServiceAsync(string serviceName)
        {
            var task = Task.Run(() =>
            {
                ServiceController sc = new ServiceController(serviceName);
                sc.Stop();

                while (GetWindowsServiceStatus(serviceName) != ServiceControllerStatus.Stopped)
                {
                }
            });
            return task;
        }

        public static SolidColorBrush GetBrushFromColorCode(string colorCode)
        {
            return (SolidColorBrush)(new BrushConverter().ConvertFrom(colorCode));
        }

        public static string ToArabicNumbers(string input)
        {
            return input.Replace('0', '\u06f0')
                        .Replace('1', '\u06f1')
                        .Replace('2', '\u06f2')
                        .Replace('3', '\u06f3')
                        .Replace('4', '\u06f4')
                        .Replace('5', '\u06f5')
                        .Replace('6', '\u06f6')
                        .Replace('7', '\u06f7')
                        .Replace('8', '\u06f8')
                        .Replace('9', '\u06f9');
        }
    }
}
