using Dapper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Models;

namespace WebAPI.Repositories.WorkHistories
{
    public class WorkHistoryRepository : RepositoryBase, IWorkHistoryRepository
    {
        public WorkHistoryRepository(string connectionString) : base(connectionString) { }

        /// <summary>
        /// Get all WorkHistory
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<WorkHistoryInfo>> GetAllAsync()
        {
            using (var conn = OpenDBConnection())
            {
                var sql = "SELECT * FROM dbo.WorkHistory WHERE Status = 1 ";
                return await conn.QueryAsync<WorkHistoryInfo>(sql);
            }
        }

        /// <summary>
        /// Get WorkHistory By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<WorkHistoryInfo> FindAsync(int id)
        {
            using (var conn = OpenDBConnection())
            {
                var sql = "SELECT * FROM dbo.WorkHistory WHERE Id = @_id AND Status = 1";
                var param = new { _id = id };
                return await conn.QueryFirstOrDefaultAsync<WorkHistoryInfo>(sql, param);
            }
        }

        /// <summary>
        /// Get WorkHistory By Id
        /// </summary>
        /// <param name="personId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<WorkHistoryInfo>> FindAsyncByPersonId(int id)
        {
            using (var conn = OpenDBConnection())
            {
                var sql = "SELECT * FROM dbo.WorkHistory WHERE PersonId = @personId AND Status = 1";
                var param = new { personId = id };
                var results = await conn.QueryAsync<WorkHistoryInfo>(sql, param);
                return results;
            }
        }

        /// <summary>
        /// Find maximum value of OrderIndex in table WorkHistory
        /// </summary>
        /// <param name="personId"></param>
        /// <returns></returns>
        public async Task<int> MaximumOrderIndexAsync(int personId)
        {
            using (var conn = OpenDBConnection())
            {
                var sql = "SELECT MAX(OrderIndex) FROM dbo.WorkHistory WHERE PersonId = @id AND Status = @status";
                var tempMaximum = await conn.ExecuteScalarAsync<int>(sql, new { id = personId, status = true });

                return tempMaximum;
            }
        }

        /// <summary>
        /// Validate PersonId
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<int> ValidatePersonId(int id)
        {
            using (var conn = OpenDBConnection())
            {
                var sql = "SELECT COUNT(*) FROM dbo.Person WHERE Id = @_id";
                var tempCount = await conn.ExecuteScalarAsync<int>(sql, new { _id = id });
                return tempCount;
            }
        }

        /// <summary>
        /// Get count WorkHistory By Person Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int CountWorkHistory(int id)
        {
            using (var conn = OpenDBConnection())
            {
                var sql = "Select Count(*) from dbo.Workhistory where PersonId = '" + id + "'";
                return conn.ExecuteScalar<int>(sql);
            }
        }

        /// <summary>
        /// Get Max OrderIndex By Person Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int GetMaxOrderIndex(int id)
        {
            using (var conn = OpenDBConnection())
            {
                var sql = "Select Max(OrderIndex) from dbo.Workhistory where PersonId = '" + id + "'";
                return conn.ExecuteScalar<int>(sql);
            }
        }

        /// <summary>
        /// Create WorkHistory
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<int> InsertAsync(WorkHistoryInfo entity)
        {
            var orderIndex = 0;
            if (CountWorkHistory(entity.PersonId) <= 0)
            {
                orderIndex = 1;
            }
            else
            {
                orderIndex = GetMaxOrderIndex(entity.PersonId) + 1;
            }

            using (var conn = OpenDBConnection())
            {
                StringBuilder sql = new StringBuilder();
                sql.Length = 0;

                sql.AppendLine(" INSERT INTO                             ");
                sql.AppendLine("      dbo.WorkHistory (Position,         ");
                sql.AppendLine("                   CompanyName,          ");
                sql.AppendLine("                   OrderIndex,           ");
                sql.AppendLine("                   StartDate,            ");
                sql.AppendLine("                   EndDate,              ");
                sql.AppendLine("                   PersonId,             ");
                sql.AppendLine("                   CreatedAt,            ");
                sql.AppendLine("                   CreatedBy)            ");
                sql.AppendLine(" VALUES (@Position,                      ");
                sql.AppendLine("         @CompanyName,                   ");
                sql.AppendLine("         @OrderIndex,                    ");
                sql.AppendLine("         @StartDate,                     ");
                sql.AppendLine("         @EndDate,                       ");
                sql.AppendLine("         @PersonId,                      ");
                sql.AppendLine("         @CreatedAt,                     ");
                sql.AppendLine("         @CreatedBy)                     ");
                sql.AppendLine("SELECT SCOPE_IDENTITY()                  ");

                var param = new
                {
                    position = entity.Position,
                    companyName = entity.CompanyName,
                    orderIndex = orderIndex,
                    startDate = entity.StartDate,
                    endDate = entity.EndDate,
                    personId = entity.PersonId,
                    createdAt = entity.CreatedAt,
                    createdBy = entity.CreatedBy
                };
                return await conn.ExecuteScalarAsync<int>(sql.ToString(), param);
            }
        }

        /// <summary>
        /// Update WorkHistory
        /// </summary>
        /// <param name="entityToUpdate"></param>
        /// <returns></returns>
        public async Task<int> UpdateAsync(WorkHistoryInfo entity)
        {
            using (var conn = OpenDBConnection())
            {
                StringBuilder sql = new StringBuilder();
                sql.Length = 0;

                sql.AppendLine(" UPDATE dbo.WorkHistory                           ");
                sql.AppendLine("    SET Position   = @Position,                   ");
                sql.AppendLine("        CompanyName  = @CompanyName,              ");
                sql.AppendLine("        OrderIndex      = @OrderIndex,            ");
                sql.AppendLine("        StartDate      = @StartDate,              ");
                sql.AppendLine("        EndDate    = @EndDate,                    ");
                sql.AppendLine("        PersonId    = @PersonId,                  ");
                sql.AppendLine("        UpdatedAt    = @UpdatedAt,                ");
                sql.AppendLine("        UpdatedBy    = @UpdatedBy                 ");
                sql.AppendLine(" WHERE Id = @Id  AND Status = 1                   ");

                var param = new
                {
                    id = entity.Id,
                    position = entity.Position,
                    companyName = entity.CompanyName,
                    orderIndex = entity.OrderIndex,
                    startDate = entity.StartDate,
                    endDate = entity.EndDate,
                    personId = entity.PersonId,
                    updatedAt = entity.UpdatedAt,
                    updatedBy = entity.UpdatedBy
                };
                return await conn.ExecuteAsync(sql.ToString(), param);
            }
        }

        /// <summary>
        /// Delete WorkHistory
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<int> DeleteAsync(int id)
        {
            using (var conn = OpenDBConnection())
            {
                StringBuilder sql = new StringBuilder();
                sql.Length = 0;
                sql.AppendLine(" UPDATE WorkHistory               ");
                sql.AppendLine("    SET Status     = @Status,     ");
                sql.AppendLine("        UpdatedAt  = @UpdatedAt,  ");
                sql.AppendLine("        UpdatedBy  = @UpdatedBy   ");
                sql.AppendLine(" WHERE Id = @Id AND Status = 1    ");
                var param = new
                {
                    Status = 0,
                    UpdatedAt = DateTime.Now,
                    UpdatedBy = WebAPI.Helpers.HttpContext.CurrentUser,
                    Id = id
                };
                return await conn.ExecuteAsync(sql.ToString(), param);
            }
        }

    }
}
