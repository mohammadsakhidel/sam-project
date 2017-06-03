using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SamWeb.Code.MvcExtensions.ModelBinders
{
    public class CreateConsolationStep2ModelBinder : DefaultModelBinder
    {
        protected override void BindProperty(ControllerContext controllerContext, ModelBindingContext bindingContext, PropertyDescriptor propertyDescriptor)
        {
            #region CategoryStates prop binding:
            if (propertyDescriptor.Name == "CategoryStates")
            {
                var form = controllerContext.HttpContext.Request.Form;
                var formKeys = form.AllKeys.Where(k => k.StartsWith("cs_"));
                var categoryStates = new List<Tuple<int, bool>>();
                foreach (var key in formKeys)
                {
                    var categoryId = Convert.ToInt32(key.Split('_')[1]);
                    var state = Convert.ToBoolean(form[key]);
                    var cs = new Tuple<int, bool>(categoryId, state);
                    categoryStates.Add(cs);
                }
                propertyDescriptor.SetValue(bindingContext.Model, categoryStates);
            }
            #endregion

            base.BindProperty(controllerContext, bindingContext, propertyDescriptor);
        }
    }
}