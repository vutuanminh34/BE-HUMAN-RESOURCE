using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Models;
using WebAPI.Models.Resource.Skill;
using WebAPI.Models.Resource.Technology;
using WebAPI.Repositories.Categories;
using WebAPI.Repositories.Skills;
using WebAPI.Repositories.Technologies;
using WebAPI.RequestModel;
using WebAPI.ViewModels;

namespace WebAPI.Services.Skills
{
    public class SkillService : BaseService<SkillViewModel>, ISkillService
    {
        ISkillRepository _skillRepository;
        ICategoryRepository _categoryRepository;
        ITechnologyRepository _technologyRepository;
        public SkillService(ISkillRepository skillRepository, ICategoryRepository categoryRepository, ITechnologyRepository technologyRepository)
        {
            this._skillRepository = skillRepository;
            this._categoryRepository = categoryRepository;
            this._technologyRepository = technologyRepository;
        }

        /// <summary>
        /// Get List Skill Resource
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public List<SkillResource> GetListSkillResource(IEnumerable<Category> list)
        {
            List<SkillResource> listResult = new List<SkillResource>();
            if (list.Any())
            {
                foreach (var item in list)
                {
                    SkillResource skillResource = new SkillResource
                    {
                        Id = item.Id,
                        Name = item.Name,
                        PersonCategoryId = item.PersonCategories.FirstOrDefault().Id,
                        OrderIndex = item.PersonCategories.FirstOrDefault().OrderIndex,
                        Technologies = new List<TechnologyResource>()
                    };
                    foreach (var itemTechnology in item.Technologies)
                    {
                        TechnologyResource technology = new TechnologyResource
                        {
                            Id = itemTechnology.Id,
                            Name = itemTechnology.Name,
                            CategoryId = itemTechnology.CategoryId,
                            CategoryName = item.Name
                        };
                        skillResource.Technologies.Add(technology);
                    }
                    listResult.Add(skillResource);
                }
            }
            return listResult;
        }

        /// <summary>
        /// Get PersonCategory
        /// </summary>
        /// <param name="skillRequestModel"></param>
        /// <returns></returns>
        public async Task<PersonCategory> GetPersonCategory(SkillRequestModel skillRequestModel)
        {
            int numberItemPersonCategory = await _skillRepository.GetNumberItemPersonCategory();
            int orderIndex = 1;
            if (numberItemPersonCategory > 0)
                orderIndex = await _skillRepository.GetMaxOrderIndex() + 1;
            PersonCategory personCategory = new PersonCategory
            {
                PersonId = skillRequestModel.PersonId,
                CategoryId = skillRequestModel.CategoryId,
                OrderIndex = orderIndex,
                CreatedBy = WebAPI.Helpers.HttpContext.CurrentUser,
                UpdatedBy = WebAPI.Helpers.HttpContext.CurrentUser
            };
            return personCategory;
        }

        /// <summary>
        /// Get List PersonTechnology
        /// </summary>
        /// <param name="skillRequestModel"></param>
        /// <returns></returns>
        public async Task<List<PersonTechnology>> GetListPersonTechnology(SkillRequestModel skillRequestModel)
        {
            List<PersonTechnology> personTechnologies = new List<PersonTechnology>();
            foreach (var item in skillRequestModel.TechnologyId)
            {
                Technology modelTechnology = await _technologyRepository.FindAsync(item);
                if (modelTechnology != null)
                {
                    PersonTechnology personTechnology = new PersonTechnology
                    {
                        PersonId = skillRequestModel.PersonId,
                        TechnologyId = modelTechnology.Id,
                        CreatedBy = WebAPI.Helpers.HttpContext.CurrentUser,
                        UpdatedBy = WebAPI.Helpers.HttpContext.CurrentUser,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };
                    personTechnologies.Add(personTechnology);
                }

            }
            return personTechnologies;
        }

