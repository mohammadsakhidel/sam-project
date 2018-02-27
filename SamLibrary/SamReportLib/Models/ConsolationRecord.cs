using System;

namespace SamReportLib.Models
{
    public class ConsolationRecord
    {
        public int ID { get; set; }
        public string MosqueName { get; set; }
        public string ObitTitle { get; set; }
        public string CustomerCellPhone { get; set; }
        public int Amount { get; set; }
        public string TemplateTitle { get; set; }
        public string Content { get; set; }
        public string Status { get; set; }
        public string TrackingNumber { get; set; }
        public DateTime CreationTime { get; set; }
    }
}