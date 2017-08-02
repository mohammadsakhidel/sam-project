using RamancoLibrary.Security.Tokens;
using SamModels.DTOs;
using SamUtils.Constants;
using SamUtils.Objects.API;
using SamUtils.Objects.Exceptions;
using SamUtils.Utils;
using SamUxLib.Code.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
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
    public partial class LoginWindow : Window
    {
        #region Constructors:
        public LoginWindow()
        {
            InitializeComponent();
        }
        #endregion

        #region Event Handlers:
        private async void btnOK_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                progress.IsBusy = true;
                using (var hc = HttpUtil.CreateClient())
                {
                    var dto = new SignInDto
                    {
                        UserName = tbUserName.Text,
                        Password = tbPassword.Password
                    };
                    var response = await hc.PostAsJsonAsync(ApiActions.account_validateuser, dto);
                    HttpUtil.EnsureSuccessStatusCode(response);
                    progress.IsBusy = false;

                    #region Process Response:
                    var apiResult = await response.Content.ReadAsAsync<ApiOperationResult>();
                    if (apiResult.Succeeded)
                    {
                        var token = apiResult.Data;
                        App.UserToken = new JwtToken(token);
                        this.DialogResult = true;
                        this.Close();
                    }
                    else if (apiResult.ErrorCode == ErrorCodes.invalid_username)
                    {
                        throw new AccessException(SamUxLib.Resources.Values.Messages.InvalidUserName);
                    }
                    else if (apiResult.ErrorCode == ErrorCodes.invalid_password)
                    {
                        throw new AccessException(SamUxLib.Resources.Values.Messages.InvalidPassword);
                    }
                    else if (apiResult.ErrorCode == ErrorCodes.account_blocked)
                    {
                        throw new AccessException(SamUxLib.Resources.Values.Messages.AccountDisabled);
                    }
                    else
                    {
                        throw new AccessException(SamUxLib.Resources.Values.Messages.LoginFailedTryAgain);
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                var typ = ex.GetType();
                progress.IsBusy = false;
                ExceptionManager.Handle(ex);
            }
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                tbUserName.Focus();
            }
            catch (Exception ex)
            {
                ExceptionManager.Handle(ex);
            }
        }
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.DialogResult = false;
                this.Close();
            }
            catch (Exception ex)
            {
                ExceptionManager.Handle(ex);
            }
        }
        #endregion
    }
}
