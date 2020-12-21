using System.Collections.Generic;
using System.Threading.Tasks;
using WebAPI.Models;
using WebAPI.Models.Token;
using WebAPI.ViewModels;

namespace WebAPI.Services.Accounts
{
    public interface IAccountService
    {
        /// <summary>
        /// Get all Account with pagging
        /// </summary>
        /// <returns></returns>
        Task<AccountViewModel> GetAllAccountWithPagging(Paggings paggings);

        /// <summary>
        /// Get all Account
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<AccountInfo>> GetAllAccount();

        /// <summary>
        /// Get Account by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<AccountInfo> GetAccountById(int id);

        /// <summary>
        /// Get Account by Name
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Task<AccountViewModel> GetAllAccountByName(AccountViewModel model);

        /// <summary>
        /// Create Account
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public Task<AccountViewModel> InsertAccount(AccountInfo account);

        /// <summary>
        /// Update Account
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public Task<AccountViewModel> UpdateAccount(AccountInfo account);

        /// <summary>
        /// update password only
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public Task<AccountViewModel> UpdatePassword(AccountInfo account);

        /// <summary>
        /// Delete Account
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<AccountViewModel> DeleteAccount(int id);

        /// <summary>
        /// Authenticate
        /// </summary>
        /// <param name="model"></param>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        public Task<AuthenticateResponse> Authenticate(AuthenticateRequest model, string ipAddress);

        /// <summary>
        /// Refresh Token
        /// </summary>
        /// <param name="token"></param>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        public Task<AuthenticateResponse> RefreshToken(string token, string ipAddress);

        /// <summary>
        /// check old password
        /// </summary>
        /// <param name="id"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool CheckPass(int id, string password);
    }
}
