using SamUxLib.Code.Utils;
using SamDesktop.Code.ViewModels;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using SamModels.DTOs;
using System.Net.Http;
using System.Collections.ObjectModel;
using SamUtils.Objects.Exceptions;
using SamUxLib.Resources.Values;
using Microsoft.Win32;
using System.Drawing;
using SamDesktop.Views.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using RamancoLibrary.Utilities;
using System.Windows.Input;
using System.Drawing.Imaging;
using SamUtils.Constants;
using System.Linq;
using SamUtils.Objects.Presenters;
using SamUtils.Utils;

namespace SamDesktop.Views.Partials
{
    public partial class TemplateEditor : UserControl
    {
        #region Fields:
        Canvas _canvas;
        Border _selectedBox;
        System.Windows.Point _mouseDownPoint;
        string _mouseDownSourceName;
        bool newBackgroundImageSelected = false;

        List<TemplateCategoryDto> _categories = null;
        #endregion

        #region Ctors:
        public TemplateEditor()
        {
            InitializeComponent();
        }
        #endregion

        #region Props:
        private TemplateDto templateToEdit = null;
        public TemplateDto TemplateToEdit
        {
            get
            {
                return templateToEdit;
            }
            set
            {
                templateToEdit = value;
            }
        }
        #endregion

        #region Event Handlers:
        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                #region load data from server:
                progress.IsBusy = true;
                using (var hc = HttpUtil.CreateClient())
                {
                    #region get categories:
                    var catResponse = await hc.GetAsync(ApiActions.categories_all);
                    HttpUtil.EnsureSuccessStatusCode(catResponse);
                    _categories = await catResponse.Content.ReadAsAsync<List<TemplateCategoryDto>>();
                    ((TemplateEditorVM)DataContext).TemplateCategories = new ObservableCollection<TemplateCategoryDto>(_categories);
                    #endregion

                    #region get editing template background image & load template:
                    if (TemplateToEdit != null)
                    {
                        var bgBytes = await hc.GetByteArrayAsync($"{ApiActions.blobs_getimage}/{TemplateToEdit.BackgroundImageID}");
                        LoadTemplate(bgBytes);
                    }
                    #endregion

                    progress.IsBusy = false;
                }
                #endregion

