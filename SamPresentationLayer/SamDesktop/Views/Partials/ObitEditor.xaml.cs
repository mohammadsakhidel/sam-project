using RamancoLibrary.Utilities;
using SamUtils.Objects.Exceptions;
using SamUxLib.Code.Utils;
using SamDesktop.Code.ViewModels;
using SamUxLib.Resources.Values;
using SamModels.DTOs;
using SamUtils.Constants;
using SamUtils.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
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
using System.Text.RegularExpressions;

namespace SamDesktop.Views.Partials
{
    public partial class ObitEditor : UserControl
    {
        #region Ctors:
        public ObitEditor()
        {
            InitializeComponent();
        }
        #endregion

        #region Props:
        public ObitDto ObitToEdit { get; set; }

        private MosqueDto mosque;
        public MosqueDto Mosque
        {
            get
            {
                return mosque;
            }
            set
            {
                mosque = value;

                var vm = DataContext as ObitEditorVM;
                if (vm != null)
                {
                    vm.Saloons = new ObservableCollection<SaloonDto>(mosque.Saloons);
                    cmbSaloon.SelectedItem = mosque.Saloons.Any() ? mosque.Saloons.First() : null;
                }
            }
        }
        #endregion

        #region Event Handler:
        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var isEditing = ObitToEdit != null;
                if (!isEditing)
                {
                    var now = DateTimeUtils.Now;
                    var shamsiNow = DateTimeUtils.ToShamsi(now.Year, now.Month, now.Day);
                    datePicker.SelectedDate = new DateTime(shamsiNow.Year, shamsiNow.Month, shamsiNow.Day);
                }
                else
                {
                    LoadObit();
                }

