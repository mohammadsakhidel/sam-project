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
using System.Windows.Shapes;

namespace SamDesktop.Views.Windows
{
    public partial class CreateBannerWindow : Window
    {
        #region Ctors:
        public CreateBannerWindow()
        {
            InitializeComponent();
        }
        #endregion

        #region Event Handlers:
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                ExceptionManager.Handle(ex);
            }
        }
        #endregion
    }
}
