namespace WebAPI.Models.Resource.Certificate
{
    public class CreateCertificateResource
    {
        
        public string Name { get; set; }
        public string Provider { get; set; }
        public object StartDate { get; set; }
        public object EndDate { get; set; }
        public int PersonId { get; set; }
    }
}
