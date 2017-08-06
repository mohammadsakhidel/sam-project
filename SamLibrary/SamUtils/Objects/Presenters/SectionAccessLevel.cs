using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamUtils.Objects.Presenters
{
    public class SectionAccessLevel
    {
        public SectionAccessLevel()
        {

        }

        public SectionAccessLevel(string name, bool create, bool read, bool update, bool delete,
            bool createNeeded, bool readNeeded, bool updateNeeded, bool deleteNeeded)
        {
            Name = name;
            Create = create;
            Read = read;
            Update = update;
            Delete = delete;
            CreateNeeded = createNeeded;
            ReadNeeded = readNeeded;
            UpdateNeeded = updateNeeded;
            DeleteNeeded = deleteNeeded;
        }

        public string Name { get; set; }

        public bool Create { get; set; }
        public bool Read { get; set; }
        public bool Update { get; set; }
        public bool Delete { get; set; }

        public bool CreateNeeded { get; set; }
        public bool ReadNeeded { get; set; }
        public bool UpdateNeeded { get; set; }
        public bool DeleteNeeded { get; set; }
    }
}