                #region Load Deceased People:
                await LoadDeceasedPeopleAsync();
                #endregion
            }
            catch (Exception ex)
            {
                ExceptionManager.Handle(ex);
            }
        }
        private void btnAddHolding_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                #region Validate Inputs:
                if (cmbSaloon.SelectedItem == null || !datePicker.SelectedDate.HasValue || !tbBeginHour.IsMaskCompleted || !tbEndHour.IsMaskCompleted)
                    throw new ValidationException(Messages.FillRequiredFields);
                #endregion

                #region Gather Inputs:
                var saloon = cmbSaloon.SelectedItem as SaloonDto;
                var selectedDate = datePicker.SelectedDate.Value;
                var beginTimeStr = tbBeginHour.Text.Split(':');
                var beginTime = new DateTime(selectedDate.Year, selectedDate.Month, selectedDate.Day, Convert.ToInt32(beginTimeStr[0]), Convert.ToInt32(beginTimeStr[1]), 0);
                var endTimeStr = tbEndHour.Text.Split(':');
                var endTime = new DateTime(selectedDate.Year, selectedDate.Month, selectedDate.Day, Convert.ToInt32(endTimeStr[0]), Convert.ToInt32(endTimeStr[1]), 0);

                if (beginTime >= endTime)
                    throw new ValidationException(Messages.InvalidInputValues);

                if ((beginTime - DateTimeUtils.Now).TotalDays > Values.max_obit_days)
                    throw new ValidationException(Messages.InvalidInputValues);
                #endregion

                #region Add To DataGrid:
                var vm = DataContext as ObitEditorVM;
                var holdings = vm.ObitHoldings ?? new ObservableCollection<ObitHoldingDto>();
                holdings.Add(new ObitHoldingDto { BeginTime = beginTime, EndTime = endTime, SaloonID = saloon.ID, SaloonName = saloon.Name });
                vm.ObitHoldings = holdings;
                #endregion

                #region Clear Inputs:
                datePicker.SelectedDate = null;
                tbBeginHour.Clear();
                tbEndHour.Clear();
                #endregion
            }
            catch (Exception ex)
            {
                ExceptionManager.Handle(ex);
            }
        }
        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var vm = DataContext as ObitEditorVM;

                #region Validate Inputs:
                if (string.IsNullOrEmpty(tbTitle.Text) || cmbObitType.SelectedItem == null
                    || string.IsNullOrEmpty(tbOwnerCellPhone.Text) || !(new Regex(Patterns.cellphone)).IsMatch(tbOwnerCellPhone.Text))
                    throw new ValidationException(Messages.FillRequiredFields);

                if (!vm.ObitHoldings.Any())
                    throw new ValidationException(Messages.SpecifyAtLeastOneHolding);
                #endregion

                #region Gather Info:
                var isEditing = ObitToEdit != null;
                var formObit = ObitToEdit ?? new ObitDto();
                var selectedIdentifier = cmbDeceasedIdentifier.SelectedItem != null ? cmbDeceasedIdentifier.SelectedValue.ToString() : "";
                var generateNewIdentifier = chNewIdentifier.IsChecked.HasValue ? chNewIdentifier.IsChecked.Value : false;
                formObit.Title = tbTitle.Text;
                formObit.DeceasedIdentifier = generateNewIdentifier ? GenerateRandomIdentifier() : selectedIdentifier;
                formObit.ObitType = cmbObitType.SelectedValue.ToString();
                formObit.ObitHoldings = vm.ObitHoldings.ToList();
                formObit.MosqueID = isEditing ? ObitToEdit.MosqueID : Mosque.ID;
                formObit.OwnerCellPhone = tbOwnerCellPhone.Text;
                #endregion

                #region Call Server:
                progress.IsBusy = true;
                using (var hc = HttpUtil.CreateClient())
                {
                    if (!isEditing)
                    {
                        var response = await hc.PostAsJsonAsync(ApiActions.obits_create, formObit);
                        HttpUtil.EnsureSuccessStatusCode(response);
                        var trackingNumber = await response.Content.ReadAsStringAsync();
                        UxUtil.ShowMessage($"{Messages.SuccessfullyDone}{Environment.NewLine}{Strings.TrackingNumber}: {trackingNumber}");
                    }
                    else
                    {
                        var response = await hc.PutAsJsonAsync(ApiActions.obits_update, formObit);
                        HttpUtil.EnsureSuccessStatusCode(response);
                        UxUtil.ShowMessage(Messages.SuccessfullyDone);
                    }
                    progress.IsBusy = false;
                    Window.GetWindow(this).DialogResult = true;
                }
                #endregion
            }
            catch (Exception ex)
            {
                progress.IsBusy = false;
                ExceptionManager.Handle(ex);
            }
        }
        private void dgRecords_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (dgRecords.SelectedItem != null)
                {
                    var vm = DataContext as ObitEditorVM;
                    var items = vm.ObitHoldings;
                    var toBeRemoved = (ObitHoldingDto)dgRecords.SelectedItem;
                    items.Remove(toBeRemoved);
                    vm.ObitHoldings = items;
                }
            }
            catch (Exception ex)
            {
                ExceptionManager.Handle(ex);
            }
        }
        #endregion

        #region Methods:
        private void LoadObit()
        {
            if (ObitToEdit != null)
            {
                var vm = DataContext as ObitEditorVM;
                tbTitle.Text = ObitToEdit.Title;
                cmbDeceasedIdentifier.SelectedValue = ObitToEdit.DeceasedIdentifier;
                cmbObitType.SelectedValue = ObitToEdit.ObitType;
                tbOwnerCellPhone.Text = ObitToEdit.OwnerCellPhone;
                vm.ObitHoldings = ObitToEdit.ObitHoldings != null && ObitToEdit.ObitHoldings.Any() ? new ObservableCollection<ObitHoldingDto>(ObitToEdit.ObitHoldings) : null;
            }
        }
        private async Task LoadDeceasedPeopleAsync()
        {
            try
            {
                progress.IsBusy = true;
                using (var hc = HttpUtil.CreateClient())
                {
                    var response = await hc.GetAsync(ApiActions.obits_getdeceasedpeople);
                    HttpUtil.EnsureSuccessStatusCode(response);
                    var people = await response.Content.ReadAsAsync<List<KeyValuePair<string, string>>>();
                    var vm = DataContext as ObitEditorVM;
                    vm.DeceasedPeople = new ObservableCollection<KeyValuePair<string, string>>(people);
                    progress.IsBusy = false;
                }
            }
            catch (Exception ex)
            {
                progress.IsBusy = false;
                ExceptionManager.Handle(ex);
            }
        }
        private string GenerateRandomIdentifier()
        {
            var baseString = DateTime.Now.ToString("yyyyMMddHHmmss");
            return $"{TextUtils.GetRandomString(2, false)}{baseString}";
        }
        #endregion
    }
}
