using RamancoLibrary.Utilities;
using SamModels.Enums;
using SamUtils.Enums;
using SamUxLib.Code.Utils;
using SamUxLib.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using SamModels.DTOs;
using SamUtils.Utils;
using System.IO;
using Microsoft.Win32;
using System.Drawing;
using System.Collections.ObjectModel;
using SamUtils.Constants;
using System.Net.Http;
using System.Drawing.Imaging;

namespace SamDesktop.Views.Partials
{
    public partial class BannerEditor : UserControl
    {
        #region Fields:
        BannerHierarchyDto _banner;
        #endregion

        #region Ctors:
        public BannerEditor()
        {
            _banner = new BannerHierarchyDto();
            InitializeComponent();
        }
        #endregion

        #region Props:
        public BannerHierarchyDto Banner
        {
            get
            {
                UpdateModel();
                return _banner;
            }
            set
            {
                _banner = value;
                UpdateForm();
                tiSpecificFields.IsEnabled = false;
            }
        }
        #endregion

        #region Event Hanlders:
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                #region Form Defaults:
                var bannerTypesDic = UxUtil.EnumToDic(typeof(BannerType), "BannerType_");
                var priorityDic = UxUtil.EnumToDic(typeof(BannerPriority), "BannerPriority_");
                cmbBannerType.ItemsSource = bannerTypesDic;
                cmbBannerType.SelectedValue = BannerType.obit.ToString();
                cmbPriority.ItemsSource = priorityDic;

                cmbProvince.ItemsSource = new ObservableCollection<ProvinceDto>(CityUtil.Provinces);

