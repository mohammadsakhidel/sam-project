using ClientModels.Models;
using SamClient.Views.Partials;
using SamClientDataAccess.Repos;
using SamModels.DTOs;
using SamUtils.Constants;
using SamUtils.Utils;
using SamUxLib.Code.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SamClient.Views.Windows
{
    public partial class MainWindow : Window
    {
        #region Ctors:
        public MainWindow()
        {
            try
            {
                #region Show Settings Window:
                using (var srepo = new ClientSettingRepo())
                {
                    var setting = srepo.Get();
                    if (!ClientSetting.IsSettingValid(setting))
                    {
                        var settingWindow = new ClientSettingsWindow();
                        var dialogRes = settingWindow.ShowDialog();
                        if (!dialogRes.HasValue || !dialogRes.Value)
                            Environment.Exit(0);
                    }
                }
                #endregion

                InitializeComponent();
            }
            catch (Exception ex)
            {
                ExceptionManager.Handle(ex);
                Environment.Exit(0);
            }
        }
        #endregion

        #region Event Handlers:
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                #region Load Mosque Info:
                using (var srepo = new ClientSettingRepo())
                using (var mrepo = new MosqueRepo())
                {
                    var setting = srepo.Get();
                    var mosque = mrepo.Get(setting.MosqueID);
                    if (mosque != null)
                    {
                        lblMosqueName.Text = mosque.Name;
                        lblMosqueAddress.Text = mosque.Address;
                    }
                }
                #endregion

                #region Update Services Status:
                UpdateServicesStatus();

                var timerServicesStatus = new Timer(4000);
                timerServicesStatus.Elapsed += (o, ee) =>
                {
                    try
                    {
                        Dispatcher.Invoke(() =>
                        {
                            UpdateServicesStatus();
                        });
                    }
                    catch (Exception ex)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            ExceptionManager.Handle(ex);
                        });
                    }
                };
                timerServicesStatus.Enabled = true;
                #endregion

                LoadContent(new HomePage(this));
            }
            catch (Exception ex)
            {
                ExceptionManager.Handle(ex);
            }
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (e.ChangedButton == MouseButton.Left)
                    this.DragMove();
            }
            catch (Exception ex)
            {
                ExceptionManager.Handle(ex);
            }
        }
        private void btnCloseWindow_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var window = new ClientSettingsWindow();
                window.ShowDialog();
            }
            catch (Exception ex)
            {
                ExceptionManager.Handle(ex);
            }
        }
        private void btnServices_Click(object sender, RoutedEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                ExceptionManager.Handle(ex);
            }
        }
        private void btnSlideShow_Click(object sender, RoutedEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                ExceptionManager.Handle(ex);
            }
        }
        #endregion

        #region Methods:
        private void UpdateServicesStatus()
        {
            btnServices.Tag = GetServicesStatus();
        }
        private string GetServicesStatus()
        {
            const string RUNNING = "running";
            const string STOPPED = "stopped";

            if (VersatileUtil.GetWindowsServiceStatus(WindowsServices.sync_service) != ServiceControllerStatus.Running)
                return STOPPED;

            return RUNNING;
        }
        public void LoadContent(UserControl content)
        {
            brdMainContent.Child = content;
        }
        #endregion
    }
}
