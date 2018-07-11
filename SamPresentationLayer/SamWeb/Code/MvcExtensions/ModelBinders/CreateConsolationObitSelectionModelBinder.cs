using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SamWeb.Code.MvcExtensions.ModelBinders
{
    public class CreateConsolationObitSelectionModelBinder : DefaultModelBinder
    {
        protected override void BindProperty(ControllerContext controllerContext, ModelBindingContext bindingContext, PropertyDescriptor propertyDescriptor)
        {
            #region RelatedObitIds binding:
            if (propertyDescriptor.Name == "RelatedObitIds")
            {
                var form = controllerContext.HttpContext.Request.Form;
                var prefix = "relatedobit_";
                var formKeys = form.AllKeys.Where(fk => fk.StartsWith(prefix));
                var relatedObitIds = new List<int>();
                foreach (var fk in formKeys)
                {
                    int id;
                    int.TryParse(fk.Substring(12, fk.Length - 12), out id);
                    relatedObitIds.Add(id);
                }
                propertyDescriptor.SetValue(bindingContext.Model, relatedObitIds);
            }
            #endregion

            base.BindProperty(controllerContext, bindingContext, propertyDescriptor);
        }
    }
}