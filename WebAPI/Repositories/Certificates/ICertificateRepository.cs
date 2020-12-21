using System.Collections.Generic;
using System.Threading.Tasks;
using WebAPI.Models;
using WebAPI.Repositories.Interfaces;

namespace WebAPI.Repositories.Certificates
{
    public interface ICertificateRepository : IRepositoryBase<CertificateInfo>
    {
        Task<IEnumerable<CertificateInfo>> GetCertificateByPersonIdAsync(int personId, bool flag = true);

        Task<int> MaximumOrderIndexAsync(int personId, bool flag = true);

        Task<int> ValidatePersonIdAsync(int personId, bool flag = true);

        Task<bool> SwapOrderIndexAsync(CertificateInfo currentObj, CertificateInfo turnedObj);
    }
}
