using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamModels.DTOs
{
    public class CustomerDto
    {
        public int ID { get; set; }

        public string FullName { get; set; }

        public bool? Gender { get; set; }

        public bool IsMember { get; set; }

        public string UserName { get; set; }

        public DateTime? RegistrationTime { get; set; }

        public string CellPhoneNumber { get; set; }
    }
}
