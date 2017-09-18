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

namespace SamDesktop.Views.Partials
{
    public partial class BannerEditor : UserControl
    {
        #region Ctors:
        public BannerEditor()
        {
            InitializeComponent();
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
                cmbPriority.ItemsSource = priorityDic;

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

            }
            catch (Exception ex)
            {
                ExceptionManager.Handle(ex);
            }
        }
        #endregion

        private void cmbProvince_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
