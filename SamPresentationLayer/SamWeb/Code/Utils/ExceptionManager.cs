using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SamWeb.Code.Utils
{
    public class ExceptionManager
    {
        public static string GetProperMessage(Exception ex)
        {
            return $"EXCEPTION: {ex.Message}";
        }
    }
}