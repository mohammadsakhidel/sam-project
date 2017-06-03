using SamModels.DTOs;
using SamWeb.Code.MvcExtensions.ModelBinders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SamWeb.Models.ViewModels
{
    [ModelBinder(typeof(CreateConsolationTemplateInfoStepModelBinder))]
    public class CreateConsolationTemplateInfoStep
    {
        public int ObitID { get; set; }

        public int TemplateID { get; set; }

        public string FullName { get; set; }

        public string CellPhoneNumber { get; set; }

        public List<TemplateFieldPresenter> Fields { get; set; } //name, displayName, value
    }
}