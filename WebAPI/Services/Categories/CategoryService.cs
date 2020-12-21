using System;
using System.Threading.Tasks;
using WebAPI.Models;
using WebAPI.Repositories.Categories;
using WebAPI.ViewModels;

namespace WebAPI.Services.Categories
{
    public class CategoryService : BaseService<CategoryViewModel>, ICategoryService
    {
        ICategoryRepository _categoryRepository;
        public CategoryService(ICategoryRepository categoryRepository)
        {
            this._categoryRepository = categoryRepository;
        }

        /// <summary>
        /// Get all Category
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<BaseModel> GetAllCategory(int? pageIndex, int? pageSize, string name)
        {
            int offset = -1;
            if (pageSize != null && pageIndex != null)
                offset = (int)((pageIndex - 1) * pageSize);
            else
                pageSize = 0;
            BaseModel baseModel = new BaseModel();
            baseModel.DataResult = await _categoryRepository.GetAllCategoryAsync((int)pageSize, offset, name);
            baseModel.totalCount = await _categoryRepository.GetTotalCount(name);
            return baseModel;
        }

        /// <summary>
        /// Create Category
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public async Task<CategoryViewModel> InsertCategory(Category category)
        {
            model.Category = category;
            model.AppResult.Result = false;
            if (String.IsNullOrEmpty(category.Name) || category.Name.Length > 255)
                model.AppResult.Message = Constants.Constant.NAME_ERROR;
            else
            {
                category.CreatedBy = WebAPI.Helpers.HttpContext.CurrentUser;
                category.UpdatedBy = WebAPI.Helpers.HttpContext.CurrentUser;
                category.UpdatedAt = DateTime.Now;
                category.CreatedAt = DateTime.Now;
                var checkCategory = await _categoryRepository.CheckCategoryAsync(category);
                if (checkCategory != null)
                {
                    model.Category = checkCategory;
                    model.AppResult.Message = "Create failure, Category already exists";
                }
                else
                {
                    var id = await _categoryRepository.InsertAsync(category);
                    model.Category = await _categoryRepository.FindAsync(id);
                    model.AppResult.DataResult = model.Category;
                    model.AppResult.Result = true;
                    model.AppResult.Message = Constants.Constant.INSERT_SUCCESS;
                }
            }
            return model;
        }

        /// <summary>
        /// Update Category
        /// </summary>
        /// <param name="category"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<CategoryViewModel> UpdateCategory(Category category, int id)
        {
            model.Category = category;
            model.AppResult.Result = false;
            if (String.IsNullOrEmpty(category.Name) || category.Name.Length > 255)
                model.AppResult.Message = Constants.Constant.NAME_ERROR;
            else if (id <= 0)
                model.AppResult.Message = Constants.Constant.ID_ERROR;
            else
            {
                var modelCategory = await _categoryRepository.FindAsync(id);
                if (modelCategory == null)
                    model.AppResult.Message = "Update failed, Category not exists";
                else
                {
                    category.Id = id;
                    var checkCategory = await _categoryRepository.CheckCategoryAsync(category);
                    if (checkCategory != null)
                        model.AppResult.Message = "Update failed, Category already exists";
                    else
                    {
                        modelCategory.UpdatedAt = DateTime.Now;
                        modelCategory.UpdatedBy = WebAPI.Helpers.HttpContext.CurrentUser;
                        modelCategory.Name = category.Name;
                        var result = await _categoryRepository.UpdateAsync(modelCategory);
                        model.AppResult.DataResult = result;
                        model.AppResult.Result = true;
                        model.AppResult.Message = Constants.Constant.UPDATE_SUCCESS;
                    }
                }
            }
            return model;
        }

        /// <summary>
        /// Delete Category
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<CategoryViewModel> DeleteCategory(int id)
        {
            model.AppResult.Result = false;
            var modelCategory = await _categoryRepository.FindAsync(id);
            if (modelCategory == null)
                model.AppResult.Message = "Deleted failed, Category not exists";
            else
            {
                modelCategory.UpdatedAt = DateTime.Now;
                modelCategory.UpdatedBy = WebAPI.Helpers.HttpContext.CurrentUser;
                modelCategory.Status = false;
                var result = await _categoryRepository.UpdateAsync(modelCategory);
                model.AppResult.DataResult = result;
                model.AppResult.Result = true;
                model.AppResult.Message = Constants.Constant.DELETE_SUCCESS;
            }
            return model;
        }
    }
}
