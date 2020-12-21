using System.Collections.Generic;
using System.Threading.Tasks;
using WebAPI.Models;
using WebAPI.Repositories.Interfaces;

namespace WebAPI.Repositories.Persons
{
    public interface IPersonRepository : IRepositoryBase<Person>
    {
        public Task<int> AmountOfPerson();
        public Task<int> GetMaxIdPerson();
        public Task<string> GetStaffIdPerson(int id);
        public Task<string> GetDescription(int id);
        public Task<int> TotalPersonNotSkill();
        public Task<int> TotalPersonAndSkill(string fullName, int location, List<int> listTechnologyId);
        public Task<int> UpdateOverview(Person entity);
        public Task<int> CheckEmailExist(string email);
        public Task<int> CheckEmailUpdate(int id, string email);
        public Task<int> CheckPhoneExisting(string phone);
        public Task<int> CheckPhoneToUpdate(int id, string phone);
        Task<int> CheckPersonExisting(int id);

        public Task<IEnumerable<Person>> SearchPersonAndSkillAsync(int limit, int checkOffset, string FullName, int Location, List<int> TechnologyId);
    }
}