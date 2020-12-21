using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using WebAPI.Models;
using WebAPI.Models.Resource.Person;
using WebAPI.ViewModels;

namespace WebAPI.Services.Persons
{
    public interface IPersonService
    {
        public Task<PersonViewModel> InsertPersonToImportFile(Person person, Image image);
        public Task<PersonViewModel> UpdatePersonToImportFile(Person person, Image image);

        /// <summary>
        /// Pagination and search
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="FullName"></param>
        /// <param name="Location"></param>
        /// <param name="TechnologyId"></param>
        /// <returns></returns>
        Task<IEnumerable<Person>> SearchPersonAndSkillAsync(int? page, int? limit, string FullName, int? Location, List<int> TechnologyId);

        /// <summary>
        /// Get Person by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Person> GetPersonById(int id);

        /// <summary>
        /// Create Person
        /// </summary>
        /// <param name="person"></param>
        /// <returns></returns>
        public Task<PersonViewModel> InsertPerson(Person person, FileUpload objectFile);

        /// <summary>
        /// Update Person
        /// </summary>
        /// <param name="person"></param>
        /// <returns></returns>
        public Task<PersonViewModel> UpdatePerson(string files, Person person, FileUpload objectFile);

        /// <summary>
        /// 
        /// </summary>
        /// </summary>
        /// <param name="person"></param>
        /// <returns></returns>
        public Task<PersonViewModel> UpdateOverview(SavePersonResource personObj);

        /// <summary>
        /// Delete Person
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<PersonViewModel> DeletePerson(int id);
    }
}
