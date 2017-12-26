using SamKiosk.Code.Utils;
using SamUxLib.Resources.Values;
using System;
using System.Collections.Generic;
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

namespace SamKiosk.Views.Partials
{
    public partial class SendingResultStep : UserControl
    {
        #region Fields:
        SendConsolationView _parent;
        #endregion

        #region Ctors:
        public SendingResultStep(SendConsolationView parent)
        {
            InitializeComponent();
            _parent = parent;
        }
        #endregion

        #region Event Handlers:
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                #region show message:
                if (_parent.VerificationSucceeded.HasValue && _parent.VerificationSucceeded.Value)
                {
                    lblMessage.Text = Messages.ConsolationSuccessfullySent;
                    lblMessage.Style = FindResource("kiosk_result_message_success") as Style;
                }
                else
                {
                    lblMessage.Text = Messages.ConsolationSendingFailed;
                    lblMessage.Style = FindResource("kiosk_result_message_error") as Style;
                }
                #endregion

                _parent.SetNavigationState(false, false, false, false);
                _parent.Reset();
            }
            catch (Exception ex)
            {
                KioskExceptionManager.Handle(ex);
            }
        }
        #endregion
    }
}
