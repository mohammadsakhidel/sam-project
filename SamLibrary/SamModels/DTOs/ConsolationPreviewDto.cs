using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamModels.DTOs
{
    public class ConsolationPreviewDto
    {
        public ConsolationDto Consolation { get; set; }
        public PaymentDto Payment { get; set; }
        public string MoreData { get; set; }
    }
}
