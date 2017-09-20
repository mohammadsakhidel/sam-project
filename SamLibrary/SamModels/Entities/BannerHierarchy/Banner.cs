using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamModels.Entities
{
    public abstract class Banner
    {
        #region Porps:
        public int ID { get; set; }

        [Required]
        [MaxLength(64)]
        public string Title { get; set; }

        [Required]
        [MaxLength(32)]
        public string ImageID { get; set; }

        public DateTime? LifeBeginTime { get; set; }

        public DateTime? LifeEndTime { get; set; }

        [Required]
        public bool IsActive { get; set; }

        [Required]
        public int Priority { get; set; }

        [Required]
        public bool ShowOnStart { get; set; }

        [Required]
        public int DurationSeconds { get; set; }

        [Required]
        public int Interval { get; set; }

        [Required]
        public DateTime CreationTime { get; set; }

        [Required]
        [MaxLength(16)]
        public string Creator { get; set; }

        [Required]
        public DateTime LastUpdateTime { get; set; }
        #endregion
    }
}
