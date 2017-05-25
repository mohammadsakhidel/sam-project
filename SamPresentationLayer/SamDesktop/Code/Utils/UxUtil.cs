using SamDesktop.Resources.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SamDesktop.Code.Utils
{
    public class UxUtil
    {
        public static MessageBoxResult ShowMessage(string message)
        {
            return Xceed.Wpf.Toolkit.MessageBox.Show(message, Strings.MessageBoxCaption, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public static MessageBoxResult ShowError(string message)
        {
            return Xceed.Wpf.Toolkit.MessageBox.Show(message, Strings.ErrorMessageBoxCaption, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public static MessageBoxResult ShowQuestion(string message)
        {
            return Xceed.Wpf.Toolkit.MessageBox.Show(message, Strings.QuestionMessageBoxCaption, MessageBoxButton.YesNo, MessageBoxImage.Question);
        }
    }
}
