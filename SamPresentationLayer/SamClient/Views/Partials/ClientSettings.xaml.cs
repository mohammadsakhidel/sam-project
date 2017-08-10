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
        }
        #endregion

        #region Event Handlers:
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                cmbProvinces.ItemsSource = CityUtil.Provinces;

                #region Load Current Settings:
                using (var srepo = new ClientSettingRepo())
                using (var mrepo = new MosqueRepo())
                {
                    var settings = srepo.Get(1);
                    if (settings != null)
                    {
                        var mosque = mrepo.Get(settings.MosqueID);
                        if (mosque != null)
                        {
                            var city = CityUtil.GetCity(mosque.CityID);
                            cmbProvinces.SelectedValue = city.ProvinceID;
                            cmbCities.SelectedValue = mosque.CityID;
                            cmbMosques.SelectedValue = mosque.ID;
                        }
                    }
                }
                #endregion
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
                ExceptionManager.Handle(ex);
            }
        }
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                #region Validate Inputs:
                if (cmbMosques.SelectedItem == null || cmbProvinces.SelectedItem == null || cmbCities.SelectedItem == null)
                    throw new ValidationException(Messages.FillRequiredFields);
                #endregion

                #region Save Settings:
                using (var ts = new TransactionScope())
                using (var srep = new ClientSettingRepo())
                using (var prep = new ProvinceRepo())
                using (var crep = new CityRepo())
                using (var mrep = new MosqueRepo())
                {
                    #region Save Province If Not Exists:
                    var selectedProvince = cmbProvinces.SelectedItem as ProvinceDto;
                    if (!prep.Exists(selectedProvince.ID))
                    {
                        var province = new Province { ID = selectedProvince.ID, Name = selectedProvince.Name };
                        prep.Add(province);
                        prep.Save();
                    }
                    #endregion

                    #region Save City If Not Exists:
                    var selectedCity = cmbCities.SelectedItem as CityDto;
                    if (!crep.Exists(selectedCity.ID))
                    {
                        var city = new City { ID = selectedCity.ID, ProvinceID = selectedProvince.ID, Name = selectedCity.Name };
                        crep.Add(city);
                        crep.Save();
                    }
                    #endregion

                    #region Save Mosque If Not Exists:
                    var selectedMosque = cmbMosques.SelectedItem as MosqueDto;
                    var mosqueExists = mrep.Exists(selectedMosque.ID);
                    var mosque = mosqueExists ? mrep.Get(selectedMosque.ID) : new Mosque();
                    mosque = Mapper.Map<MosqueDto, Mosque>(selectedMosque);
                    if (!mosqueExists)
                        mrep.Add(mosque);
                    mrep.Save();
                    #endregion

                    #region Save Settings:
                    if (srep.Exists(1))
                    {
                        var setting = srep.Get(1);
                        setting.MosqueID = ((MosqueDto)cmbMosques.SelectedItem).ID;
                    }
                    else
                    {
                        var setting = new ClientSetting { ID = 1, MosqueID = ((MosqueDto)cmbMosques.SelectedItem).ID };
                        srep.Add(setting);
                    }
                    srep.Save();
                    #endregion

                    ts.Complete();
                    UxUtil.ShowMessage(Messages.SuccessfullyDone);
                }
                #endregion
            }
            catch (Exception ex)
            {
                ExceptionManager.Handle(ex);
            }
        }
        #endregion
    }
}
