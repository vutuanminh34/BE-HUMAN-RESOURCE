using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebAPI.Models;
using Dapper;
using System.Text;
using System.Data;
using WebAPI.ViewModels;
using WebAPI.Models.Token;
using WebAPI.Common;

namespace WebAPI.Repositories.Accounts
{
    public class AccountRepository : RepositoryBase, IAccountRepository
    {
        public AccountRepository(string connectionString) : base(connectionString) { }

        public async Task<IEnumerable<AccountInfo>> GetAllWithPaggingAsync(Paggings paggings)
        {
            using (var conn = OpenDBConnection())
            {
                StringBuilder sql = new StringBuilder();
                sql.Length = 0;
                sql.AppendLine("SELECT  USER_NO, USER_CD,                ");
                sql.AppendLine("        USER_NAME, FULL_NAME,            ");
                sql.AppendLine("        EMAIL, PHONE,                    ");
                sql.AppendLine("        ADDRESS, ROLE,                   ");
                sql.AppendLine("       TOTAL_COUNT = COUNT(*) OVER()     ");
                sql.AppendLine("FROM dbo.Accounts                        ");
                sql.AppendLine("ORDER BY USER_NO DESC                    ");
                sql.AppendLine("OFFSET (@PageNum-1) * @PageSize ROWS     ");
                sql.AppendLine("FETCH NEXT @PageSize ROWS ONLY           ");
                var param = new
                {
                    PageNum = paggings.PageNum,
                    PageSize = paggings.PageSize,
                };
                return await conn.QueryAsync<AccountInfo>(sql.ToString(), param);
            }
        }

        public async Task<IEnumerable<AccountInfo>> GetAccountByNameAsync(AccountViewModel model)
        {
            using (var conn = OpenDBConnection())
            {
                StringBuilder sql = new StringBuilder();
                sql.Length = 0;
                sql.AppendLine("SELECT  USER_NO, USER_CD,                                                          ");
                sql.AppendLine("        USER_NAME, FULL_NAME,                                                      ");
                sql.AppendLine("        EMAIL, PHONE,                                                              ");
                sql.AppendLine("        ADDRESS, ROLE,                                                             ");
                sql.AppendLine("       TOTAL_COUNT = COUNT(*) OVER()                                               ");
                sql.AppendLine("FROM dbo.Accounts WHERE USER_NO IS NOT NULL                                        ");
                if(!string.IsNullOrEmpty(model.AccountInfo.FULL_NAME))
                    sql.AppendLine("AND FULL_NAME like N'%" + model.AccountInfo.FULL_NAME + "%'                     ");
                if(!string.IsNullOrEmpty(model.AccountInfo.EMAIL))
                    sql.AppendLine("AND EMAIL like '%" + model.AccountInfo.EMAIL + "%'                             ");
                if(!string.IsNullOrEmpty(model.AccountInfo.PHONE))
                    sql.AppendLine("AND PHONE like '%" + model.AccountInfo.PHONE + "%'                             ");
                if(!string.IsNullOrEmpty(model.AccountInfo.ADDRESS))
                    sql.AppendLine("AND ADDRESS like N'%" + model.AccountInfo.ADDRESS + "%'                         ");
                sql.AppendLine("ORDER BY USER_NO DESC                                                              ");
                sql.AppendLine("OFFSET (@PageNum-1) * @PageSize ROWS                                               ");
                sql.AppendLine("FETCH NEXT @PageSize ROWS ONLY                                                     ");
                var param = new
                {
                    PageNum = model.Pagging.PageNum,
                    PageSize = model.Pagging.PageSize,
                };
                return await conn.QueryAsync<AccountInfo>(sql.ToString(), param);
            }
        }

        /// <summary>
        /// Get all Account
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<AccountInfo>> GetAllAsync()
        {
            using (var conn = OpenDBConnection())
            {
                StringBuilder sql = new StringBuilder();
                sql.Length = 0;
                sql.AppendLine("SELECT  USER_NO, USER_CD,                ");
                sql.AppendLine("        USER_NAME, FULL_NAME,            ");
                sql.AppendLine("        EMAIL, PHONE,                    ");
                sql.AppendLine("        ADDRESS, ROLE                    ");
                sql.AppendLine("FROM dbo.Accounts                        ");
                return await conn.QueryAsync<AccountInfo>(sql.ToString());
            }
        }

        /// <summary>
        /// Check Account
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="userNo"></param>
        /// <returns></returns>
        public bool CheckAccount(IDbConnection conn, int userNo)
        {
            StringBuilder sql = new StringBuilder();
            sql.Length = 0;
            sql.AppendLine("SELECT SUM(CNT)                           ");
            sql.AppendLine("FROM (                                    ");
            sql.AppendLine("        SELECT                            ");
            sql.AppendLine("           COUNT(*) AS CNT                ");
            sql.AppendLine("        FROM dbo.Accounts                 ");
            sql.AppendLine("        WHERE USER_NO = @USER_NO          ");
            sql.AppendLine("     )                                    ");

            int count = conn.ExecuteScalar<int>(sql.ToString(), new { user_no = userNo });
            return count > 0;
        }

