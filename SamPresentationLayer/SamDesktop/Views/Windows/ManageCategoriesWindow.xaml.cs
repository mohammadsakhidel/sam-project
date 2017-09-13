using RamancoCC;
using SamModels.DTOs;
using SamUtils.Constants;
using SamUtils.Utils;
using SamUxLib.Code.Utils;
using SamUxLib.Resources.Values;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace SamDesktop.Views.Windows
{
    public partial class ManageCategoriesWindow : Window
    {
        #region Fields:
        TemplateCategoryDto categoryToEdit = null;
        #endregion

        #region Ctors:
        public ManageCategoriesWindow()
        {
            InitializeComponent();
        }
        #endregion

        #region Event Handlers:
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                await LoadCategoriesAsync();
            }
            catch (Exception ex)
            {
                progress.IsBusy = false;
                ExceptionManager.Handle(ex);
            }
        }
        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var sourceButton = e.Source as ImageButton;
                var category = sourceButton.Tag as TemplateCategoryDto;
                if (category != null)
                {
                    var res = UxUtil.ShowQuestion(Messages.DeleteCategoryWarning);
                    if (res == MessageBoxResult.Yes)
                    {
                        progress.IsBusy = true;
                        using (var hc = HttpUtil.CreateClient())
                        {
                            var response = await hc.DeleteAsync($"{ApiActions.categories_delete}/{category.ID}");
                            #region show not allowed delete message:
                            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                                throw new Exception(Messages.NotAllowedToDeleteItem);
                            #endregion
                            response.EnsureSuccessStatusCode();
                            await LoadCategoriesAsync();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                progress.IsBusy = false;
                ExceptionManager.Handle(ex);
            }
        }
        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var sourceButton = e.Source as ImageButton;
                var category = sourceButton.Tag as TemplateCategoryDto;
                if (category != null)
                {
                    FillForm(category);
                }
            }
            catch (Exception ex)
            {
                ExceptionManager.Handle(ex);
            }
        }
        private async void btnOK_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var model = GetUpdatedModel();
                var isEditing = categoryToEdit != null;
                progress.IsBusy = true;
                #region Create:
                if (!isEditing)
                {
                    using (var hc = HttpUtil.CreateClient())
                    {
                        var response = await hc.PostAsJsonAsync(ApiActions.categories_create, model);
                        response.EnsureSuccessStatusCode();
                        UxUtil.ShowMessage(Messages.SuccessfullyDone);
                        ClearForm();
                        await LoadCategoriesAsync();
                    }
                }
                #endregion
                #region Update:
                else
                {
                    using (var hc = HttpUtil.CreateClient())
                    {
                        var response = await hc.PutAsJsonAsync(ApiActions.categories_update, model);
                        response.EnsureSuccessStatusCode();
                        UxUtil.ShowMessage(Messages.SuccessfullyDone);
                        ClearForm();
                        await LoadCategoriesAsync();
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                progress.IsBusy = false;
                ExceptionManager.Handle(ex);
            }
        }
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ClearForm();
            }
            catch (Exception ex)
            {
                ExceptionManager.Handle(ex);
            }
        }
        #endregion

        #region Methods:
        async Task LoadCategoriesAsync()
        {
            progress.IsBusy = true;
            using (var hc = HttpUtil.CreateClient())
            {
                var response = await hc.GetAsync(ApiActions.categories_all);
                response.EnsureSuccessStatusCode();
                var categories = await response.Content.ReadAsAsync<List<TemplateCategoryDto>>();
                dgItems.ItemsSource = categories;
                progress.IsBusy = false;
            }
        }
        void FillForm(TemplateCategoryDto category)
        {
            tbName.Text = category.Name;
            tbOrder.Text = category.Order.ToString();
            tbDesc.Text = category.Description;
            chVisible.IsChecked = category.Visible;

            categoryToEdit = category;
        }
        void ClearForm()
        {
            tbName.Clear();
            tbOrder.Clear();
            tbDesc.Clear();
            chVisible.IsChecked = false;

            categoryToEdit = null;
        }
        TemplateCategoryDto GetUpdatedModel()
        {
            var model = (categoryToEdit != null ? categoryToEdit : new TemplateCategoryDto());
            model.Name = tbName.Text;
            model.Description = tbDesc.Text;
            model.Order = Convert.ToInt32(tbOrder.Text);
            model.Visible = chVisible.IsChecked.HasValue && chVisible.IsChecked.Value;
            return model;
        }
        #endregion
    }
}