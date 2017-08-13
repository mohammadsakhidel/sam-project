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
using SamModels.Entities.Core;
using AutoMapper;
using SamClientDataAccess.Repos;
using ClientModels.Models;

namespace SamClient.Views.Partials
{
    public partial class ClientSettings : UserControl
    {
        #region Ctors:
        public ClientSettings()
        {
            InitializeComponent();
            LoadProvinces();
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

        public int ProvinceID
        {
            set
            {
                cmbProvinces.SelectedValue = value;
            }
        }

        public int CityID
        {
            set
            {
                cmbCities.SelectedValue = value;
            }
        }
        #endregion

        #region Methods:
        private void LoadProvinces()
        {
            try
            {
                cmbProvinces.ItemsSource = CityUtil.Provinces;
            }
            catch (Exception ex)
            {
                ExceptionManager.Handle(ex);
            }
        }
        private void UpdateForm()
        {
            if (_clientSetting != null)
            {
                cmbMosques.SelectedValue = _clientSetting.MosqueID;
                cmbSaloons.SelectedValue = _clientSetting.SaloonID;
                tbDownloadIntervalSeconds.Text = (_clientSetting.DownloadIntervalMilliSeconds / 1000).ToString();
                tbDownloadDelaySeconds.Text = (_clientSetting.DownloadDelayMilliSeconds / 1000).ToString();
                chAutoSlideShow.IsChecked = _clientSetting.AutoSlideShow;
            }
        }
        private void UpdateModel()
        {
            if (_clientSetting == null)
                _clientSetting = new ClientSetting();

            _clientSetting.ID = 1;

            _selectedMosque = cmbMosques.SelectedItem as MosqueDto;
            _clientSetting.MosqueID = _selectedMosque != null ? _selectedMosque.ID : -1;

            var selectedSaloon = cmbSaloons.SelectedItem as SaloonDto;
            _clientSetting.SaloonID = selectedSaloon != null ? selectedSaloon.ID : "";

            _clientSetting.DownloadIntervalMilliSeconds = Convert.ToInt32(tbDownloadIntervalSeconds.Text) * 1000;
            _clientSetting.DownloadDelayMilliSeconds = Convert.ToInt32(tbDownloadDelaySeconds.Text) * 1000;
            _clientSetting.AutoSlideShow = chAutoSlideShow.IsChecked.HasValue && chAutoSlideShow.IsChecked.Value;
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