using RamancoLibrary.Utilities;
using SamDesktop.Code.Utils;
using SamDesktop.Code.ViewModels;
using SamDesktop.Resources.Values;
using SamDesktop.Views.Windows;
using SamModels.DTOs;
using SamUtils.Constants;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
                if (SelectedMosque != null)
                {
                    #region Set SelectedMosque:
                    var vm = DataContext as ObitsVM;
                    vm.SelectedMosque = SelectedMosque;
                    #endregion
                    #region Info:
                    var province = CityUtil.GetProvince(SelectedMosque.CityID);
                    var city = CityUtil.GetCity(SelectedMosque.CityID);
                    lblInfo.Content = $"{Strings.Province}: {province.Name}\t {Strings.City}: {city.Name}\t {Strings.Mosque}: {SelectedMosque.Name}";
                    #endregion
                }

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
                var window = new CreateObitWindow(SelectedMosque);
                var res = window.ShowDialog();
                if (res.HasValue && res.Value)
                {
                    var selectedDate = ucPersianDateNavigator.GetMiladyDate();
                    if (selectedDate.HasValue)
                        await LoadObits(SelectedMosque.ID, ucPersianDateNavigator.GetMiladyDate().Value);
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
                    var window = new EditObitWindow(obitToEdit);
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
                ExceptionManager.Handle(ex);
            }
        }
        private void btnDelete_Click(object sender, RoutedEventArgs e)
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

        #region Methods:
        private async Task LoadObits(int mosqueId, DateTime miladyDate)
        {
            try
            {
                progress.IsBusy = true;
                using (var hc = HttpUtil.CreateClient())
                {
                    var response = await hc.GetAsync($"{ApiActions.obits_getholdings}?mosqueId={mosqueId}&date={miladyDate.ToString(StringFormats.date_short)}");
                    response.EnsureSuccessStatusCode();
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