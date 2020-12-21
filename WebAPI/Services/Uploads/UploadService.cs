using Microsoft.Extensions.Options;
using System;
using System.Collections;
using System.IO;
using System.Threading.Tasks;
using WebAPI.Common;
using WebAPI.Models;
using WebAPI.Repositories.Persons;

namespace WebAPI.Services.Uploads
{
    public class UploadService : IUploadService
    {
        IPersonRepository _personRepository;
        IUploadRepository _uploadRepository;
        public UploadService(IPersonRepository personRepository, IUploadRepository uploadRepository)
        {
            this._personRepository = personRepository;
            this._uploadRepository = uploadRepository;
        }
        /// <summary>
        /// Check condition for upload image
        /// </summary>
        /// <param name="path"></param>
        /// <param name="objectFile"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IEnumerable> UploadImage(string path, FileUpload objectFile, int id)
        {
            if (await _personRepository.CheckPersonExisting(id) <= 0)
            {
                return null;
            }
            else
            {
                string StaffId = string.Empty;
                var getdate = DateTime.Now.ToString("yyyyMMdd");
                DateTime.Now.ToString();
                try
                {
                    if (Functions.HasImageExtension(Path.GetExtension(objectFile.files.FileName)) == false)
                    {
                        return null;
                    }
                    else
                    {
                        if (objectFile.files.Length > 0)
                        {
                            if (!Directory.Exists(path))
                            {
                                Directory.CreateDirectory(path);
                            }
                            StaffId = string.Format($"{getdate + id}");
                            string fileName = StaffId + Path.GetExtension(objectFile.files.FileName);
                            using (FileStream fileStream = System.IO.File.Create(path + fileName))
                            {
                                objectFile.files.CopyTo(fileStream);
                                fileStream.Flush();
                                await _uploadRepository.UploadImage(fileName, path);
                                return string.Concat("~\\Avatar\\", fileName);
                            }
                        }
                        else
                        {
                            return "Not any image Uploaded!";
                        }
                    }
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }
    }
}