        /// <summary>
        /// Insert List PersonTechnology
        /// </summary>
        /// <param name="personTechnologies"></param>
        /// <returns></returns>
        public async Task<int> InsertListPersonTechnology(List<PersonTechnology> personTechnologies)
        {
            var result = 0;
            List<PersonTechnology> personTechnologiesUpdate = new List<PersonTechnology>();
            List<PersonTechnology> personTechnologiesInsert = new List<PersonTechnology>();
            foreach (var item in personTechnologies)
            {
                if (await _skillRepository.CheckPersonTechnologyOnDeleteAsync(item))
                {
                    item.UpdatedAt = DateTime.Now;
                    personTechnologiesUpdate.Add(item);
                }
                else
                {
                    if (!await _skillRepository.CheckPersonTechnologyAsync(item))
                        personTechnologiesInsert.Add(item);
                }
            }
            if (personTechnologiesUpdate.Count > 0)
                result += await _skillRepository.UpdatePersonTechnologyToInsertAsync(personTechnologiesUpdate);
            result += await _skillRepository.InsertListPersonTechnologyAsync(personTechnologiesInsert);
            return result;
        }

        /// <summary>
        /// Get Skill By PersonId
        /// </summary>
        /// <param name="personId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<SkillResource>> GetSkillByPerson(int personId)
        {
            IEnumerable<Category> list = await _skillRepository.GetSkillByPersonAsync(personId);
            return GetListSkillResource(list);
        }

        /// <summary>
        /// Inser Skill
        /// </summary>
        /// <param name="skillRequestModel"></param>
        /// <returns></returns>
        public async Task<SkillViewModel> InserSkill(SkillRequestModel skillRequestModel)
        {
            model.AppResult.Result = false;
            if (skillRequestModel.PersonId <= 0)
                model.AppResult.Message = "PersonId not exist!";
            else if (skillRequestModel.CategoryId <= 0)
                model.AppResult.Message = "CategoryId not exist!";
            else if (skillRequestModel.TechnologyId.Count <= 0)
                model.AppResult.Message = "TechnologyId not exist!";
            else
            {
                model.SkillRequestModel = skillRequestModel;
                PersonCategory personCategory = await GetPersonCategory(skillRequestModel);
                List<PersonTechnology> personTechnologies = await GetListPersonTechnology(skillRequestModel);
                Category modelCategory = new Category();
                int idPersonCategory = 0;
                int idPerson = 0;
                modelCategory = await _categoryRepository.FindAsync(personCategory.CategoryId);
                if (modelCategory == null)
                    model.AppResult.Message = "Create failure, Category not exists";
                else
                {
                    if (personTechnologies.Count > 0)
                    {
                        if (await _skillRepository.CheckPersonCategoryAsync(personCategory))
                        {
                            if (await _skillRepository.CheckNumberTechnology(personCategory))
                            {
                                model.AppResult.Message = "Create failure, Skill already exists";
                                return model;
                            }
                            else
                            {
                                personCategory.OrderIndex = await _skillRepository.GetMaxOrderIndex() + 1;
                                personCategory.UpdatedAt = DateTime.Now;
                                var resultNumberCategory = await _skillRepository.UpdatePersonCategoryToInsertAsync(personCategory);
                                if (resultNumberCategory > 0)
                                {
                                    idPerson = personCategory.PersonId;
                                    idPersonCategory = (await _skillRepository.GetPersonCategoryAsync(null, personCategory.PersonId, personCategory.CategoryId)).Id;
                                    var resultNumberTechnology = await InsertListPersonTechnology(personTechnologies);
                                    if (resultNumberTechnology <= 0)
                                        model.AppResult.Message = "Create failure, Technology not exists";
                                }
                                else
                                {
                                    model.AppResult.Message = "Create failure, Category not exists";
                                }
                            }
                        }
                        else
                        {
                            PersonCategory modelPersonCategory = await _skillRepository.CheckPersonCategoryOnDeleteAsync(personCategory);
                            if (modelPersonCategory == null)
                            {
                                idPersonCategory = await _skillRepository.InsertPersonCategoryAsync(personCategory);
                                idPerson = skillRequestModel.PersonId;
                            }
                            else
                            {
                                modelPersonCategory.OrderIndex = await _skillRepository.GetMaxOrderIndex() + 1;
                                modelPersonCategory.UpdatedAt = DateTime.Now;
                                var resultNumberCategor = await _skillRepository.UpdatePersonCategoryToInsertAsync(modelPersonCategory);
                                if (resultNumberCategor > 0)
                                {
                                    idPersonCategory = modelPersonCategory.Id;
                                    idPerson = modelPersonCategory.PersonId;
                                }
                                else
                                    model.AppResult.Message = "Create failure, Category not exists";
                            }
                            var resultNumberTechnology = await InsertListPersonTechnology(personTechnologies);
                            if (resultNumberTechnology <= 0)
                                model.AppResult.Message = "Create failure, Technology not exists";
                        }
                        if (idPersonCategory > 0 && idPerson > 0)
                        {
                            List<Category> listResult = new List<Category>();
                            modelCategory = await _skillRepository.GetSkillInsertAsync(idPersonCategory, idPerson);
                            if (modelCategory != null)
                            {
                                model.AppResult.Result = true;
                                model.AppResult.Message = Constants.Constant.INSERT_SUCCESS;
                                listResult.Add(modelCategory);
                                model.AppResult.DataResult = GetListSkillResource(listResult).FirstOrDefault();
                            }
                        }

                    }
                    else
                        model.AppResult.Message = "Create failure, Technology not exists";
                }
            }
            return model;
        }

