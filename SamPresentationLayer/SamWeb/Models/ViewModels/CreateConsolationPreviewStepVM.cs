using SamModels.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SamWeb.Models.ViewModels
{
    public class CreateConsolationPreviewStepVM
    {
        public ConsolationDto CreatedConsolation { get; set; }

        public string PaymentID { get; set; }

        public string PaymentToken { get; set; }

        public string BankPageUrl { get; set; }
    }
}