using SamKiosk.Code.Utils;
using SamKiosk.Views.Windows;
using SamUxLib.Code.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SamKiosk.Views.Partials
{
    public partial class Home : UserControl
    {
        #region Fields:
        MainWindow _mainWindow;
        #endregion

        #region Constructors:
        public Home(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
        }
        #endregion

        #region Event Handlers:
        private void btnSendConsolation_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var uc = new SendConsolationView(_mainWindow);
                _mainWindow.SetContent(uc);
            }
            catch (Exception ex)
            {
                KioskExceptionManager.Handle(ex);
            }
        }

        private void btnAbout_Click(object sender, RoutedEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                KioskExceptionManager.Handle(ex);
            }
        }
        #endregion
    }
}
