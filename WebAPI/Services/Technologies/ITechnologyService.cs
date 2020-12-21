using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Models;
using WebAPI.Models.Resource.Technology;
using WebAPI.ViewModels;

namespace WebAPI.Services.Technologies
{
    public interface ITechnologyService
    {

        /// <summary>
        /// Get all Technology
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="name"></param>
        /// <param name="category"></param>
        /// <returns></returns>
        Task<BaseModel> GetAllTechnology(int? pageIndex, int? pageSize, string name, int? category);

        /// <summary>
        /// Create Technology
        /// </summary>
        /// <param name="technology"></param>
        /// <returns></returns>
        public Task<TechnologyViewModel> InsertTechnology(Technology technology);

        /// <summary>
        /// Update Technology
        /// </summary>
        /// <param name="technology"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<TechnologyViewModel> UpdateTechnology(Technology technology, int id);

        /// <summary>
        /// Delete Technology
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<TechnologyViewModel> DeleteTechnology(int id);

        /// <summary>
        /// Get Technology By Person
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<IEnumerable<TechnologyResource>> GetTechnologyByPerson(int id);

        /// <summary>
        /// Get Technology By Category
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<IEnumerable<TechnologyResource>> GetTechnologyByCategory(int id);

        /// <summary>
        /// Get Technology By PersonId And Name Technology
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public Task<int> GetTechnologyByPersonAndNameAsync(int personId, string name);
    }
}
