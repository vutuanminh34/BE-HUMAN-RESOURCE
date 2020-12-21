using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Models;
using WebAPI.Models.Resource.Technology;
using WebAPI.Repositories.Categories;
using WebAPI.Repositories.Technologies;
using WebAPI.ViewModels;

namespace WebAPI.Services.Technologies
{
    public class TechnologyService : BaseService<TechnologyViewModel>, ITechnologyService
    {
        ITechnologyRepository _technologyRepository;
        ICategoryRepository _categoryRepository;
        public TechnologyService(ITechnologyRepository technologyRepository, ICategoryRepository categoryRepository)
        {
            this._technologyRepository = technologyRepository;
            this._categoryRepository = categoryRepository;
        }

        /// <summary>
        /// Get List TechnologyResource
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public List<TechnologyResource> GetListTechnologyResource(IEnumerable<Technology> list)
        {
            List<TechnologyResource> listResult = new List<TechnologyResource>();
            if (list.Any())
            {
                foreach (var item in list)
                {
                    TechnologyResource technologyViewModel = new TechnologyResource
                    {
                        Id = item.Id,
                        Name = item.Name,
                        CreatedAt = item.CreatedAt,
                        CategoryId = item.Category.Id,
                        CategoryName = item.Category.Name
                    };
                    listResult.Add(technologyViewModel);
                }
            }
            return listResult;
        }

        /// <summary>
        /// Get All Technology
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<BaseModel> GetAllTechnology(int? pageIndex, int? pageSize, string name, int? category)
        {
            List<TechnologyResource> listResult = new List<TechnologyResource>();
            int offset = -1;
            if (pageSize != null && pageIndex != null)
                offset = (int)((pageIndex - 1) * pageSize);
            else
                pageSize = 0;
            int categoryId = 0;
            if (category != null)
                categoryId = (int)category;
            IEnumerable<Technology> list = await _technologyRepository.GetAllTechnologyAsync((int)pageSize, offset, name, categoryId);
            listResult = GetListTechnologyResource(list);
            BaseModel baseModel = new BaseModel();
            baseModel.DataResult = listResult;
            baseModel.totalCount = await _technologyRepository.GetTotalCount(name);
            return baseModel;
        }

        /// <summary>
        /// Get Technology By Person
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IEnumerable<TechnologyResource>> GetTechnologyByPerson(int id)
        {
            List<TechnologyResource> listResult = new List<TechnologyResource>();
            IEnumerable<Technology> list = await _technologyRepository.GetTechnologyByPersonAsync(id);
            listResult = GetListTechnologyResource(list);
            return listResult;
        }

        /// <summary>
        /// Get Technology By Category
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IEnumerable<TechnologyResource>> GetTechnologyByCategory(int id)
        {
            List<TechnologyResource> listResult = new List<TechnologyResource>();
            IEnumerable<Technology> list = await _technologyRepository.GetTechnologyByCategoryAsync(id);
            listResult = GetListTechnologyResource(list);
            return listResult;
        }

        /// <summary>
        /// Create Technology
        /// </summary>
        /// <param name="technology"></param>
        /// <returns></returns>
        public async Task<TechnologyViewModel> InsertTechnology(Technology technology)
        {
            model.AppResult.Result = false;
            if (technology.CategoryId <= 0)
                model.AppResult.Message = Constants.Constant.ID_ERROR;
            else if (String.IsNullOrEmpty(technology.Name) || technology.Name.Length > 2555)
                model.AppResult.Message = Constants.Constant.NAME_ERROR;
            else
            {
                technology.CreatedBy = WebAPI.Helpers.HttpContext.CurrentUser;
                technology.UpdatedBy = WebAPI.Helpers.HttpContext.CurrentUser;
                technology.CreatedAt = DateTime.Now;
                technology.UpdatedAt = DateTime.Now;
                var modelCategory = await _categoryRepository.FindAsync(technology.CategoryId);
                if (modelCategory == null)
                    model.AppResult.Message = "Create failure, Category not exists";
                else
                {
                    var checkTechnology = await _technologyRepository.CheckTechnologyAsync(technology);
                    if (checkTechnology != null)
                    {
                        model.AppResult.Message = "Create failure, Technology already exists";
                        model.Technology.Id = checkTechnology.Id;
                    }
                    else
                    {
                        var id = await _technologyRepository.InsertAsync(technology);
                        Technology modelTechnology = await _technologyRepository.FindAsync(id);
                        TechnologyResource resultModel = new TechnologyResource();
                        if (modelTechnology != null)
                        {
                            model.Technology.Id = modelTechnology.Id;
                            resultModel.Id = modelTechnology.Id;
                            resultModel.Name = modelTechnology.Name;
                            resultModel.CreatedAt = modelTechnology.CreatedAt;
                            resultModel.CategoryId = modelTechnology.Category.Id;
                            resultModel.CategoryName = modelTechnology.Category.Name;
                        }
                        if (resultModel != null)
                            model.AppResult.DataResult = resultModel;
                        model.AppResult.Result = true;
                        model.AppResult.Message = Constants.Constant.INSERT_SUCCESS;
                    }
                }
            }
            return model;
        }

