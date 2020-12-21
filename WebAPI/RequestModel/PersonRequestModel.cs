using System.Collections.Generic;

namespace WebAPI.RequestModel
{
    public class PersonRequestModel
    {
        public string FullName { get; set; }
        public int Location { get; set; }
        public List<int> TechnologyId { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}
