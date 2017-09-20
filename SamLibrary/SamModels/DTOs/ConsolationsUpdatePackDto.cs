using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamModels.DTOs
{
    public class ConsolationsUpdatePackDto
    {
        public MosqueDto Mosque { get; set; }
        public ObitDto[] Obits { get; set; }
        public TemplateDto[] Templates { get; set; }
        public ImageBlobDto[] ImageBlobs { get; set; }
        public ConsolationDto[] Consolations { get; set; }
        public BannerHierarchyDto[] Banners { get; set; }
        public RemovedEntityDto[] RemovedEntities { get; set; }
        public DateTime QueryTime { get; set; }
    }
}
