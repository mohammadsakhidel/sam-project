using SamDesktop.Code.ViewModels;
using SamDesktop.Views.Windows;
using SamModels.DTOs;
using SamUtils.Constants;
using SamUtils.Utils;
using SamUxLib.Code.Utils;
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
    public partial class Banners : UserControl
    {
        #region Ctors:
        public Banners()
        {
            InitializeComponent();
        }
        #endregion

        #region Event Handlers:
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                //LoadRecords();
            }
            catch (Exception ex)
            {
                progress.IsBusy = false;
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
        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            try
            {

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
                var window = new CreateBannerWindow();
                var result = window.ShowDialog();
                if (result.HasValue && result.Value)
                {
                    //await LoadRecords();
                }
            }
            catch (Exception ex)
            {
                ExceptionManager.Handle(ex);
            }
        }
        #endregion

        #region Private Methods:
        private async Task LoadRecords()
        {
            progress.IsBusy = true;
            using (var client = HttpUtil.CreateClient())
            {
                var response = await client.GetAsync($"{ApiActions.banners_getlatests}");
                HttpUtil.EnsureSuccessStatusCode(response);
                var list = await response.Content.ReadAsAsync<List<BannerHierarchyDto>>();
                ((BannersVM)DataContext).Banners = new ObservableCollection<BannerHierarchyDto>(list);
                progress.IsBusy = false;
            }
        }
        #endregion
    }
}
