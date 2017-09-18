using SamModels.DTOs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamDesktop.Code.ViewModels
{
    public class BannersVM : ViewModelBase
    {
        #region Mosques:
        private ObservableCollection<BannerHierarchyDto> banners;

        public ObservableCollection<BannerHierarchyDto> Banners
        {
            get { return banners; }
            set
            {
                banners = value;
                RaisePropertyChanged("Banners");
            }
        }
        #endregion
    }
}
