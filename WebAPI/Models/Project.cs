using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models
{
    public class Project
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// CreatedAt
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// CreatedBy
        /// </summary>
        public string CreatedBy { get; set; }

        /// <summary>
        /// UpdatedAt
        /// </summary>
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// UpdateBy
        /// </summary>
        public string UpdatedBy { get; set; }

        /// <summary>
        /// Status
        /// </summary>
        public Boolean Status { get; set; }

        /// <summary>
        /// StartDate
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// EndDate
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// position
        /// </summary>
        public string Position { get; set; }
        /// <summary>
        /// Responsibilities
        /// </summary>
        public string Responsibilities { get; set; }

        /// <summary>
        /// TeamSize
        /// </summary>
        public int TeamSize { get; set; }

        /// <summary>
        /// OrderIndex
        /// </summary>
        public int OrderIndex { get; set; }
        
        /// <summary>
        /// PersonId
        /// </summary>
        public int PersonId { get; set; }

        public IList<Technology> Technologies { get; set; }
    }
}
