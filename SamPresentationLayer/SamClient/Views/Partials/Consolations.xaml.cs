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
        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _mainWindow.LoadContent(new HomePage(_mainWindow));
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
