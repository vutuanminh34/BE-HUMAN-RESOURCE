namespace WebAPI.Models.Resource.WorkHistory
{
    public class SaveWorkHistoryResource
    {
        public int Id { get; set; }
        public string Position { get; set; }
        public string CompanyName { get; set; }
        public int OrderIndex { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public int PersonId { get; set; }
    }
}