                dtLifeBegin.SetMiladyDate(DateTimeUtils.Now);
                dtLifeEnd.SetMiladyDate(DateTimeUtils.Now);
                #endregion
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
                    var bitmap = new Bitmap(openfiledialog.FileName);
                    imgImage.Source = ImageUtils.ToBitmapSource(bitmap);
                }
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
        private async void cmbCity_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var type = EnumUtil.GetEnum<BannerType>(cmbBannerType.SelectedValue.ToString());
                if (type == BannerType.mosque || type == BannerType.obit)
                {
                    var cityId = cmbCity.SelectedValue as int?;
                    if (cityId.HasValue || cmbCity.Tag != null)
                    {
                        var mosques = await GetMosquesAsync((cityId.HasValue ? cityId.Value : Convert.ToInt32(cmbCity.Tag)));
                        cmbMosque.ItemsSource = new ObservableCollection<MosqueDto>(mosques);
                    }
                    else
                    {
                        cmbMosque.ItemsSource = null;
                    }
                }
            }
            catch (Exception ex)
            {
                progress.IsBusy = false;
                ExceptionManager.Handle(ex);
            }
        }
        private async void cmbMosque_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var type = EnumUtil.GetEnum<BannerType>(cmbBannerType.SelectedValue.ToString());
                if (type == BannerType.obit)
                {
                    var mosqueId = cmbMosque.SelectedValue as int?;
                    if (mosqueId.HasValue || cmbMosque.Tag != null)
                    {
                        var obitsDic = await GetObitsAsync((mosqueId.HasValue ? mosqueId.Value : Convert.ToInt32(cmbMosque.Tag)));
                        cmbObit.ItemsSource = new ObservableCollection<KeyValuePair<string, string>>(obitsDic);
                    }
                    else
                    {
                        cmbObit.ItemsSource = null;
                    }
                }
            }
            catch (Exception ex)
            {
                progress.IsBusy = false;
                ExceptionManager.Handle(ex);
            }
        }
        #endregion

        #region Methods:
        private void UpdateModel()
        {
            var type = EnumUtil.GetEnum<BannerType>(cmbBannerType.SelectedValue.ToString());

            #region commons:
            _banner.Title = tbTitle.Text;
            #region base64 image:
            if (imgImage.Source != null)
            {
                var bitmap = ImageUtils.FromBitmapSource((BitmapSource)imgImage.Source);
                var bytes = IOUtils.BitmapToByteArray(bitmap, ImageFormat.Jpeg);
                _banner.ImageBase64 = Convert.ToBase64String(bytes);
            }
            else
            {
                _banner.ImageBase64 = "";
            }
            #endregion
            _banner.IsActive = chIsActive.IsChecked.HasValue && chIsActive.IsChecked.Value;
            _banner.Priority = cmbPriority.SelectedItem != null
                ? (int)EnumUtil.GetEnum<BannerPriority>(cmbPriority.SelectedValue.ToString()) : -1;
            _banner.ShowOnStart = chShowOnStart.IsChecked.HasValue && chShowOnStart.IsChecked.Value;
            _banner.DurationSeconds = !string.IsNullOrEmpty(tbDuration.Text) ? Convert.ToInt32(tbDuration.Text) : -1;
            _banner.Interval = !string.IsNullOrEmpty(tbInterval.Text) ? Convert.ToInt32(tbInterval.Text) : -1;
            _banner.Creator = App.UserName;
            _banner.Type = type.ToString();
            #endregion
            #region life:
            if (type != BannerType.obit)
            {
                _banner.LifeBeginTime = chHasBeginLife.IsChecked.HasValue && chHasBeginLife.IsChecked.Value
                                        ? dtLifeBegin.GetMiladyDateTime() : null;
                _banner.LifeEndTime = chHasEndLife.IsChecked.HasValue && chHasEndLife.IsChecked.Value
                                        ? dtLifeEnd.GetMiladyDateTime() : null;
            }
            #endregion
            #region area spesific:
            if (type == BannerType.area)
            {
                _banner.ProvinceID = cmbProvince.SelectedItem != null
                    ? Convert.ToInt32(cmbProvince.SelectedValue) : (int?)null;
                _banner.CityID = cmbCity.SelectedItem != null
                    ? Convert.ToInt32(cmbCity.SelectedValue) : (int?)null;
            }
            #endregion
            #region mosque specific:
            if (type == BannerType.mosque)
            {
                _banner.MosqueID = cmbMosque.SelectedItem != null
                    ? Convert.ToInt32(cmbMosque.SelectedValue) : (int?)null;
            }
            #endregion
            #region obit specific:
            if (type == BannerType.obit)
            {
                _banner.ObitID = cmbObit.SelectedItem != null
                    ? Convert.ToInt32(cmbObit.SelectedValue) : (int?)null;
            }
            #endregion
        }
        private void UpdateForm()
        {
            var type = EnumUtil.GetEnum<BannerType>(_banner.Type);

            #region base fields:
            tbTitle.Text = _banner.Title;
            chIsActive.IsChecked = _banner.IsActive;
            cmbPriority.SelectedValue = ((BannerPriority)_banner.Priority).ToString();
            chShowOnStart.IsChecked = _banner.ShowOnStart;
            tbDuration.Text = _banner.DurationSeconds.ToString();
            tbInterval.Text = _banner.Interval.ToString();
            cmbBannerType.SelectedValue = _banner.Type;
            #endregion
            #region image:
            if (!string.IsNullOrEmpty(_banner.ImageBase64))
            {
                var bytes = Convert.FromBase64String(_banner.ImageBase64);
                var bitmap = IOUtils.ByteArrayToBitmap(bytes);
                imgImage.Source = ImageUtils.ToBitmapSource(bitmap);
            }
            else
            {
                imgImage.Source = null;
            }
            #endregion
            #region nullable fields:
            chHasBeginLife.IsChecked = _banner.LifeBeginTime.HasValue;
            if (_banner.LifeBeginTime.HasValue)
                dtLifeBegin.SetMiladyDate(_banner.LifeBeginTime.Value);

            chHasEndLife.IsChecked = _banner.LifeEndTime.HasValue;
            if (_banner.LifeEndTime.HasValue)
                dtLifeEnd.SetMiladyDate(_banner.LifeEndTime.Value);

            if (_banner.ProvinceID.HasValue)
                cmbProvince.SelectedValue = _banner.ProvinceID.Value;

            if (_banner.CityID.HasValue)
            {
                cmbCity.Tag = _banner.CityID.Value;
                cmbCity.SelectedValue = _banner.CityID.Value;
            }

            if (_banner.MosqueID.HasValue)
            {
                cmbMosque.Tag = _banner.MosqueID.Value;
                cmbMosque.SelectedValue = _banner.MosqueID.Value;
            }

            if (_banner.ObitID.HasValue)
                cmbObit.SelectedValue = _banner.ObitID.Value;
            #endregion
        }
        public Tuple<bool, string> IsValid()
        {
            UpdateModel();

            // commons:
            if (_banner == null || string.IsNullOrEmpty(_banner.Title) || string.IsNullOrEmpty(_banner.ImageBase64)
                || _banner.Priority <= 0 || _banner.DurationSeconds <= 0 || _banner.Interval <= 0)
                return new Tuple<bool, string>(false, SamUxLib.Resources.Values.Messages.FillRequiredFields);

            // area specific:
            if (_banner.Type == BannerType.area.ToString() && !_banner.ProvinceID.HasValue && !_banner.CityID.HasValue)
                return new Tuple<bool, string>(false, SamUxLib.Resources.Values.Messages.SpecifyBannerArea);

            // mosque specific:
            if (_banner.Type == BannerType.mosque.ToString() && !_banner.MosqueID.HasValue)
                return new Tuple<bool, string>(false, SamUxLib.Resources.Values.Messages.SpecifyMosque);

            // obit specific:
            if (_banner.Type == BannerType.obit.ToString() && !_banner.ObitID.HasValue)
                return new Tuple<bool, string>(false, SamUxLib.Resources.Values.Messages.SpecifyHolding);

            return new Tuple<bool, string>(true, "");
        }
        private async Task<List<MosqueDto>> GetMosquesAsync(int cityId)
        {
            progress.IsBusy = true;
            using (var hc = HttpUtil.CreateClient())
            {
                var response = await hc.GetAsync($"{ApiActions.mosques_findbycity}?cityid={cityId}");
                response.EnsureSuccessStatusCode();
                var mosques = await response.Content.ReadAsAsync<List<MosqueDto>>();
                progress.IsBusy = false;
                return mosques;
            }
        }
        private async Task<Dictionary<string, string>> GetObitsAsync(int mosqueId)
        {
            progress.IsBusy = true;
            using (var hc = HttpUtil.CreateClient())
            {
                var response = await hc.GetAsync($"{ApiActions.obits_getallobits}?mosqueId={mosqueId}");
                HttpUtil.EnsureSuccessStatusCode(response);
                var obits = await response.Content.ReadAsAsync<List<ObitDto>>();
                progress.IsBusy = false;
                return obits.ToDictionary(o => o.ID.ToString(),
                    o => $"{ResourceManager.GetValue($"ObitType_{o.ObitType.ToString()}", "Enums")}: {o.Title}");
            }
        }
        #endregion
    }
}