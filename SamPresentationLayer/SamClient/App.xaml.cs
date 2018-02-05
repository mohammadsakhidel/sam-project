using AutoMapper;
using SamModels.DTOs;
using SamModels.Enums;
using SamUtils.Utils;
using SamUxLib.Code.Utils;
using SamUxLib.Resources.Values;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace SamClient
{
    public partial class App : Application
    {
        #region Consts:
        const int PERSIAN_CULTURE_ID = 1065;
        const int ENGLISH_CULTURE_ID = 1033;
        #endregion

        #region OnStart:
        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                #region Culture Setting, Used by Persian Wpf Toolkit:
                Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(PERSIAN_CULTURE_ID);
                Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;
                #endregion

                InitializeCityUtil();
                InitializeMapper();

                base.OnStartup(e);
            }
            catch (Exception ex)
            {
                ExceptionManager.Handle(ex);
            }
        }
        #endregion

        #region Methods:
        private void InitializeMapper()
        {
            Mapper.Initialize(MappingUtil.ClientsConfiguration);
        }
        private void InitializeCityUtil()
        {
            CityUtil.Func_GetXMLContent = () => {
                try
                {
                    using (var stream = typeof(Strings).Assembly.GetManifestResourceStream("SamUxLib.Resources.XML.ir-cities.xml"))
                    {
                        using (var sr = new StreamReader(stream))
                        {
                            return sr.ReadToEnd();
                        }
                    }
                }
                catch (Exception ex)
                {
                    ExceptionManager.Handle(ex);
                    return "";
                }
            };
        }
        #endregion
    }
}
