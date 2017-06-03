using SamWeb.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SamWeb.Code.MvcExtensions.ModelBinders
{
    public class CreateConsolationTemplateInfoStepModelBinder : DefaultModelBinder
    {
        protected override void BindProperty(ControllerContext controllerContext, ModelBindingContext bindingContext, PropertyDescriptor propertyDescriptor)
        {
            if (propertyDescriptor.Name == "Fields")
            {
                var form = controllerContext.HttpContext.Request.Form;
                var prefix = "tf_";
                var descPrefix = "tfdc_";
                var displayNamePrefix = "tfdn_";
                var formKeys = form.AllKeys.Where(k => k.StartsWith(prefix));
                var list = new List<TemplateFieldPresenter>();
                foreach (var key in formKeys)
                {
                    var fieldName = key.Substring(prefix.Length, key.Length - prefix.Length);
                    var fieldValue = form[key];
                    var fieldDesc = form[$"{descPrefix}{fieldName}"];
                    var fieldDisplayName = form[$"{displayNamePrefix}{fieldName}"];
                    list.Add(new TemplateFieldPresenter { Name = fieldName, Value = fieldValue, Description = fieldDesc, DisplayName = fieldDisplayName });
                }
                propertyDescriptor.SetValue(bindingContext.Model, list);
            }

            base.BindProperty(controllerContext, bindingContext, propertyDescriptor);
        }
    }
}