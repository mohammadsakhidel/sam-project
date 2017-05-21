using SamModels.DTOs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamDesktop.Code.ViewModels
{
    public class TemplatesVM : ViewModelBase
    {
        #region Mosques:
        private ObservableCollection<TemplateDto> templates;

        public ObservableCollection<TemplateDto> Templates
        {
            get { return templates; }
            set
            {
                templates = value;
                RaisePropertyChanged("Templates");
            }
        }
        #endregion
    }
}
