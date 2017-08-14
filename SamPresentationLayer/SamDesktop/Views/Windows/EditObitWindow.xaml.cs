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
    public partial class EditObitWindow : Window
    {
        public EditObitWindow(MosqueDto mosque, ObitDto obitToEdit)
        {
            InitializeComponent();
            ucObitEditor.ObitToEdit = obitToEdit;
            ucObitEditor.Mosque = mosque;
        }
    }
}
