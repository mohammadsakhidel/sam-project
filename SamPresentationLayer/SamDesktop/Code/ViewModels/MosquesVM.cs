using SamModels.DTOs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamDesktop.Code.ViewModels
{
    public class MosquesVM : ViewModelBase
    {
        #region Provinces:
        private ObservableCollection<ProvinceDto> provinces;

        public ObservableCollection<ProvinceDto> Provinces
        {
            get { return provinces; }
            set
            {
                provinces = value;
                RaisePropertyChanged("Provinces");
            }
        }
        #endregion

        #region Cities:
        private ObservableCollection<CityDto> cities;

        public ObservableCollection<CityDto> Cities
        {
            get { return cities; }
            set
            {
                cities = value;
                RaisePropertyChanged("Cities");
            }
        }
        #endregion

        #region Mosques:
        private ObservableCollection<MosqueDto> mosques;

        public ObservableCollection<MosqueDto> Mosques
        {
            get { return mosques; }
            set
            {
                mosques = value;
                RaisePropertyChanged("Mosques");
            }
        }
        #endregion
    }
}
