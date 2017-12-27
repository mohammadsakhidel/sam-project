using System;
using System.ComponentModel.DataAnnotations;

namespace SamModels.Entities
{
    public class Consolation
    {
        public int ID { get; set; }

        [Required]
        public int ObitID { get; set; }

        [Required]
        public int CustomerID { get; set; }

        [Required]
        public int TemplateID { get; set; }

        /// <summary>
        /// This field is a container for additional info for selected template, such as relationship
        /// of deseaced person and audience. These informations whill be stored as a JSON object.
        /// </summary>
        [MaxLength(1024)]
        public string TemplateInfo { get; set; }

        [MaxLength(512)]
        public string Audience { get; set; }

        [MaxLength(512)]
        public string From { get; set; }

        [Required]
        [MaxLength(16)]
        public string Status { get; set; }

        [Required]
        [MaxLength(16)]
        public string PaymentStatus { get; set; }

        [MaxLength(32)]
        public string PaymentID { get; set; }

        [Required]
        public DateTime CreationTime { get; set; }

        public DateTime? LastUpdateTime { get; set; }

        [Required]
        [MaxLength(16)]
        public string TrackingNumber { get; set; }

        [Required]
        public double AmountToPay { get; set; }

        [MaxLength(512)]
        public string ExtraData { get; set; }

        #region Navigation Props:
        public virtual Obit Obit { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual Template Template { get; set; }
        #endregion
    }
}
