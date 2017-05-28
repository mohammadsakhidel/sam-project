using SamDesktop.Code.Utils;
using SamDesktop.Code.ViewModels;
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

namespace SamDesktop.Views.Windows
{
    public partial class CreateTemplateWindow : Window
    {
        #region CTOR:
        public CreateTemplateWindow()
        {
            InitializeComponent();
        }
        #endregion

        #region Event Handlers:
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Top = SystemParameters.WorkArea.Height * 0.05;
                Height = SystemParameters.WorkArea.Height * 0.9;
                Width = SystemParameters.WorkArea.Width * 0.8;
                Left = SystemParameters.WorkArea.Width * 0.1;
            }
            catch (Exception ex)
            {
                ExceptionManager.Handle(ex);
            }
        }
        #endregion
    }
}
