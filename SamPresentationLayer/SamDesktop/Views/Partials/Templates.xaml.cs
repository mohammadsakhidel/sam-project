using SamUtils.Constants;
using SamUxLib.Code.Utils;
using SamDesktop.Code.ViewModels;
using SamUxLib.Resources.Values;
using SamDesktop.Views.Windows;
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
using SamUtils.Utils;

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
        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                await LoadRecords();
            }
            catch (Exception ex)
            {
                progress.IsBusy = false;
                ExceptionManager.Handle(ex);
            }
        }
        private async void btnNew_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var createTemplateWindow = new CreateTemplateWindow();
                var res = createTemplateWindow.ShowDialog();
                if (res.HasValue && res.Value)
                {
                    await LoadRecords();
                }
            }
            catch (Exception ex)
            {
                ExceptionManager.Handle(ex);
            }
        }
        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dgTemplates.SelectedItem != null)
                {
                    var templateToEdit = dgTemplates.SelectedItem as TemplateDto;
                    var editTemplateWindow = new EditTemplateWindow(templateToEdit);
                    editTemplateWindow.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                ExceptionManager.Handle(ex);
            }
        }
        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dgTemplates.SelectedItem != null)
                {
                    var result = UxUtil.ShowQuestion(Messages.AreYouSureToDelete);
                    if (result == MessageBoxResult.Yes)
                    {
                        var templateToDelete = dgTemplates.SelectedItem as TemplateDto;
                        #region Call Server To Delete:
                        progress.IsBusy = true;
                        using (var hc = HttpUtil.CreateClient())
                        {
                            var response = await hc.DeleteAsync($"{ApiActions.templates_delete}/{templateToDelete.ID}");
                            #region show not allowed delete message:
                            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                                throw new Exception(Messages.NotAllowedToDeleteItem);
                            #endregion
                            HttpUtil.EnsureSuccessStatusCode(response);
                            UxUtil.ShowMessage(Messages.SuccessfullyDone);
                            await LoadRecords();
                        }
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                progress.IsBusy = false;
                ExceptionManager.Handle(ex);
            }
        }
        private void btnCategories_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var window = new ManageCategoriesWindow();
                window.ShowDialog();
            }
            catch (Exception ex)
            {
                ExceptionManager.Handle(ex);
            }
        }
        #endregion

        #region Private Methods:
        private async Task LoadRecords()
        {
            progress.IsBusy = true;
            using (var client = HttpUtil.CreateClient())
            {
                var response = await client.GetAsync($"{ApiActions.templates_all}?onlyactives=false");
                HttpUtil.EnsureSuccessStatusCode(response);
                var list = await response.Content.ReadAsAsync<List<TemplateDto>>();
                ((TemplatesVM)DataContext).Templates = new ObservableCollection<TemplateDto>(list);
                progress.IsBusy = false;
            }
        }
        #endregion
    }
}
