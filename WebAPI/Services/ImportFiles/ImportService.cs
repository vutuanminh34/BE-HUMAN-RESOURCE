using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using WebAPI.HandlingFiles;
using WebAPI.Models;
using WebAPI.Models.Resource.Certificate;
using WebAPI.Models.Resource.Education;
using WebAPI.Models.Resource.Skill;
using WebAPI.Models.Resource.WorkHistory;
using WebAPI.RequestModel;
using WebAPI.Services.Categories;
using WebAPI.Services.Certificates;
using WebAPI.Services.Educations;
using WebAPI.Services.Persons;
using WebAPI.Services.Projects;
using WebAPI.Services.ProjectTechnologies;
using WebAPI.Services.Skills;
using WebAPI.Services.Technologies;
using WebAPI.Services.WorkHistories;

namespace WebAPI.Services.ImportFiles
{
    public class ImportService : IImportService
    {
        private readonly IPersonService _personService;
        private readonly ICategoryService _categoryService;
        private readonly ITechnologyService _technologyService;
        private readonly ISkillService _skillService;
        private readonly IWorkHistoryService _workHistoryService;
        private readonly IEducationService _educationService;
        private readonly ICertificateService _certificateService;
        private readonly IProjectService _projectService;
        private readonly IProjectTechnologyService _projectTechnologyService;
        public ImportService(IPersonService personService,
            IWorkHistoryService workHistoryService, IEducationService educationService,
            ICategoryService categoryService, ITechnologyService technologyService,
            ISkillService skillService, IProjectService projectService, IProjectTechnologyService projectTechnologyService,
            ICertificateService certificateService)
        {
            this._personService = personService;
            this._workHistoryService = workHistoryService;
            this._educationService = educationService;
            this._categoryService = categoryService;
            this._technologyService = technologyService;
            this._skillService = skillService;
            this._projectService = projectService;
            this._projectTechnologyService = projectTechnologyService;
            this._certificateService = certificateService;
        }
        public async Task<AppResult> ImportFile(FileUpload file, int? id)
        {
            AppResult appResult = new AppResult();
            var fileName = file.files.FileName;
            if (!ImportFileHandling.HasFileExtension(Path.GetExtension(fileName)))
            {
                appResult.Result = false;
                appResult.Message = "CV Import Failed, Invalid file format!";
                return appResult;
            }
            var list = await ImportFileHandling.GetInformationCV(file.files);
            if (list.Count > 0)
            {
                var resultCheckFile = ImportFileHandling.CheckFile(list);
                if (resultCheckFile.Result)
                {
                    int idPerson = 0;
                    List<SkillRequestModel> listSkill = new List<SkillRequestModel>();
                    List<int> listIdWorkHistory = new List<int>();
                    List<SaveWorkHistoryResource> saveWorkHistoryResources = new List<SaveWorkHistoryResource>();
                    List<int> listIdEducation = new List<int>();
                    List<SaveEducationResource> saveEducationResources = new List<SaveEducationResource>();
                    List<int> listIdCertificate = new List<int>();
                    List<SaveCertificateResource> saveCertificateResources = new List<SaveCertificateResource>();
                    List<int> listIdSkill = new List<int>();
                    List<SkillResource> skillRequestModels = new List<SkillResource>();
                    List<int> listIdProject = new List<int>();
                    List<Project> listProject = new List<Project>();
                    try
                    {
                        using (TransactionScope txScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                        {
                            Person person = ImportFileHandling.GetPerson(list);
                            Image image = await ImportFileHandling.GetImageCV(file.files);
                            if (id == null)
                            {
                                var resultPerson = await _personService.InsertPersonToImportFile(person, image);
                                appResult = resultPerson.AppResult;
                                idPerson = resultPerson.PersonInfo.Id;
                            }
                            else
                            {
                                idPerson = (int)id;
                                if (idPerson > 0)
                                {
                                    var tempPerson = await _personService.GetPersonById(idPerson);
                                    var modelPerson = (Person)tempPerson;
                                    if (modelPerson != null)
                                    {
                                        person.Id = (int)id;
                                        var resultPerson = await _personService.UpdatePersonToImportFile(person, image);
                                        appResult = resultPerson.AppResult;
                                        if (appResult.Result)
                                        {
                                            var listWorkHistoryResourceByPersonId = await _workHistoryService.GetWorkHistoryByPersonId(idPerson);
                                            saveWorkHistoryResources = listWorkHistoryResourceByPersonId.ToList();
                                            if (saveWorkHistoryResources != null)
                                            {
                                                foreach (var item in saveWorkHistoryResources)
                                                    listIdWorkHistory.Add(item.Id);
                                                if (listIdWorkHistory != null)
                                                {
                                                    foreach (var item in listIdWorkHistory)
                                                        await _workHistoryService.DeleteWorkHistory(item);
                                                }
                                            }
                                            var listEducationByPersonId = await _educationService.GetEducationByPersonId(idPerson);
                                            saveEducationResources = listEducationByPersonId.ToList();
                                            if (saveEducationResources != null)
                                            {
                                                foreach (var item in saveEducationResources)
                                                    listIdEducation.Add(item.Id);
                                                if (listIdEducation != null)
                                                {
                                                    foreach (var item in listIdEducation)
                                                        await _educationService.DeleteEducation(item);
                                                }
                                            }
                                            var listCertificateByPersonId = await _certificateService.GetCertificateByPersonId(idPerson);
                                            saveCertificateResources = listCertificateByPersonId.ToList();
                                            if (saveCertificateResources != null)
                                            {
                                                foreach (var item in saveCertificateResources)
                                                    listIdCertificate.Add(item.Id);
                                                if (listIdCertificate != null)
                                                {
                                                    foreach (var item in listIdCertificate)
                                                        await _certificateService.DeleteCertificate(item);
                                                }
                                            }
                                            var listSkillByPeronId = await _skillService.GetSkillByPerson(idPerson);
                                            skillRequestModels = listSkillByPeronId.ToList();
                                            if (skillRequestModels != null)
                                            {
                                                foreach (var item in skillRequestModels)
                                                    listIdSkill.Add(item.PersonCategoryId);
                                                if (listIdSkill != null)
                                                {
                                                    foreach (var item in listIdSkill)
                                                        await _skillService.DeleteSkill(item);
                                                }
                                            }
                                            var listProjectByPersonId = await _projectService.GetProjectByPersonId(idPerson);
                                            listProject = listProjectByPersonId.ToList();
                                            if (listProject != null)
                                            {
                                                foreach (var item in listProject)
                                                    listIdProject.Add(item.Id);
                                                if (listIdProject != null)
                                                {
                                                    foreach (var item in listIdProject)
                                                        await _projectService.DeleteProject(item);
                                                }
                                            }
                                        }
                                        idPerson = resultPerson.PersonInfo.Id;
                                    }
                                    else
                                    {
                                        appResult.Result = false;
                                        appResult.Message = "CV Import Failed, CV not exist";
                                        return appResult;
                                    }

                                }
                            }
                            if (idPerson > 0)
                            {
                                List<CreateWorkHistoryResource> workHistoryResources = ImportFileHandling.GetListWorkHistory(list, idPerson);
                                if (workHistoryResources != null)
                                {
                                    foreach (var item in workHistoryResources)
                                    {
                                        if (item != null)
                                            await _workHistoryService.CreateWorkHistory(item);
                                    }
                                }
                                List<CreateEducationResource> createEducationResources = ImportFileHandling.GetListEducation(list, idPerson);
                                if (createEducationResources != null)
                                {
                                    foreach (var item in createEducationResources)
                                    {
                                        if (item != null)
                                            await _educationService.CreateEducation(item);
                                    }

                                }
                                List<CreateCertificateResource> createCertificateResources = ImportFileHandling.GetListCertifiate(list, idPerson);
                                if (createCertificateResources != null)
                                {
                                    foreach (var item in createCertificateResources)
                                    {
                                        if (item != null)
                                            await _certificateService.CreateCertificate(item);
                                    }
                                }
                                List<Category> categories = ImportFileHandling.GetListCategory(list);
                                List<int> Category = new List<int>();
                                if (categories != null)
                                {
                                    listSkill = new List<SkillRequestModel>();
                                    foreach (var groupItem in categories)
                                    {
                                        var resultCategory = await _categoryService.InsertCategory(groupItem);
                                        int idCategory = resultCategory.Category.Id;
                                        Category.Add(idCategory);
                                        List<int> listTechnology = new List<int>();
                                        foreach (var item in groupItem.Technologies)
                                        {
                                            item.CategoryId = idCategory;
                                            var resultTechnology = await _technologyService.InsertTechnology(item);
                                            int idTechnology = resultTechnology.Technology.Id;
                                            listTechnology.Add(idTechnology);
                                        }
                                        SkillRequestModel skill = new SkillRequestModel
                                        {
                                            PersonId = idPerson,
                                            CategoryId = idCategory,
                                            TechnologyId = listTechnology
                                        };
                                        listSkill.Add(skill);
                                    }
                                    if (listSkill.Count > 0)
                                    {
                                        foreach (var item in listSkill)
                                            await _skillService.InserSkill(item);
                                    }
                                }
                                List<Project> projects = ImportFileHandling.GetListProject(list, idPerson);
                                if (projects != null)
                                {
                                    foreach (var gorupItem in projects)
                                    {
                                        List<ProjectTechnology> projectTechnologies = new List<ProjectTechnology>();
                                        var resultProject = await _projectService.InsertProject(gorupItem);
                                        int idProject = resultProject.Project.Id;
                                        foreach (var item in gorupItem.Technologies)
                                        {
                                            ProjectTechnology projectTechnology = new ProjectTechnology
                                            {
                                                ProjectId = idProject,
                                                TechnologyId = await _technologyService.GetTechnologyByPersonAndNameAsync(idPerson, item.Name)
                                            };
                                            projectTechnologies.Add(projectTechnology);
                                        }
                                        await _projectTechnologyService.InsertListTechnologyAsync(projectTechnologies);
                                    }
                                }
                            }
                            txScope.Complete();
                        }
                    }
                    catch
                    {
                        appResult.Result = false;
                        appResult.Message = "CV Import Failed, file is invalid!";
                    }
                }
                else
                {
                    appResult.Result = false;
                    appResult.Message = resultCheckFile.Message;
                }

            }
            else
            {
                appResult.Result = false;
                appResult.Message = "CV Import Failed, file is invalid!";
            }
            return appResult;
        }
    }
}
