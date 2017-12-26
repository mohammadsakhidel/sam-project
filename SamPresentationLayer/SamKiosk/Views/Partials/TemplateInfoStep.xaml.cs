using Newtonsoft.Json;
using SamKiosk.Code.Utils;
using SamModels.DTOs;
using SamUtils.Constants;
using SamUtils.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
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
    public partial class TemplateInfoStep : UserControl
    {
        #region Fields:
        SendConsolationView _parent;
        #endregion

        #region Constructors:
        public TemplateInfoStep(SendConsolationView parent)
        {
            InitializeComponent();
            _parent = parent;
        }
        #endregion

        #region Event Handlers:
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                _parent.SetNavigationState(true, false, true, true);
                CreateForm();

                #region state:
                if (_parent.Fields != null && _parent.Fields.Any())
                {
                    foreach (var fKey in _parent.Fields.Keys)
                    {
                        var input = fieldsContainer.Children.OfType<TextBox>().SingleOrDefault(i => i.Name == fKey);
                        if (input != null)
                        {
                            input.Text = _parent.Fields[fKey];
                        }
                    }
                }
                #endregion

                #region on next action:
                _parent.OnNextAction = async () => {
                    try
                    {
                        #region instantiate consolation dto object:
                        var dto = new ConsolationDto();
                        dto.ObitID = _parent.SelectedObit.ID;
                        dto.TemplateID = _parent.SelectedTemplate.ID;
                        dto.Customer = _parent.SelectedCustomer;
                        dto.TemplateInfo = JsonConvert.SerializeObject(_parent.Fields);
                        dto.Audience = _parent.Fields["Audience"];
                        dto.From = _parent.Fields["From"];
                        dto.PayedByPOS = true;
                        #endregion

                        #region call server:
                        progress.IsBusy = true;
                        using (var hc = HttpUtil.CreateClient())
                        {
                            #region update:
                            if (_parent.CreatedConsolationID > 0)
                            {
                                dto.ID = _parent.CreatedConsolationID;
                                var response = await hc.PutAsJsonAsync(ApiActions.consolations_updatev2, dto);
                                HttpUtil.EnsureSuccessStatusCode(response);
                            }
                            #endregion
                            #region create:
                            else
                            {
                                var response = await hc.PostAsJsonAsync(ApiActions.consolations_create, dto);
                                HttpUtil.EnsureSuccessStatusCode(response);
                                var info = await response.Content.ReadAsAsync<Dictionary<string, string>>();
                                var consolationId = Convert.ToInt32(info["ID"]);
                                _parent.CreatedConsolationID = consolationId;
                            }
                            #endregion
                        }
                        progress.IsBusy = false;
                        #endregion
                    }
                    catch (Exception ex)
                    {
                        progress.IsBusy = false;
                        KioskExceptionManager.Handle(ex);
                    }
                };
                #endregion
            }
            catch (Exception ex)
            {
                KioskExceptionManager.Handle(ex);
            }
        }
        private void FormTextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                ValidateForm();
            }
            catch (Exception ex)
            {
                KioskExceptionManager.Handle(ex);
            }
        }
        #endregion

        #region Methods:
        void CreateForm()
        {
            var fields = _parent.SelectedTemplate.TemplateFields;
            if (fields != null && fields.Any())
            {
                foreach (var field in fields)
                {
                    var label = new Label();
                    label.Content = field.DisplayName;
                    label.Style = FindResource("kiosk_label") as Style;

                    var textBox = new TextBox();
                    textBox.Name = field.Name;
                    textBox.Style = FindResource("kiosk_textbox") as Style;
                    textBox.TextChanged += FormTextChanged;

                    var desc = new TextBlock();
                    desc.Text = field.Description;
                    desc.Style = FindResource("kiosk_item_desc") as Style;

                    fieldsContainer.Children.Add(label);
                    fieldsContainer.Children.Add(textBox);
                    fieldsContainer.Children.Add(desc);
                }
            }
        }
        void ValidateForm()
        {
            var fields = new Dictionary<string, string>();
            var isValid = true;

            foreach (var input in fieldsContainer.Children)
            {
                if (input is TextBox)
                {
                    var t = ((TextBox)input);

                    if (string.IsNullOrEmpty(t.Text))
                        isValid = false;
                    else
                        fields.Add(t.Name, t.Text);
                }
            }

            if (isValid)
            {
                _parent.Fields = fields;
                _parent.SetNavigationState(true, true, true, true);
            }
            else
            {
                _parent.SetNavigationState(true, false, true, true);
            }
        }
        #endregion
    }
}
