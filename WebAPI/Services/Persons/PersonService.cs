using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using WebAPI.Common;
using WebAPI.Constants;
using WebAPI.Models;
using WebAPI.Models.Resource.Person;
using WebAPI.Repositories.Persons;
using WebAPI.ViewModels;

namespace WebAPI.Services.Persons
{
    public class PersonService : BaseService<PersonViewModel>, IPersonService
    {
        IPersonRepository _personRepository;
        IUploadRepository _uploadRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly AppSettings _appSettings;
        public PersonService(IPersonRepository personRepository, IWebHostEnvironment webHostEnvironment, IUploadRepository uploadRepository, IOptions<AppSettings> appSettings)
        {
            _webHostEnvironment = webHostEnvironment;
            this._personRepository = personRepository;
            this._uploadRepository = uploadRepository;
            this._appSettings = appSettings.Value;
        }

        /// <summary>
        /// GET PERSON BY ID PERSON
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Person> GetPersonById(int id)
        {
            if (await _personRepository.CheckPersonExisting(id) == 0)
            {
                return null;
            }
            model.PersonInfo.Id = id;
            //return info person by id person
            var temp = await _personRepository.FindAsync(id);
            return temp;
        }

        /// <summary>
        /// VALIDATE INFO PERSON AND AVATAR BEFORE INSERT TO DATABASE
        /// </summary>
        /// <param name="person"></param>
        /// <param name="objectFile"></param>
        /// <returns></returns>
        public async Task<PersonViewModel> InsertPerson(Person person, FileUpload objectFile)
        {
            model.PersonInfo = person;
            model.PersonInfo.CreatedBy = WebAPI.Helpers.HttpContext.CurrentUser;
            var genderValue = Convert.ToInt32(model.PersonInfo.Gender);
            var getdate = DateTime.Now.ToString("yyyyMMdd");
            int newestIdPerson = 1;
            DateTime.Now.ToString();
            if (person.FullName == "" || person.FullName == null || person.FullName.GetType() != typeof(string) || person.FullName.Length > 256)
            {
                model.AppResult.Message = Constant.NAME_ERROR;
                return model;
            }
            if (Functions.CheckLocation(Convert.ToInt32(model.PersonInfo.Location)) != true || model.PersonInfo.Location.ToString() == null)
            {
                model.AppResult.Message = Constant.LOCATION_INVALID;
                return model;
            }
            else if (Functions.IsPhoneNumber(person.Phone) == false || person.Phone == null || await _personRepository.CheckPhoneExisting(person.Phone) > 0)
            {
                model.AppResult.Message = Constant.PHONE_ERROR;
                return model;
            }
            else if (genderValue < 0 || genderValue > 2)
            {
                model.AppResult.Message = Constant.GENDER_INVALID;
                return model;
            }
            else if (Functions.HandlingEmail(person.Email) == false || await _personRepository.CheckEmailExist(person.Email) > 0)
            {
                model.AppResult.Message = Constant.EMAIL_ERROR;
                return model;
            }
            #region Validate phone 
            /* else if (Functions.IsPhoneNumber(person.Phone) == false)
             {
                 model.AppResult = new AppResult { Result = false, StatusCd = "400", Message = "Phone invalid.!", DataResult = null };
                 return model;
             }*/
            #endregion

            else if (!Functions.ValidateYearOfBirth(person.YearOfBirth))
            {
                model.AppResult.Message = Constant.YEAROFBIRTH_INVALID;
                return model;
            }
            else
            {
                if (await _personRepository.AmountOfPerson() > 0)
                {
                    newestIdPerson = await _personRepository.GetMaxIdPerson() + 1;
                    person.StaffId = string.Format($"{getdate + newestIdPerson}");
                }
                else
                {
                    person.StaffId = string.Format($"{getdate + newestIdPerson}");
                }
                person.Avatar = "";
                model.PersonInfo.Status = true;
                int id = await _personRepository.InsertAsync(model.PersonInfo);
                model.PersonInfo.Id = newestIdPerson;
                //Insert image 
                //get path \\Avatar\\ from appseting.json

                var ImagePath = _appSettings.ImagePath.ToString();

                //return domain
                var host = WebAPI.Helpers.HttpContext.Current.Request.Host.Value;



                string path = _webHostEnvironment.WebRootPath + ImagePath;
                DateTime.Now.ToString();
                if (objectFile.files == null)
                {
                    string imageName = "avatar-default.png";
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    await _uploadRepository.UploadImage(imageName, path);
                    model.PersonInfo.Avatar = imageName;
                    model.AppResult.Message = Constant.CREATE_PERSON_SUCCESS;
                    model.AppResult.DataResult = model.PersonInfo;
                    return model;
                }
                if (Functions.HasImageExtension(Path.GetExtension(objectFile.files.FileName.ToLower())) == false)
                {
                    model.AppResult.Message = Constant.IMAGE_INVALID;
                    return model;
                }
                else
                {
                    if (objectFile.files.Length > 0)
                    {
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        string fileName = person.StaffId + Path.GetExtension(objectFile.files.FileName.ToLower());
                        using (FileStream fileStream = System.IO.File.Create(path + fileName))
                        {
                            objectFile.files.CopyTo(fileStream);
                            fileStream.Flush();
                            await _uploadRepository.UploadImage(fileName, path);
                            model.PersonInfo.Avatar = fileName;
                            //return http request and infor person created

                            model.AppResult.Message = Constant.CREATE_PERSON_SUCCESS;
                            model.AppResult.DataResult = model.PersonInfo;
                            return model;
                        }
                    }
                    else
                    {
                        //return http error request 
                        model.AppResult.Message = "Create person Failed!";
                        model.AppResult.DataResult = model.PersonInfo;
                        return model;
                    }
                }
            }
        }
        /// <summary>
        /// Update person
        /// </summary>
        /// <param name="person"></param>
        /// <param name="objectFile"></param>
        /// <returns></returns>
        public async Task<PersonViewModel> UpdatePerson(string file, Person person, FileUpload objectFile)
        {
            var personModel = await _personRepository.FindAsync(person.Id);
            if (personModel==null)
            {
                model.AppResult.Message = Constant.PERSONID_ERROR;
                return model;
            }
            //check existing person by id person
            var countPerson = _personRepository.CheckPersonExisting(person.Id);
            if (countPerson.Result == 0)
            {
                model.AppResult.Message = Constant.PERSONID_ERROR;
                return model;
            }
            model.PersonInfo = person;
            model.PersonInfo.UpdatedBy = WebAPI.Helpers.HttpContext.CurrentUser;
            var genderValue = Convert.ToInt32(model.PersonInfo.Gender);
            if (person.FullName == "" || person.FullName == null || person.FullName.GetType() != typeof(string) || person.FullName.Length > 256)
            {
                model.AppResult.Message = Constant.NAME_ERROR;
                return model;
            }
            else if (Functions.CheckLocation(Convert.ToInt32(model.PersonInfo.Location)) != true)
            {
                model.AppResult.Message = Constant.LOCATION_INVALID;
                return model;
            }

            else if (Functions.IsPhoneNumber(person.Phone) == false || person.Phone == null || await _personRepository.CheckPhoneToUpdate(person.Id, person.Phone) > 0)
            {
                model.AppResult.Message = Constant.PHONE_ERROR;
                return model;
            }
            else if (genderValue < 0 || genderValue > 2)
            {
                model.AppResult.Message = Constant.GENDER_INVALID;
                return model;
            }

            #region Validate phone
            /* else if (Functions.IsPhoneNumber(person.Phone) == false)
             {
                 model.AppResult = new AppResult { Result = false, StatusCd = "400", Message = "Phone invalid.!", DataResult = null };
                 return model;
             }*/
            #endregion

            else if (Functions.HandlingEmail(person.Email) != true || await _personRepository.CheckEmailUpdate(person.Id, person.Email) > 0)
            {
                model.AppResult.Message = Constant.EMAIL_ERROR;
                return model;
            }
            else if (!Functions.ValidateYearOfBirth(person.YearOfBirth))
            {
                model.AppResult.Message = Constant.YEAROFBIRTH_INVALID;
                return model;
            }
            else
            {
                //get \\Avatar\\ from appseting.json
                var ImagePath = _appSettings.ImagePath.ToString();
                //return domains
                var host = WebAPI.Helpers.HttpContext.Current.Request.Host.Value;

                string path = _webHostEnvironment.WebRootPath + ImagePath;
                //Reset avatar
                if (file == "reset" && objectFile.files == null)
                {
                    string imageName = "avatar-default.png";
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    await _uploadRepository.UpdateImage(imageName, person.Id);
                    await _personRepository.UpdateAsync(model.PersonInfo);
                    model.PersonInfo.Description = await _personRepository.GetDescription(person.Id);
                    model.PersonInfo.Avatar = imageName;
                    model.AppResult.Message = Constant.UPDATE_PERSON_SUCCESS;
                    model.AppResult.DataResult = model.PersonInfo;
                    return model;
                }
                //Update person not choose avatar
                if (objectFile.files == null)
                {
                    string imageName = personModel.Avatar;
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    await _uploadRepository.UpdateImage(imageName, person.Id);
                    await _personRepository.UpdateAsync(model.PersonInfo);
                    model.PersonInfo.Description = await _personRepository.GetDescription(person.Id);
                    model.PersonInfo.Avatar = imageName;
                    model.AppResult.Message = Constant.UPDATE_PERSON_SUCCESS;
                    model.AppResult.DataResult = model.PersonInfo;
                    return model;
                }

                //Update person and choose avatar
                if (Functions.HasImageExtension(Path.GetExtension(objectFile.files.FileName.ToLower())) == false)
                {
                    model.AppResult.Message = Constant.IMAGE_INVALID;
                    return model;
                }
                else
                {
                    if (objectFile.files.Length > 0)
                    {
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        var imageName = await _personRepository.GetStaffIdPerson(person.Id);
                        string fileName = imageName + Path.GetExtension(objectFile.files.FileName.ToLower());
                        using (FileStream fileStream = System.IO.File.Create(path + fileName))
                        {
                            objectFile.files.CopyTo(fileStream);
                            fileStream.Flush();
                            await _uploadRepository.UpdateImage(fileName, person.Id);
                            //return http request and infor person created
                            await _personRepository.UpdateAsync(model.PersonInfo);
                            model.PersonInfo.Description = await _personRepository.GetDescription(person.Id);
                            model.PersonInfo.Avatar = fileName;
                            model.AppResult.Message = Constant.UPDATE_PERSON_SUCCESS;
                            model.AppResult.DataResult = model.PersonInfo;
                            return model;
                        }
                    }
                    else
                    {
                        //return http error request 
                        model.AppResult.Message = "Create person Failed!";
                        model.AppResult.DataResult = model.PersonInfo;
                        return model;
                    }
                }
            }
        }

