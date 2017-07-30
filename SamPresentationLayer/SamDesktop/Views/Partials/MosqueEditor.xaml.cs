using RamancoLibrary.Utilities;
using SamModels.DTOs;
using SamUtils.Constants;
using SamUtils.Objects.Exceptions;
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
    public partial class MosqueEditor : UserControl
    {
        #region Ctors:
        public MosqueEditor()
        {
            _mosque = new MosqueDto();
            InitializeComponent();
        }
        #endregion

        #region Props:
        private MosqueDto _mosque;
        public MosqueDto Mosque
        {
            get
            {
                UpdateModel();
                return _mosque;
            }
            set
            {
                _mosque = value;
                UpdateForm();
            }
        }
        #endregion

        #region Private Methods:
        private void UpdateModel()
        {
            // specifications:
            var city = cmbCity.SelectedItem as CityDto;
            _mosque.CityID = city != null ? city.ID : -1;
            _mosque.Name = tbName.Text;
            _mosque.ImamName = tbImamName.Text;
            _mosque.InterfaceName = tbInterfaceName.Text;
            _mosque.ImamCellPhone = tbImamCellPhone.Text;
            _mosque.InterfaceCellPhone = tbInterfaceCellPhone.Text;
            _mosque.PhoneNumber = tbPhoneNumber.Text;
            if (!string.IsNullOrEmpty(tbLatitude.Text) && !string.IsNullOrEmpty(tbLongitude.Text))
            {
                var locStr = $"{tbLatitude.Text}, {tbLongitude.Text}";
                _mosque.Location = new RamancoLibrary.Objects.Location(locStr).ToString();
            }
            _mosque.Address = tbAddress.Text;

            // saloons:
            var saloons = dgSaloons.ItemsSource as ObservableCollection<SaloonDto>;
            _mosque.Saloons = saloons != null ? saloons.ToList() : new List<SaloonDto>();
        }
        private void UpdateForm()
        {
            // specifications:
            if (_mosque.CityID > 0)
            {
                var city = CityUtil.GetCity(_mosque.CityID);
                var prov = CityUtil.GetProvince(city.ProvinceID);
                cmbProvince.SelectedItem = ((IEnumerable<ProvinceDto>)cmbProvince.ItemsSource).SingleOrDefault(p => p.ID == prov.ID);
                cmbCity.SelectedItem = ((IEnumerable<CityDto>)cmbCity.ItemsSource).SingleOrDefault(c => c.ID == city.ID);
            }
            tbName.Text = _mosque.Name;
            tbImamName.Text = _mosque.ImamName;
            tbInterfaceName.Text = _mosque.InterfaceName;
            tbImamCellPhone.Text = _mosque.ImamCellPhone;
            tbInterfaceCellPhone.Text = _mosque.InterfaceCellPhone;
            tbAddress.Text = _mosque.Address;
            tbPhoneNumber.Text = _mosque.PhoneNumber;
            if (!string.IsNullOrEmpty(_mosque.Location))
            {
                var loc = new RamancoLibrary.Objects.Location(_mosque.Location);
                tbLatitude.Text = loc.Latitude.ToString();
                tbLongitude.Text = loc.Longitude.ToString();
            }

            // saloons:
            if (_mosque.Saloons != null && _mosque.Saloons.Any())
                dgSaloons.ItemsSource = new ObservableCollection<SaloonDto>(_mosque.Saloons);
        }
        public Tuple<bool, string> IsValid()
        {
            UpdateModel();

            if (_mosque == null || _mosque.CityID <= 0 || string.IsNullOrEmpty(_mosque.Name)
                || string.IsNullOrEmpty(_mosque.Address) || string.IsNullOrEmpty(_mosque.PhoneNumber))
                return new Tuple<bool, string>(false, SamUxLib.Resources.Values.Messages.FillRequiredFields);

            var rgxCellPhone = new Regex(Patterns.cellphone);
            if (!rgxCellPhone.IsMatch(tbImamCellPhone.Text) || !rgxCellPhone.IsMatch(tbInterfaceCellPhone.Text))
                return new Tuple<bool, string>(false, SamUxLib.Resources.Values.Messages.InvalidCellPhone);

            var rgxPhoneNumber = new Regex(Patterns.phone_number);
            if (!rgxPhoneNumber.IsMatch(tbPhoneNumber.Text))
                return new Tuple<bool, string>(false, SamUxLib.Resources.Values.Messages.InvalidPhoneNumber);

            return new Tuple<bool, string>(true, "");
        }
        #endregion

        #region Event Handlers:
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var provinces = CityUtil.Provinces;
                cmbProvince.ItemsSource = new ObservableCollection<ProvinceDto>(provinces);
            }
            catch (Exception ex)
            {
                ExceptionManager.Handle(ex);
            }
        }
        private void cmbProvince_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (cmbProvince.SelectedItem != null)
                {
                    var prov = (ProvinceDto)cmbProvince.SelectedItem;
                    var cities = CityUtil.GetProvinceCities(prov.ID);
                    cmbCity.ItemsSource = new ObservableCollection<CityDto>(cities);
                }
            }
            catch (Exception ex)
            {
                ExceptionManager.Handle(ex);
            }
        }
        private void btnAddSaloon_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                #region Validation:
                if (String.IsNullOrEmpty(tbSaloonName.Text))
                    throw new ValidationException(SamUxLib.Resources.Values.Messages.FillRequiredFields);

                if (chSaloonHasIP.IsChecked.HasValue && chSaloonHasIP.IsChecked.Value
                    && (String.IsNullOrEmpty(tbSaloonIP.Text) || !(new Regex(Patterns.ip)).IsMatch(tbSaloonIP.Text)))
                    throw new ValidationException(SamUxLib.Resources.Values.Messages.FillRequiredFields);
                #endregion

                #region Add To Datagrid:
                var saloons = (dgSaloons.ItemsSource != null ? ((ObservableCollection<SaloonDto>)dgSaloons.ItemsSource).ToList() : new List<SaloonDto>());
                saloons.Add(new SaloonDto
                {
                    Name = tbSaloonName.Text,
                    EndpointIP = tbSaloonIP.Text
                });
                dgSaloons.ItemsSource = new ObservableCollection<SaloonDto>(saloons);
                #endregion

                #region Clear:
                tbSaloonName.Text = "";
                tbSaloonIP.Text = "";
                #endregion
            }
            catch (Exception ex)
            {
                ExceptionManager.Handle(ex);
            }
        }
        private void dgSaloons_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (dgSaloons.SelectedItem != null)
                {
                    var selectedSaloon = dgSaloons.SelectedItem as SaloonDto;
                    var saloons = (dgSaloons.ItemsSource as ObservableCollection<SaloonDto>).ToList();
                    saloons.Remove(selectedSaloon);
                    dgSaloons.ItemsSource = new ObservableCollection<SaloonDto>(saloons);
                }
            }
            catch (Exception ex)
            {
                ExceptionManager.Handle(ex);
            }
        }
        #endregion
    }
}