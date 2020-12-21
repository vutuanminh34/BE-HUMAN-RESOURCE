namespace WebAPI.Models.Resource.WorkHistory
{
    public class CreateWorkHistoryResource
    {
        public string Position { get; set; }
        public string CompanyName { get; set; }
        public object StartDate { get; set; }
        public object EndDate { get; set; }
        public int PersonId { get; set; }
        
    }
}