        /// <summary>
        /// Update overview
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public async Task<PersonViewModel> UpdateOverview(SavePersonResource obj)
        {
            model.PersonInfo = await _personRepository.FindAsync(obj.Id);
            if (model.PersonInfo == null)
            {
                model.AppResult.Message = Constant.PERSONID_ERROR;
                return model;
            }
            // Set infomation of PersonInfo
            model.PersonInfo.Id = obj.Id;
            model.PersonInfo.Description = obj.Description;
            model.PersonInfo.UpdatedBy = WebAPI.Helpers.HttpContext.CurrentUser;
            var temp = await _personRepository.UpdateOverview(model.PersonInfo);
            model.AppResult.Message = Constant.UPDATE_SUCCESS;
            model.AppResult.DataResult = model.PersonInfo;
            return model;
        }

        /// <summary>
        /// VALIDATE ID PERSON BEFORE DELETE PERSON BY ID THIS PERSON
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<PersonViewModel> DeletePerson(int id)
        {
            //check existing person by id person
            var countPerson = _personRepository.CheckPersonExisting(id);
            if (countPerson.Result == 0)
            {
                model.AppResult.Message = Constant.PERSONID_ERROR;
                return model;
            }
            else
            {
                int resultDelete = await _personRepository.DeleteAsync(id);
                model.AppResult.Message = Constant.DELETE_SUCCESS;
                return model;
            }
        }

