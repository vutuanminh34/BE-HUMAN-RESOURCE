namespace WebAPI.Models.Resource.Education
{
    public class CreateEducationResource
    {
        public string CollegeName { get; set; }
        public string Major { get; set; }
        public object StartDate { get; set; }
        public object EndDate { get; set; }
        public int PersonId { get; set; }
    }
}
