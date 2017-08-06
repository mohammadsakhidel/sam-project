using SamDesktop.Code.ViewModels;
using SamDesktop.Views.Windows;
using SamModels.DTOs;
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
    public partial class Roles : UserControl
    {
        #region Ctors:
        public Roles()
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
                var window = new CreateRoleWindow();
                var res = window.ShowDialog();
                if (res.HasValue && res.Value)
                {
                    await LoadRecords();
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
                var selectedRole = dgRoles.SelectedItem as IdentityRoleDto;
                if (selectedRole != null)
                {
                    var window = new EditRoleWindow(selectedRole);
                    var res = window.ShowDialog();
                    if (res.HasValue && res.Value)
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
        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedRole = dgRoles.SelectedItem as IdentityRoleDto;
                if (selectedRole != null)
                {
                    var result = UxUtil.ShowQuestion(Messages.AreYouSureToDelete);
                    if (result == MessageBoxResult.Yes)
                    {
                        #region Call Server To Delete:
                        progress.IsBusy = true;
                        using (var hc = HttpUtil.CreateClient())
                        {
                            var response = await hc.DeleteAsync($"{ApiActions.account_deleterole}/{selectedRole.Id}");
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
        #endregion

        #region Private Methods:
        private async Task LoadRecords()
        {
            progress.IsBusy = true;
            using (var client = HttpUtil.CreateClient())
            {
                var response = await client.GetAsync(ApiActions.account_roles);
                HttpUtil.EnsureSuccessStatusCode(response);
                var list = await response.Content.ReadAsAsync<List<IdentityRoleDto>>();
                ((RolesVM)DataContext).Roles = new ObservableCollection<IdentityRoleDto>(list);
                progress.IsBusy = false;
            }
        }
        #endregion
    }
}
