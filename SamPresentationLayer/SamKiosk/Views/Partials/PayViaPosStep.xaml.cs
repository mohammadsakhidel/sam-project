using RamancoLibrary.Utilities;
using SamKiosk.Code.Utils;
using SamUtils.Constants;
using SamUtils.Utils;
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
    public partial class PayViaPosStep : UserControl
    {
        #region Fields:
        SendConsolationView _parent;
        #endregion

        #region Ctors:
        public PayViaPosStep(SendConsolationView parent)
        {
            InitializeComponent();
            _parent = parent;
        }
        #endregion

        #region Event Handlers:
        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                _parent.SetNavigationState(false, false, true, true);

                #region load preview image:
                progress.IsBusy = true;
                using (var hc = HttpUtil.CreateClient())
                {
                    var bytes = await hc.GetByteArrayAsync($"{ApiActions.consolations_getpreview}/{_parent.CreatedConsolationID}?thumb=false");
                    var bitmap = IOUtils.ByteArrayToBitmap(bytes);
                    var source = ImageUtils.ToBitmapSource(bitmap);
                    imgPreview.Source = source;
                    progress.IsBusy = false;
                }
                #endregion
            }
            catch (Exception ex)
            {
                progress.IsBusy = false;
                KioskExceptionManager.Handle(ex);
            }
        }
        #endregion

        private void btnConfirmAndPay_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
