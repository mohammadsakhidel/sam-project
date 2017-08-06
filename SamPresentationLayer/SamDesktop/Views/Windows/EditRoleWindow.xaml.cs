using SamModels.DTOs;
using SamUtils.Constants;
using SamUtils.Objects.API;
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
    public partial class EditRoleWindow : Window
    {
        #region Ctor:
        public EditRoleWindow(IdentityRoleDto role)
        {
            InitializeComponent();
            ucRoleEditor.Role = role;
        }
        #endregion

        #region Event Handlers:
        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                #region Validation:
                var validationResult = ucRoleEditor.IsValid();
                if (!validationResult.Item1)
                    throw new ValidationException(validationResult.Item2);
                #endregion

                #region Call Server:
                progress.IsBusy = true;
                using (var hc = HttpUtil.CreateClient())
                {
                    // get user:
                    var role = ucRoleEditor.Role;
                    // call api:
                    var response = await hc.PutAsJsonAsync(ApiActions.account_updaterole, role);
                    HttpUtil.EnsureSuccessStatusCode(response);
                    var apiResult = await response.Content.ReadAsAsync<ApiOperationResult>();
                    #region ui reaction:
                    if (apiResult.Succeeded)
                    {
                        progress.IsBusy = false;
                        UxUtil.ShowMessage(Messages.SuccessfullyDone);
                        DialogResult = true;
                        Close();
                    }
                    else
                    {
                        HandleErrorCode(apiResult.ErrorCode);
                    }
                    #endregion
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

        #region Methods:
        void HandleErrorCode(string errCode)
        {
            if (errCode == ErrorCodes.role_exists)
                throw new Exception(Messages.DuplicateRoleName);
            if (errCode == ErrorCodes.sysadmin_rolename_changing)
                throw new Exception(Messages.SysAdminRoleNameImutable);
            else
                throw new Exception(Messages.ErrorOccurredTryAgain);
        }
        #endregion
    }
}