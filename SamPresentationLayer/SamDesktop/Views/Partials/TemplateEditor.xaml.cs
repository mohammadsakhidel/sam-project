using SamDesktop.Code.Utils;
using SamDesktop.Code.ViewModels;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using SamModels.DTOs;
using System.Net.Http;
using System.Collections.ObjectModel;
using SamDesktop.Code.Exceptions;
using SamDesktop.Resources.Values;
using Microsoft.Win32;
using System.Drawing;
using SamDesktop.Views.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using RamancoLibrary.Utilities;
using System.Windows.Input;

namespace SamDesktop.Views.Partials
{
    public partial class TemplateEditor : UserControl
    {
        #region Fields:
        Canvas _canvas;
        Border _selectedBox;
        System.Windows.Point _mouseDownPoint;
        string _mouseDownSourceName;
        #endregion

        #region Ctors:
        public TemplateEditor()
        {
            InitializeComponent();
        }
        #endregion

        #region Event Handlers:
        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                #region load categories:
                progress.IsBusy = true;
                using (var hc = HttpUtil.CreateClient())
                {
                    var response = await hc.GetAsync("categories/all");
                    response.EnsureSuccessStatusCode();
                    var categories = await response.Content.ReadAsAsync<List<TemplateCategoryDto>>();
                    ((TemplateEditorVM)DataContext).TemplateCategories = new ObservableCollection<TemplateCategoryDto>(categories);
                    progress.IsBusy = false;
                }
                #endregion

                Window.GetWindow(this).SizeChanged += (o, ee) => {
                    DrawDesigner();
                };
            }
            catch (Exception ex)
            {
                progress.IsBusy = false;
                ExceptionManager.Handle(ex);
            }
        }
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var vm = (TemplateEditorVM)DataContext;
                
            }
            catch (Exception ex)
            {
                ExceptionManager.Handle(ex);
            }
        }
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            e.Handled = true;
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
        private void btnNewExtraField_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var window = new CreateTemplateExtraField();
                var res = window.ShowDialog();
                if (res.HasValue && res.Value)
                {
                    var field = window.ExtraField;
                    var fields = ((TemplateEditorVM)DataContext).ExtraFields ?? new ObservableCollection<TemplateExtraFieldDto>();
                    fields.Add(field);
                    ((TemplateEditorVM)DataContext).ExtraFields = fields;
                    DrawDesigner();
                }
            }
            catch (Exception ex)
            {
                ExceptionManager.Handle(ex);
            }
        }
        private void lvExtraFields_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (lvExtraFields.Items.Count > 0 && lvExtraFields.SelectedItem != null)
                {
                    var index = lvExtraFields.SelectedIndex;
                    var window = new EditTemplateExtraField(lvExtraFields.SelectedItem as TemplateExtraFieldDto);
                    var res = window.ShowDialog();
                    if (res.HasValue && res.Value)
                    {
                        var field = window.ExtraField;
                        var fields = ((TemplateEditorVM)DataContext).ExtraFields ?? new ObservableCollection<TemplateExtraFieldDto>();
                        fields[index] = field;
                        ((TemplateEditorVM)DataContext).ExtraFields = fields;
                        DrawDesigner();
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionManager.Handle(ex);
            }
        }
        private void btnDeleteExtraField_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (lvExtraFields.SelectedItem != null)
                {
                    ((TemplateEditorVM)DataContext).ExtraFields.Remove(lvExtraFields.SelectedItem as TemplateExtraFieldDto);
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

                        ((TemplateExtraFieldDto)_selectedBox.Tag).X = newX / _canvas.ActualWidth * 100;
                        ((TemplateExtraFieldDto)_selectedBox.Tag).Y = newY / _canvas.ActualHeight * 100;
                    }
                    else if (_mouseDownSourceName == "top")
                    {
                        var boxTop = Canvas.GetTop(_selectedBox);
                        var newTop = e.GetPosition(_canvas).Y;

                        Canvas.SetTop(_selectedBox, newTop);
                        _selectedBox.Height += boxTop - newTop;

                        ((TemplateExtraFieldDto)_selectedBox.Tag).Y = newTop / _canvas.ActualHeight * 100;
                        ((TemplateExtraFieldDto)_selectedBox.Tag).BoxHeight = _selectedBox.Height * 100 / _canvas.ActualHeight;
                    }
                    else if (_mouseDownSourceName == "left")
                    {
                        var boxLeft = Canvas.GetLeft(_selectedBox);
                        var newLeft = e.GetPosition(_canvas).X;

                        Canvas.SetLeft(_selectedBox, newLeft);
                        _selectedBox.Width += boxLeft - newLeft;

                        ((TemplateExtraFieldDto)_selectedBox.Tag).X = newLeft / _canvas.ActualWidth * 100;
                        ((TemplateExtraFieldDto)_selectedBox.Tag).BoxWidth = _selectedBox.Width * 100 / _canvas.ActualWidth;
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
                if (vm.ExtraFields != null)
                {
                    _canvas.Measure(new System.Windows.Size(double.PositiveInfinity, double.PositiveInfinity));
                    _canvas.Arrange(new Rect(0, 0, canvasContainer.Width, canvasContainer.Height));
                    foreach (var field in vm.ExtraFields)
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

        private Border GetDrawableBox(TemplateExtraFieldDto field, double width, double height)
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

        #endregion
    }
}
