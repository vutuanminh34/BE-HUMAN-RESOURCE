#nullable enable
using Dapper;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Models;

namespace WebAPI.Repositories.Certificates
{
    public class CertificateRepository : RepositoryBase, ICertificateRepository
    {
        public CertificateRepository(string connectionString) : base(connectionString) { }

        /// <summary>
        /// Get all field from table Certificate with personId
        /// </summary>
        /// <param name="personId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<CertificateInfo>> GetCertificateByPersonIdAsync(int personId, bool flag = true)
        {
            using (var conn = OpenDBConnection())
            {
                string sql = string.Format(@"SELECT * FROM dbo.Certificate WHERE PersonId = @personid AND Status = @status");
                var param = new
                {
                    personid = personId,
                    status = flag
                };
                var tempCertificateInfo = await conn.QueryAsync<CertificateInfo>(sql, param);
                return tempCertificateInfo;
            }
        }

        /// <summary>
        /// Find a field from Certificate with Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<CertificateInfo> FindAsync(int id)
        {
            using (var conn = OpenDBConnection())
            {
                var sql = "SELECT * FROM dbo.Certificate WHERE Id = @_id AND Status = @status";
                var param = new { _id = id, status = true };
                return await conn.QueryFirstOrDefaultAsync<CertificateInfo>(sql, param);
            }
        }

        /// <summary>
        /// Validate PersonId
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<int> ValidatePersonIdAsync(int id, bool flag = true)
        {
            using (var conn = OpenDBConnection())
            {
                var sql = "SELECT COUNT(*) FROM dbo.Person WHERE Id = @_id AND Status = @status";
                var tempCount = await conn.ExecuteScalarAsync<int>(sql, new { _id = id, status = flag });

                return tempCount;
            }
        }

        /// <summary>
        /// Find maximum value of OrderIndex in table Certificate
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        public async Task<int> MaximumOrderIndexAsync(int personId, bool flag = true)
        {
            using (var conn = OpenDBConnection())
            {
                var sql = "SELECT MAX(OrderIndex) FROM dbo.Certificate WHERE PersonId = @id AND Status = @status";
                var tempMaximum = await conn.ExecuteScalarAsync<int>(sql, new { id = personId, status = flag });

                return tempMaximum;
            }
        }

        /// <summary>
        /// Create a field into Certificate
        /// </summary>
        /// <param name="certificateObj"></param>
        /// <returns></returns>
        public async Task<int> InsertAsync(CertificateInfo certificateObj)
        {
            using (var conn = OpenDBConnection())
            {
                StringBuilder sql = new StringBuilder(string.Empty);

                sql.AppendLine("  INSERT INTO                  ");
                sql.AppendLine("       dbo.Certificate (Name,  ");
                sql.AppendLine("       Provider,               ");
                sql.AppendLine("       OrderIndex,             ");
                sql.AppendLine("       Status,                 ");
                sql.AppendLine("       StartDate,              ");
                sql.AppendLine("       EndDate,                ");
                sql.AppendLine("       CreatedAt,              ");
                sql.AppendLine("       CreatedBy,              ");
                sql.AppendLine("       UpdatedAt,              ");
                sql.AppendLine("       UpdatedBy,              ");
                sql.AppendLine("       PersonId)               ");
                sql.AppendLine("  OUTPUT INSERTED.ID           ");
                sql.AppendLine("  VALUES (@Name,               ");
                sql.AppendLine("       @Provider,              ");
                sql.AppendLine("       @OrderIndex,            ");
                sql.AppendLine("       @Status,                ");
                sql.AppendLine("       @StartDate,             ");
                sql.AppendLine("       @EndDate,               ");
                sql.AppendLine("       @CreatedAt,             ");
                sql.AppendLine("       @CreatedBy,             ");
                sql.AppendLine("       @UpdatedAt,             ");
                sql.AppendLine("       @UpdatedBy,             ");
                sql.AppendLine("       @PersonId)              ");

                var param = new
                {
                    name = certificateObj.Name,
                    provider = certificateObj.Provider,
                    orderIndex = certificateObj.OrderIndex,
                    status = certificateObj.Status,
                    startDate = certificateObj.StartDate,
                    endDate = certificateObj.EndDate,
                    createdAt = certificateObj.CreatedAt,
                    createdBy = certificateObj.CreatedBy,
                    updatedAt = certificateObj.UpdatedAt,
                    updatedBy = certificateObj.UpdatedBy,
                    personId = certificateObj.PersonId
                };

                return await conn.ExecuteScalarAsync<int>(sql.ToString(), param);
            }
        }

        /// <summary>
        /// Update a field from table Certificate
        /// </summary>
        /// <param name="certificateObj"></param>
        /// <returns></returns>
        public async Task<int> UpdateAsync(CertificateInfo certificateObj)
        {
            using (var conn = OpenDBConnection())
            {
                StringBuilder sql = new StringBuilder(string.Empty);

                sql.AppendLine("  UPDATE dbo.Certificate          ");
                sql.AppendLine("  SET Name = @Name,               ");
                sql.AppendLine("       Provider = @Provider,      ");
                sql.AppendLine("       OrderIndex = @OrderIndex,  ");
                sql.AppendLine("       Status = @Status,          ");
                sql.AppendLine("       StartDate = @StartDate,    ");
                sql.AppendLine("       EndDate = @EndDate,        ");
                sql.AppendLine("       UpdatedAt = @UpdatedAt,    ");
                sql.AppendLine("       UpdatedBy = @UpdatedBy,    ");
                sql.AppendLine("       PersonId = @PersonId       ");
                sql.AppendLine("  WHERE Id = @Id                  ");

                var param = new
                {
                    id = certificateObj.Id,
                    name = certificateObj.Name,
                    provider = certificateObj.Provider,
                    orderIndex = certificateObj.OrderIndex,
                    status = certificateObj.Status,
                    startDate = certificateObj.StartDate,
                    endDate = certificateObj.EndDate,
                    updatedAt = certificateObj.UpdatedAt,
                    updatedBy = certificateObj.UpdatedBy,
                    personId = certificateObj.PersonId,
                };
                return await conn.ExecuteAsync(sql.ToString(), param);
            }
        }

