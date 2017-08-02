using SamDesktop.Code.ViewModels;
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
    public partial class Accounts : UserControl
    {
        #region Ctors:
        public Accounts()
        {
            InitializeComponent();
        }
        #endregion

        #region Event Handlers:
        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
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

        private async void btnNew_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var hc = HttpUtil.CreateClient())
                {
                    var res = await hc.PostAsJsonAsync("account/create", new IdentityUserDto { UserName = "samadaamin" });
                    HttpUtil.EnsureSuccessStatusCode(res);
                }
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

        #region Private Methods:
        private async Task LoadRecords()
        {
            progress.IsBusy = true;
            using (var client = HttpUtil.CreateClient())
            {
                var response = await client.GetAsync(ApiActions.account_all);
                HttpUtil.EnsureSuccessStatusCode(response);
                var list = await response.Content.ReadAsAsync<List<IdentityUserDto>>();
                ((AccountsVM)DataContext).Accounts = new ObservableCollection<IdentityUserDto>(list);
                progress.IsBusy = false;
            }
        }
        #endregion
    }
}
