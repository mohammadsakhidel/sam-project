using SamUxLib.Code.Utils;
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
