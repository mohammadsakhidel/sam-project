using SamModels.DTOs;
using SamReportLib.Models;
using SamUtils.Constants;
using SamUtils.Utils;
using SamUxLib.Code.Objects;
using SamUxLib.Code.Utils;
using Stimulsoft.Report;
using System;
using System.Collections.Generic;
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
            }
            else
            {
                pnlReportDateRange.Visibility = Visibility.Collapsed;
                pnlMosqueSelection.Visibility = Visibility.Collapsed;
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
        private void btnViewReport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var items = new List<MosqueTurnoverRecord>() {
                    new MosqueTurnoverRecord { MosqueName = "ccc", ObitsCount = 100, ConsolationCount = 1000, TotalIncome = 100000 },
                    new MosqueTurnoverRecord { MosqueName = "ccc", ObitsCount = 100, ConsolationCount = 1000, TotalIncome = 100000 },
                    new MosqueTurnoverRecord { MosqueName = "ccc", ObitsCount = 100, ConsolationCount = 1000, TotalIncome = 100000 },
                    new MosqueTurnoverRecord { MosqueName = "ccc", ObitsCount = 100, ConsolationCount = 1000, TotalIncome = 100000 }
                };
                

                var report = new StiReport();
                report.Load(@"E:\TeamProjects\Git\SamGitProject\SamPresentationLayer\SamReportLib\Reports\MosqueTurnoverReport.mrt");

                report.RegData("dsMosqueTurnover", "dsMosqueTurnover", items);
                //report.RegBusinessObject("dsMosqueTurnover", "dsMosqueTurnover", items);
                report.Compile();

                reportViewer.Report = report;
                reportViewer.Refresh();

            }
            catch (Exception ex)
            {
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
        #endregion
    }
}
