using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamModels.DTOs
{
    public class ConsolationDto
    {
        public int ID { get; set; }

        public int ObitID { get; set; }

        public int CustomerID { get; set; }

        public int TemplateID { get; set; }

        public string TemplateInfo { get; set; }

        public string Audience { get; set; }

        public string From { get; set; }

        public string Status { get; set; }

        public string PaymentStatus { get; set; }

        public string PaymentID { get; set; }

        public double AmountToPay { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime? LastUpdateTime { get; set; }

        public string TrackingNumber { get; set; }

        public string ExtraData { get; set; }

        public string OtherObits { get; set; }

        #region Navigation:
        public CustomerDto Customer { get; set; }
        public ObitDto Obit { get; set; }
        public TemplateDto Template { get; set; }
        #endregion

        #region Additional Fields:
        public bool PayedByPOS { get; set; } = false;
        #endregion
    }
}