        /// <summary>
        /// Find Account
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<AccountInfo> FindAsync(int id)
        {
            using (var conn = OpenDBConnection())
            {
                StringBuilder sql = new StringBuilder();
                sql.Length = 0;
                sql.AppendLine("SELECT  USER_NO, USER_CD,                     ");
                sql.AppendLine("        USER_NAME, FULL_NAME,                 ");
                sql.AppendLine("        EMAIL, PHONE,                         ");
                sql.AppendLine("        ADDRESS, ROLE                         ");
                sql.AppendLine("FROM dbo.Accounts WHERE USER_NO = @user_no    ");
                var param = new { user_no = id };
                return await conn.QueryFirstOrDefaultAsync<AccountInfo>(sql.ToString(), param);
            }
        }

        /// <summary>
        /// Create Account
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<int> InsertAsync(AccountInfo entity)
        {
            using (var conn = OpenDBConnection())
            {
                StringBuilder sql = new StringBuilder();
                sql.Length = 0;

                sql.AppendLine(" INSERT INTO                            ");
                sql.AppendLine("      dbo.Accounts (USER_CD,            ");
                sql.AppendLine("                    USER_NAME,          ");
                sql.AppendLine("                    PASSWORD,           ");
                sql.AppendLine("                    FULL_NAME,          ");
                sql.AppendLine("                    EMAIL,              ");
                sql.AppendLine("                    PHONE,              ");
                sql.AppendLine("                    ADDRESS,            ");
                sql.AppendLine("                    ROLE,               ");
                sql.AppendLine("                    CREATED_AT,         ");
                sql.AppendLine("                    CREATED_BY)         ");
                sql.AppendLine(" VALUES (@USER_CD,                      ");
                sql.AppendLine("         @USER_NAME,                    ");
                sql.AppendLine("         @PASSWORD,                     ");
                sql.AppendLine("         @FULL_NAME,                    ");
                sql.AppendLine("         @EMAIL,                        ");
                sql.AppendLine("         @PHONE,                        ");
                sql.AppendLine("         @ADDRESS,                      ");
                sql.AppendLine("         @ROLE,                         ");
                sql.AppendLine("         getdate(),                     ");
                sql.AppendLine("         @CREATED_BY)                   ");
                sql.AppendLine("SELECT SCOPE_IDENTITY()                 ");
                entity.USER_CD = Guid.NewGuid().ToString().GetHashCode().ToString("x");
                entity.PASSWORD = Functions.MD5Hash(entity.PASSWORD);
                var param = new
                {
                    user_cd = entity.USER_CD,
                    user_name = entity.USER_NAME,
                    password = entity.PASSWORD,
                    full_name = entity.FULL_NAME,
                    email = entity.EMAIL,
                    phone = entity.PHONE,
                    address = entity.ADDRESS,
                    role = entity.ROLE,
                    created_by = entity.CREATED_BY
                };
                return await conn.ExecuteScalarAsync<int>(sql.ToString(), param);
            }
        }

        /// <summary>
        /// Update Account without password
        /// </summary>
        /// <param name="entityToUpdate"></param>
        /// <returns></returns>
        public async Task<int> UpdateAsync(AccountInfo entity)
        {
            using (var conn = OpenDBConnection())
            {
                StringBuilder sql = new StringBuilder();
                sql.Length = 0;

                sql.AppendLine(" UPDATE dbo.Accounts                    ");
                sql.AppendLine(" SET    FULL_NAME   = @FULL_NAME,       ");
                sql.AppendLine("        EMAIL       = @EMAIL,           ");
                sql.AppendLine("        PHONE       = @PHONE,           ");
                sql.AppendLine("        ADDRESS     = @ADDRESS,         ");
                sql.AppendLine("        ROLE        = @ROLE,            ");
                sql.AppendLine("        UPDATED_AT = getdate(),         ");
                sql.AppendLine("        UPDATED_BY = @UPDATED_BY        ");
                sql.AppendLine(" WHERE USER_NO = @USER_NO               ");

                var param = new
                {
                    full_name = entity.FULL_NAME,
                    email = entity.EMAIL,
                    phone = entity.PHONE,
                    address = entity.ADDRESS,
                    role = entity.ROLE,
                    updated_by = entity.UPDATED_BY,
                    user_no = entity.USER_NO
                };

                return await conn.ExecuteAsync(sql.ToString(), param);
            }
        }

