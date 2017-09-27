using SamModels.DTOs;
using SamUtils.Constants;
using SamUtils.Enums;
using SamUtils.Objects.Presenters;
using SamUtils.Utils;
using SamUxLib.Code.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SamDesktop.Views.Partials
{
    public partial class RoleEditor : UserControl
    {
        #region Ctors:
        public RoleEditor()
        {
            _role = new IdentityRoleDto();
            InitializeComponent();

            try
            {
                lbAccessLevel.ItemsSource = new ObservableCollection<SectionAccessLevel>(AccessUtil.GetDefaults());
            }
            catch (Exception ex)
            {
                ExceptionManager.Handle(ex);
            }

        }
        #endregion

        #region Props:
        IdentityRoleDto _role;
        public IdentityRoleDto Role
        {
            get
            {
                UpdateModel();
                return _role;
            }
            set
            {
                _role = value;
                UpdateForm();
            }
        }
        #endregion

        #region Methods:
        private void UpdateModel()
        {
            _role.Name = tbLatinName.Text;
            _role.DisplayName = tbDisplayName.Text;
            if (_role.Type != RoleType.admin.ToString())
            {
                var list = lbAccessLevel.ItemsSource != null ? ((ObservableCollection<SectionAccessLevel>)lbAccessLevel.ItemsSource).ToList() : new List<SectionAccessLevel>();
                _role.AccessLevel = AccessUtil.Serialize(list);
            }
        }
        private void UpdateForm()
        {
            tbLatinName.Text = _role.Name;
            tbDisplayName.Text = _role.DisplayName;
            if (_role.Type != RoleType.admin.ToString())
            {
                var accessList = AccessUtil.Deserialize(_role.AccessLevel);
                var sourceList = (lbAccessLevel.ItemsSource as ObservableCollection<SectionAccessLevel>).ToList();
                for (int i = 0; i < sourceList.Count; i++)
                {
                    var al = accessList.SingleOrDefault(asd => asd.Name == sourceList[i].Name);
                    if (al != null)
                    {
                        sourceList[i].Create = al.Create;
                        sourceList[i].Read = al.Read;
                        sourceList[i].Update = al.Update;
                        sourceList[i].Delete = al.Delete;
                    }
                }
                lbAccessLevel.ItemsSource = new ObservableCollection<SectionAccessLevel>(sourceList);
            }
            else
            {
                lbAccessLevel.IsEnabled = false;
            }
        }
        public Tuple<bool, string> IsValid()
        {
            UpdateModel();

            // check required:
            if (_role == null || string.IsNullOrEmpty(_role.Name) || string.IsNullOrEmpty(_role.DisplayName))
                return new Tuple<bool, string>(false, SamUxLib.Resources.Values.Messages.FillRequiredFields);

            // check user name regex:
            var rgxRoleName = new Regex(Patterns.rolename);
            if (!string.IsNullOrEmpty(tbLatinName.Text) && !rgxRoleName.IsMatch(tbLatinName.Text))
                return new Tuple<bool, string>(false, SamUxLib.Resources.Values.Messages.InvalidRoleNameFormat);

            return new Tuple<bool, string>(true, "");
        }
        #endregion
    }
}
