using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Models;
using WebAPI.Repositories.Interfaces;

namespace WebAPI.Repositories.Technologies
{
    public interface ITechnologyRepository:IRepositoryBase<Technology>
    {

        /// <summary>
        /// Get Technology By Person
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<IEnumerable<Technology>> GetTechnologyByPersonAsync(int id);

        /// <summary>
        /// Get Technology By Category
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<IEnumerable<Technology>> GetTechnologyByCategoryAsync(int id);

        /// <summary>
        /// Get All Technology
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="offset"></param>
        /// <param name="name"></param>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        Task<IEnumerable<Technology>> GetAllTechnologyAsync(int pageSize, int offset, string name, int categoryId);

        /// <summary>
        /// Check Technology
        /// </summary>
        /// <param name="technology"></param>
        /// <returns></returns>
        Task<Technology> CheckTechnologyAsync(Technology technology);


        /// <summary>
        /// Get Total Count
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Task<int> GetTotalCount(string name);

        /// <summary>
        /// Get Technology By PersonId And Name Technology
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public Task<int> GetTechnologyByPersonAndNameAsync(int personId, string name);
    }
}
