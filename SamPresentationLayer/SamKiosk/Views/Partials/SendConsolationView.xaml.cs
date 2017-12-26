using SamKiosk.Code.Utils;
using SamKiosk.Views.Windows;
using SamModels.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
    public partial class SendConsolationView : UserControl
    {
        #region Fields:
        byte _step = 1;
        MainWindow _mainWindow;
        #endregion

        #region Ctors:
        public SendConsolationView(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
        }
        #endregion

        #region Event Handlrs:
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                _step = 1;
                ShowStep();
            }
            catch (Exception ex)
            {
                KioskExceptionManager.Handle(ex);
            }
        }
        private async void btnNext_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await NextAsync();
            }
            catch (Exception ex)
            {
                KioskExceptionManager.Handle(ex);
            }
        }
        private void btnPrevious_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Previous();
            }
            catch (Exception ex)
            {
                KioskExceptionManager.Handle(ex);
            }
        }
        #endregion

        #region Methods:
        public void SetNavigationState(bool isNextVisible, bool isNextEnabled, bool isPrevVisible, bool isPrevEnabled)
        {
            btnNext.Visibility = (isNextVisible ? Visibility.Visible : Visibility.Collapsed);
            btnNext.IsEnabled = isNextEnabled;

            btnPrevious.Visibility = (isPrevVisible ? Visibility.Visible : Visibility.Collapsed);
            btnPrevious.IsEnabled = isPrevEnabled;
        }
        public async Task NextAsync()
        {
            if (OnNextAction != null)
            {
                await OnNextAction();
            }

            _step++;
            ShowStep();
        }
        public void NextNoAction()
        {
            _step++;
            ShowStep();
        }
        public void Previous()
        {
            _step--;
            ShowStep();
            OnNextAction = null;
        }
        void ShowStep()
        {
            switch (_step)
            {
                case 1:
                    LoadPartial(new ObitSelectionStep(this));
                    break;
                case 2:
                    LoadPartial(new CustomerInfoStep(this));
                    break;
                case 3:
                    LoadPartial(new TemplateSelectionStep(this));
                    break;
                case 4:
                    LoadPartial(new TemplateInfoStep(this));
                    break;
                case 5:
                    LoadPartial(new PayViaPosStep(this));
                    break;
                case 6:
                    LoadPartial(new SendingResultStep(this));
                    break;
            }
        }
        void LoadPartial(UserControl uc)
        {
            sendConsolationContainer.Content = uc;
        }
        public void Reset()
        {
            SelectedObit = null;
            SelectedTemplate = null;
            SelectedCustomer = null;
            Fields = null;
            CreatedConsolationID = 0;
            OnNextAction = null;
            VerificationSucceeded = null;
            _step = 1;
        }
        #endregion

        #region Props:
        public ObitDto SelectedObit { get; set; }
        public TemplateDto SelectedTemplate { get; set; }
        public CustomerDto SelectedCustomer { get; set; }
        public Dictionary<string, string> Fields { get; set; }
        public int CreatedConsolationID { get; set; }
        public Func<Task> OnNextAction { get; set; }
        public bool? VerificationSucceeded { get; set; }
        #endregion
    }
}
