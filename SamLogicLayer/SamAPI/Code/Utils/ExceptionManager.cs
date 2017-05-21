using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SamAPI.Code.Utils
{
    public class ExceptionManager
    {
        public static string GetProperApiMessage(Exception ex)
        {
            return $"API ERROR: {ex.Message}";
        }
    }
}