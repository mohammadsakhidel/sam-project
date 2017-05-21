using SamModels.DTOs;
using SamModels.Entities.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SamDesktop.Code.Utils
{
    public class CityUtil
    {
        public static List<ProvinceDto> GetProvinces(string filePath)
        {
            var doc = XDocument.Load(filePath);
            var provinces = doc.Root.Elements().Select(el => new ProvinceDto() {
                ID = Convert.ToInt32(el.Attribute("ID").Value),
                Name = el.Attribute("Name").Value.ToString()
            }).ToList();
            return provinces;
        }

        public static List<CityDto> GetProvinceCities(int provId, string filePath)
        {
            var doc = XDocument.Load(filePath);
            var list = doc.Root.Elements()
                .SingleOrDefault(p => Convert.ToInt32(p.Attribute("ID").Value) == provId)
                .Elements().Select(el => new CityDto() {
                    ID = Convert.ToInt32(el.Attribute("ID").Value),
                    Name = el.Attribute("Name").Value.ToString()
                }).ToList();
            return list;
        }
    }
}
