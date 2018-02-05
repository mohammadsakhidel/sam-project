using SamClientDataAccess.ClientModels;
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
        #region Fields:
        List<Timer> _timers;
        #endregion

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
                {
                    var setting = srepo.Get();
                    if (setting != null)
                    {
                        lblMosqueName.Text = setting.MosqueName;
                        lblMosqueAddress.Text = setting.MosqueAddress;
                    }
                }
                #endregion

                #region Timer ::: Update Service Icons Status:
                UpdateServicesStatus();

                _timers = new List<Timer>();

                var serviceStatusUpdateTimer = new Timer(2000);
                serviceStatusUpdateTimer.Elapsed += (o, ee) =>
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
                serviceStatusUpdateTimer.Enabled = true;
                _timers.Add(serviceStatusUpdateTimer);
                #endregion

                #region Timer ::: AutoPlay Check:
                var autoPlayCheckTimer = new Timer(20000);
                autoPlayCheckTimer.Enabled = true;
                autoPlayCheckTimer.Elapsed += AutoPlayCheckTimer_Elapsed;
                _timers.Add(autoPlayCheckTimer);
                #endregion

                LoadContent(new HomePage(this));
                ShowPlayer();
            }
            catch (Exception ex)
            {
                ExceptionManager.Handle(ex);
            }
        }
        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_timers != null && _timers.Any())
                {
                    foreach (var t in _timers)
                    {
                        t.Dispose();
                    }
                }
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
            WindowState = WindowState.Minimized;
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
                LoadContent(new Partials.WindowsServices(this));
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
                ShowPlayer();
            }
            catch (Exception ex)
            {
                ExceptionManager.Handle(ex);
            }
        }
        private void taryQuit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Application.Current.Shutdown();
            }
            catch (Exception ex)
            {
                ExceptionManager.Handle(ex);
            }
        }
        private void tarySettings_Click(object sender, RoutedEventArgs e)
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
        private void tarySlideShow_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ShowPlayer();
            }
            catch (Exception ex)
            {
                ExceptionManager.Handle(ex);
            }
        }
        private void taryOpenApp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ActivateWindow();
            }
            catch (Exception ex)
            {
                ExceptionManager.Handle(ex);
            }
        }
        private void MaximizeCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            try
            {
                e.CanExecute = true;
            }
            catch (Exception ex)
            {
                ExceptionManager.Handle(ex);
            }
        }
        private void MaximizeCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                ActivateWindow();
            }
            catch (Exception ex)
            {
                ExceptionManager.Handle(ex);
            }
        }
        private void AutoPlayCheckTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                var isPlaying = IsPlaying();
                if (!isPlaying)
                {
                    using (var srep = new ClientSettingRepo())
                    using (var orep = new LocalObitRepo(srep.Context))
                    {
                        var setting = srep.Get();
                        if (setting != null && setting.AutoSlideShow)
                        {
                            var activeObits = orep.GetActiveObits();
                            if (activeObits.Any())
                            {
                                Dispatcher.Invoke(() => {
                                    ShowPlayer();
                                });
                            }
                        }
                    }
                }
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

            if (VersatileUtil.GetWindowsServiceStatus(SamUtils.Constants.WindowsServices.sync_service) != ServiceControllerStatus.Running)
                return STOPPED;

            return RUNNING;
        }
        public void LoadContent(UserControl content)
        {
            brdMainContent.Child = content;
        }
        private bool IsPlaying()
        {
            return Dispatcher.Invoke(() =>
            {
                return Application.Current.Windows.OfType<PlayerWindow>().Any();
            });
        }
        private void ActivateWindow()
        {
            WindowState = WindowState.Normal;
            Activate();
            Topmost = true;
            Topmost = false;
            Focus();
        }
        public void ShowPlayer()
        {
            var isPlaying = IsPlaying();
            if (!isPlaying)
            {
                var player = new PlayerWindow();
                player.Show();
            }
        }
        #endregion
    }
}
