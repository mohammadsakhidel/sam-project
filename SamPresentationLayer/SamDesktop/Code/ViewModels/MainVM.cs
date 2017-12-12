using SamDesktop.Views.Partials;
using System.Windows;

namespace SamDesktop.Code.ViewModels
{
    public class MainVM : ViewModelBase
    {
        #region Props:
        private FrameworkElement selectedSectionContent = new DefaultPage();
        public FrameworkElement SelectedSectionContent
        {
            get
            {
                return selectedSectionContent;
            }
            set
            {
                selectedSectionContent = value;
                RaisePropertyChanged("SelectedSectionContent");
            }
        }

        private string selectedSectionName;
        public string SelectedSectionName
        {
            get { return selectedSectionName; }
            set
            {
                selectedSectionName = value;
                RaisePropertyChanged("SelectedSectionName");
            }
        }
        #endregion
    }
}
