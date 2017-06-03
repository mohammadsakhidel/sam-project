using SamDesktop.Code.Utils;
using SamModels.DTOs;
using SamUtils.Classes;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SamDesktop.Views.Partials
{
    public partial class TemplateFieldEditor : UserControl
    {
        #region Ctors:
        public TemplateFieldEditor()
        {
            InitializeComponent();
        }
        #endregion

        #region Props:
        private TemplateFieldDto fieldToEdit = null;
        public TemplateFieldDto FieldToEdit
        {
            get
            {
                return fieldToEdit;
            }
            set
            {
                fieldToEdit = value;
                LoadForm();
            }
        }
        #endregion

        #region Methods:
        private void LoadForm()
        {
            cmbName.Text = FieldToEdit.Name;
            tbDisplayName.Text = FieldToEdit.DisplayName;
            tbDescription.Text = FieldToEdit.Description;
            if (!String.IsNullOrEmpty(FieldToEdit.FontFamily))
                cmbFontFamily.Text = FieldToEdit.FontFamily;
            if (!String.IsNullOrEmpty(FieldToEdit.FontSize))
                cmbFontSize.SelectedValue = FieldToEdit.FontSize;
            if (!String.IsNullOrEmpty(FieldToEdit.TextColor))
            {
                var brushConverter = new BrushConverter();
                var brush = (SolidColorBrush)brushConverter.ConvertFrom(FieldToEdit.TextColor);
                colorText.SelectedColor = brush.Color;
            }
            if (!String.IsNullOrEmpty(FieldToEdit.HorizontalContentAlignment))
                cmbHorizontalAlignment.SelectedValue = FieldToEdit.HorizontalContentAlignment;
            if (!String.IsNullOrEmpty(FieldToEdit.VerticalContentAlignment))
                cmbVerticalAlignment.SelectedValue = FieldToEdit.VerticalContentAlignment;
            chBold.IsChecked = FieldToEdit.Bold.HasValue && FieldToEdit.Bold.Value;
            chLeftToRightDirection.IsChecked = FieldToEdit.FlowDirection == SamUtils.Enums.FlowDirection.ltr.ToString();
            chWrapContent.IsChecked = FieldToEdit.WrapContent.HasValue && FieldToEdit.WrapContent.Value;
        }
        public TemplateFieldDto GetTemplateField()
        {
            var templateField = FieldToEdit ?? new TemplateFieldDto();
            templateField.Name = cmbName.Text;
            templateField.DisplayName = tbDisplayName.Text;
            templateField.Description = tbDescription.Text;
            templateField.FontFamily = (cmbFontFamily.SelectedItem != null ? cmbFontFamily.Text : null);
            templateField.FontSize = (cmbFontSize.SelectedItem != null ? ((FontSizePresenter)cmbFontSize.SelectedItem).Size.ToString() : null);
            templateField.Bold = chBold.IsChecked;
            templateField.TextColor = colorText.SelectedColor.HasValue ? string.Format("#{0:X2}{1:X2}{2:X2}", colorText.SelectedColor.Value.R, colorText.SelectedColor.Value.G, colorText.SelectedColor.Value.B) : null;
            templateField.FlowDirection = chLeftToRightDirection.IsChecked.HasValue && chLeftToRightDirection.IsChecked.Value ? SamUtils.Enums.FlowDirection.ltr.ToString() : SamUtils.Enums.FlowDirection.rtl.ToString();
            templateField.HorizontalContentAlignment = (cmbHorizontalAlignment.SelectedItem != null ? ((TextAlignmentPresenter)cmbHorizontalAlignment.SelectedItem).Alignment.ToString() : null);
            templateField.VerticalContentAlignment = (cmbVerticalAlignment.SelectedItem != null ? ((TextAlignmentPresenter)cmbVerticalAlignment.SelectedItem).Alignment.ToString() : null);
            templateField.WrapContent = chWrapContent.IsChecked.HasValue && chWrapContent.IsChecked.Value;
            return templateField;
        }
        public bool IsValid()
        {
            if (String.IsNullOrEmpty(cmbName.Text) || String.IsNullOrEmpty(tbDisplayName.Text))
                return false;

            return true;
        }
        #endregion
    }
}
