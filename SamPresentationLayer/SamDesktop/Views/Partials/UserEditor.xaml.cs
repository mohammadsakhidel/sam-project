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
using System.Text.RegularExpressions;

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

        #region Props & Fields:
        bool _isEditing = false;

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
                _isEditing = true;
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
            _user.PlainPassword = tbPassword.Password;
            _user.RoleName = cmbRole.SelectedItem != null ? cmbRole.SelectedValue.ToString() : "";
            _user.PhoneNumber = tbPhoneNumber.Text;
            _user.IsApproved = chIsApproved.IsChecked.HasValue && chIsApproved.IsChecked.Value;
        }
        private void UpdateForm()
        {
            tbFirstName.Text = _user.FirstName;
            tbSurname.Text = _user.Surname;
            tbUserName.Text = _user.UserName;
            cmbRole.SelectedValue = _user.RoleName;
            tbPhoneNumber.Text = _user.PhoneNumber;
            chIsApproved.IsChecked = _user.IsApproved;

            tbUserName.IsEnabled = _user.UserName != Values.def_admin_name;
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
        public Tuple<bool, string> IsValid()
        {
            UpdateModel();

            // check required:
            if (_user == null || string.IsNullOrEmpty(_user.FirstName) || string.IsNullOrEmpty(_user.Surname)
                || string.IsNullOrEmpty(_user.UserName) || string.IsNullOrEmpty(_user.PhoneNumber) || cmbRole.SelectedItem == null)
                return new Tuple<bool, string>(false, SamUxLib.Resources.Values.Messages.FillRequiredFields);

            // check user name regex:
            var rgxUserName = new Regex(Patterns.username);
            if (!string.IsNullOrEmpty(tbUserName.Text) && !rgxUserName.IsMatch(tbPhoneNumber.Text))
                return new Tuple<bool, string>(false, SamUxLib.Resources.Values.Messages.InvalidUserNameFormat);

            // check cell phone regex:
            var rgxCellPhone = new Regex(Patterns.cellphone);
            if (!string.IsNullOrEmpty(tbPhoneNumber.Text) && !rgxCellPhone.IsMatch(tbPhoneNumber.Text))
                return new Tuple<bool, string>(false, SamUxLib.Resources.Values.Messages.InvalidCellPhone);

            #region password validations:
            if (!_isEditing || (_isEditing && (!string.IsNullOrEmpty(tbPassword.Password) || !string.IsNullOrEmpty(tbPasswordConfirm.Password))))
            {
                // check password confirmation:
                if (tbPassword.Password != tbPasswordConfirm.Password)
                    return new Tuple<bool, string>(false, SamUxLib.Resources.Values.Messages.InvalidPasswordConfirmation);

                // check user name regex:
                var rgxPassword = new Regex(Patterns.password);
                if (!string.IsNullOrEmpty(tbPassword.Password) && !rgxPassword.IsMatch(tbPassword.Password))
                    return new Tuple<bool, string>(false, SamUxLib.Resources.Values.Messages.InvalidPasswordFormat);
            }
            #endregion

            return new Tuple<bool, string>(true, "");
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