        /// <summary>
        /// update account with password
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<int> UpdateWithPass(AccountInfo entity)
        {
            using (var conn = OpenDBConnection())
            {
                StringBuilder sql = new StringBuilder();
                sql.Length = 0;

                sql.AppendLine(" UPDATE dbo.Accounts                    ");
                sql.AppendLine(" SET    PASSWORD    = @PASSWORD,         ");
                sql.AppendLine("        FULL_NAME   = @FULL_NAME,       ");
                sql.AppendLine("        EMAIL       = @EMAIL,           ");
                sql.AppendLine("        PHONE       = @PHONE,           ");
                sql.AppendLine("        ADDRESS     = @ADDRESS,         ");
                sql.AppendLine("        ROLE        = @ROLE,            ");
                sql.AppendLine("        UPDATED_AT = getdate(),         ");
                sql.AppendLine("        UPDATED_BY = @UPDATED_BY        ");
                sql.AppendLine(" WHERE USER_NO = @USER_NO               ");
                entity.PASSWORD = Functions.MD5Hash(entity.PASSWORD);
                var param = new
                {
                    password = entity.PASSWORD,
                    full_name = entity.FULL_NAME,
                    email = entity.EMAIL,
                    phone = entity.PHONE,
                    address = entity.ADDRESS,
                    role = entity.ROLE,
                    updated_by = entity.UPDATED_BY,
                    user_no = entity.USER_NO
                };
                return await conn.ExecuteAsync(sql.ToString(), param);
            }
        }

        /// <summary>
        /// Update password only
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<int> UpdatePassword(AccountInfo entity)
        {
            using (var conn = OpenDBConnection())
            {
                StringBuilder sql = new StringBuilder();
                sql.Length = 0;
                sql.AppendLine(" UPDATE dbo.Accounts                    ");
                sql.AppendLine(" SET    PASSWORD = @PASSWORD            ");
                sql.AppendLine(" WHERE USER_NO = @USER_NO               ");

                entity.PASSWORD = Functions.MD5Hash(entity.PASSWORD);
                var param = new
                {
                    password = entity.PASSWORD,
                    user_no = entity.USER_NO
                };
                return await conn.ExecuteAsync(sql.ToString(), param);
            }
        }
        /// <summary>
        /// check old password
        /// </summary>
        /// <param name="id"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<int> CheckPassword(int id, string password)
        {
            using(var conn = OpenDBConnection())
            {
                string pass = Functions.MD5Hash(password);
                var sql = "SELECT COUNT(*) FROM dbo.Accounts WHERE USER_NO = '" + id + "' and PASSWORD = '" + pass + "'";
                return await conn.ExecuteScalarAsync<int>(sql);
            }
        }

        /// <summary>
        /// Check email for insert
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public async Task<int> CheckEmailForInsert(string email)
        {
            using(var conn = OpenDBConnection())
            {
                var sql = "SELECT COUNT(*) from dbo.Accounts WHERE EMAIL = '" + email + "'";
                return await conn.ExecuteScalarAsync<int>(sql);
            }
        }

        /// <summary>
        /// Check phone for insert
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        public async Task<int> CheckPhoneForInsert(string phone)
        {
            using (var conn = OpenDBConnection())
            {
                var sql = "SELECT COUNT(*) from dbo.Accounts WHERE PHONE = '" + phone + "'";
                return await conn.ExecuteScalarAsync<int>(sql);
            }
        }

        /// <summary>
        /// Check email for update
        /// </summary>
        /// <param name="id"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public async Task<int> CheckEmailForUpdate(int id, string email)
        {
            using (var conn = OpenDBConnection())
            {
                var sql = "SELECT COUNT(*) from dbo.Accounts WHERE EMAIL = '" + email + "' and USER_NO <> '" + id + "'";
                return await conn.ExecuteScalarAsync<int>(sql);
            }
        }

        /// <summary>
        /// Check phone for update
        /// </summary>
        /// <param name="id"></param>
        /// <param name="phone"></param>
        /// <returns></returns>
        public async Task<int> CheckPhoneForUpdate(int id, string phone)
        {
            using (var conn = OpenDBConnection())
            {
                var sql = "SELECT COUNT(*) from dbo.Accounts WHERE PHONE = '" + phone + "' and USER_NO <> '" + id + "'";
                return await conn.ExecuteScalarAsync<int>(sql);
            }
        }

        /// <summary>
        /// Delete Account
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<int> DeleteAsync(int id)
        {
            using (var conn = OpenDBConnection())
            {
                var sql = "DELETE FROM dbo.Accounts WHERE USER_NO = @USER_NO";
                return await conn.ExecuteAsync(sql.ToString(), new { user_no = id });
            }
        }

