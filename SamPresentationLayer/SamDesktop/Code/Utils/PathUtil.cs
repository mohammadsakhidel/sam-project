
using System;
using System.IO;

namespace SamDesktop.Code.Utils
{
    public class PathUtil
    {
        public static string GetCitiesFilePath()
        {
            var path = Path.Combine(Environment.CurrentDirectory, @"Resources\XML\ir-cities.xml");
            return path;
        }
    }
}
