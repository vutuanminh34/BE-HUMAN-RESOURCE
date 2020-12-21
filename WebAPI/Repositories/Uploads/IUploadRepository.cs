using System.Threading.Tasks;
using WebAPI.Models;
using WebAPI.Repositories.Interfaces;

namespace WebAPI.Repositories.Persons
{
    public interface IUploadRepository : IRepositoryBase<FileUpload>
    {
        public Task<int> UploadImage(string fileName, string path);
        public Task<int> UpdateImage(string fileName, int id);
    }
}
