using RestSharp;
using SamUtils.Utils;
using SamUxLib.Code.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WPFTabTip;

namespace SamKiosk
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                TabTipAutomation.BindTo<TextBox>();
                //TabTipAutomation.IgnoreHardwareKeyboard = HardwareKeyboardIgnoreOptions.IgnoreAll;
            }
            catch (Exception ex)
            {
                ExceptionManager.Handle(ex);
            }

            base.OnStartup(e);
        }

        #region Global Static:
        private static HttpClient apiClient = null;
        public static HttpClient ApiClient
        {
            get
            {
                if (apiClient == null)
                    apiClient = HttpUtil.CreateClient();

                return apiClient;
            }
        }

        private static RestClient restClient = null;
        public static RestClient RestClient
        {
            get
            {
                if (restClient == null)
                    restClient = HttpUtil.CreateRestClient();

                return restClient;
            }
        }
        #endregion
    }
}
