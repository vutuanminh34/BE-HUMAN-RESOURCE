using System.Collections.Generic;
using System.Threading.Tasks;
using WebAPI.Models.Resource.Certificate;
using WebAPI.RequestModel;
using WebAPI.ViewModels;

namespace WebAPI.Services.Certificates
{
    public interface ICertificateService
    {
        Task<IEnumerable<SaveCertificateResource>> GetCertificateByPersonId(int personId);

        Task<CertificateViewModel<SaveCertificateResource>> CreateCertificate(CreateCertificateResource certificateObj);

        Task<CertificateViewModel<SaveCertificateResource>> UpdateOrderIndexCertificate(SwapOrderIndexRequestModel swapId);

        Task<CertificateViewModel<SaveCertificateResource>> UpdateInfomationCertificate(int certificateId, UpdateCertificateResource certificateObj);

        Task<CertificateViewModel<SaveCertificateResource>> DeleteCertificate(int certificateId);
    }
}
