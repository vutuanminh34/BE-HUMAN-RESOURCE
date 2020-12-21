using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Models;
using WebAPI.Models.Token;
using WebAPI.Repositories.Accounts;
using WebAPI.ViewModels;
using WebAPI.Common;
using WebAPI.Constants;

namespace WebAPI.Services.Accounts
{
    public class AccountService : BaseService<AccountViewModel>, IAccountService 
    {
        private readonly AppSettings _appSettings;
        IAccountRepository _accountRepository;
        string errorPassword = Constant.PASSWORD_ERROR;
        public AccountService(IAccountRepository accountRepository, IOptions<AppSettings> appSettings)
        {
            this._accountRepository = accountRepository;
            this._appSettings = appSettings.Value;
        }

        public async Task<AccountViewModel> GetAllAccountWithPagging(Paggings paggings)
        {
            var oObject =  await _accountRepository.GetAllWithPaggingAsync(paggings);
            paggings.TotalNum = oObject.FirstOrDefault<AccountInfo>().TOTAL_COUNT;
            model.Pagging = paggings;
            model.ListAccount = oObject;
            return model;
        }

        /// <summary>
        /// Get all Account
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<AccountInfo>> GetAllAccount()
        {
            return await _accountRepository.GetAllAsync();
        }

        /// <summary>
        /// Get Account by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<AccountInfo> GetAccountById(int id)
        {
            return await _accountRepository.FindAsync(id);
        }

        /// <summary>
        /// Get Account by Name
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<AccountViewModel> GetAllAccountByName(AccountViewModel model)
        {
            var oObject = await _accountRepository.GetAccountByNameAsync(model);
            if(!oObject.GetEnumerator().MoveNext())
            {
                model.AppResult.Result = false;
                model.AppResult.Message = Constant.IPAGENUM_ERROR;
                return model;
            }
            model.Pagging.TotalNum = oObject.FirstOrDefault<AccountInfo>().TOTAL_COUNT;
            model.ListAccount = oObject;
            return model;
        }

        /// <summary>
        /// Create Account
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public async Task<AccountViewModel> InsertAccount(AccountInfo account) 
        {
            model.AccountInfo = account;
            model.AccountInfo.CREATED_BY = WebAPI.Helpers.HttpContext.CurrentUser;
            int countUserName = await _accountRepository.CheckUserName(account.USER_NAME);
            int countEmail = await _accountRepository.CheckEmailForInsert(account.EMAIL);
            int countPhone = await _accountRepository.CheckPhoneForInsert(account.PHONE);
            if(String.IsNullOrEmpty(account.USER_NAME) || account.USER_NAME.Length > 50 || countUserName > 0)
            {
                model.AppResult.Message = Constant.USERNAME_ERROR;
                return model;
            }
            else if (String.IsNullOrEmpty(account.FULL_NAME) || account.FULL_NAME.Length > 50)
            {
                model.AppResult.Message = Constant.FULLNAME_ERROR;
                return model;
            }
            else if(!Functions.ValidatePassword(account.PASSWORD, out errorPassword))
            {
                model.AppResult.Message = errorPassword;
                return model;
            }
            else if (!Functions.HandlingEmail(account.EMAIL) || string.IsNullOrEmpty(account.EMAIL) || countEmail > 0)
            {
                model.AppResult.Message = Constant.EMAIL_ERROR;
                return model;
            }
            else if (!Functions.ValidatePhone(account.PHONE) || countPhone > 0)
            {
                model.AppResult.Message = Constant.PHONE_ERROR;
                return model;
            }
            else if (String.IsNullOrEmpty(account.ADDRESS) || account.ADDRESS.Length > 500)
            {
                model.AppResult.Message = Constant.ADDRESS_ERROR;
                return model;
            }
            else if (String.IsNullOrEmpty(account.ROLE) || !Functions.ValidateRole(account.ROLE))
            {
                model.AppResult.Message = Constant.ROLE_ERROR;
                return model;
            }
            else
            {
                int id = await _accountRepository.InsertAsync(model.AccountInfo);
                var result = await GetAccountById(id);
                model.AppResult.Message = Constant.INSERT_SUCCESS; 
                model.AppResult.DataResult = result;
                return model;
            }
            
        }

