using Newtonsoft.Json;
using RamancoLibrary.Utilities;
using RestSharp;
using SamKiosk.Code.Utils;
using SamModels.DTOs;
using SamUtils.Constants;
using SamUtils.Utils;
using SamUxLib.Code.DI;
using SamUxLib.Code.Utils;
using SamUxLib.Resources.Values;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
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
                var request = new RestRequest($"{ApiActions.consolations_getpreview}/{_parent.CreatedConsolationID}?thumb=false");
                var response = await App.RestClient.ExecuteGetTaskAsync(request);
                HttpUtil.EnsureRestSuccessStatusCode(response);
                var bytes = response.RawBytes;
                var bitmap = IOUtils.ByteArrayToBitmap(bytes);
                var source = ImageUtils.ToBitmapSource(bitmap);
                imgPreview.Source = source;
                progress.IsBusy = false;
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
                var sent = _pos.PayRequest(amount, false, true);
                if (sent)
                    pnlPosResponse.Visibility = Visibility.Visible;
                else
                    throw new Exception(Strings.RequestToPOSFailed);
            }
            catch (Exception ex)
            {
                KioskExceptionManager.Handle(ex);
            }
        }
        private void _pos_Response(object sender, PosResponseEventArgs e)
        {
            try
            {
                if (e.Succeeded)
                {
                    #region verify on the server:
                    var task = Task.Run(() =>
                    {
                        PosPaymentVerificationDto dto = new PosPaymentVerificationDto();
                        try
                        {
                            Dispatcher.Invoke(() => progress.IsBusy = true);
                            #region call:
                            dto.ConsolationID = _parent.CreatedConsolationID;
                            dto.PaymentData = (e.Data.Length > 512 ? e.Data.Substring(0, 512) : e.Data);
                            var request = new RestRequest(ApiActions.payment_verifypos, Method.PUT);
                            request.AddJsonBody(dto);
                            var response = App.RestClient.Execute(request);
                            HttpUtil.EnsureRestSuccessStatusCode(response);
                            #endregion
                            Dispatcher.Invoke(() => progress.IsBusy = false);
                        }
                        catch
                        {
                            #region save payment locally to verify in background:
                            var filePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $@"verify\{_parent.CreatedConsolationID}.json");
                            var json = JsonConvert.SerializeObject(dto);
                            File.WriteAllText(filePath, json);
                            #endregion
                        }
                        finally
                        {
                            #region next:
                            Dispatcher.Invoke(() =>
                            {
                                _parent.VerificationSucceeded = true;
                                _parent.NextNoAction();
                            });
                            #endregion
                        }
                    });
                    #endregion
                }
                else
                {
                    Dispatcher.Invoke(() =>
                    {
                        _parent.VerificationSucceeded = false;
                        _parent.NextNoAction();
                    });
                }
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
        #endregion
    }
}
