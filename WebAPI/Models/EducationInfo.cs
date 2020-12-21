using System;

namespace WebAPI.Models
{
    public class EducationInfo
    {
        public int Id { get; set; }
        public string CollegeName { get; set; }
        public string Major { get; set; }
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
