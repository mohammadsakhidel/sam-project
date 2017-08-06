using Newtonsoft.Json;
using SamUtils.Constants;
using SamUtils.Objects.Presenters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamUtils.Utils
{
    public class AccessUtil
    {
        #region SectionNames:
        public const string section_mosques = "section_mosques";
        public const string section_obits = "section_obits";
        public const string section_templates = "section_templates";
        public const string section_accounts = "section_accounts";
        public const string section_roles = "section_roles";
        #endregion

        #region Methods:
        public static List<SectionAccessLevel> GetDefaults()
        {
            return new List<SectionAccessLevel> {
                    new SectionAccessLevel(section_accounts, false, false, false, false, true, true, true, true),
                    new SectionAccessLevel(section_mosques, false, false, false, false, true, true, true, true),
                    new SectionAccessLevel(section_obits, false, false, false, false, true, true, true, true),
                    new SectionAccessLevel(section_roles, false, false, false, false, true, true, true, true),
                    new SectionAccessLevel(section_templates, false, false, false, false, true, true, true, true)
                };
        }

        public static string Serialize(IEnumerable<SectionAccessLevel> accessLevels)
        {
            var objects = accessLevels.Select(o => new
            {
                name = o.Name,
                access = $"{(o.Create ? "1" : "0")}{(o.Read ? "1" : "0")}{(o.Update ? "1" : "0")}{(o.Delete ? "1" : "0")}"
            });
            return JsonConvert.SerializeObject(objects);
        }

        public static List<SectionAccessLevel> Deserialize(string accessJson)
        {
            var result = new List<SectionAccessLevel>();
            var objects = JsonConvert.DeserializeObject<IEnumerable<dynamic>>(accessJson);
            var defaults = GetDefaults();
            foreach (var o in objects)
            {
                var def = defaults.SingleOrDefault(d => d.Name == (o.name as string)) != null ? defaults.SingleOrDefault(d => d.Name == (o.name as string)) : null;
                result.Add(new SectionAccessLevel
                {
                    Name = o.name,
                    Create = (Convert.ToString(o.access))[0] == '1',
                    CreateNeeded = def != null ? def.CreateNeeded : true,
                    Read = (Convert.ToString(o.access))[1] == '1',
                    ReadNeeded = def != null ? def.ReadNeeded : true,
                    Update = (Convert.ToString(o.access))[2] == '1',
                    UpdateNeeded = def != null ? def.UpdateNeeded : true,
                    Delete = (Convert.ToString(o.access))[3] == '1',
                    DeleteNeeded = def != null ? def.DeleteNeeded : true,

                });
            }
            return result;
        }
        #endregion
    }
}
