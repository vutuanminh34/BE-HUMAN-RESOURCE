using System.Collections.Generic;
using System.Threading.Tasks;
using WebAPI.Models;
using WebAPI.Models.Token;
using WebAPI.Repositories.Interfaces;
using WebAPI.ViewModels;

namespace WebAPI.Repositories.Accounts
{
    public interface IAccountRepository : IRepositoryBase<AccountInfo>
    {
        public Task<IEnumerable<AccountInfo>> GetAllWithPaggingAsync(Paggings paggings);

        public Task<IEnumerable<AccountInfo>> GetAccountByNameAsync(AccountViewModel model);

        /// <summary>
        /// Authenticate Account
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public Task<AccountInfo> AuthenticateAsync(string userName, string password);

        /// <summary>
        /// Get Refresh Token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public Task<RefreshToken> RefreshTokenAsync(string token);

        /// <summary>
        /// Update Refresh Token
        /// </summary>
        /// <param name="token"></param>
        /// <param name="userNo"></param>
        /// <returns></returns>
        public Task UpdateRefreshTokenAsync(AccountInfo entity);

        /// <summary>
        /// Check User Name
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public Task<int> CheckUserName(string userName);

        /// <summary>
        /// Update password only
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public Task<int> UpdatePassword(AccountInfo entity);

        /// <summary>
        /// Update Infor account and password
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public Task<int> UpdateWithPass(AccountInfo entity);

        /// <summary>
        /// check old password
        /// </summary>
        /// <param name="id"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public Task<int> CheckPassword(int id, string password);

        /// <summary>
        /// check email for insert
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public Task<int> CheckEmailForInsert(string email);

        /// <summary>
        /// Check phone for insert
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        public Task<int> CheckPhoneForInsert(string phone);

        /// <summary>
        /// Check email for update
        /// </summary>
        /// <param name="id"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public Task<int> CheckEmailForUpdate(int id, string email);

        /// <summary>
        /// Check phone for update
        /// </summary>
        /// <param name="id"></param>
        /// <param name="phone"></param>
        /// <returns></returns>
        public Task<int> CheckPhoneForUpdate(int id, string phone);
    }
}
