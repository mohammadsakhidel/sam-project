using SamUxLib.Resources;
using SamModels.DTOs;
using SamUtils.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        #region ObitTypes:
        public ObservableCollection<KeyValuePair<string, string>> ObitTypes
        {
            get
            {
                List<KeyValuePair<string, string>> coll = new List<KeyValuePair<string, string>>();
                var values = Enum.GetValues(typeof(ObitType));
                foreach (var val in values)
                {
                    coll.Add(new KeyValuePair<string, string>(((ObitType)val).ToString(), ResourceManager.GetValue($"ObitType_{(ObitType)val}", "Enums")));
                }
                return new ObservableCollection<KeyValuePair<string, string>>(coll);
            }
        }
        #endregion
    }
}