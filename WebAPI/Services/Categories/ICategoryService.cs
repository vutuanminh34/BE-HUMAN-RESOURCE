using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Models;
using WebAPI.ViewModels;

namespace WebAPI.Services.Categories
{
    public interface ICategoryService
    {


        /// <summary>
        /// Get all Category
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        Task<BaseModel> GetAllCategory(int? pageIndex, int? pageSize, string name);

        /// <summary>
        /// Create Category
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public Task<CategoryViewModel> InsertCategory(Category category);

        /// <summary>
        /// Update Category
        /// </summary>
        /// <param name="category"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<CategoryViewModel> UpdateCategory(Category category, int id);

        /// <summary>
        /// Delete Category
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<CategoryViewModel> DeleteCategory(int id);
    }
}
