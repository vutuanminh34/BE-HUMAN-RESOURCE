namespace WebAPI.Models.Resource.WorkHistory
{
    public class UpdateWorkHistoryResource
    {
        public string Position { get; set; }
        public string CompanyName { get; set; }
        public object StartDate { get; set; }
        public object EndDate { get; set; }
    }
}
