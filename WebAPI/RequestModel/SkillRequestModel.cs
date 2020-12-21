using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.RequestModel
{
    public class SkillRequestModel
    {
        public int PersonId { get; set; }
        public int CategoryId { get; set; }
        public int OrderIndex { get; set; }
        public List<int> TechnologyId { get; set; }
    }
}
