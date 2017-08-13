using AutoMapper;
using RamancoLibrary.Utilities;
using SamClient.Views.Windows;
using SamModels.DTOs;
using SamModels.Entities.Blobs;
using SamModels.Entities.Core;
using SamUtils.Constants;
using SamUtils.Utils;
using SamUtils.Objects.Exceptions;
using SamUxLib.Code.Utils;
using SamUxLib.Resources.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Transactions;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SamClientDataAccess.Repos;

namespace SamClient.Views.Partials
{
    public partial class Consolations : UserControl
    {
        #region Fields:
        MainWindow _mainWindow;
        #endregion

        #region Ctors:
        public Consolations(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
            InitializeComponent();
        }
        #endregion

        #region Event Handlers:
        private void ucPersianDateNavigator_OnChange(object sender, SamUxLib.UserControls.DateChangedEventArgs e)
        {
            try
            {
                LoadConsolations(e.NewDate);
            }
            catch (Exception ex)
            {
                ExceptionManager.Handle(ex);
            }
        }
        //private async void btnDownload_Click(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        using (var srep = new ClientSettingRepo())
        //        using (var crep = new ConsolationRepo())
        //        using (var orep = new ObitRepo())
        //        using (var urep = new CustomerRepo())
        //        using (var trep = new TemplateRepo())
        //        using (var erep = new TemplateCategoryRepo())
        //        using (var brep = new BlobRepo())
        //        {
        //            var setting = srep.Get(1);
        //            if (setting == null)
        //                throw new ValidationException(Messages.SetSettingsBeforeSync);

        //            progress.IsBusy = true;
        //            using (var hc = HttpUtil.CreateClient())
        //            {
        //                var response = await hc.GetAsync($"{ApiActions.consolations_getupdates}?mosqueId={setting.MosqueID}");
        //                var updatePack = await response.Content.ReadAsAsync<ConsolationsUpdatePackDto>();
        //                var dtos = updatePack.Consolations;
        //                var consolations = dtos.Select(dto => Mapper.Map<ConsolationDto, Consolation>(dto)).ToList();
        //                using (var ts = new TransactionScope())
        //                {
        //                    foreach (var consolation in consolations)
        //                    {
        //                        #region Add Obit:
        //                        if (!orep.Exists(consolation.ObitID))
        //                            orep.AddWithSave(consolation.Obit);
        //                        consolation.Obit = null;
        //                        #endregion
        //                        #region Add Customer:
        //                        if (!urep.Exists(consolation.CustomerID))
        //                            urep.AddWithSave(consolation.Customer);
        //                        consolation.Customer = null;
        //                        #endregion
        //                        #region Add Template:
        //                        if (!erep.Exists(consolation.Template.TemplateCategoryID))
        //                            erep.AddWithSave(consolation.Template.Category);
        //                        consolation.Template.Category = null;

        //                        if (!trep.Exists(consolation.TemplateID))
        //                            trep.AddWithSave(consolation.Template);

        //                        #region download image:
        //                        if (!brep.Exists(consolation.Template.BackgroundImageID))
        //                        {
        //                            var bgBytes = await hc.GetByteArrayAsync($"{ApiActions.blobs_getimage}/{consolation.Template.BackgroundImageID}");
        //                            var blob = new ImageBlob
        //                            {
        //                                ID = consolation.Template.BackgroundImageID,
        //                                Bytes = bgBytes,
        //                                CreationTime = DateTimeUtils.Now
        //                            };
        //                            brep.AddWithSave(blob);
        //                        }
        //                        #endregion

        //                        consolation.Template = null;
        //                        #endregion
        //                        #region Add Consolation:
        //                        if (!crep.Exists(consolation.ID))
        //                            crep.AddWithSave(consolation);
        //                        #endregion
        //                    }
        //                    ts.Complete();
        //                }
        //                progress.IsBusy = false;
        //                //UxUtil.ShowMessage(Messages.SuccessfullyDone);
        //                #region Reload:
        //                LoadConsolations(ucPersianDateNavigator.GetMiladyDate().HasValue ? ucPersianDateNavigator.GetMiladyDate().Value : DateTimeUtils.Now);
        //                #endregion
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        progress.IsBusy = false;
        //        ExceptionManager.Handle(ex);
        //    }
        //}
        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var player = new Player();
                player.Show();
            }
            catch (Exception ex)
            {
                ExceptionManager.Handle(ex);
            }
        }
        #endregion

        #region Methods:
        private void LoadConsolations(DateTime date)
        {
            using (var crep = new ConsolationRepo())
            {
                var consolations = crep.GetAll(date);
                dgConsolations.ItemsSource = consolations;
            }
        }
        #endregion
    }
}
