using RamancoLibrary.Utilities;
using SamKiosk.Code.Utils;
using SamModels.DTOs;
using SamUtils.Constants;
using SamUtils.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
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

namespace SamKiosk.Views.Partials
{
    public partial class TemplateSelectionStep : UserControl
    {
        #region Fields:
        SendConsolationView _parent;
        #endregion

        #region Ctors:
        public TemplateSelectionStep(SendConsolationView parent)
        {
            InitializeComponent();
            _parent = parent;
        }
        #endregion

        #region Event Handlers:
        private async void lbTemplates_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var template = lbTemplates.SelectedItem as TemplateDto;
                if (template != null)
                {
                    _parent.SelectedTemplate = template;
                    _parent.SetNavigationState(true, true, true, true);

                    progress.IsBusy = true;
                    using (var hc = HttpUtil.CreateClient())
                    {
                        var bytes = await hc.GetByteArrayAsync($"{ApiActions.blobs_getimage}/{template.BackgroundImageID}?thumb=false");
                        var bitmap = IOUtils.ByteArrayToBitmap(bytes);
                        var source = ImageUtils.ToBitmapSource(bitmap);
                        imgTemplate.Source = source;
                        progress.IsBusy = false;
                    }
                }
            }
            catch (Exception ex)
            {
                progress.IsBusy = false;
                KioskExceptionManager.Handle(ex);
            }
        }
        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                _parent.SetNavigationState(true, false, true, true);

                progress.IsBusy = true;
                using (var hc = HttpUtil.CreateClient())
                {
                    var response = await hc.GetAsync($"{ApiActions.templates_all}");
                    var templates = await response.Content.ReadAsAsync<TemplateDto[]>();
                    lbTemplates.ItemsSource = new ObservableCollection<TemplateDto>(templates);
                    progress.IsBusy = false;
                }

                #region state:
                if (_parent.SelectedTemplate != null)
                {
                    lbTemplates.SelectedValue = _parent.SelectedTemplate.ID;
                }
                #endregion
            }
            catch (Exception ex)
            {
                progress.IsBusy = false;
                KioskExceptionManager.Handle(ex);
            }
        }
        #endregion

        #region Methods:
        #endregion


    }
}
