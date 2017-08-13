using SamClient.Views.Windows;
using SamUxLib.Code.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
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

namespace SamClient.Views.Partials
{
    public partial class WindowsServices : UserControl
    {
        #region Fields:
        MainWindow _mainWindow;
        #endregion

        #region Ctors:
        public WindowsServices(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
            InitializeComponent();
        }
        #endregion

        #region Event Handlers:
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadServiceStatuses();
            }
            catch (Exception ex)
            {
                ExceptionManager.Handle(ex);
            }
        }
        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _mainWindow.LoadContent(new HomePage(_mainWindow));
            }
            catch (Exception ex)
            {
                ExceptionManager.Handle(ex);
            }
        }
        private async void btnSyncServiceStatus_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var status = VersatileUtil.GetWindowsServiceStatus(SamUtils.Constants.WindowsServices.sync_service);
                if (status == ServiceControllerStatus.Running)
                {
                    await VersatileUtil.StopServiceAsync(SamUtils.Constants.WindowsServices.sync_service);
                }
                else
                {
                    await VersatileUtil.StartServiceAsync(SamUtils.Constants.WindowsServices.sync_service);
                }
                LoadServiceStatuses();
            }
            catch (Exception ex)
            {
                ExceptionManager.Handle(ex);
            }
        }
        #endregion

        #region Methods:
        private void LoadServiceStatuses()
        {
            var RUNNING_COLOR = "#00df6c";
            var STOPPED_COLOR = "#ff2d3f";
            var syncStatus = VersatileUtil.GetWindowsServiceStatus(SamUtils.Constants.WindowsServices.sync_service);

            elSyncService.Fill = (syncStatus == ServiceControllerStatus.Running ? VersatileUtil.GetBrushFromColorCode(RUNNING_COLOR) : VersatileUtil.GetBrushFromColorCode(STOPPED_COLOR));
            btnSyncServiceStatus.Content = (syncStatus == ServiceControllerStatus.Running ? SamUxLib.Resources.ResourceManager.GetValue("StopService") : SamUxLib.Resources.ResourceManager.GetValue("StartService"));
        }
        #endregion
    }
}
