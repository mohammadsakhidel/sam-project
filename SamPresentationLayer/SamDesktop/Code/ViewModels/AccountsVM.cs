using SamModels.DTOs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamDesktop.Code.ViewModels
{
    public class AccountsVM : ViewModelBase
    {
        #region Accounts:
        private ObservableCollection<IdentityUserDto> accounts;
        public ObservableCollection<IdentityUserDto> Accounts
        {
            get { return accounts; }
            set
            {
                accounts = value;
                RaisePropertyChanged("Accounts");
            }
        }
        #endregion
    }
}
