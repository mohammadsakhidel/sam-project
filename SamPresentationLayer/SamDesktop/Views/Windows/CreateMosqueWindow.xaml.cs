using SamUtils.Constants;
using SamUtils.Objects.Exceptions;
using SamUtils.Utils;
using SamUxLib.Code.Utils;
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
    public partial class CreateMosqueWindow : Window
    {
        #region Ctors:
        public CreateMosqueWindow()
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
                var validationResult = ucMosqueEditor.IsValid();
                if (!validationResult.Item1)
                    throw new ValidationException(validationResult.Item2);
                #endregion

                #region Call Server:
                progress.IsBusy = true;
                using (var hc = HttpUtil.CreateClient())
                {
                    // get mosque:
                    var mosque = ucMosqueEditor.Mosque;
                    mosque.Creator = App.UserName;
                    // call api:
                    var response = await hc.PostAsJsonAsync(ApiActions.mosques_create, mosque);
                    response.EnsureSuccessStatusCode();
                    // ui reaction:
                    progress.IsBusy = false;
                    UxUtil.ShowMessage(SamUxLib.Resources.Values.Messages.SuccessfullyDone);
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
