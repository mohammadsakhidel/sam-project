using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamModels.DTOs
{
    public class PaymentDto
    {
        #region Entity Props:
        public string ID { get; set; }

        public string Token { get; set; }

        public int Amount { get; set; }

        public string Provider { get; set; }

        public string Status { get; set; }

        public string ReferenceCode { get; set; }

        public string Type { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime LastUpdateTime { get; set; }
        #endregion

        #region Extensions:
        public string BankPageUrl { get; set; }
        #endregion
    }
}
