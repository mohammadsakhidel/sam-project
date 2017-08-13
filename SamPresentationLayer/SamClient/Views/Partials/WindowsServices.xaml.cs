using SamClient.Views.Windows;
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

namespace SamClient.Views.Partials
{
    public partial class WindowsServices : UserControl
    {
        #region Fields:
        MainWindow _mainWindow;
        #endregion

        #region Ctors:
        public WindowsServices(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
            InitializeComponent();
        }
        #endregion

        #region Event Handlers:
        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _mainWindow.LoadContent(new HomePage(_mainWindow));
            }
            catch (Exception ex)
            {
                ExceptionManager.Handle(ex);
            }
        }
        #endregion

    }
}
