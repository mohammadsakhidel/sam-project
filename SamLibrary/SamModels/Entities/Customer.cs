using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamModels.Entities
{
    public class Customer
    {
        public int ID { get; set; }

        [Required]
        [MaxLength(64)]
        public string FullName { get; set; }

        public bool? Gender { get; set; }

        [Required]
        public bool IsMember { get; set; }

        [MaxLength(32)]
        public string UserName { get; set; }

        public DateTime? RegistrationTime { get; set; }

        [MaxLength(16)]
        [Required]
        public string CellPhoneNumber { get; set; }
    }
}
