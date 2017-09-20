using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamModels.Entities
{
    public class RemovedEntity
    {
        public int ID { get; set; }

        [Required]
        [MaxLength(32)]
        public string EntityType { get; set; }

        [Required]
        public DateTime RemovingTime { get; set; }

        [MaxLength(128)]
        public string metadata { get; set; }
    }
}
