using RamancoLibrary.Utilities;
using SamModels.DTOs;
using SamReportLib.Models;
using SamReportLib.Queries;
using SamUtils.Constants;
using SamUtils.Objects.Exceptions;
using SamUtils.Utils;
using SamUxLib.Code.Objects;
using SamUxLib.Code.Utils;
using SamUxLib.Resources.Values;
using Stimulsoft.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
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
using Newtonsoft.Json;
using SamUtils.Enums;
using SamUxLib.Resources;

namespace SamDesktop.Views.Partials
{
    public partial class Reports : UserControl
    {
        #region Constructors:
        public Reports()
        {
            InitializeComponent();
        }
        #endregion

        #region Event Handlers:
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                cmbReportType.ItemsSource = ReportDefinition.All;
                cmbProvince.ItemsSource = CityUtil.Provinces;
                dateReportBeginDate.SetMiladyDate(DateTime.Now.AddMonths(-1));
                dateReportEndDate.SetMiladyDate(DateTime.Now);
                cmbConsolationStatus.ItemsSource = MiscellaneousUtils.GetEnumValues<ConsolationStatus>()
                    .ToDictionary(a => a.ToString(),
                        a => ResourceManager.GetValue($"ConsolationStatus_" + a.ToString(), "Enums"));
            }
            catch (Exception ex)
            {
                ExceptionManager.Handle(ex);
            }
        }
        private void cmbReportType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbReportType.SelectedIndex == 0)
            {
                pnlReportDateRange.Visibility = Visibility.Visible;
                pnlMosqueSelection.Visibility = Visibility.Visible;
                pnlConsolationData.Visibility = Visibility.Collapsed;
            }
            else if (cmbReportType.SelectedIndex == 1)
            {
                pnlReportDateRange.Visibility = Visibility.Visible;
                pnlMosqueSelection.Visibility = Visibility.Visible;
                pnlConsolationData.Visibility = Visibility.Collapsed;
            }
            else if (cmbReportType.SelectedIndex == 2)
            {
                pnlReportDateRange.Visibility = Visibility.Visible;
                pnlMosqueSelection.Visibility = Visibility.Visible;
                pnlConsolationData.Visibility = Visibility.Visible;
            }
            else
            {
                pnlReportDateRange.Visibility = Visibility.Collapsed;
                pnlMosqueSelection.Visibility = Visibility.Collapsed;
                pnlConsolationData.Visibility = Visibility.Collapsed;
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
                    cmbCity.ItemsSource = cities;
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
                var cityId = cmbCity.SelectedValue as int?;
                if (cityId.HasValue || cmbCity.Tag != null)
                {
                    var mosques = await GetMosquesAsync((cityId.HasValue ? cityId.Value : Convert.ToInt32(cmbCity.Tag)));
                    cmbMosque.ItemsSource = mosques;
                }
                else
                {
                    cmbMosque.ItemsSource = null;
                }
            }
            catch (Exception ex)
            {
                progress.IsBusy = false;
                ExceptionManager.Handle(ex);
            }
        }
        private async void btnViewReport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cmbReportType.SelectedIndex == 0 || cmbReportType.SelectedIndex == 1)
                {
                    await CollectAndShowMosquesTurnoverReport(cmbReportType.SelectedIndex == 1);
                }
                if (cmbReportType.SelectedIndex == 2)
                {
                    await CollectAndShowConsolationsReport();
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
        private async Task CollectAndShowMosquesTurnoverReport(bool showChart)
        {
            #region validate:
            if (!dateReportBeginDate.GetMiladyDateTime().HasValue ||
                !dateReportEndDate.GetMiladyDateTime().HasValue)
                throw new ValidationException(Messages.SpecifyReportBeginAndEndDate);
            #endregion

            #region collect inputs:

            var queryObject = new MosquesTurnoverQuery
            {
                BeginDate = (dateReportBeginDate.GetMiladyDateTime() ?? DateTime.MinValue),
                EndDate = (dateReportEndDate.GetMiladyDateTime() ?? DateTime.MaxValue),
                ProvinceID = (cmbProvince.SelectedItem != null ? (int?)cmbProvince.SelectedValue : null),
                CityID = (cmbCity.SelectedItem != null ? (int?)cmbCity.SelectedValue : null),
                MosqueID = (cmbMosque.SelectedItem != null ? (int?)cmbMosque.SelectedValue : null)
            };

            #endregion

            #region call api server:
            progress.IsBusy = true;
            List<MosqueTurnoverRecord> dataItems;
            using (var hc = HttpUtil.CreateClient())
            {
                var response = await hc.PostAsJsonAsync<MosquesTurnoverQuery>(ApiActions.reports_mosqueturnoverrecords, queryObject);
                HttpUtil.EnsureSuccessStatusCode(response);
                dataItems = await response.Content.ReadAsAsync<List<MosqueTurnoverRecord>>();
            }
            #endregion

            #region create report document:
            var report = new StiReport();
            report.Load(Assembly.Load("SamReportLib").GetManifestResourceStream($"SamReportLib.Reports.MosqueTurnoverReport{(showChart ? "Chart" : "")}.mrt"));
            report.RegBusinessObject("MosqueTurnoverRecord", dataItems);
            report.Dictionary.Variables["ReportDate"].Value = DateTimeUtils.ToShamsi(DateTimeUtils.Now).ToString();
            report.Dictionary.Variables["BeginDate"].Value = DateTimeUtils.ToShamsi(queryObject.BeginDate).ToString();
            report.Dictionary.Variables["EndDate"].Value = DateTimeUtils.ToShamsi(queryObject.EndDate).ToString();
            report.Compile();
            report.Render();
            #endregion

            #region show in report viewer:
            reportViewer.Report = report;
            reportViewer.Refresh();
            reportViewer.InvokeZoomPageWidth();
            #endregion

            progress.IsBusy = false;
        }
        private async Task CollectAndShowConsolationsReport()
        {
            #region validate:

            if (!dateReportBeginDate.GetMiladyDateTime().HasValue ||
                !dateReportEndDate.GetMiladyDateTime().HasValue)
                throw new ValidationException(Messages.SpecifyReportBeginAndEndDate);

            #endregion

            #region collect inputs:

            var queryObject =
                new ConsolationsQuery
                {
                    BeginDate = dateReportBeginDate.GetMiladyDateTime() ?? DateTime.MinValue,
                    EndDate = dateReportEndDate.GetMiladyDateTime() ?? DateTime.MaxValue,
                    ProvinceID = (cmbProvince.SelectedItem != null ? (int?)cmbProvince.SelectedValue : null),
                    CityID = (cmbCity.SelectedItem != null ? (int?)cmbCity.SelectedValue : null),
                    MosqueID = (cmbMosque.SelectedItem != null ? (int?)cmbMosque.SelectedValue : null),
                    Status = (cmbConsolationStatus.SelectedItem != null ? cmbConsolationStatus.SelectedValue.ToString() : ""),
                    TrackingNumber = tbTrackingNumber.Text,
                    CustomerCellphone = tbCustomer.Text
                };

            #endregion

            #region call api server:

            progress.IsBusy = true;
            List<ConsolationRecord> dataItems;
            using (var hc = HttpUtil.CreateClient())
            {
                var response = await hc.PostAsJsonAsync<ConsolationsQuery>(ApiActions.reports_consolationsreport, queryObject);
                HttpUtil.EnsureSuccessStatusCode(response);
                dataItems = await response.Content.ReadAsAsync<List<ConsolationRecord>>();
            }

            #endregion

            #region refine data item displays:
            foreach (var dataItem in dataItems)
            {
                // status text:
                dataItem.Status = ResourceManager.GetValue($"ConsolationStatus_{dataItem.Status}", "Enums");

                // consolation content:
                var values = JsonConvert.DeserializeObject<Dictionary<string, string>>(dataItem.Content);
                var temp = values.OrderBy(d => d.Key)
                    .Aggregate("", (current, kv) => current + $"{(!string.IsNullOrEmpty(current) ? Environment.NewLine : "")}{kv.Value}");

                dataItem.Content = temp;
            }
            #endregion

            #region create report document:
            var report = new StiReport();
            report.Load(Assembly.Load("SamReportLib").GetManifestResourceStream($"SamReportLib.Reports.ConsolationsReport.mrt"));
            report.RegBusinessObject("ConsolationRecord", dataItems);
            report.Dictionary.Variables["ReportDate"].Value = DateTimeUtils.ToShamsi(DateTimeUtils.Now).ToString();
            report.Dictionary.Variables["BeginDate"].Value = DateTimeUtils.ToShamsi(queryObject.BeginDate).ToString();
            report.Dictionary.Variables["EndDate"].Value = DateTimeUtils.ToShamsi(queryObject.EndDate).ToString();
            report.Compile();
            report.Render();
            #endregion

            #region show in report viewer:
            reportViewer.Report = report;
            reportViewer.Refresh();
            reportViewer.InvokeZoomPageWidth();
            #endregion

            progress.IsBusy = false;
        }
        #endregion
    }
}
