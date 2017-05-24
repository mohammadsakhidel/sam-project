﻿using SamDesktop.Code.Exceptions;
using SamDesktop.Code.Utils;
using SamDesktop.Resources.Values;
using SamModels.DTOs;
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

namespace SamDesktop.Views.Windows
{
    public partial class EditTemplateExtraField : Window
    {
        TemplateExtraFieldDto _fieldToEdit;

        #region Ctors:
        public EditTemplateExtraField(TemplateExtraFieldDto fieldToEdit)
        {
            InitializeComponent();
            _fieldToEdit = fieldToEdit;
        }
        #endregion

        #region Event Handlers:
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                ucExtraFieldEditor.FieldToEdit = _fieldToEdit;
            }
            catch (Exception ex)
            {
                ExceptionManager.Handle(ex);
            }
        }
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!ucExtraFieldEditor.IsValid())
                    throw new ValidationException(Messages.FillRequiredFields);

                ExtraField = ucExtraFieldEditor.GetExtraField();
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                ExceptionManager.Handle(ex);
            }
        }
        #endregion

        #region Porps:
        public TemplateExtraFieldDto ExtraField { get; set; }
        #endregion
    }
}
