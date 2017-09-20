using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SamModels.Entities
{
    public class Mosque
    {
        #region Props:
        public int ID { get; set; }

        [Required]
        [MaxLength(32)]
        public string Name { get; set; }

        [MaxLength(32)]
        public string ImamName { get; set; }

        [MaxLength(16)]
        public string ImamCellPhone { get; set; }

        [MaxLength(32)]
        public string InterfaceName { get; set; }

        [MaxLength(16)]
        public string InterfaceCellPhone { get; set; }

        [Required]
        public int CityID { get; set; }

        [Required]
        [MaxLength(256)]
        public string Address { get; set; }

        [MaxLength(32)]
        public string Location { get; set; }

        [Required]
        [MaxLength(16)]
        public string PhoneNumber { get; set; }

        [Required]
        public DateTime CreationTime { get; set; }

        public DateTime? LastUpdateTime { get; set; }

        [Required]
        [MaxLength(32)]
        public string Creator { get; set; }

        [MaxLength(32)]
        public string ImageID { get; set; }
        #endregion

        #region Navigation Props:
        public virtual ICollection<Obit> Obits { get; set; }
        public virtual ICollection<Saloon> Saloons { get; set; }
        #endregion
    }
}
