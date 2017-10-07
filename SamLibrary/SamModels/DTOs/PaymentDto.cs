using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamModels.DTOs
{
    public class PaymentDto
    {
        public string ID { get; set; }

        public string Token { get; set; }

        public int Amount { get; set; }

        public string Provider { get; set; }

        public string Status { get; set; }

        public string ReferenceCode { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime LastUpdateTime { get; set; }
    }
}
