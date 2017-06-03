using SamDesktop.Code.Utils;
using SamDesktop.Code.ViewModels;
using SamModels.Entities.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
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
using System.Net.Http.Formatting;
using SamModels.DTOs;
using SamUtils.Constants;
using SamUtils.Utils;

namespace SamDesktop.Views.Partials
{
    public partial class Mosques : UserControl
    {
        #region Ctors:
        public Mosques()
        {
            InitializeComponent();
        }
        #endregion

        #region Event Handlers:
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var provinces = CityUtil.Provinces;
                var vm = DataContext as MosquesVM;
                vm.Provinces = new ObservableCollection<ProvinceDto>(provinces);
            }
            catch (Exception ex)
            {
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
                    var vm = DataContext as MosquesVM;
                    vm.Cities = new ObservableCollection<CityDto>(cities);
                }
            }
            catch (Exception ex)
            {
                ExceptionManager.Handle(ex);
            }
        }
        private async void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var city = cmbCity.SelectedItem as CityDto;
                if (city != null)
                {
                    await LoadRecords(city.ID);
                }
            }
            catch (Exception ex)
            {
                progress.IsBusy = false;
                ExceptionManager.Handle(ex);
            }
        }
        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBox.Show("NNN");
            }
            catch (Exception ex)
            {
                ExceptionManager.Handle(ex);
            }
        }
        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {

        }
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {

        }
        private void btnObits_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dgMosques.SelectedItem != null)
                {
                    var mosque = dgMosques.SelectedItem as MosqueDto;
                    var window = new SamDesktop.Views.Windows.ObitsWindow(mosque);
                    window.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                ExceptionManager.Handle(ex);
            }
        }
        #endregion

        #region Private Methods:
        private async Task LoadRecords(int cityId)
        {
            progress.IsBusy = true;
            using (var client = HttpUtil.CreateClient())
            {
                var response = await client.GetAsync($"{ApiActions.mosques_findbycity}?cityid={cityId}");
                response.EnsureSuccessStatusCode();
                var list = await response.Content.ReadAsAsync<List<MosqueDto>>();
                ((MosquesVM)DataContext).Mosques = new ObservableCollection<MosqueDto>(list);
                progress.IsBusy = false;
            }
        }
        #endregion
    }
}