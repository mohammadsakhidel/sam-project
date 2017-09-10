using Microsoft.Win32;
using RamancoLibrary.Utilities;
using SamModels.DTOs;
using SamUtils.Constants;
using SamUtils.Objects.Exceptions;
using SamUtils.Utils;
using SamUxLib.Code.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
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

            LoadCities();
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
            // image:
            if (imgMosqueImage.Source != null)
            {
                Bitmap backgroundImage = ImageUtils.FromBitmapSource(imgMosqueImage.Source as BitmapSource);
                var bytes = IOUtils.BitmapToByteArray(backgroundImage, ImageFormat.Jpeg);
                _mosque.ImageBase64 = Convert.ToBase64String(bytes);
            }
            else
            {
                _mosque.ImageBase64 = "";
            }

            // saloons:
            var saloons = dgSaloons.ItemsSource as ObservableCollection<SaloonDto>;
            _mosque.Saloons = saloons != null ? saloons.ToList() : new List<SaloonDto>();
        }
        private void UpdateForm()
        {
            // specifications:
            if (_mosque.CityID > 0)
            {
                var prov = CityUtil.GetProvince(_mosque.CityID);
                cmbProvince.SelectedItem = ((IEnumerable<ProvinceDto>)cmbProvince.ItemsSource).SingleOrDefault(p => p.ID == prov.ID);
                cmbCity.SelectedItem = ((IEnumerable<CityDto>)cmbCity.ItemsSource).SingleOrDefault(c => c.ID == _mosque.CityID);
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
                tbLatitude.Text = loc.Latitude.ToString(CultureInfo.InvariantCulture);
                tbLongitude.Text = loc.Longitude.ToString(CultureInfo.InvariantCulture);
            }
            // image:
            if (!string.IsNullOrEmpty(_mosque.ImageBase64))
            {
                var bytes = Convert.FromBase64String(_mosque.ImageBase64);
                var bitmap = IOUtils.ByteArrayToBitmap(bytes);
                imgMosqueImage.Source = ImageUtils.ToBitmapSource(bitmap);
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
            if ((!string.IsNullOrEmpty(tbImamCellPhone.Text) && !rgxCellPhone.IsMatch(tbImamCellPhone.Text)) ||
                (!string.IsNullOrEmpty(tbInterfaceCellPhone.Text) && !rgxCellPhone.IsMatch(tbInterfaceCellPhone.Text)))
                return new Tuple<bool, string>(false, SamUxLib.Resources.Values.Messages.InvalidCellPhone);

            var rgxPhoneNumber = new Regex(Patterns.phone_number);
            if (!rgxPhoneNumber.IsMatch(tbPhoneNumber.Text))
                return new Tuple<bool, string>(false, SamUxLib.Resources.Values.Messages.InvalidPhoneNumber);

            if (dgSaloons.Items.Count == 0)
                return new Tuple<bool, string>(false, SamUxLib.Resources.Values.Messages.SpecifyAtLeastOneSaloon);

            return new Tuple<bool, string>(true, "");
        }
        private void LoadCities()
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
        #endregion

        #region Event Handlers:
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
                if (String.IsNullOrEmpty(tbSaloonName.Text) || string.IsNullOrWhiteSpace(tbSaloonID.Text))
                    throw new ValidationException(SamUxLib.Resources.Values.Messages.FillRequiredFields);

                if (chSaloonHasIP.IsChecked.HasValue && chSaloonHasIP.IsChecked.Value
                    && (String.IsNullOrEmpty(tbSaloonIP.Text) || !(new Regex(Patterns.ip)).IsMatch(tbSaloonIP.Text)))
                    throw new ValidationException(SamUxLib.Resources.Values.Messages.FillRequiredFields);
                #endregion

                #region Add To Datagrid:
                var saloons = (dgSaloons.ItemsSource != null ? ((ObservableCollection<SaloonDto>)dgSaloons.ItemsSource).ToList() : new List<SaloonDto>());
                saloons.Add(new SaloonDto
                {
                    ID = tbSaloonID.Text,
                    Name = tbSaloonName.Text,
                    EndpointIP = tbSaloonIP.Text
                });
                dgSaloons.ItemsSource = new ObservableCollection<SaloonDto>(saloons);
                #endregion

                #region Clear:
                tbSaloonID.Text = "";
                tbSaloonName.Text = "";
                tbSaloonIP.Text = "";
                chSaloonHasIP.IsChecked = false;
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
        private void btnSelectImage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var openfiledialog = new OpenFileDialog();
                openfiledialog.Filter = "Image Files (*.jpg, *.jpeg, *.png) | *.jpg; *.jpeg; *.png";
                openfiledialog.Multiselect = false;
                var res = openfiledialog.ShowDialog();
                if (res.HasValue && res.Value)
                {
                    Bitmap bitmap = new Bitmap(openfiledialog.FileName);
                    imgMosqueImage.Source = ImageUtils.ToBitmapSource(bitmap);
                }
            }
            catch (Exception ex)
            {
                ExceptionManager.Handle(ex);
            }
        }
        private void btnRemoveImage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                imgMosqueImage.Source = null;
            }
            catch (Exception ex)
            {
                ExceptionManager.Handle(ex);
            }
        }
        #endregion
    }
}