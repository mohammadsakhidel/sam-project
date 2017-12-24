using SamKiosk.Code.Utils;
using SamKiosk.Views.Partials;
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
using System.Windows.Shapes;

namespace SamKiosk.Views.Windows
{
    public partial class MainWindow : Window
    {
        #region Constructors:
        public MainWindow()
        {
            InitializeComponent();
        }
        #endregion

        #region Event Handlers:
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                SetContent(new Home(this));
            }
            catch (Exception ex)
            {
                KioskExceptionManager.Handle(ex);
            }
        }
        private void btnHome_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SetContent(new Home(this));
            }
            catch (Exception ex)
            {
                KioskExceptionManager.Handle(ex);
            }
        }
        #endregion

        #region Methods:
        public void SetContent(UserControl userControl)
        {
            container.Content = userControl;
        }
        #endregion
    }
}