        /// <summary>
        /// Swap OrderIndex value of Object
        /// </summary>
        /// <param name="currentObj"></param>
        /// <param name="turnedObj"></param>
        /// <returns></returns>
        public async Task<bool> SwapOrderIndexAsync(CertificateInfo currentObj, CertificateInfo turnedObj)
        {
            using (var conn = OpenDBConnection())
            using (var transaction = conn.BeginTransaction())
            {
                #region Current/Turned
                StringBuilder sqlCurrent = new StringBuilder(string.Empty);
                // Query statement for Current Object
                sqlCurrent.AppendLine("  UPDATE dbo.Certificate                 ");
                sqlCurrent.AppendLine("  SET Name = @NameCurrent,               ");
                sqlCurrent.AppendLine("       Provider = @ProviderCurrent,      ");
                sqlCurrent.AppendLine("       OrderIndex = @OrderIndexCurrent,  ");
                sqlCurrent.AppendLine("       Status = @StatusCurrent,          ");
                sqlCurrent.AppendLine("       StartDate = @StartDateCurrent,    ");
                sqlCurrent.AppendLine("       EndDate = @EndDateCurrent,        ");
                sqlCurrent.AppendLine("       UpdatedAt = @UpdatedAtCurrent,    ");
                sqlCurrent.AppendLine("       UpdatedBy = @UpdatedByCurrent,    ");
                sqlCurrent.AppendLine("       PersonId = @PersonIdCurrent       ");
                sqlCurrent.AppendLine("  WHERE Id = @IdCurrent                  ");

                var paramCurrent = new
                {
                    idCurrent = currentObj.Id,
                    nameCurrent = currentObj.Name,
                    providerCurrent = currentObj.Provider,
                    orderIndexCurrent = currentObj.OrderIndex,
                    statusCurrent = currentObj.Status,
                    startDateCurrent = currentObj.StartDate,
                    endDateCurrent = currentObj.EndDate,
                    updatedAtCurrent = currentObj.UpdatedAt,
                    updatedByCurrent = currentObj.UpdatedBy,
                    personIdCurrent = currentObj.PersonId,
                };
                // Query statement for Turned Object
                StringBuilder sqlTurned = new StringBuilder(string.Empty);
                sqlTurned.AppendLine("  UPDATE dbo.Certificate                ");
                sqlTurned.AppendLine("  SET Name = @NameTurned,               ");
                sqlTurned.AppendLine("       Provider = @ProviderTurned,      ");
                sqlTurned.AppendLine("       OrderIndex = @OrderIndexTurned,  ");
                sqlTurned.AppendLine("       Status = @StatusTurned,          ");
                sqlTurned.AppendLine("       StartDate = @StartDateTurned,    ");
                sqlTurned.AppendLine("       EndDate = @EndDateTurned,        ");
                sqlTurned.AppendLine("       UpdatedAt = @UpdatedAtTurned,    ");
                sqlTurned.AppendLine("       UpdatedBy = @UpdatedByTurned,    ");
                sqlTurned.AppendLine("       PersonId = @PersonIdTurned       ");
                sqlTurned.AppendLine("  WHERE Id = @IdTurned                  ");

                var paramTurned = new
                {
                    idTurned = turnedObj.Id,
                    nameTurned = turnedObj.Name,
                    providerTurned = turnedObj.Provider,
                    orderIndexTurned = turnedObj.OrderIndex,
                    statusTurned = turnedObj.Status,
                    startDateTurned = turnedObj.StartDate,
                    endDateTurned = turnedObj.EndDate,
                    updatedAtTurned = turnedObj.UpdatedAt,
                    updatedByTurned = turnedObj.UpdatedBy,
                    personIdTurned = turnedObj.PersonId,
                };
                #endregion
                int isSuccessCurrent = await conn.ExecuteAsync(sqlCurrent.ToString(), paramCurrent, transaction);
                int isSuccessTurned = await conn.ExecuteAsync(sqlTurned.ToString(), paramTurned, transaction);
                if (isSuccessCurrent > 0 && isSuccessTurned > 0)
                {
                    transaction.Commit();
                    return true;
                }
                return false;
            }
        }
        #region Not use
        /// <summary>
        /// Delete Certificate
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<int> DeleteAsync(int id)
        {
            using (var conn = OpenDBConnection())
            {
                var sql = "DELETE FROM dbo.Certificate WHERE Id = @Id";
                return await conn.ExecuteAsync(sql.ToString(), new { Id = id });
            }
        }

        /// <summary>
        /// Get all Certificate
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<CertificateInfo>> GetAllAsync()
        {
            using (var conn = OpenDBConnection())
            {
                var sql = "SELECT * FROM dbo.Certificate";
                return await conn.QueryAsync<CertificateInfo>(sql);
            }
        }
        #endregion
    }
}
