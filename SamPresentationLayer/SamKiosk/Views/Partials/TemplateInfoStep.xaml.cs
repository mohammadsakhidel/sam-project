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

namespace SamKiosk.Views.Partials
{
    public partial class TemplateInfoStep : UserControl
    {
        #region Fields:
        SendConsolationView _parent;
        #endregion

        #region Constructors:
        public TemplateInfoStep(SendConsolationView parent)
        {
            InitializeComponent();
            _parent = parent;
        }
        #endregion
    }
}
