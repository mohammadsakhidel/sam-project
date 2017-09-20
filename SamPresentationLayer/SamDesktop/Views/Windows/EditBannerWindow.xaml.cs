using SamModels.DTOs;
using SamUtils.Constants;
using SamUtils.Objects.Exceptions;
using SamUtils.Utils;
using SamUxLib.Code.Utils;
using SamUxLib.Resources.Values;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace SamDesktop.Views.Windows
{
    public partial class EditBannerWindow : Window
    {
        #region Fields:
        BannerHierarchyDto _bannerToEdit;
        #endregion

        #region Ctors:
        public EditBannerWindow(BannerHierarchyDto bannerToEdit)
        {
            _bannerToEdit = bannerToEdit;
            InitializeComponent();
        }
        #endregion

        #region Event Handlers:
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                #region Call API to get data:
                progress.IsBusy = true;
                using (var hc = HttpUtil.CreateClient())
                {
                    // get banner data:
                    var response = await hc.GetAsync($"{ApiActions.banners_find}/{_bannerToEdit.ID}");
                    response.EnsureSuccessStatusCode();
                    _bannerToEdit = await response.Content.ReadAsAsync<BannerHierarchyDto>();
                    // get image bytes:
                    var imageResponse = await hc.GetByteArrayAsync($"{ApiActions.blobs_getimage}/{_bannerToEdit.ImageID}");
                    _bannerToEdit.ImageBase64 = Convert.ToBase64String(imageResponse);
                    progress.IsBusy = false;
                }
                #endregion

                #region set banners province id:
                if (_bannerToEdit.CityID.HasValue)
                {
                    var province = CityUtil.GetProvince(_bannerToEdit.CityID.Value);
                    _bannerToEdit.ProvinceID = province.ID;
                }
                #endregion

                ucBannerEditor.Banner = _bannerToEdit;
            }
            catch (Exception ex)
            {
                progress.IsBusy = false;
                ExceptionManager.Handle(ex);
            }
        }
        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                #region Validation:
                var validationResult = ucBannerEditor.IsValid();
                if (!validationResult.Item1)
                    throw new ValidationException(validationResult.Item2);
                #endregion

                #region Call API:
                progress.IsBusy = true;
                using (var hc = HttpUtil.CreateClient())
                {
                    var banner = ucBannerEditor.Banner;
                    var response = await hc.PutAsJsonAsync(ApiActions.banners_update, banner);
                    response.EnsureSuccessStatusCode();
                    progress.IsBusy = false;
                    UxUtil.ShowMessage(Messages.SuccessfullyDone);
                    DialogResult = true;
                    Close();
                }
                #endregion
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
