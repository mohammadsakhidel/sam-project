using SamModels.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamClientDataAccess.ClientModels
{
    public class ConsolationImage
    {
        [Key]
        [Required]
        public int ConsolationID { get; set; }

        [Required]
        public byte[] Bytes { get; set; }

        [Required]
        public DateTime CreationTime { get; set; }

        [Required]
        public DateTime LastUpdateTime { get; set; }
    }
}