        /// <summary>
        /// Pagination and search
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="FullName"></param>
        /// <param name="Location"></param>
        /// <param name="TechnologyId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Person>> SearchPersonAndSkillAsync(int? page, int? limit, string FullName, int? Location, List<int> TechnologyId)
        {
            //computing offset before go to database get data
            int Offset = 0;
            if (page != null || limit != null)
            {
                Offset = ((int)page - 1) * (int)limit;
            }
            if (Location == null)
            {
                Location = -1;
            }
            return await _personRepository.SearchPersonAndSkillAsync((int)limit, Offset, FullName, (int)Location, TechnologyId);
        }

        /// <summary>
        /// VALIDATE INFOR PERSON BEFORE INSERT INTO DATABASE 
        /// </summary>
        /// <param name="person"></param>
        /// <param name="image"></param>
        /// <returns></returns>
        public async Task<PersonViewModel> InsertPersonToImportFile(Person person, Image image)
        {
            model.AppResult.Result = false;
            model.AppResult.DataResult = null;
            model.PersonInfo = person;
            model.PersonInfo.CreatedBy = WebAPI.Helpers.HttpContext.CurrentUser;
            model.PersonInfo.UpdatedBy = WebAPI.Helpers.HttpContext.CurrentUser;
            model.PersonInfo.UpdatedAt = DateTime.Now;
            model.PersonInfo.CreatedAt = DateTime.Now;
            if (await _personRepository.CheckPhoneExisting(person.Phone) > 0 || await _personRepository.CheckEmailExist(person.Email) > 0)
            {
                model.PersonInfo.Id = 0;
                model.AppResult.Message = "CV already exists!";
                return model;
            }
            var genderValue = Convert.ToInt32(model.PersonInfo.Gender);
            var getdate = DateTime.Now.ToString("yyyyMMdd");
            int newestIdPerson = 1;
            if (person.FullName == "" || person.FullName == null || person.FullName.GetType() != typeof(string))
            {
                model.AppResult.Message = Constant.NAME_ERROR;
                return model;
            }
            if (Functions.CheckLocation(Convert.ToInt32(model.PersonInfo.Location)) != true || model.PersonInfo.Location.ToString() == null)
            {
                model.AppResult.Message = Constant.LOCATION_INVALID;
                return model;
            }
            else if (Functions.IsPhoneNumber(person.Phone) == false || person.Phone == null || await _personRepository.CheckPhoneExisting(person.Phone) > 0)
            {
                model.AppResult.Message = Constant.PHONE_ERROR;
                return model;
            }
            else if (genderValue < 0 || genderValue > 2)
            {
                model.AppResult.Message = Constant.GENDER_INVALID;
                return model;
            }
            else if (Functions.HandlingEmail(person.Email) == false || await _personRepository.CheckEmailExist(person.Email) > 0)
            {
                model.AppResult.Message = Constant.EMAIL_ERROR;
                return model;
            }
            else if (!Functions.ValidateYearOfBirth(person.YearOfBirth))
            {
                model.AppResult.Message = Constant.YEAROFBIRTH_INVALID;
                return model;
            }
            else
            {
                if (await _personRepository.AmountOfPerson() > 0)
                {
                    newestIdPerson = await _personRepository.GetMaxIdPerson() + 1;
                    person.StaffId = string.Format($"{getdate + newestIdPerson}");
                }
                else
                {
                    person.StaffId = string.Format($"{getdate + newestIdPerson}");
                }
                person.Avatar = "";
                model.PersonInfo.Status = true;
                int id = await _personRepository.InsertAsync(model.PersonInfo);
                model.PersonInfo.Id = id;
                var ImagePath = _appSettings.ImagePath.ToString();
                string path = _webHostEnvironment.WebRootPath + ImagePath;
                if (image == null)
                {
                    string imageName = "avatar-default.png";
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    await _uploadRepository.UpdateImage(imageName, id);
                }
                else
                {
                    var imageName = await _personRepository.GetStaffIdPerson(person.Id) + Path.GetExtension(image.Tag.ToString());
                    string fileName = imageName;
                    if (Functions.HasImageExtension(Path.GetExtension(image.Tag.ToString())) == true)
                    {
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        image.Save(path + fileName);
                        await _uploadRepository.UpdateImage(imageName, id);
                    }
                }
                model.AppResult.Result = true;
                model.AppResult.Message = "Import CV Successful!";
                model.AppResult.DataResult = id;
            }
            return model;
        }


