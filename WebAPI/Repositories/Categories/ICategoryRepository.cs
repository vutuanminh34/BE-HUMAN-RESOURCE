using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Models;
using WebAPI.Repositories.Interfaces;
using WebAPI.ViewModels;

namespace WebAPI.Repositories.Categories
{
    public interface ICategoryRepository : IRepositoryBase<Category>
    {

        /// <summary>
        /// Get All Category
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="offset"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        Task<IEnumerable<Category>> GetAllCategoryAsync(int pageSize, int offset, string name);

        /// <summary>
        /// Check Category
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        Task<Category> CheckCategoryAsync(Category category);

        /// <summary>
        /// Get Total Count
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Task<int> GetTotalCount(string name);
    }
}
