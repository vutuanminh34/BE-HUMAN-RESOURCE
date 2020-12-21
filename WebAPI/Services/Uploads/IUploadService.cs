using System.Collections;
using System.Threading.Tasks;
using WebAPI.Models;
using WebAPI.ViewModels;

namespace WebAPI.Services.Uploads
{
    public interface IUploadService
    {
        /// <summary>
        /// Interface upload image
        /// </summary>
        /// <param name="path"></param>
        /// <param name="objectFile"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<IEnumerable> UploadImage(string path, FileUpload objectFile, int id);
    }
}
