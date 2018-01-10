using RestSharp;
using SamKiosk.Code.Utils;
using SamKiosk.Views.Windows;
using SamModels.DTOs;
using SamUtils.Constants;
using SamUtils.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public partial class ObitSelectionStep : UserControl
    {
        #region Fields:
        SendConsolationView _parent;
        #endregion

        #region Constructors:
        public ObitSelectionStep(SendConsolationView parent)
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
                _parent.SetNavigationState(false, false, false, false);

                #region load obits:
                progress.IsBusy = true;
                var mosqueId = Convert.ToInt32(ConfigurationManager.AppSettings["MosqueID"]);
                var request = new RestRequest($"{ApiActions.obits_gethenceforwardobits}?mosqueid={mosqueId}");
                var response = await App.RestClient.ExecuteGetTaskAsync<List<ObitDto>>(request);
                HttpUtil.EnsureRestSuccessStatusCode(response);
                var obits = response.Data;

                lbObits.ItemsSource = new ObservableCollection<ObitDto>(obits);
                progress.IsBusy = false;
                #endregion
            }
            catch (Exception ex)
            {
                progress.IsBusy = false;
                KioskExceptionManager.Handle(ex);
            }
        }
        private async void lbObits_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var obit = lbObits.SelectedItem as ObitDto;
                if (obit != null)
                {
                    _parent.SelectedObit = obit;
                    await _parent.NextAsync();
                }
            }
            catch (Exception ex)
            {
                KioskExceptionManager.Handle(ex);
            }
        }
        #endregion
    }
}