        /// <summary>
        /// Authenticate Account
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<AccountInfo> AuthenticateAsync(string username, string password)
        {
            using (var conn = OpenDBConnection())
            {
                StringBuilder sql = new StringBuilder();
                sql.Length = 0;
                sql.AppendLine("SELECT  USER_NO, USER_CD,                                                  ");
                sql.AppendLine("        USER_NAME, FULL_NAME,                                              ");
                sql.AppendLine("        EMAIL, PHONE,                                                      ");
                sql.AppendLine("        ADDRESS, ROLE                                                      ");
                sql.AppendLine("FROM dbo.Accounts WHERE USER_NAME = @USER_NAME AND PASSWORD = @PASSWORD    ");
                var param = new { user_name = username, password = password };
                return await conn.QuerySingleOrDefaultAsync<AccountInfo>(sql.ToString(), param);
            }
        }

        /// <summary>
        /// Get Refresh Token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<RefreshToken> RefreshTokenAsync(string token)
        {
            using (var conn = OpenDBConnection())
            {
                var sql = "SELECT * FROM dbo.RefreshToken WHERE Token = @JWT_TOKEN";
                var param = new { jwt_token = token };
                return await conn.QuerySingleOrDefaultAsync<RefreshToken>(sql, param);
            }
        }

        /// <summary>
        /// Check UserName already exist
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public async Task<int> CheckUserName(string userName)
        {
            using (var conn = OpenDBConnection())
            {
                var sql = "SELECT COUNT(*) FROM dbo.Accounts WHERE USER_NAME = N'" + userName + "'";
                return await conn.ExecuteScalarAsync<int>(sql);
            }
        }


        /// <summary>
        /// Update Refresh Token
        /// </summary>
        /// <param name="token"></param>
        /// <param name="userNo"></param>
        /// <returns></returns>
        public async Task UpdateRefreshTokenAsync(AccountInfo entity)
        {
            StringBuilder sql = new StringBuilder();
            sql.Length = 0;
            using (var transaction = DBTransactionScopeAsync)
            {
                using (var conn = OpenDBConnection())
                {
                    sql.AppendLine(" UPDATE dbo.Accounts                             ");
                    sql.AppendLine("    SET REFRESH_TOKEN   = @REFRESH_TOKEN         ");
                    sql.AppendLine(" WHERE USER_NO = @USER_NO                        ");
                    var param1 = new { refresh_token = entity.RefreshToken.Token, user_no = entity.USER_NO };
                    conn.Execute(sql.ToString(), param1);

                    sql.Length = 0;
                    sql.AppendLine(" DELETE dbo.RefreshToken                         ");
                    sql.AppendLine(" WHERE ID = @USER_NO                             ");
                    var param2 = new { user_no = entity.USER_NO };
                    conn.Execute(sql.ToString(), param2);

                    sql.Length = 0;
                    sql.AppendLine(" INSERT INTO                                     ");
                    sql.AppendLine("      dbo.RefreshToken (Id,                      ");
                    sql.AppendLine("                    Token,                       ");
                    sql.AppendLine("                    Expires,                     ");
                    sql.AppendLine("                    IsExpired,                   ");
                    sql.AppendLine("                    Created,                     ");
                    sql.AppendLine("                    CreatedByIp,                 ");
                    sql.AppendLine("                    Revoked,                     ");
                    sql.AppendLine("                    RevokedByIp,                 ");
                    sql.AppendLine("                    ReplacedByToken,             ");
                    sql.AppendLine("                    IsActive)                    ");
                    sql.AppendLine(" VALUES (@Id,                                    ");
                    sql.AppendLine("         @Token,                                 ");
                    sql.AppendLine("         @Expires,                               ");
                    sql.AppendLine("         @IsExpired,                             ");
                    sql.AppendLine("         @Created,                               ");
                    sql.AppendLine("         @CreatedByIp,                           ");
                    sql.AppendLine("         @Revoked,                               ");
                    sql.AppendLine("         @RevokedByIp,                           ");
                    sql.AppendLine("         @ReplacedByToken,                       ");
                    sql.AppendLine("         @IsActive)                              ");
                    var param3 = new
                    {
                        id = entity.USER_NO,
                        token = entity.RefreshToken.Token,
                        expires = entity.RefreshToken.Expires,
                        isexpired = entity.RefreshToken.IsExpired,
                        created = entity.RefreshToken.Created,
                        createdbyip = entity.RefreshToken.CreatedByIp,
                        revoked = entity.RefreshToken.Revoked,
                        revokedbyip = entity.RefreshToken.RevokedByIp,
                        replacedbytoken = entity.RefreshToken.ReplacedByToken,
                        isactive = entity.RefreshToken.IsActive
                    };
                    await conn.ExecuteAsync(sql.ToString(), param3);
                }
                transaction.Complete();
            }
        }
    }
}
