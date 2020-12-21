namespace WebAPI.Models.Resource.Education
{
    public class SaveEducationResource
    {
        public int Id { get; set; }
        public string CollegeName { get; set; }
        public string Major { get; set; }
        public int OrderIndex { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }
}
