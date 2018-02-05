using SamModels.DTOs;
using SamUtils.Constants;
using SamUtils.Utils;
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
using System.Net.Http;
using SamUxLib.Resources.Values;
using SamUtils.Objects.Exceptions;
using System.Transactions;
using AutoMapper;
using SamClientDataAccess.Repos;
using SamClientDataAccess.ClientModels;

namespace SamClient.Views.Partials
{
    public partial class ClientSettings : UserControl
    {
        #region Ctors:
        public ClientSettings()
        {
            InitializeComponent();
        }
        #endregion

        #region Props:
        private ClientSetting _clientSetting;
        public ClientSetting ClientSetting
        {
            get
            {
                UpdateModel();
                return _clientSetting;
            }
            set
            {
                _clientSetting = value;
                UpdateForm();
            }
        }

        private MosqueDto _selectedMosque;
        public MosqueDto SelectedMosque
        {
            get
            {
                UpdateModel();
                return _selectedMosque;
            }
        }
        #endregion

        #region Methods:
        private void LoadProvinces()
        {
            cmbProvinces.ItemsSource = CityUtil.Provinces;
        }
        private void UpdateForm()
        {
            if (_clientSetting != null)
            {
                cmbCities.SelectedValue = _clientSetting.CityID;
                if (_clientSetting.CityID > 0)
                {
                    cmbProvinces.SelectedItem = CityUtil.GetProvince(_clientSetting.CityID).ID;
                }
                cmbMosques.SelectedValue = _clientSetting.MosqueID;
                cmbSaloons.SelectedValue = _clientSetting.SaloonID;
                tbDownloadIntervalSeconds.Text = (_clientSetting.DownloadIntervalMilliSeconds / 1000).ToString();
                tbDownloadDelaySeconds.Text = (_clientSetting.DownloadDelayMilliSeconds / 1000).ToString();
                chAutoSlideShow.IsChecked = _clientSetting.AutoSlideShow;
                tbDefaultSlideShowDuration.Text = (_clientSetting.DefaultSlideDurationMilliSeconds / 1000).ToString();
            }
        }
        private void UpdateModel()
        {
            if (_clientSetting == null)
                _clientSetting = new ClientSetting();

            _clientSetting.ID = 1;

            _clientSetting.CityID = cmbCities.SelectedItem != null ? Convert.ToInt32(cmbCities.SelectedValue) : -1;

            _selectedMosque = cmbMosques.SelectedItem as MosqueDto;
            _clientSetting.MosqueID = _selectedMosque != null ? _selectedMosque.ID : -1;
            _clientSetting.MosqueName = _selectedMosque != null ? _selectedMosque.Name : "";
            _clientSetting.MosqueAddress = _selectedMosque != null ? _selectedMosque.Address : "";

            var selectedSaloon = cmbSaloons.SelectedItem as SaloonDto;
            _clientSetting.SaloonID = selectedSaloon != null ? selectedSaloon.ID : "";

            _clientSetting.DownloadIntervalMilliSeconds = Convert.ToInt32(tbDownloadIntervalSeconds.Text) * 1000;
            _clientSetting.DownloadDelayMilliSeconds = Convert.ToInt32(tbDownloadDelaySeconds.Text) * 1000;
            _clientSetting.AutoSlideShow = chAutoSlideShow.IsChecked.HasValue && chAutoSlideShow.IsChecked.Value;
            _clientSetting.DefaultSlideDurationMilliSeconds = Convert.ToInt32(tbDefaultSlideShowDuration.Text) * 1000;
        }
        public Tuple<bool, string> IsValid()
        {
            UpdateModel();

            if (_clientSetting == null || _clientSetting.MosqueID < 0 || string.IsNullOrEmpty(_clientSetting.SaloonID))
                return new Tuple<bool, string>(false, Messages.FillRequiredFields);

            if (!ClientSetting.IsSettingValid(_clientSetting))
                return new Tuple<bool, string>(false, Messages.InvalidInputValues);

            return new Tuple<bool, string>(true, "");
        }
        #endregion

        #region Event Handlers:
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadProvinces();
            }
            catch (Exception ex)
            {
                ExceptionManager.Handle(ex);
            }
        }
        private void cmbProvinces_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (cmbProvinces.SelectedItem != null)
                {
                    var province = cmbProvinces.SelectedItem as ProvinceDto;
                    var cities = CityUtil.GetProvinceCities(province.ID);
                    cmbCities.ItemsSource = cities;
                }
                else
                {
                    cmbCities.ItemsSource = null;
                }
            }
            catch (Exception ex)
            {
                ExceptionManager.Handle(ex);
            }
        }
        private async void cmbCities_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (cmbCities.SelectedItem != null)
                {
                    cmbMosques.IsEnabled = false;
                    var city = cmbCities.SelectedItem as CityDto;
                    using (var hc = HttpUtil.CreateClient())
                    {
                        var response = await hc.GetAsync($"{ApiActions.mosques_findbycity}?cityId={city.ID}");
                        HttpUtil.EnsureSuccessStatusCode(response);
                        var mosques = await response.Content.ReadAsAsync<List<MosqueDto>>();
                        cmbMosques.ItemsSource = mosques;
                        cmbMosques.IsEnabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                cmbMosques.IsEnabled = true;
                ExceptionManager.Handle(ex);
            }
        }
        private void cmbMosques_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var selectedMosque = cmbMosques.SelectedItem as MosqueDto;
                if (selectedMosque != null)
                {
                    cmbSaloons.ItemsSource = selectedMosque.Saloons;
                }
            }
            catch (Exception ex)
            {
                ExceptionManager.Handle(ex);
            }
        }
        #endregion
    }
}