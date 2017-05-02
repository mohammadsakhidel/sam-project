using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SamModels.Entities.Core
{
    public class Obit
    {
        public int ID { get; set; }

        [Required]
        public int MosqueID { get; set; }

        [Required]
        public DateTime CreationTime { get; set; }

        #region Navigation Props:
        public virtual Mosque Mosque { get; set; }
        public virtual ICollection<DeceasedPerson> DeceasedPersons { get; set; }
        public virtual ICollection<Consolation> Consolations { get; set; }
        public virtual ICollection<ObitHolding> ObitHoldings { get; set; }
        #endregion
    }
}
