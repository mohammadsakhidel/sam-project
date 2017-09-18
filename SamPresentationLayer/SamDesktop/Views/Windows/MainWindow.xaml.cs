using SamDesktop.Code.Enums;
using SamUxLib.Code.Utils;
using SamDesktop.Code.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
using SamUxLib.Resources.Values;
using RamancoLibrary.Security.Tokens;
using SamUtils.Enums;

namespace SamDesktop.Views.Windows
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            var loginWindow = new LoginWindow();
            var res = loginWindow.ShowDialog();
            if (res.HasValue && res.Value)
            {
                InitializeComponent();
            }
            else
            {
                Environment.Exit(0);
            }
        }

        #region Event Handlers:
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                lblUserName.Content = $"{Strings.User}: {App.FullName}";
            }
            catch (Exception ex)
            {
                ExceptionManager.Handle(ex);
            }
        }

        private void SectionItem_Clicked(object sender, RoutedEventArgs e)
        {
            try
            {
                var item = (Button)e.Source;
                var vm = (MainVM)DataContext;
                var uc = (UserControl)Assembly.GetExecutingAssembly().CreateInstance($"SamDesktop.Views.Partials.{item.Tag.ToString()}");
                vm.SelectedSectionName = item.Tag.ToString();
                vm.SelectedSectionContent = uc;

                HighlightSelectedItem(item);
            }
            catch (Exception ex)
            {
                ExceptionManager.Handle(ex);
            }
        }
        #endregion

        #region Private Methods:
        private void HighlightSelectedItem(Button selectedButton)
        {
            var style = FindResource("sidebar_item") as Style;
            var selectedStyle = FindResource("sidebar_item_selected") as Style;

            //clear all:
            btnMosques.Style = style;
            btnTemplates.Style = style;
            btnBanners.Style = style;
            btnSettings.Style = style;
            btnAccounts.Style = style;
            btnAccessLevels.Style = style;

            selectedButton.Style = selectedStyle;
        }
        #endregion

        private void btnTest_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBox.Show(SamUxLib.Resources.Values.Messages.AreYouSureToDelete);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
