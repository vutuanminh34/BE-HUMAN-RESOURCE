using System;

namespace WebAPI.Models
{
    public class CertificateInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Provider { get; set; }
        public int OrderIndex { get; set; }
        public bool Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public int PersonId { get; set; }
    }
}
