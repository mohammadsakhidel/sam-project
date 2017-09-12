using SamModels.DTOs;
using SamUxLib.Resources.Values;
using SamWeb.Code.MvcExtensions.ModelBinders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SamWeb.Models.ViewModels
{
    [ModelBinder(typeof(CreateConsolationStep2ModelBinder))]
    public class CreateConsolationTemplateSelectionStepVM
    {
        [Required(ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "Required_Template")]
        public int? TemplateID { get; set; }

        #region View Specific:
        public List<TemplateCategoryDto> Categories { get; set; }
        public List<TemplateDto> Templates { get; set; }
        public List<Tuple<int, bool>> CategoryStates { get; set; }
        #endregion
    }
}