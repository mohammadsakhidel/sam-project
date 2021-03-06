using SamUxLib.Resources.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamUxLib.Resources
{
    public class ResourceManager
    {
        public static string GetValue(string key, string className = "Strings")
        {
            var resManager = new System.Resources.ResourceManager($"SamUxLib.Resources.Values.{className}", typeof(Strings).Assembly);
            return resManager.GetString(key, System.Threading.Thread.CurrentThread.CurrentUICulture);
        }
    }
}
