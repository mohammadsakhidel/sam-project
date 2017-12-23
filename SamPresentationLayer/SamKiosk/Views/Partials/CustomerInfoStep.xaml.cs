using SamKiosk.Code.Utils;
using SamModels.DTOs;
using SamUtils.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    public partial class CustomerInfoStep : UserControl
    {
        #region Fields:
        SendConsolationView _parent;
        #endregion

        #region Ctors:
        public CustomerInfoStep(SendConsolationView parent)
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
                _parent.SetNavigationState(true, false, true, true);

                #region state:
                if (_parent.SelectedCustomer != null)
                {
                    tbFullName.Text = _parent.SelectedCustomer.FullName;
                    tbCellPhoneNumber.Text = _parent.SelectedCustomer.CellPhoneNumber;
                }
                #endregion
            }
            catch (Exception ex)
            {
                KioskExceptionManager.Handle(ex);
            }
        }
        private void FormTextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                ValidateForm();
            }
            catch (Exception ex)
            {
                KioskExceptionManager.Handle(ex);
            }
        }
        #endregion

        #region Methods:
        void ValidateForm()
        {
            var isValid = false;

            var hasFullName = !string.IsNullOrEmpty(tbFullName.Text);
            var hasCellPhoneNumber = !string.IsNullOrEmpty(tbCellPhoneNumber.Text) && Regex.IsMatch(tbCellPhoneNumber.Text, Patterns.cellphone);
            isValid = hasFullName && hasCellPhoneNumber;

            if (isValid)
            {
                _parent.SelectedCustomer = new CustomerDto
                {
                    FullName = tbFullName.Text,
                    CellPhoneNumber = tbCellPhoneNumber.Text
                };
                _parent.SetNavigationState(true, true, true, true);
            }
            else
            {
                _parent.SetNavigationState(true, false, true, true);
            }
        }
        #endregion
    }
}
