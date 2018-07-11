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
    [ModelBinder(typeof(CreateConsolationObitSelectionModelBinder))]
    public class CreateConsolationObitSelectionStepVM
    {
        public int? ProvinceID { get; set; }

        public int? CityID { get; set; }

        public int? MosqueID { get; set; }

        [Required(ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "Required_Obit")]
        public int? ObitID { get; set; }

        public List<CityDto> Cities { get; set; }

        public List<MosqueDto> Mosques { get; set; }

        public List<ObitDto> Obits { get; set; }

        public List<ObitDto> AllRelatedObits { get; set; }

        public List<int> RelatedObitIds { get; set; }
    }
}