        /// <summary>
        /// Update Skill
        /// </summary>
        /// <param name="id"></param>
        /// <param name="skillRequestModel"></param>
        /// <returns></returns>
        public async Task<SkillViewModel> UpdateSkill(int id, SkillRequestModel skillRequestModel)
        {
            model.AppResult.Result = false;
            model.SkillRequestModel = skillRequestModel;
            if (id <= 0)
                model.AppResult.Message = "PersonId not exist!";
            else if (skillRequestModel.TechnologyId.Count <= 0)
                model.AppResult.Message = "TechnologyId not exist!";
            else if (skillRequestModel.CategoryId <= 0)
                model.AppResult.Message = "CategoryId not exist!";
            else
            {
                PersonCategory personCategory = await _skillRepository.GetPersonCategoryAsync(id, null, null);
                if (personCategory == null)
                    model.AppResult.Message = "Update failure, Skill not exists";
                else
                {
                    var modelCategory = await _categoryRepository.FindAsync(skillRequestModel.CategoryId);
                    if (modelCategory == null)
                        model.AppResult.Message = "Update failure, Category not exists";
                    else
                    {
                        List<PersonTechnology> personTechnologies = new List<PersonTechnology>();
                        personTechnologies = await GetListPersonTechnology(skillRequestModel);
                        if (personTechnologies.Count > 0)
                        {
                            personCategory.CategoryId = skillRequestModel.CategoryId;
                            personCategory.CreatedBy = WebAPI.Helpers.HttpContext.CurrentUser;
                            personCategory.UpdatedBy = WebAPI.Helpers.HttpContext.CurrentUser;
                            await _skillRepository.DeletePersonTechnologyToUpdateAsync(personCategory);
                            await _skillRepository.InsertListPersonTechnologyAsync(personTechnologies);
                            if (id > 0 && skillRequestModel.PersonId > 0)
                            {
                                List<Category> listResult = new List<Category>();
                                modelCategory = await _skillRepository.GetSkillInsertAsync(id, skillRequestModel.PersonId);
                                if (modelCategory != null)
                                {
                                    model.AppResult.Result = true;
                                    model.AppResult.Message = Constants.Constant.UPDATE_SUCCESS;
                                    listResult.Add(modelCategory);
                                    model.AppResult.DataResult = GetListSkillResource(listResult).FirstOrDefault();
                                }
                            }
                        }
                        else
                            model.AppResult.Message = "Update failure, Technology not exists";
                    }
                }

            }
            return model;
        }

