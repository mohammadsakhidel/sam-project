using SamUxLib.Resources;
using SamModels.DTOs;
using SamUtils.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamUtils.Objects.Presenters;

namespace SamDesktop.Code.ViewModels
{
    public class ObitEditorVM : ViewModelBase
    {
        #region ObitHoldings:
        private ObservableCollection<ObitHoldingDto> obitHoldings;
        public ObservableCollection<ObitHoldingDto> ObitHoldings
        {
            get { return obitHoldings; }
            set
            {
                obitHoldings = value;
                RaisePropertyChanged("ObitHoldings");
            }
        }
        #endregion

        #region Saloons:
        private ObservableCollection<SaloonDto> saloons;
        public ObservableCollection<SaloonDto> Saloons
        {
            get { return saloons; }
            set
            {
                saloons = value;
                RaisePropertyChanged("Saloons");
            }
        }
        #endregion

        #region ObitTypes:
        public ObservableCollection<ObitTypePresenter> ObitTypes
        {
            get
            {
                List<ObitTypePresenter> coll = new List<ObitTypePresenter>();
                var values = Enum.GetValues(typeof(ObitType));
                foreach (var val in values)
                {
                    var kv = new ObitTypePresenter(((ObitType)val).ToString(), ResourceManager.GetValue($"ObitType_{(ObitType)val}", "Enums"));
                    coll.Add(kv);
                }
                return new ObservableCollection<ObitTypePresenter>(coll);
            }
        }
        #endregion
    }
}