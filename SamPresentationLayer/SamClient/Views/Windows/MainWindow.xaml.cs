using ClientModels.Models;
using SamClientDataAccess.Repos;
using SamModels.DTOs;
using SamUtils.Utils;
using SamUxLib.Code.Utils;
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
            Environment.Exit(0);
        }
        #endregion
    }
}
