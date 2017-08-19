using Newtonsoft.Json;
using SamModels.DTOs;
using SamUtils.Constants;
using SamUtils.Enums;
using SamUtils.Objects.Exceptions;
using SamUtils.Utils;
using SamUxLib.Code.Utils;
using SamUxLib.Resources.Values;
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
using System.Windows.Shapes;

namespace SamDesktop.Views.Windows
{
    public partial class EditConsolationWindow : Window
    {
        #region Fields:
        ConsolationDto _consolationToEdit = null;
        #endregion

        #region Ctors:
        public EditConsolationWindow(ConsolationDto consolationToEdit)
        {
            InitializeComponent();
            _consolationToEdit = consolationToEdit;
            this.DataContext = consolationToEdit;
        }
        #endregion

        #region Event Handlers:
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadConsolationFields();
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
                #region Gather Fields:
                var fieldsDic = new Dictionary<string, string>();
                foreach (var child in gridFields.Children)
                {
                    if (child is TextBox)
                    {
                        var input = (TextBox)child;
                        fieldsDic.Add(input.Tag.ToString(), input.Text);
                    }
                }
                #endregion

                #region Validate:
                if (fieldsDic.Where(d => string.IsNullOrEmpty(d.Value)).Any())
                    throw new ValidationException(Messages.FillRequiredFields);
                #endregion

                #region Send Request To server;
                progress.IsBusy = true;
                using (var hc = HttpUtil.CreateClient())
                {
                    var confirmed = ConsolationStatus.confirmed.ToString();
                    var pending = ConsolationStatus.pending.ToString();
                    var canceled = ConsolationStatus.canceled.ToString();
                    var displayed = ConsolationStatus.displayed.ToString();

                    var newstatus = (_consolationToEdit.Status == confirmed || _consolationToEdit.Status == displayed ? canceled : (_consolationToEdit.Status == pending || _consolationToEdit.Status == canceled ? confirmed : ""));
                    var url = $"{ApiActions.consolations_update}/{_consolationToEdit.ID}?newstatus={newstatus}";
                    var response = await hc.PutAsJsonAsync(url, fieldsDic);
                    response.EnsureSuccessStatusCode();
                    progress.IsBusy = false;
                    UxUtil.ShowMessage(Messages.SuccessfullyDone);
                    DialogResult = true;
                    Close();
                }
                #endregion
            }
            catch (Exception ex)
            {
                progress.IsBusy = false;
                ExceptionManager.Handle(ex);
            }
        }
        #endregion

        #region Methods:
        private void LoadConsolationFields()
        {
            if (_consolationToEdit != null)
            {
                var dic = JsonConvert.DeserializeObject<Dictionary<string, string>>(_consolationToEdit.TemplateInfo);

                #region Define Grid Rows:
                foreach (var key in dic.Keys)
                {
                    gridFields.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                    gridFields.RowDefinitions.Add(new RowDefinition() { Height = (GridLength)TryFindResource("grid_row_spacing") });
                }
                #endregion

                #region Add Controls To Grid:
                var keys = dic.Keys.ToList();
                for (var i = 0; i < keys.Count(); i++)
                {
                    var key = keys[i];
                    #region add label:
                    var label = new Label()
                    {
                        Style = TryFindResource("form_label_required") as Style,
                        Content = _consolationToEdit.Template.TemplateFields.SingleOrDefault(f => f.Name == key)?.DisplayName
                    };
                    Grid.SetRow(label, i * 2);
                    gridFields.Children.Add(label);
                    #endregion
                    #region add textbox:
                    var textbox = new TextBox()
                    {
                        Tag = key,
                        Text = dic[key],
                        AcceptsReturn = true,
                        Height = 60,
                        VerticalContentAlignment = VerticalAlignment.Top
                    };
                    Grid.SetRow(textbox, i * 2);
                    Grid.SetColumn(textbox, 1);
                    gridFields.Children.Add(textbox);
                    #endregion
                }
                #endregion
            }
        }
        #endregion
    }
}
