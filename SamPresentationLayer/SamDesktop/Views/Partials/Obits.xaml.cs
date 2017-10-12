using RamancoLibrary.Utilities;
using SamUxLib.Code.Utils;
using SamDesktop.Code.ViewModels;
using SamUxLib.Resources.Values;
using SamDesktop.Views.Windows;
using SamModels.DTOs;
using SamUtils.Constants;
using SamUtils.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Net.Http;
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

namespace SamDesktop.Views.Partials
{
    public partial class Obits : UserControl
    {
        #region Ctors:
        public Obits()
        {
            InitializeComponent();
        }
        #endregion

        #region Props:
        public MosqueDto SelectedMosque { get; set; }
        #endregion

        #region Event Handlers:
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var vm = DataContext as ObitsVM;
                vm.Token = App.UserToken.ToString();

                var provinces = CityUtil.Provinces;
                cmbProvince.ItemsSource = new ObservableCollection<ProvinceDto>(provinces);

                ucPersianDateNavigator.SetMiladyDate(DateTimeUtils.Now);
            }
            catch (Exception ex)
            {
                ExceptionManager.Handle(ex);
            }
        }
        private async void ucPersianDateNavigator_OnChange(object sender, DateChangedEventArgs e)
        {
            try
            {
                if (SelectedMosque != null)
                    await LoadObits(SelectedMosque.ID, e.NewDate);
            }
            catch (Exception ex)
            {
                ExceptionManager.Handle(ex);
            }
        }
        private async void btnNew_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var mosque = cmbMosque.SelectedItem as MosqueDto;
                var window = new CreateObitWindow(mosque);
                var res = window.ShowDialog();
                if (res.HasValue && res.Value)
                {
                    var selectedDate = ucPersianDateNavigator.GetMiladyDate();
                    if (selectedDate.HasValue)
                        await LoadObits(mosque.ID, ucPersianDateNavigator.GetMiladyDate().Value);
                }
            }
            catch (Exception ex)
            {
                ExceptionManager.Handle(ex);
            }
        }
        private async void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dgRecords.SelectedItem != null)
                {
                    var obitToEdit = ((ObitHoldingDto)dgRecords.SelectedItem).Obit;
                    var window = new EditObitWindow(SelectedMosque, obitToEdit);
                    var res = window.ShowDialog();
                    if (res.HasValue && res.Value)
                    {
                        var selectedDate = ucPersianDateNavigator.GetMiladyDate();
                        if (selectedDate.HasValue)
                            await LoadObits(SelectedMosque.ID, ucPersianDateNavigator.GetMiladyDate().Value);
                    }
                }
            }
            catch (Exception ex)
            {
                progress.IsBusy = false;
                ExceptionManager.Handle(ex);
            }
        }
        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dgRecords.SelectedItem != null)
                {
                    var result = UxUtil.ShowQuestion(Messages.AreYouSureToDelete);
                    if (result == MessageBoxResult.Yes)
                    {
                        var obitToDeleteId = (dgRecords.SelectedItem as ObitHoldingDto).ObitID;
                        #region Call Server To Delete:
                        progress.IsBusy = true;
                        using (var hc = HttpUtil.CreateClient())
                        {
                            var response = await hc.DeleteAsync($"{ApiActions.obits_delete}/{obitToDeleteId}");
                            HttpUtil.EnsureSuccessStatusCode(response);
                            UxUtil.ShowMessage(Messages.SuccessfullyDone);
                            await LoadObits(SelectedMosque.ID, ucPersianDateNavigator.GetMiladyDate().Value);
                        }
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                progress.IsBusy = false;
                ExceptionManager.Handle(ex);
            }
        }
        private void cmbProvince_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (cmbProvince.SelectedItem != null)
                {
                    var prov = (ProvinceDto)cmbProvince.SelectedItem;
                    var cities = CityUtil.GetProvinceCities(prov.ID);
                    cmbCity.ItemsSource = cities;
                }
            }
            catch (Exception ex)
            {
                ExceptionManager.Handle(ex);
            }
        }
        private async void cmbCity_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (cmbCity.SelectedItem != null)
                {
                    cmbMosque.IsEnabled = false;
                    var city = cmbCity.SelectedItem as CityDto;
                    using (var hc = HttpUtil.CreateClient())
                    {
                        var response = await hc.GetAsync($"{ApiActions.mosques_findbycity}?cityId={city.ID}");
                        HttpUtil.EnsureSuccessStatusCode(response);
                        var mosques = await response.Content.ReadAsAsync<List<MosqueDto>>();
                        cmbMosque.ItemsSource = mosques;
                        cmbMosque.IsEnabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                cmbMosque.IsEnabled = true;
                ExceptionManager.Handle(ex);
            }
        }
        private async void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SelectedMosque = cmbMosque.SelectedItem as MosqueDto;
                if (SelectedMosque != null)
                {
                    var dt = ucPersianDateNavigator.GetMiladyDate().HasValue ? ucPersianDateNavigator.GetMiladyDate().Value : DateTimeUtils.Now;
                    await LoadObits(SelectedMosque.ID, dt);
                }
            }
            catch (Exception ex)
            {
                ExceptionManager.Handle(ex);
            }
        }
        #endregion

        #region Methods:
        private async Task LoadObits(int mosqueId, DateTime miladyDate)
        {
            try
            {
                progress.IsBusy = true;
                using (var hc = HttpUtil.CreateClient())
                {
                    var response = await hc.GetAsync($"{ApiActions.obits_getholdings}?mosqueId={mosqueId}&date={miladyDate.ToString(StringFormats.date_short, new CultureInfo("en"))}");
                    HttpUtil.EnsureSuccessStatusCode(response);
                    var obitHoldings = await response.Content.ReadAsAsync<List<ObitHoldingDto>>();
                    var vm = DataContext as ObitsVM;
                    vm.ObitHoldings = new ObservableCollection<ObitHoldingDto>(obitHoldings);
                    progress.IsBusy = false;
                }
            }
            catch (Exception ex)
            {
                progress.IsBusy = false;
                ExceptionManager.Handle(ex);
            }
        }
        #endregion
    }
}