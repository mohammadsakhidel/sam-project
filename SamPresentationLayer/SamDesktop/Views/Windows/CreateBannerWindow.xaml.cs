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
    public partial class CreateBannerWindow : Window
    {
        #region Ctors:
        public CreateBannerWindow()
        {
            InitializeComponent();
        }
        #endregion

        #region Event Handlers:
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
                    var response = await hc.PostAsJsonAsync<BannerHierarchyDto>(ApiActions.banners_create, banner);
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
