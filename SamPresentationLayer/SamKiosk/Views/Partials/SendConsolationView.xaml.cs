using SamKiosk.Code.Utils;
using SamKiosk.Views.Windows;
using SamModels.DTOs;
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
    public partial class SendConsolationView : UserControl
    {
        #region Fields:
        byte _step = 1;
        MainWindow _mainWindow;
        ObitDto _selectedObit;
        TemplateDto _selectedTemplate;
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
        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _step++;
                ShowStep();
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
                _step--;
                ShowStep();
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
        public void Next()
        {
            _step++;
            ShowStep();
        }
        public void Previous()
        {
            _step--;
            ShowStep();
        }
        void ShowStep()
        {
            switch (_step)
            {
                case 1:
                    LoadPartial(new ObitSelectionStep(this));
                    break;
                case 2:
                    LoadPartial(new TemplateSelectionStep(this));
                    break;
                case 3:
                    LoadPartial(new TemplateInfoStep(this));
                    break;
            }
        }
        void LoadPartial(UserControl uc)
        {
            sendConsolationContainer.Content = uc;
        }
        #endregion

        #region Props:
        public ObitDto SelectedObit
        {
            get { return _selectedObit; }
            set { _selectedObit = value; }
        }
        public TemplateDto SelectedTemplate
        {
            get { return _selectedTemplate; }
            set { _selectedTemplate = value; }
        }
        #endregion
    }
}
