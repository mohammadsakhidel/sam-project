using RamancoLibrary.Utilities;
using SamKiosk.Code.Utils;
using SamModels.DTOs;
using SamUtils.Constants;
using SamUtils.Utils;
using SamUxLib.Code.DI;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
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
        IPOS _pos;
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
                InitPos();
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
        private void btnConfirmAndPay_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var amount = (int)_parent.SelectedTemplate.Price;
                _pos.PayRequest(amount, false, true);

                pnlPosResponse.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                KioskExceptionManager.Handle(ex);
            }
        }
        private async void _pos_Response(object sender, PosResponseEventArgs e)
        {
            try
            {
                if (e.Succeeded)
                {
                    await VerifyAsync(e.Data);
                }
                else
                {
                    _parent.VerificationSucceeded = false;
                    Dispatcher.Invoke(() => {
                        _parent.NextNoAction();
                    });
                }
            }
            catch (Exception ex)
            {
                KioskExceptionManager.Handle(ex);
            }
        }
        private void btnRetry_Click(object sender, RoutedEventArgs e)
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

        #region Methods:
        void InitPos()
        {
            var portName = ConfigurationManager.AppSettings["pos_port"];
            _pos = new SamanSerialPOS(portName);
            _pos.PosResponse += _pos_Response;
        }
        void ShowRetryView()
        {

        }
        async Task VerifyAsync(string data)
        {
            Dispatcher.Invoke(() =>
            {
                progress.IsBusy = true;
            });
            using (var hc = HttpUtil.CreateClient())
            {
                var dto = new PosPaymentVerificationDto()
                {
                    ConsolationID = _parent.CreatedConsolationID,
                    PaymentData = (data.Length > 512 ? data.Substring(0, 512) : data)
                };
                #region call with retry:
                var waitTime = 2000;
                try
                {
                    try
                    {
                        var response = await hc.PutAsJsonAsync($"{ApiActions.payment_verifypos}", dto);
                        HttpUtil.EnsureSuccessStatusCode(response);
                        _parent.VerificationSucceeded = true;
                        Dispatcher.Invoke(() => _parent.NextNoAction());
                    }
                    catch
                    {
                        Thread.Sleep(waitTime);
                        var response = await hc.PutAsJsonAsync($"{ApiActions.payment_verifypos}", dto);
                        HttpUtil.EnsureSuccessStatusCode(response);
                        _parent.VerificationSucceeded = true;
                        Dispatcher.Invoke(() => _parent.NextNoAction());
                    }
                }
                catch
                {
                    ShowRetryView();
                }
                #endregion
            }
        }
        #endregion
    }
}
