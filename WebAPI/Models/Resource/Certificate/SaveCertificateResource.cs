namespace WebAPI.Models.Resource.Certificate
{
    public class SaveCertificateResource
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Provider { get; set; }
        public int OrderIndex { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }
}
