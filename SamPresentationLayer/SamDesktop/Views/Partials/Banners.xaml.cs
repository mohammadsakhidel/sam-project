using RamancoLibrary.Utilities;
using SamDesktop.Code.ViewModels;
using SamDesktop.Views.Windows;
using SamModels.DTOs;
using SamModels.Enums;
using SamUtils.Constants;
using SamUtils.Utils;
using SamUxLib.Code.Utils;
using SamUxLib.Resources.Values;
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
        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var bannerTypesDic = UxUtil.EnumToDic(typeof(BannerType), "BannerType_");
                var bannerTypePairs = bannerTypesDic.ToList();
                bannerTypePairs.Insert(0, new KeyValuePair<string, string>("", Strings.All));
                cmbBannerType.ItemsSource = bannerTypePairs;

                var vm = DataContext as BannersVM;
                vm.Token = App.UserToken.ToString();
                await LoadRecords();
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
                if (dgBanners.SelectedItems != null && dgBanners.SelectedItems.Count > 0)
                {
                    var result = UxUtil.ShowQuestion(Messages.AreYouSureToDelete);
                    if (result == MessageBoxResult.Yes)
                    {
                        var bannersToDelete = dgBanners.SelectedItems.Cast<BannerHierarchyDto>();
                        #region Call Server To Delete:
                        progress.IsBusy = true;
                        using (var hc = HttpUtil.CreateClient())
                        {
                            var response = await hc.PutAsJsonAsync($"{ApiActions.banners_deleteall}", bannersToDelete.Select(b => b.ID).ToArray());
                            HttpUtil.EnsureSuccessStatusCode(response);
                            UxUtil.ShowMessage(Messages.SuccessfullyDone);
                            await LoadRecords();
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
        private async void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var banner = dgBanners.SelectedItem as BannerHierarchyDto;
                if (banner != null)
                {
                    var window = new EditBannerWindow(banner);
                    var result = window.ShowDialog();
                    if (result.HasValue && result.Value)
                    {
                        await LoadRecords();
                    }
                }
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
                    await LoadRecords();
                }
            }
            catch (Exception ex)
            {
                ExceptionManager.Handle(ex);
            }
        }
        private async void dgBanners_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var banner = dgBanners.SelectedItem as BannerHierarchyDto;
                if (banner != null)
                {
                    progress.IsBusy = true;
                    using (var hc = HttpUtil.CreateClient())
                    {
                        var bytes = await hc.GetByteArrayAsync($"{ApiActions.blobs_getimage}/{banner.ImageID}");
                        var bitmap = IOUtils.ByteArrayToBitmap(bytes);
                        var window = new ImageViewer(bitmap);
                        progress.IsBusy = false;
                        window.ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                progress.IsBusy = false;
                ExceptionManager.Handle(ex);
            }
        }
        private async void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await LoadRecords();
            }
            catch (Exception ex)
            {
                progress.IsBusy = false;
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
                if (cmbBannerType.SelectedIndex > 0)
                {
                    #region get by type:
                    var bannerType = cmbBannerType.SelectedValue.ToString();
                    var url = $"{ApiActions.banners_findbytype}?bannerType={bannerType}{(cmbCount.SelectedItem != null ? "&count=" + cmbCount.SelectedItem.ToString() : "")}";
                    var response = await client.GetAsync(url);
                    HttpUtil.EnsureSuccessStatusCode(response);
                    var list = await response.Content.ReadAsAsync<List<BannerHierarchyDto>>();
                    ((BannersVM)DataContext).Banners = new ObservableCollection<BannerHierarchyDto>(list);
                    #endregion
                }
                else
                {
                    #region get latests:
                    var url = $"{ApiActions.banners_getlatests}{(cmbCount.SelectedItem != null ? "?count=" + cmbCount.SelectedItem.ToString() : "")}";
                    var response = await client.GetAsync(url);
                    HttpUtil.EnsureSuccessStatusCode(response);
                    var list = await response.Content.ReadAsAsync<List<BannerHierarchyDto>>();
                    ((BannersVM)DataContext).Banners = new ObservableCollection<BannerHierarchyDto>(list);
                    #endregion
                }

                progress.IsBusy = false;
            }
        }
        #endregion
    }
}