        /// <summary>
        /// Update Technology
        /// </summary>
        /// <param name="technology"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<TechnologyViewModel> UpdateTechnology(Technology technology, int id)
        {
            model.AppResult.Result = false;
            if (id <= 0)
                model.AppResult.Message = Constants.Constant.ID_ERROR;
            else if (String.IsNullOrEmpty(technology.Name) || technology.Name.Length > 2555)
                model.AppResult.Message = Constants.Constant.NAME_ERROR;
            else
            {
                var modelCategory = await _categoryRepository.FindAsync(technology.CategoryId);
                if (modelCategory == null)
                    model.AppResult.Message = "Update failure, Category update not exists";
                else
                {
                    technology.Id = id;
                    technology.UpdatedAt = DateTime.Now;
                    technology.UpdatedBy = WebAPI.Helpers.HttpContext.CurrentUser;
                    var modelTechnology = await _technologyRepository.FindAsync(id);
                    if (modelTechnology == null)
                        model.AppResult.Message = "Update failed, Technology not exists";
                    else
                    {
                        var checkTechnology = await _technologyRepository.CheckTechnologyAsync(technology);
                        if (checkTechnology != null)
                        {
                            model.AppResult.Message = "Update failure, Technology already exists";
                            model.Technology.Id = checkTechnology.Id;
                        }
                        else
                        {
                            technology.CreatedAt = modelTechnology.CreatedAt;
                            technology.CreatedBy = modelTechnology.CreatedBy;
                            technology.UpdatedAt = DateTime.Now;
                            technology.Status = modelTechnology.Status;
                            var result = await _technologyRepository.UpdateAsync(technology);
                            if (result > 0)
                            {
                                model.AppResult.DataResult = result;
                                model.AppResult.Result = true;
                                model.AppResult.Message = Constants.Constant.UPDATE_SUCCESS;
                            }
                            else
                                model.AppResult.Message = "Update failed, Technology not exists";
                        }

                    }
                }
            }
            return model;
        }

        /// <summary>
        /// Delete Technology
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<TechnologyViewModel> DeleteTechnology(int id)
        {
            model.AppResult.Result = false;
            var modelTechnology = await _technologyRepository.FindAsync(id);
            if (modelTechnology == null)
                model.AppResult.Message = "Deleted failed, Technology not exists";
            else
            {
                modelTechnology.UpdatedAt = DateTime.Now;
                modelTechnology.UpdatedBy = WebAPI.Helpers.HttpContext.CurrentUser;
                modelTechnology.Status = false;
                var result = await _technologyRepository.UpdateAsync(modelTechnology);
                if (result > 0)
                {
                    model.AppResult.DataResult = result;
                    model.AppResult.Result = true;
                    model.AppResult.Message = Constants.Constant.DELETE_SUCCESS;
                }
                else
                    model.AppResult.Message = "Deleted failed, Technology not exists";
            }
            return model;
        }

        /// <summary>
        /// Get Technology By PersonId And Name Technology
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<int> GetTechnologyByPersonAndNameAsync(int personId, string name)
        {
            return await _technologyRepository.GetTechnologyByPersonAndNameAsync(personId, name);
        }
    }
}
