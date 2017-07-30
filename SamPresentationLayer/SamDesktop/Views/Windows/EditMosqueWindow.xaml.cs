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
    public partial class EditMosqueWindow : Window
    {
        #region Fields:
        MosqueDto _mosque;
        #endregion

        #region Ctors:
        public EditMosqueWindow(MosqueDto mosque)
        {
            _mosque = mosque;
            InitializeComponent();
        }
        #endregion

        #region Event Handlers:

        #endregion

        #region Methods:

        #endregion
    }
}
