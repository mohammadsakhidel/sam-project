using SamUtils.Constants;
using SamWeb.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SamWeb.Models.ViewModels
{
    public class CreateConsolationCustomerInfoStep
    {
        [Required(ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "Required_FullName")]
        public string FullName { get; set; }

        [Required(ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "Required_CellPhoneNumber")]
        [RegularExpression(Patterns.cellphone, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "Invalid_CellPhoneNumber")]
        public string CellPhoneNumber { get; set; }
    }
}