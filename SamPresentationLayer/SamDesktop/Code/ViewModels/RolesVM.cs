using SamModels.DTOs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamDesktop.Code.ViewModels
{
    public class RolesVM : ViewModelBase
    {
        #region Roles:
        private ObservableCollection<IdentityRoleDto> roles;
        public ObservableCollection<IdentityRoleDto> Roles
        {
            get { return roles; }
            set
            {
                roles = value;
                RaisePropertyChanged("Roles");
            }
        }
        #endregion
    }
}
