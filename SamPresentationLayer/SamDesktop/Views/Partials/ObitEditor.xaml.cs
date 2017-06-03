using RamancoLibrary.Utilities;
using SamDesktop.Code.Exceptions;
using SamDesktop.Code.Utils;
using SamDesktop.Code.ViewModels;
using SamDesktop.Resources.Values;
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
        public MosqueDto Mosque { get; set; }
        #endregion

        #region Event Handler:
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
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
                if (!datePicker.SelectedDate.HasValue || !tbBeginHour.IsMaskCompleted || !tbEndHour.IsMaskCompleted)
                    throw new ValidationException(Messages.FillRequiredFields);
                #endregion

                #region Gather Inputs:
                var selectedDate = datePicker.SelectedDate.Value;
                var beginTimeStr = tbBeginHour.Text.Split(':');
                var beginTime = new DateTime(selectedDate.Year, selectedDate.Month, selectedDate.Day, Convert.ToInt32(beginTimeStr[0]), Convert.ToInt32(beginTimeStr[1]), 0);
                var endTimeStr = tbEndHour.Text.Split(':');
                var endTime = new DateTime(selectedDate.Year, selectedDate.Month, selectedDate.Day, Convert.ToInt32(endTimeStr[0]), Convert.ToInt32(endTimeStr[1]), 0);

                if (beginTime >= endTime)
                    throw new ValidationException(Messages.InvalidInputValues);
                #endregion

                var vm = DataContext as ObitEditorVM;
                var holdings = vm.ObitHoldings ?? new System.Collections.ObjectModel.ObservableCollection<ObitHoldingDto>();
                holdings.Add(new ObitHoldingDto { BeginTime = beginTime, EndTime = endTime });
                vm.ObitHoldings = holdings;
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
                #region Validate Inputs:
                if (string.IsNullOrEmpty(tbTitle.Text) || cmbObitType.SelectedItem == null)
                    throw new ValidationException(Messages.FillRequiredFields);
                #endregion

                #region Gather Info:
                var isEditing = ObitToEdit != null;
                var vm = DataContext as ObitEditorVM;
                var formObit = ObitToEdit ?? new ObitDto();
                formObit.Title = tbTitle.Text;
                formObit.ObitType = cmbObitType.SelectedValue.ToString();
                formObit.ObitHoldings = vm.ObitHoldings.ToList();
                formObit.MosqueID = isEditing ? ObitToEdit.MosqueID : Mosque.ID;
                #endregion

                #region Call Server:
                progress.IsBusy = true;
                using (var hc = HttpUtil.CreateClient())
                {
                    if (!isEditing)
                    {
                        var response = await hc.PostAsJsonAsync(ApiActions.obits_create, formObit);
                        response.EnsureSuccessStatusCode();
                    }
                    else
                    {
                        var response = await hc.PutAsJsonAsync(ApiActions.obits_update, formObit);
                        response.EnsureSuccessStatusCode();
                    }
                    progress.IsBusy = false;
                    UxUtil.ShowMessage(Messages.SuccessfullyDone);
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
                cmbObitType.SelectedItem = vm.ObitTypes.SingleOrDefault(t => t.Key == ObitToEdit.ObitType);
                vm.ObitHoldings = ObitToEdit.ObitHoldings != null && ObitToEdit.ObitHoldings.Any() ? new ObservableCollection<ObitHoldingDto>(ObitToEdit.ObitHoldings) : null;
            }
        }
        #endregion
    }
}
