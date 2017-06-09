using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamModels.DTOs
{
    public class ConsolationsUpdatePackDto
    {
        public List<ConsolationDto> Consolations { get; set; }
        public DateTime QueryTime { get; set; }
    }
}
