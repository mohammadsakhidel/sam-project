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
    public partial class EditUserWindow : Window
    {
        #region Ctors:
        public EditUserWindow(IdentityUserDto user)
        {
            InitializeComponent();
            ucUserEditor.User = user;
        }
        #endregion

        #region Event Handlers:
        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                #region Validation:
                var validationResult = ucUserEditor.IsValid();
                if (!validationResult.Item1)
                    throw new ValidationException(validationResult.Item2);
                #endregion

                #region Call Server:
                progress.IsBusy = true;
                using (var hc = HttpUtil.CreateClient())
                {
                    // get user:
                    var user = ucUserEditor.User;
                    // call api:
                    var response = await hc.PutAsJsonAsync(ApiActions.account_update, user);
                    HttpUtil.EnsureSuccessStatusCode(response);
                    var apiResult = await response.Content.ReadAsAsync<ApiOperationResult>();
                    #region ui reaction:
                    if (apiResult.Succeeded)
                    {
                        progress.IsBusy = false;
                        UxUtil.ShowMessage(SamUxLib.Resources.Values.Messages.SuccessfullyDone);
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
            if (errCode == ErrorCodes.duplicate_username)
                throw new Exception(Messages.DuplicateUserName);
            else if (errCode == ErrorCodes.invalid_username)
                throw new Exception(Messages.InvalidUserNameFormat);
            else if (errCode == ErrorCodes.invalid_password)
                throw new Exception(Messages.InvalidPasswordFormat);
            else if (errCode == ErrorCodes.sysadmin_username_changing)
                throw new Exception(Messages.SysAdminUserNameImutable);
            else
                throw new Exception(Messages.ErrorOccurredTryAgain);
        }
        #endregion
    }
}