        /// <summary>
        /// Update Account
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public async Task<AccountViewModel> UpdateAccount(AccountInfo account)
        {
            model.AccountInfo = account;
            model.AccountInfo.UPDATED_BY = WebAPI.Helpers.HttpContext.CurrentUser;
            var index = await GetAccountById(account.USER_NO);
            int countEmail = await _accountRepository.CheckEmailForUpdate(account.USER_NO, account.EMAIL);
            int countPhone = await _accountRepository.CheckPhoneForUpdate(account.USER_NO, account.PHONE);
            if (index is null)
            {
                model.AppResult.Message = Constant.ACCOUNT_ERROR;
                return model;
            }
            else if (String.IsNullOrEmpty(account.FULL_NAME) || account.FULL_NAME.Length > 50)
            {
                model.AppResult.Message = Constant.FULLNAME_ERROR;
                return model;
            }
            else if (!Functions.HandlingEmail(account.EMAIL) || string.IsNullOrEmpty(account.EMAIL) || countEmail > 0)
            {
                model.AppResult.Message = Constant.EMAIL_ERROR;
                return model;
            }
            else if (!Functions.ValidatePhone(account.PHONE) || countPhone > 0)
            {
                model.AppResult.Message = Constant.PHONE_ERROR;
                return model;
            }
            else if (String.IsNullOrEmpty(account.ADDRESS) || account.ADDRESS.Length > 500)
            {
                model.AppResult.Message = Constant.ADDRESS_ERROR;
                return model;
            }
            else if (String.IsNullOrEmpty(account.ROLE) || !Functions.ValidateRole(account.ROLE))
            {
                model.AppResult.Message = Constant.ROLE_ERROR;
                return model;
            }
            else
            {
                int result = 0;
                if (!string.IsNullOrEmpty(account.PASSWORD))
                {
                    if (!Functions.ValidatePassword(account.PASSWORD, out errorPassword))
                    {
                        model.AppResult.Message = errorPassword;
                        return model;
                    }
                    result = await _accountRepository.UpdateWithPass(model.AccountInfo);
                }
                else
                {
                    result = await _accountRepository.UpdateAsync(model.AccountInfo);
                }
                model.AppResult.Message = Constant.UPDATE_SUCCESS;
                model.AppResult.DataResult = result;
                return model;
            }
        }

        /// <summary>
        /// Delete Account
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<AccountViewModel> DeleteAccount(int id)
        {
            var index = await GetAccountById(id);
            if (index is null)
            {
                model.AppResult.Message = Constant.ACCOUNT_ERROR;
                return model;
            }
            await _accountRepository.DeleteAsync(id);
            model.AppResult.Message = Constant.DELETE_SUCCESS;
            model.AppResult.DataResult = "OK";
            return model;
        }

        /// <summary>
        /// Authenticate
        /// </summary>
        /// <param name="model"></param>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        public async Task<AuthenticateResponse> Authenticate(AuthenticateRequest model, string ipAddress)
        {
            model.Password = Functions.MD5Hash(model.Password);
            var user = await _accountRepository.AuthenticateAsync(model.Username, model.Password);

            // return null if user not found
            if (user == null) return null;

            // authentication successful so generate jwt and refresh tokens
            var jwtToken = GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken(ipAddress);
            // save refresh token
            user.RefreshToken = refreshToken;
            await _accountRepository.UpdateRefreshTokenAsync(user);

            return new AuthenticateResponse(user, jwtToken, refreshToken.Token);
        }

        /// <summary>
        /// RefreshToken
        /// </summary>
        /// <param name="token"></param>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        public async Task<AuthenticateResponse> RefreshToken(string token, string ipAddress)
        {
            var refreshToken = await _accountRepository.RefreshTokenAsync(token);

            // return null if no user found with token
            if (refreshToken == null) return null;

            // return null if token is no longer active
            if (!refreshToken.IsActive) return null;

            AccountInfo user = await _accountRepository.FindAsync(refreshToken.Id);
            // replace old refresh token with a new one and save
            var newRefreshToken = GenerateRefreshToken(ipAddress);
            refreshToken.Revoked = DateTime.UtcNow;
            refreshToken.RevokedByIp = ipAddress;
            refreshToken.ReplacedByToken = newRefreshToken.Token;
            user.RefreshToken = newRefreshToken;
            await _accountRepository.UpdateRefreshTokenAsync(user);

            // generate new jwt
            var jwtToken = GenerateJwtToken(user);

            return new AuthenticateResponse(user, jwtToken, newRefreshToken.Token);
        }

        /// <summary>
        /// Generate Jwt Token
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private string GenerateJwtToken(AccountInfo user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.USER_NAME.ToString()),
                    new Claim(ClaimTypes.Role, user.ROLE.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        /// <summary>
        /// Generate Refresh Token
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        private RefreshToken GenerateRefreshToken(string ipAddress)
        {
            using (var rngCryptoServiceProvider = new RNGCryptoServiceProvider())
            {
                var randomBytes = new byte[64];
                rngCryptoServiceProvider.GetBytes(randomBytes);
                return new RefreshToken
                {
                    Token = Convert.ToBase64String(randomBytes),
                    Expires = DateTime.UtcNow.AddMonths(1),
                    Created = DateTime.UtcNow,
                    CreatedByIp = ipAddress
                };
            }
        }

        /// <summary>
        /// Update password only
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public async Task<AccountViewModel> UpdatePassword(AccountInfo account)
        {
            model.AccountInfo = account;
            model.AccountInfo.UPDATED_BY = WebAPI.Helpers.HttpContext.CurrentUser;
            var index = await GetAccountById(account.USER_NO);
            if (index is null)
            {
                model.AppResult.Message = Constant.ACCOUNT_ERROR;
                return model;
            }
            else if (!Functions.ValidatePassword(account.PASSWORD, out errorPassword))
            {
                model.AppResult.Message = errorPassword;
                return model;
            }
            else
            {
                var result = await _accountRepository.UpdatePassword(model.AccountInfo);
                model.AppResult.Message = Constant.UPDATE_SUCCESS;
                model.AppResult.DataResult = result;
                return model;
            }
        }

        public bool CheckPass(int id, string password)
        {
            var check = _accountRepository.CheckPassword(id, password).Result;
            if (check == 1) return true;
            else return false;
        }
    }
}
