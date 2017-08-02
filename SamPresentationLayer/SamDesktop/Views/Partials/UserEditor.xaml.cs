using SamModels.DTOs;
using SamUtils.Constants;
using SamUtils.Utils;
using SamUxLib.Code.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
using System.Net.Http;

namespace SamDesktop.Views.Partials
{
    public partial class UserEditor : UserControl
    {
        #region Ctors:
        public UserEditor()
        {
            _user = new IdentityUserDto();
            InitializeComponent();
        }
        #endregion

        #region Props:
        IdentityUserDto _user;
        public IdentityUserDto User
        {
            get
            {
                UpdateModel();
                return _user;
            }
            set
            { 
                _user = value;
                UpdateForm();
            }
        }
        #endregion

        #region Methods:
        private void UpdateModel()
        {
            _user.FirstName = tbFirstName.Text;
            _user.Surname = tbSurname.Text;
            _user.UserName = tbUserName.Text;
            _user.PlainPassword = tbPassword.Text;
            _user.RoleName = cmbRole.SelectedItem != null ? cmbRole.SelectedValue.ToString() : "";
            _user.PhoneNumber = tbPhoneNumber.Text;
            _user.IsApproved = chIsApproved.IsChecked.HasValue && chIsApproved.IsChecked.Value;
        }
        private void UpdateForm()
        {
            tbFirstName.Text = _user.FirstName;
            tbSurname.Text = _user.Surname;
            tbUserName.Text = _user.UserName;
            tbPassword.Text = _user.PlainPassword;
            cmbRole.SelectedValue = _user.RoleName;
            tbPhoneNumber.Text = _user.PhoneNumber;
            chIsApproved.IsChecked = _user.IsApproved;
        }
        private async Task LoadRoles()
        {
            using (var hc = HttpUtil.CreateClient())
            {
                var response = await hc.GetAsync(ApiActions.account_roles);
                HttpUtil.EnsureSuccessStatusCode(response);
                var roles = await response.Content.ReadAsAsync<List<IdentityRoleDto>>();
                cmbRole.ItemsSource = new ObservableCollection<IdentityRoleDto>(roles);
            }
        }
        #endregion

        #region Event Handlers:
        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                await LoadRoles();
            }
            catch (Exception ex)
            {
                ExceptionManager.Handle(ex);
            }
        }
        #endregion
    }
}