        /// <summary>
        /// VALIDATE INFOR PERSON BEFORE UPDATE INTO DATABASE 
        /// </summary>
        /// <param name="person"></param>
        /// <returns></returns>
        public async Task<PersonViewModel> UpdatePersonToImportFile(Person person, Image image)
        {
            model.AppResult.Result = false;
            model.AppResult.DataResult = null;
            var modelPerson = await _personRepository.FindAsync(person.Id);
            if (modelPerson == null)
            {
                model.PersonInfo.Id = 0;
                model.AppResult.Message = "CV not exists!";
                return model;
            }
            else
            {
                if (await _personRepository.CheckPhoneToUpdate(person.Id, person.Phone) > 0 || await _personRepository.CheckEmailUpdate(person.Id, person.Email) > 0)
                {
                    model.PersonInfo.Id = 0;
                    model.AppResult.Message = "CV already exists!";
                    return model;
                }
                modelPerson.UpdatedAt = DateTime.Now;
                modelPerson.UpdatedBy = WebAPI.Helpers.HttpContext.CurrentUser;
                modelPerson.FullName = person.FullName;
                modelPerson.Description = person.Description;
                modelPerson.Gender = person.Gender;
                modelPerson.YearOfBirth = person.YearOfBirth;
                modelPerson.Location = person.Location;
                modelPerson.Email = person.Email;
                modelPerson.Phone = person.Phone;
                var genderValue = Convert.ToInt32(model.PersonInfo.Gender);
                if (person.FullName == "" || person.FullName == null || person.FullName.GetType() != typeof(string))
                {
                    model.AppResult.Message = Constant.NAME_ERROR;
                    return model;
                }
                else if (Functions.CheckLocation(Convert.ToInt32(model.PersonInfo.Location)) != true)
                {
                    model.AppResult.Message = Constant.LOCATION_INVALID;
                    return model;
                }
                else if (genderValue < 0 || genderValue > 2)
                {
                    model.AppResult.Message = Constant.GENDER_INVALID;
                    return model;
                }
                else if (!Functions.ValidateYearOfBirth(person.YearOfBirth))
                {
                    model.AppResult.Message = Constant.YEAROFBIRTH_INVALID;
                    return model;
                }
                else
                {
                    var ImagePath = _appSettings.ImagePath.ToString();
                    var host = WebAPI.Helpers.HttpContext.Current.Request.Host.Value;
                    string path = _webHostEnvironment.WebRootPath + ImagePath;
                    if (image != null)
                    {
                        var imageName = await _personRepository.GetStaffIdPerson(person.Id) + Path.GetExtension(image.Tag.ToString());
                        string fileName = imageName;
                        if (Functions.HasImageExtension(Path.GetExtension(image.Tag.ToString())) == true)
                        {
                            if (!Directory.Exists(path))
                            {
                                Directory.CreateDirectory(path);
                            }
                            image.Save(path + fileName);
                            await _uploadRepository.UpdateImage(imageName, modelPerson.Id);
                        }
                    }
                    var result = await _personRepository.UpdateAsync(modelPerson);
                    result += await _personRepository.UpdateOverview(modelPerson);
                    model.AppResult.Result = true;
                    model.AppResult.Message = "Import CV Successful!";
                    model.AppResult.DataResult = modelPerson.Id;
                    if (result > 1)
                        model.PersonInfo.Id = modelPerson.Id;
                    else
                        model.PersonInfo.Id = 0;
                    return model;
                }
            }

        }


    }
}


