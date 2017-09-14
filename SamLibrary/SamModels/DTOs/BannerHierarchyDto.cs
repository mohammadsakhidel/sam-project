using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamModels.DTOs
{
    public class BannerHierarchyDto
    {
        #region Base:
        public int ID { get; set; }
        public string Title { get; set; }
        public string ImageID { get; set; }
        public DateTime? LifeBeginTime { get; set; }
        public DateTime? LifeEndTime { get; set; }
        public bool IsActive { get; set; }
        public int Priority { get; set; }
        public bool ShowOnStart { get; set; }
        public int DurationSeconds { get; set; }
        public int Interval { get; set; }
        public DateTime CreationTime { get; set; }
        public string Creator { get; set; }
        public DateTime LastUpdateTime { get; set; }
        #endregion
        #region Area:
        public int? CityID { get; set; }
        public int? ProvinceID { get; set; }
        #endregion
        #region Holding:
        public int ObitHoldingID { get; set; }
        #endregion
        #region Mosque:
        public int MosqueID { get; set; }
        #endregion
    }
}
