using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamUtils.Objects.Presenters
{
    public class ObitTypePresenter
    {
        public ObitTypePresenter(string key, string displayName)
        {
            Key = key;
            DisplayName = displayName;
        }

        public string Key { get; set; }
        public string DisplayName { get; set; }

        public override string ToString()
        {
            return DisplayName;
        }
    }
}