        /// <summary>
        /// Delete Skill
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<SkillViewModel> DeleteSkill(int id)
        {
            model.AppResult.Result = false;
            PersonCategory personCategory = await _skillRepository.GetPersonCategoryAsync(id, null, null);
            if (personCategory != null)
            {
                var result = await _skillRepository.DeletePersonCategoryAsync(personCategory);
                if (result > 0)
                {
                    model.AppResult.Result = true;
                    await _skillRepository.DeletePersonTechnologyAsync(personCategory);
                    model.AppResult.Message = Constants.Constant.DELETE_SUCCESS;
                }
                else
                    model.AppResult.Message = "Deletion failed, Skill not exists";
            }
            else
                model.AppResult.Message = "Deletion failed, Skill not exists";
            return model;
        }


        /// <summary>
        /// Get Skill By Category
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<TechnologyResource>> GetSkillByCategory(int personId, int categoryId)
        {
            PersonCategory personCategory = new PersonCategory
            {
                PersonId = personId,
                CategoryId = categoryId
            };
            IEnumerable<Technology> list = await _skillRepository.GetSkilByCategoryAsync(personCategory);
            List<TechnologyResource> listResult = new List<TechnologyResource>();
            if (list.Any())
            {
                foreach (var item in list)
                {
                    TechnologyResource technologyViewModel = new TechnologyResource
                    {
                        Id = item.Id,
                        Name = item.Name,
                        CategoryId = item.Category.Id,
                        CategoryName = item.Category.Name
                    };
                    listResult.Add(technologyViewModel);
                }
            }
            return listResult;
        }

        /// <summary>
        /// Swap OrderIndex
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<SkillViewModel> SwapOrderIndex(SwapOrderIndexRequestModel modelSwap)
        {
            if (modelSwap.CurrentId > 0 && modelSwap.TurnedId > 0)
            {
                var tempCurrentPersonCategory = await _skillRepository.GetPersonCategoryAsync(modelSwap.CurrentId, null, null);
                var tempTeurnedPersonCategory = await _skillRepository.GetPersonCategoryAsync(modelSwap.TurnedId, null, null);
                if (tempCurrentPersonCategory == null || tempTeurnedPersonCategory == null)
                    model.AppResult.Message = "Swap fail, Skill not exists";
                else
                {
                    int orderIndexCurrentPersonCategory = tempCurrentPersonCategory.OrderIndex;
                    int orderIndexTeurnedPersonCategory = tempTeurnedPersonCategory.OrderIndex;
                    tempCurrentPersonCategory.OrderIndex = orderIndexTeurnedPersonCategory;
                    tempCurrentPersonCategory.UpdatedAt = DateTime.Now;
                    tempCurrentPersonCategory.UpdatedBy = WebAPI.Helpers.HttpContext.CurrentUser;
                    tempTeurnedPersonCategory.OrderIndex = orderIndexCurrentPersonCategory;
                    tempTeurnedPersonCategory.UpdatedAt = DateTime.Now;
                    tempTeurnedPersonCategory.UpdatedBy = WebAPI.Helpers.HttpContext.CurrentUser;
                    List<PersonCategory> personCategories = new List<PersonCategory>();
                    personCategories.Add(tempCurrentPersonCategory);
                    personCategories.Add(tempTeurnedPersonCategory);
                    var result = await _skillRepository.UpdateOrderIndexPersonCategory(personCategories);
                    model.AppResult = new AppResult { Result = true, StatusCd = "200", Message = "Success" };
                }
            }
            else
                model.AppResult.Message = "Swap fail, Invalid CurrentId or TurnedId";
            return model;
        }
    }
}
