
using System.Threading.Tasks;
using WebAPI.Models;

namespace WebAPI.Services.ImportFiles
{
    public interface IImportService
    {
        Task<AppResult> ImportFile(FileUpload file, int? id);
    }
}
