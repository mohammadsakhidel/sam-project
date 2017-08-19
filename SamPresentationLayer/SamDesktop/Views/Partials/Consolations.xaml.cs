using SamDesktop.Code.ViewModels;
using SamDesktop.Views.Windows;
using SamModels.DTOs;
using SamUtils.Constants;
using SamUtils.Enums;
using SamUtils.Utils;
using SamUxLib.Code.Utils;
using SamUxLib.Resources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace SamDesktop.Views.Partials
{
    public partial class Consolations : UserControl
    {
        #region Fields:
        int _autoRefreshInterval = 30000;
        List<System.Timers.Timer> _timers;
        #endregion

        #region Ctors:
        public Consolations()
        {
            InitializeComponent();
        }
        #endregion

        #region Event Handlers:
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var vm = DataContext as ConsolationsVM;

                #region load provices combo items:
                var provinces = CityUtil.Provinces;
                provinces.Insert(0, new ProvinceDto { ID = 0, Name = ResourceManager.GetValue("All") });
                vm.Provinces = new ObservableCollection<ProvinceDto>(provinces);
                #endregion

                #region load statuses combo items:
                var kvPairs = Enum.GetValues(typeof(ConsolationStatus))
                    .Cast<ConsolationStatus>().ToList()
                    .Select(s => new KeyValuePair<string, string>(s.ToString(),
                        ResourceManager.GetValue($"ConsolationStatus_{s.ToString()}", "Enums")))
                    .ToList();
                kvPairs.Insert(0, new KeyValuePair<string, string>("", ResourceManager.GetValue("All")));
                vm.Statuses = new ObservableCollection<KeyValuePair<string, string>>(kvPairs);
                #endregion

                #region start auto refresh timer:
                _timers = new List<System.Timers.Timer>();

                var autoRefreshTimer = new System.Timers.Timer(_autoRefreshInterval);
                autoRefreshTimer.Elapsed += (oo, ee) =>
                {
                    try
                    {
                        Dispatcher.Invoke(async () =>
                        {
                            await FilterRecords();
                        });
                    }
                    catch (Exception ex)
                    {
                        ExceptionManager.Handle(ex);
                    }
                };
                autoRefreshTimer.Enabled = true;

                _timers.Add(autoRefreshTimer);
                #endregion
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
                try
                {
                    if (cmbProvince.SelectedItem != null)
                    {
                        if (cmbProvince.SelectedIndex > 0)
                        {
                            var prov = (ProvinceDto)cmbProvince.SelectedItem;
                            var cities = CityUtil.GetProvinceCities(prov.ID);
                            cities.Insert(0, new CityDto { ID = 0, Name = ResourceManager.GetValue("All") });
                            var vm = DataContext as ConsolationsVM;
                            vm.Cities = new ObservableCollection<CityDto>(cities);
                        }
                        else
                        {
                            var cities = new List<CityDto>();
                            cities.Insert(0, new CityDto { ID = 0, Name = ResourceManager.GetValue("All") });
                            var vm = DataContext as ConsolationsVM;
                            vm.Cities = new ObservableCollection<CityDto>(cities);
                        }
                        cmbCity.SelectedIndex = 0;
                    }
                }
                catch (Exception ex)
                {
                    ExceptionManager.Handle(ex);
                }
            }
            catch (Exception ex)
            {
                ExceptionManager.Handle(ex);
            }
        }
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                ExceptionManager.Handle(ex);
            }
        }
        private async void btnFilter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await FilterRecords();
            }
            catch (Exception ex)
            {
                ExceptionManager.Handle(ex);
            }
        }
        private async void ChangeConsolation_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var btn = e.Source as Button;
                var consolationToEdit = btn.Tag as ConsolationDto;
                if (consolationToEdit != null)
                {
                    var editWindow = new EditConsolationWindow(consolationToEdit);
                    var res = editWindow.ShowDialog();
                    if (res.HasValue && res.Value)
                    {
                        await FilterRecords();
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionManager.Handle(ex);
            }
        }
        #endregion

        #region Methods:
        private async Task FilterRecords()
        {
            var selectedCity = cmbCity.SelectedIndex > 0 ? cmbCity.SelectedItem as CityDto : null;
            var selectedStatus = cmbStatus.SelectedIndex > 0 ? cmbStatus.SelectedValue.ToString() : "";
            var count = Convert.ToInt32((cmbCount.SelectedItem as ComboBoxItem).Content.ToString());
            await LoadRecords(selectedCity != null ? selectedCity.ID : 0, selectedStatus, count);
        }
        private async Task LoadRecords(int cityId, string status, int count)
        {
            progress.IsBusy = true;
            using (var client = HttpUtil.CreateClient())
            {
                var url = $"{ApiActions.consolations_filter}?cityid={cityId}&status={status}{(count > 0 ? $"&count={count}" : "")}";
                var response = await client.GetAsync(url);
                HttpUtil.EnsureSuccessStatusCode(response);
                var list = await response.Content.ReadAsAsync<List<ConsolationDto>>();
                ((ConsolationsVM)DataContext).Consolations = new ObservableCollection<ConsolationDto>(list);
                progress.IsBusy = false;
            }
        }
        #endregion
    }
}
