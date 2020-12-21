using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models
{
    public class ProjectTechnology
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public bool Status { get; set; }
        public int ProjectId { get; set; }
        public int TechnologyId { get; set; }

        public virtual Project Project { get; set; }
        public virtual Technology Technology { get; set; }
    }
}
