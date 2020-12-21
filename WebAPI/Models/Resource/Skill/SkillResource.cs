using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Models.Resource.Technology;

namespace WebAPI.Models.Resource.Skill
{
    public class SkillResource
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<TechnologyResource> Technologies { get; set; }
        public int PersonCategoryId { get; set; }
        public int OrderIndex { get; set; }

    }
}
