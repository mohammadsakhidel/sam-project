using SamUxLib.Resources.Values;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SamWeb.Models.ViewModels
{
    public class SignInVM
    {
        [Required(ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "RequiredUserName")]
        public string UserName { get; set; }

        [Required(ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "RequiredPassword")]
        public string Password { get; set; }

        [Required]
        public bool RememberMe { get; set; }
    }
}