﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models
{
    public class PersonTechnology
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// PersonId
        /// </summary>
        public int PersonId { get; set; }

        /// <summary>
        /// TechnologyId
        /// </summary>
        public int TechnologyId { get; set; }

        /// <summary>
        /// Status
        /// </summary>
        public bool Status { get; set; }

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
        /// UpdatedBy
        /// </summary>
        public string UpdatedBy { get; set; }

    }
}
