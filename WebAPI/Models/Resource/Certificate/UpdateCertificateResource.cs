using System;

namespace WebAPI.Models.Resource.Certificate
{
    public class UpdateCertificateResource
    {
        public string Name { get; set; }
        public string Provider { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }
}
