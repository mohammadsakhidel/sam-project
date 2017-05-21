using SamDesktop.Code.Utils;
using SamDesktop.Code.ViewModels;
using SamModels.DTOs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
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

namespace SamDesktop.Views.Partials
{
    public partial class Templates : UserControl
    {
        #region Ctors:
        public Templates()
        {
            InitializeComponent();
        }
        #endregion

        #region Event Handlers:
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadRecords();
            }
            catch (Exception ex)
            {
                progress.IsBusy = false;
                ExceptionManager.Handle(ex);
            }
        }
        #endregion

        #region Private Methods:
        private async void LoadRecords()
        {
            progress.IsBusy = true;
            using (var client = HttpUtil.CreateClient())
            {
                var response = await client.GetAsync("templates/all");
                response.EnsureSuccessStatusCode();
                var list = await response.Content.ReadAsAsync<List<TemplateDto>>();
                ((TemplatesVM)DataContext).Templates = new ObservableCollection<TemplateDto>(list);
                progress.IsBusy = false;
            }
        }
        #endregion

    }
}
