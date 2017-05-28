using SamDesktop.Code.Utils;
using SamModels.DTOs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamDesktop.Code.ViewModels
{
    public class ObitsVM : ViewModelBase
    {
        #region Selected Mosque:
        private MosqueDto selectedMosque;
        public MosqueDto SelectedMosque
        {
            get { return selectedMosque; }
            set
            {
                selectedMosque = value;
                RaisePropertyChanged("SelectedMosque");
            }
        }
        #endregion

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
    }
}
