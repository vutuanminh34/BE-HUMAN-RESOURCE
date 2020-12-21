using System.Threading.Tasks;

namespace WebAPI.Services.ExportFiles
{
    public interface IExportFileService
    {
        Task<string> ExportFile(int id, string type);
        Task<string> DownloadTemplateCV();
    }
}
