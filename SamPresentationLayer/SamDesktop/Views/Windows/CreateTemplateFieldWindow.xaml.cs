using SamUtils.Objects.Exceptions;
using SamUxLib.Code.Utils;
using SamUxLib.Resources.Values;
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
    public partial class CreateTemplateFieldWindow : Window
    {
        #region Ctors:
        public CreateTemplateFieldWindow()
        {
            InitializeComponent();
        }
        #endregion

        #region Porps:
        public TemplateFieldDto Field { get; set; }
        #endregion

        #region Event Handlers:
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!ucTemplateFieldEditor.IsValid())
                    throw new ValidationException(Messages.FillRequiredFields);

                Field = ucTemplateFieldEditor.GetTemplateField();
                Field.X = 0;
                Field.Y = 0;
                Field.BoxWidth = 0;
                Field.BoxHeight = 0;
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                ExceptionManager.Handle(ex);
            }
        }
        #endregion
    }
}
