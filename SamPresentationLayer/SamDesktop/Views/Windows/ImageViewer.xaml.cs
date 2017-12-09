using RamancoLibrary.Utilities;
using SamUxLib.Code.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
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
    public partial class ImageViewer : Window
    {
        #region Fields:
        Bitmap _bitmap;
        #endregion

        #region Ctors:
        public ImageViewer(Bitmap bitmap)
        {
            InitializeComponent();
            _bitmap = bitmap;
        }
        #endregion

        #region Event Handlers:
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Height = Math.Min(_bitmap.Height, Height) + SystemParameters.CaptionHeight;
                this.Width = Math.Min(_bitmap.Width, Width);

                img.Source = ImageUtils.ToBitmapSource(_bitmap);
            }
            catch (Exception ex)
            {
                ExceptionManager.Handle(ex);
            }
        }
        #endregion
    }
}
