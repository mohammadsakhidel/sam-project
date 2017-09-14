using AutoMapper;
using SamClientDataAccess.Repos;
using SamModels.DTOs;
using SamModels.Entities;
using SamUtils.Objects.Exceptions;
using SamUtils.Utils;
using SamUxLib.Code.Utils;
using SamUxLib.Resources.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SamClient.Views.Windows
{
    public partial class ClientSettingsWindow : Window
    {
        #region Ctors:
        public ClientSettingsWindow()
        {
            InitializeComponent();
        }
        #endregion

        #region Event HandlerS:
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                #region Validation:
                var validationResult = ucClientSettings.IsValid();
                if (!validationResult.Item1)
                    throw new ValidationException(validationResult.Item2);
                #endregion

                #region Save Settings:
                using (var ts = new TransactionScope())
                using (var srep = new ClientSettingRepo())
                using (var mrep = new MosqueRepo(srep.Context))
                {
                    var newClientSetting = ucClientSettings.ClientSetting;

                    #region Save Mosque If Not Exists:
                    var selectedMosque = ucClientSettings.SelectedMosque;
                    var mosqueExists = mrep.Exists(selectedMosque.ID);
                    if (!mosqueExists)
                    {
                        var mosque = Mapper.Map<MosqueDto, Mosque>(selectedMosque);
                        mrep.AddWithSave(mosque);
                    }
                    #endregion

                    #region Save Settings:
                    if (srep.Exists(1))
                    {
                        srep.Update(newClientSetting);
                    }
                    else
                    {
                        srep.Add(newClientSetting);
                    }
                    srep.Save();
                    #endregion

                    ts.Complete();

                    DialogResult = true;
                    Close();
                }
                #endregion
            }
            catch (Exception ex)
            {
                ExceptionManager.Handle(ex);
            }
        }
        #endregion

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                #region Load Current Settings:
                using (var srepo = new ClientSettingRepo())
                using (var mrepo = new MosqueRepo(srepo.Context))
                {
                    var settings = srepo.Get();
                    if (settings != null)
                    {
                        var mosque = mrepo.Get(settings.MosqueID);
                        if (mosque != null)
                        {
                            var city = CityUtil.GetCity(mosque.CityID);
                            ucClientSettings.ProvinceID = city.ProvinceID;
                            ucClientSettings.CityID = city.ID;
                            ucClientSettings.ClientSetting = settings;
                        }
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                ExceptionManager.Handle(ex);
            }
        }
    }
}