                #region set parent window size changed event handler:
                Window.GetWindow(this).SizeChanged += (o, ee) =>
                {
                    DrawDesigner();
                };
                #endregion
            }
            catch (Exception ex)
            {
                progress.IsBusy = false;
                ExceptionManager.Handle(ex);
            }
        }
        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!ValidateInputs())
                    throw new ValidationException(Messages.FillRequiredFields);

                var isEditing = TemplateToEdit != null;
                var vm = DataContext as TemplateEditorVM;
                progress.IsBusy = true;

                #region Gather Inputs:
                var templateDto = TemplateToEdit ?? new TemplateDto();
                templateDto.Name = vm.Name;
                templateDto.Order = vm.Order;
                templateDto.TemplateCategoryID = vm.TemplateCategory.ID;
                templateDto.BackgroundImageBase64 = isEditing && !newBackgroundImageSelected ? "" : Convert.ToBase64String(IOUtils.BitmapToByteArray(new Bitmap(vm.BackgroundImage), ImageFormat.Jpeg));
                templateDto.Text = vm.Text;
                templateDto.Price = vm.Price;
                templateDto.WidthRatio = vm.AspectRatio.WidthRatio;
                templateDto.HeightRatio = vm.AspectRatio.HeightRatio;
                templateDto.IsActive = vm.IsActive;
                templateDto.TemplateFields = vm.Fields != null && vm.Fields.Count > 0 ? (new List<TemplateFieldDto>(vm.Fields)).ToArray() : null;
                templateDto.Creator = App.UserName;
                #endregion
                #region CALL API ::: Craete Template:
                if (!isEditing)
                {
                    using (var hc = HttpUtil.CreateClient())
                    {
                        var response = await hc.PostAsJsonAsync(ApiActions.templates_create, templateDto);
                        HttpUtil.EnsureSuccessStatusCode(response);
                        progress.IsBusy = false;
                        UxUtil.ShowMessage(Messages.SuccessfullyDone);
                        ClearInputs();
                        Window.GetWindow(this).DialogResult = true;
                    }
                }
                #endregion
                #region CALL API ::: Update Template:
                else
                {
                    using (var hc = HttpUtil.CreateClient())
                    {
                        var response = await hc.PutAsJsonAsync(ApiActions.templates_update, templateDto);
                        HttpUtil.EnsureSuccessStatusCode(response);
                        progress.IsBusy = false;
                        UxUtil.ShowMessage(Messages.SuccessfullyDone);
                        Window.GetWindow(this).DialogResult = true;
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
        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (e.Source is TabControl)
                {
                    var tabControl = (TabControl)e.Source;
                    if (tabControl.SelectedIndex == 1 && !IsReadyForDesign())
                    {
                        throw new ValidationException(Messages.FillBeforeDesignTemplate);
                    }
                }
            }
            catch (ValidationException ve)
            {
                ((TabControl)e.Source).SelectedIndex = 0;
                ExceptionManager.Handle(ve);
            }
            catch (Exception ex)
            {
                ExceptionManager.Handle(ex);
            }
        }
        private void btnSelectBackgroundImage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var openfiledialog = new OpenFileDialog();
                openfiledialog.Filter = "Image Files (*.jpg, *.jpeg, *.png) | *.jpg; *.jpeg; *.png";
                openfiledialog.Multiselect = false;
                var res = openfiledialog.ShowDialog();
                if (res.HasValue && res.Value)
                {
                    newBackgroundImageSelected = true;
                    Bitmap backgroundImage = new Bitmap(openfiledialog.FileName);
                    ((TemplateEditorVM)DataContext).BackgroundImage = backgroundImage;
                    tbBackgroundImageFileName.Text = openfiledialog.FileName;
                }
            }
            catch (Exception ex)
            {
                ExceptionManager.Handle(ex);
            }
        }
        private void btnNewTemplateField_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var window = new CreateTemplateFieldWindow();
                var res = window.ShowDialog();
                if (res.HasValue && res.Value)
                {
                    var field = window.Field;
                    var fields = ((TemplateEditorVM)DataContext).Fields ?? new ObservableCollection<TemplateFieldDto>();
                    fields.Add(field);
                    ((TemplateEditorVM)DataContext).Fields = fields;
                    DrawDesigner();
                }
            }
            catch (Exception ex)
            {
                ExceptionManager.Handle(ex);
            }
        }
        private void lvTemplateFields_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (lvTemplateFields.Items.Count > 0 && lvTemplateFields.SelectedItem != null)
                {
                    var index = lvTemplateFields.SelectedIndex;
                    var window = new EditTemplateFieldWindow(lvTemplateFields.SelectedItem as TemplateFieldDto);
                    var res = window.ShowDialog();
                    if (res.HasValue && res.Value)
                    {
                        var field = window.Field;
                        var fields = ((TemplateEditorVM)DataContext).Fields ?? new ObservableCollection<TemplateFieldDto>();
                        fields[index] = field;
                        ((TemplateEditorVM)DataContext).Fields = fields;
                        DrawDesigner();
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionManager.Handle(ex);
            }
        }
        private void btnDeleteTemplateField_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (lvTemplateFields.SelectedItem != null)
                {
                    ((TemplateEditorVM)DataContext).Fields.Remove(lvTemplateFields.SelectedItem as TemplateFieldDto);
                    DrawDesigner();
                }
            }
            catch (Exception ex)
            {
                ExceptionManager.Handle(ex);
            }
        }
        private void pallet_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                DrawDesigner();
            }
            catch (Exception ex)
            {
                ExceptionManager.Handle(ex);
            }
        }
        private void _canvas_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.LeftButton == MouseButtonState.Pressed && _selectedBox != null && _mouseDownPoint != null)
                {
                    if (_mouseDownSourceName == "box" || _mouseDownSourceName == "text")
                    {
                        var newX = e.GetPosition(_canvas).X - _mouseDownPoint.X;
                        var newY = e.GetPosition(_canvas).Y - _mouseDownPoint.Y;
                        Canvas.SetLeft(_selectedBox, newX);
                        Canvas.SetTop(_selectedBox, newY);

                        ((TemplateFieldDto)_selectedBox.Tag).X = newX / _canvas.ActualWidth * 100;
                        ((TemplateFieldDto)_selectedBox.Tag).Y = newY / _canvas.ActualHeight * 100;
                    }
                    else if (_mouseDownSourceName == "top")
                    {
                        var boxTop = Canvas.GetTop(_selectedBox);
                        var newTop = e.GetPosition(_canvas).Y;

                        Canvas.SetTop(_selectedBox, newTop);
                        _selectedBox.Height += boxTop - newTop;

                        ((TemplateFieldDto)_selectedBox.Tag).Y = newTop / _canvas.ActualHeight * 100;
                        ((TemplateFieldDto)_selectedBox.Tag).BoxHeight = _selectedBox.Height * 100 / _canvas.ActualHeight;
                    }
                    else if (_mouseDownSourceName == "left")
                    {
                        var boxLeft = Canvas.GetLeft(_selectedBox);
                        var newLeft = e.GetPosition(_canvas).X;

                        Canvas.SetLeft(_selectedBox, newLeft);
                        _selectedBox.Width += boxLeft - newLeft;

                        ((TemplateFieldDto)_selectedBox.Tag).X = newLeft / _canvas.ActualWidth * 100;
                        ((TemplateFieldDto)_selectedBox.Tag).BoxWidth = _selectedBox.Width * 100 / _canvas.ActualWidth;
                    }
                }
            }
            catch { }
        }
        private void _canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                _selectedBox = null;
            }
            catch { }
        }
        private void Box_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var box = (Border)sender;
                _mouseDownSourceName = ((FrameworkElement)e.Source).Name;
                _selectedBox = box;
                _mouseDownPoint = e.GetPosition(box);
            }
            catch { }
        }
        private void Box_MouseUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                _selectedBox = null;
            }
            catch { }
        }
        private void btnNewCategory_Click(object sender, RoutedEventArgs e)
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

        #region Methods:
        private bool IsReadyForDesign()
        {
            var template = (TemplateEditorVM)DataContext;
            if (template.AspectRatio == null || template.BackgroundImage == null)
                return false;

            return true;
        }
        private void DrawDesigner()
        {
            if (IsReadyForDesign())
            {
                var vm = (TemplateEditorVM)DataContext;
                #region Draw Canvas:
                var canvasContainer = new Border();
                canvasContainer.Style = (Style)FindResource("template_designer_canvas_container");
                canvasContainer.Height = pallet.ActualHeight - 10;
                canvasContainer.Width = (canvasContainer.Height * vm.AspectRatio.WidthRatio) / vm.AspectRatio.HeightRatio;
                canvasContainer.VerticalAlignment = VerticalAlignment.Center;
                canvasContainer.HorizontalAlignment = HorizontalAlignment.Center;

                _canvas = new Canvas();
                _canvas.Background = new ImageBrush(ImageUtils.ToBitmapSource(vm.BackgroundImage));
                _canvas.FlowDirection = FlowDirection.LeftToRight;
                _canvas.ClipToBounds = true;
                _canvas.MouseMove += _canvas_MouseMove;
                _canvas.MouseUp += _canvas_MouseUp;
                #endregion
                #region Draw Field Boxes:
                if (vm.Fields != null)
                {
                    _canvas.Measure(new System.Windows.Size(double.PositiveInfinity, double.PositiveInfinity));
                    _canvas.Arrange(new Rect(0, 0, canvasContainer.Width, canvasContainer.Height));
                    foreach (var field in vm.Fields)
                    {
                        var boxWidth = (field.BoxWidth > 0 ? field.BoxWidth * _canvas.ActualWidth / 100 : 90);
                        var boxHeight = (field.BoxHeight > 0 ? field.BoxHeight * _canvas.ActualHeight / 100 : 25);
                        var box = GetDrawableBox(field, boxWidth, boxHeight);
                        var boxTop = (field.Y > 0 ? field.Y * _canvas.ActualHeight / 100 : 10);
                        var boxLeft = (field.X > 0 ? field.X * _canvas.ActualWidth / 100 : 10);
                        box.MouseDown += Box_MouseDown;
                        box.MouseUp += Box_MouseUp;

                        Canvas.SetTop(box, boxTop);
                        Canvas.SetLeft(box, boxLeft);

                        _canvas.Children.Add(box);
                    }
                }
                #endregion
                #region Aggregation:
                canvasContainer.Child = _canvas;
                pallet.Child = canvasContainer;
                #endregion
            }
        }
        private void ClearDesigner()
        {
            pallet.Child = null;
        }
        private Border GetDrawableBox(TemplateFieldDto field, double width, double height)
        {
            #region TextBlock And MainBox:
            var textblock = new TextBlock();
            textblock.Name = "text";
            textblock.Text = field.DisplayName;
            textblock.VerticalAlignment = ToVerticalAlignment(field.VerticalContentAlignment);
            textblock.HorizontalAlignment = ToHorizontalAlignment(field.HorizontalContentAlignment);
            textblock.Foreground = (!String.IsNullOrEmpty(field.TextColor) ? (SolidColorBrush)(new BrushConverter().ConvertFrom(field.TextColor)) : System.Windows.Media.Brushes.Black);
            textblock.FontWeight = field.Bold.HasValue && field.Bold.Value ? FontWeights.Bold : FontWeights.Normal;

            var box = new Border();
            box.Name = "box";
            box.Style = (Style)FindResource("template_designer_box");
            box.Child = textblock;
            #endregion

            #region DockPanel And Borders:
            var rightBorder = new Border();
            rightBorder.Name = "right";
            rightBorder.Style = (Style)FindResource("template_designer_box_border_right");
            DockPanel.SetDock(rightBorder, Dock.Right);

            var topBorder = new Border();
            topBorder.Name = "top";
            topBorder.Style = (Style)FindResource("template_designer_box_border_top");
            DockPanel.SetDock(topBorder, Dock.Top);

            var leftBorder = new Border();
            leftBorder.Name = "left";
            leftBorder.Style = (Style)FindResource("template_designer_box_border_left");
            DockPanel.SetDock(leftBorder, Dock.Left);

            var bottomBorder = new Border();
            bottomBorder.Name = "bottom";
            bottomBorder.Style = (Style)FindResource("template_designer_box_border_bottom");
            DockPanel.SetDock(bottomBorder, Dock.Bottom);

            var dock = new DockPanel();
            dock.LastChildFill = true;
            dock.Children.Add(rightBorder);
            dock.Children.Add(topBorder);
            dock.Children.Add(leftBorder);
            dock.Children.Add(bottomBorder);
            dock.Children.Add(box);
            #endregion

            #region BoxContainer:
            var boxContainer = new Border();
            boxContainer.Width = width;
            boxContainer.Height = height;
            boxContainer.Tag = field;
            boxContainer.Child = dock;
            #endregion

            return boxContainer;
        }
        private VerticalAlignment ToVerticalAlignment(string alignment)
        {
            if (alignment == SamUtils.Enums.TextAlignment.center.ToString())
                return VerticalAlignment.Center;
            else if (alignment == SamUtils.Enums.TextAlignment.top.ToString())
                return VerticalAlignment.Top;
            else if (alignment == SamUtils.Enums.TextAlignment.bottom.ToString())
                return VerticalAlignment.Bottom;

            return VerticalAlignment.Center;
        }
        private HorizontalAlignment ToHorizontalAlignment(string alignment)
        {
            if (alignment == SamUtils.Enums.TextAlignment.center.ToString())
                return HorizontalAlignment.Center;
            else if (alignment == SamUtils.Enums.TextAlignment.left.ToString())
                return HorizontalAlignment.Left;
            else if (alignment == SamUtils.Enums.TextAlignment.right.ToString())
                return HorizontalAlignment.Right;

            return HorizontalAlignment.Center;
        }
        private bool ValidateInputs()
        {
            var vm = (TemplateEditorVM)DataContext;

            if (string.IsNullOrEmpty(vm.Name))
                return false;
            if (vm.TemplateCategory == null)
                return false;
            if (vm.BackgroundImage == null)
                return false;
            if (string.IsNullOrEmpty(vm.Text))
                return false;
            if (vm.Price <= 0)
                return false;
            if (vm.Order <= 0)
                return false;
            if (vm.AspectRatio == null)
                return false;

            return true;
        }
        private void ClearInputs()
        {
            var vm = DataContext as TemplateEditorVM;
            vm.Name = "";
            vm.Order = 0;
            vm.TemplateCategory = null;
            vm.BackgroundImage = null;
            vm.Text = "";
            vm.Price = 0;
            vm.AspectRatio = null;
            vm.IsActive = false;
            vm.Fields = null;
            tbBackgroundImageFileName.Clear();
            ClearDesigner();
            tabDesigner.SelectedIndex = 0;
        }

        /// <summary>
        /// This method should be called after categories list loaded.
        /// </summary>
        private void LoadTemplate(byte[] backgroundImageBytes)
        {
            if (TemplateToEdit != null)
            {
                var vm = DataContext as TemplateEditorVM;
                vm.Name = TemplateToEdit.Name;
                vm.Order = TemplateToEdit.Order;
                vm.TemplateCategory = _categories != null ? _categories.SingleOrDefault(c => c.ID == TemplateToEdit.TemplateCategoryID) : null;
                vm.BackgroundImage = IOUtils.ByteArrayToBitmap(backgroundImageBytes);
                vm.Text = TemplateToEdit.Text;
                vm.Price = TemplateToEdit.Price;
                cmbAspectRatio.SelectedValue = vm.AspectRatios.SingleOrDefault(r => r.WidthRatio == TemplateToEdit.WidthRatio && r.HeightRatio == TemplateToEdit.HeightRatio)?.ID;
                vm.IsActive = TemplateToEdit.IsActive;
                vm.Fields = new ObservableCollection<TemplateFieldDto>(TemplateToEdit.TemplateFields);
            }
        }
        #endregion
    }
}