#nullable enable
using Dapper;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Models;

namespace WebAPI.Repositories.Educations
{
    public class EducationRepository : RepositoryBase, IEducationRepository
    {
        public EducationRepository(string connectionString) : base(connectionString) { }

        /// <summary>
        /// Get all field from table Education with personId
        /// </summary>
        /// <param name="personId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<EducationInfo>> GetEducationByPersonIdAsync(int personId, bool flag = true)
        {
            using (var conn = OpenDBConnection())
            {
                string sql = string.Format(@"SELECT * FROM dbo.Education WHERE PersonId = @personid AND Status = @status");
                var param = new { personid = personId,
                    status = flag
                };
                var tempEducationInfo = await conn.QueryAsync<EducationInfo>(sql, param);
                return tempEducationInfo;
            }
        }

        /// <summary>
        /// Find a field from Education with Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<EducationInfo> FindAsync(int id)
        {
            using (var conn = OpenDBConnection())
            {
                var sql = "SELECT * FROM dbo.Education WHERE Id = @_id AND Status = @status";
                var param = new { _id = id, status = true };
                return await conn.QueryFirstOrDefaultAsync<EducationInfo>(sql, param);
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
        /// Find maximum value of OrderIndex in table Education
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        public async Task<int> MaximumOrderIndexAsync(int personId, bool flag = true)
        {
            using (var conn = OpenDBConnection())
            {
                var sql = "SELECT MAX(OrderIndex) FROM dbo.Education WHERE PersonId = @id AND Status = @status";
                var tempMaximum = await conn.ExecuteScalarAsync<int>(sql, new {id = personId, status = flag });

                return tempMaximum;
            }
        }

        /// <summary>
        /// Create a field into Education
        /// </summary>
        /// <param name="educationObj"></param>
        /// <returns></returns>
        public async Task<int> InsertAsync(EducationInfo educationObj)
        {
            using (var conn = OpenDBConnection())
            {
                StringBuilder sql = new StringBuilder(string.Empty);

                sql.AppendLine("  INSERT INTO                       ");
                sql.AppendLine("       dbo.Education (CollegeName,  ");
                sql.AppendLine("       Major,                       ");
                sql.AppendLine("       OrderIndex,                  ");
                sql.AppendLine("       Status,                      ");
                sql.AppendLine("       StartDate,                   ");
                sql.AppendLine("       EndDate,                     ");
                sql.AppendLine("       CreatedAt,                   ");
                sql.AppendLine("       CreatedBy,                   ");
                sql.AppendLine("       UpdatedAt,                   ");
                sql.AppendLine("       UpdatedBy,                   ");
                sql.AppendLine("       PersonId)                    ");
                sql.AppendLine("  OUTPUT INSERTED.ID                ");
                sql.AppendLine("  VALUES (@CollegeName,             ");
                sql.AppendLine("       @Major,                      ");
                sql.AppendLine("       @OrderIndex,                 ");
                sql.AppendLine("       @Status,                     ");
                sql.AppendLine("       @StartDate,                  ");
                sql.AppendLine("       @EndDate,                    ");
                sql.AppendLine("       @CreatedAt,                  ");
                sql.AppendLine("       @CreatedBy,                  ");
                sql.AppendLine("       @UpdatedAt,                  ");
                sql.AppendLine("       @UpdatedBy,                  ");
                sql.AppendLine("       @PersonId)                   ");

                var param = new
                {
                    collegeName = educationObj.CollegeName,
                    major = educationObj.Major,
                    orderIndex = educationObj.OrderIndex,
                    status = educationObj.Status,
                    startDate = educationObj.StartDate,
                    endDate = educationObj.EndDate,
                    createdAt = educationObj.CreatedAt,
                    createdBy = educationObj.CreatedBy,
                    updatedAt = educationObj.UpdatedAt,
                    updatedBy = educationObj.UpdatedBy,
                    personId = educationObj.PersonId
                };

                return await conn.ExecuteScalarAsync<int>(sql.ToString(), param);
            }
        }

        /// <summary>
        /// Update a field from table Education
        /// </summary>
        /// <param name="educationObj"></param>
        /// <returns></returns>
        public async Task<int> UpdateAsync(EducationInfo educationObj)
        {
            using (var conn = OpenDBConnection())
            {
                StringBuilder sql = new StringBuilder(string.Empty);

                sql.AppendLine("  UPDATE dbo.Education             ");
                sql.AppendLine("  SET CollegeName = @CollegeName,  ");
                sql.AppendLine("       Major = @Major,             ");
                sql.AppendLine("       OrderIndex = @OrderIndex,   ");
                sql.AppendLine("       Status = @Status,           ");
                sql.AppendLine("       StartDate = @StartDate,     ");
                sql.AppendLine("       EndDate = @EndDate,         ");
                sql.AppendLine("       UpdatedAt = @UpdatedAt,     ");
                sql.AppendLine("       UpdatedBy = @UpdatedBy,     ");
                sql.AppendLine("       PersonId = @PersonId        ");
                sql.AppendLine("  WHERE Id = @Id                   ");

                var param = new
                {
                    id = educationObj.Id,
                    collegeName = educationObj.CollegeName,
                    major = educationObj.Major,
                    orderIndex = educationObj.OrderIndex,
                    status = educationObj.Status,
                    startDate = educationObj.StartDate,
                    endDate = educationObj.EndDate,
                    updatedAt = educationObj.UpdatedAt,
                    updatedBy = educationObj.UpdatedBy,
                    personId = educationObj.PersonId,
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
        public async Task<bool> SwapOrderIndexAsync(EducationInfo currentObj, EducationInfo turnedObj) 
        {
            using (var conn = OpenDBConnection())
            using (var transaction = conn.BeginTransaction())
            {
                #region Current/Turned
                StringBuilder sqlCurrent = new StringBuilder(string.Empty);
                // Query statement for Current Object
                sqlCurrent.AppendLine("  UPDATE dbo.Education                    ");
                sqlCurrent.AppendLine("  SET CollegeName = @CollegeNameCurrent,  ");
                sqlCurrent.AppendLine("       Major = @MajorCurrent,             ");
                sqlCurrent.AppendLine("       OrderIndex = @OrderIndexCurrent,   ");
                sqlCurrent.AppendLine("       Status = @StatusCurrent,           ");
                sqlCurrent.AppendLine("       StartDate = @StartDateCurrent,     ");
                sqlCurrent.AppendLine("       EndDate = @EndDateCurrent,         ");
                sqlCurrent.AppendLine("       UpdatedAt = @UpdatedAtCurrent,     ");
                sqlCurrent.AppendLine("       UpdatedBy = @UpdatedByCurrent,     ");
                sqlCurrent.AppendLine("       PersonId = @PersonIdCurrent        ");
                sqlCurrent.AppendLine("  WHERE Id = @IdCurrent                   ");
                
                var paramCurrent = new
                {
                    idCurrent = currentObj.Id,
                    collegeNameCurrent = currentObj.CollegeName,
                    majorCurrent = currentObj.Major,
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
                sqlTurned.AppendLine("  UPDATE dbo.Education                   ");
                sqlTurned.AppendLine("  SET CollegeName = @CollegeNameTurned,  ");
                sqlTurned.AppendLine("       Major = @MajorTurned,             ");
                sqlTurned.AppendLine("       OrderIndex = @OrderIndexTurned,   ");
                sqlTurned.AppendLine("       Status = @StatusTurned,           ");
                sqlTurned.AppendLine("       StartDate = @StartDateTurned,     ");
                sqlTurned.AppendLine("       EndDate = @EndDateTurned,         ");
                sqlTurned.AppendLine("       UpdatedAt = @UpdatedAtTurned,     ");
                sqlTurned.AppendLine("       UpdatedBy = @UpdatedByTurned,     ");
                sqlTurned.AppendLine("       PersonId = @PersonIdTurned        ");
                sqlTurned.AppendLine("  WHERE Id = @IdTurned;                  ");

                var paramTurned = new
                {
                    idTurned = turnedObj.Id,
                    collegeNameTurned = turnedObj.CollegeName,
                    majorTurned = turnedObj.Major,
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
        /// Delete Education
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<int> DeleteAsync(int id)
        {
            using (var conn = OpenDBConnection())
            {
                var sql = "DELETE FROM dbo.Education WHERE Id = @Id";
                return await conn.ExecuteAsync(sql.ToString(), new { Id = id });
            }
        }
        
        /// <summary>
        /// Get all Education
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<EducationInfo>> GetAllAsync()
        {
            using (var conn = OpenDBConnection())
            {
                var sql = "SELECT * FROM dbo.Education";
                return await conn.QueryAsync<EducationInfo>(sql);
            }
        }
        #endregion
    }
}
