using SamModels.DTOs;
using SamModels.Entities.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamDesktop.Code.ViewModels
{
    public class ConsolationsVM : ViewModelBase
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

        #region Consolations:
        private ObservableCollection<ConsolationDto> consolations;

        public ObservableCollection<ConsolationDto> Consolations
        {
            get { return consolations; }
            set
            {
                consolations = value;
                RaisePropertyChanged("Consolations");
            }
        }
        #endregion

        #region Statuses:
        private ObservableCollection<KeyValuePair<string, string>> statuses;

        public ObservableCollection<KeyValuePair<string, string>> Statuses
        {
            get { return statuses; }
            set
            {
                statuses = value;
                RaisePropertyChanged("Statuses");
            }
        }
        #endregion
    }
}
