using SamDesktop.Code.Enums;
using SamDesktop.Code.Utils;
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

namespace SamDesktop.Views.Windows
{
    public partial class Main : Window
    {
        public Main()
        {
            InitializeComponent();
        }

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
            btnMonitoring.Style = style;
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
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
