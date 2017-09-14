using SamModels.DTOs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace SamUtils.Utils
{
    public class CityUtil
    {
        public static List<ProvinceDto> Provinces
        {
            get
            {
                if (Func_GetXMLContent == null)
                    throw new ArgumentException("GetXMLContent Function is not specified.");

                var xml = Func_GetXMLContent.Invoke();
                var doc = XDocument.Parse(xml);
                var provinces = doc.Root.Elements().Select(el => new ProvinceDto()
                {
                    ID = Convert.ToInt32(el.Attribute("ID").Value),
                    Name = el.Attribute("Name").Value.ToString()
                }).ToList();
                return provinces;
            }
        }

        public static List<CityDto> GetProvinceCities(int provId)
        {
            var xml = Func_GetXMLContent.Invoke();
            var doc = XDocument.Parse(xml);
            var list = doc.Root.Elements()
                .SingleOrDefault(p => Convert.ToInt32(p.Attribute("ID").Value) == provId)
                .Elements().Select(el => new CityDto() {
                    ID = Convert.ToInt32(el.Attribute("ID").Value),
                    Name = el.Attribute("Name").Value.ToString()
                }).ToList();
            return list;
        }

        public static ProvinceDto GetProvince(int cityId)
        {
            var xml = Func_GetXMLContent.Invoke();
            var doc = XDocument.Parse(xml);
            var province = doc.Root.Elements()
                .FirstOrDefault(p => p.Elements().Any(c => Convert.ToInt32(c.Attribute("ID").Value) == cityId));
            return new ProvinceDto {
                ID = Convert.ToInt32(province.Attribute("ID").Value),
                Name = province.Attribute("Name").Value.ToString()
            };
        }

        public static CityDto GetCity(int cityId)
        {
            var xml = Func_GetXMLContent.Invoke();
            var doc = XDocument.Parse(xml);
            var city = doc.Root.Descendants("City")
                .SingleOrDefault(c => Convert.ToInt32(c.Attribute("ID").Value) == cityId);
            return new CityDto
            {
                ID = Convert.ToInt32(city.Attribute("ID").Value),
                Name = city.Attribute("Name").Value.ToString(),
                ProvinceID = Convert.ToInt32(city.Parent.Attribute("ID").Value)
            };
        }

        public static Func<string> Func_GetXMLContent { get; set; }
    }
}
