using Dapper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Models;

namespace WebAPI.Repositories.Categories
{
    public class CategoryRepository : RepositoryBase, ICategoryRepository
    {
        public CategoryRepository(string connectionString) : base(connectionString) { }

        /// <summary>
        /// Get All Category
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="offset"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Category>> GetAllCategoryAsync(int pageSize, int offset, string name)
        {
            using (var conn = OpenDBConnection())
            {
                StringBuilder sql = new StringBuilder();
                sql.Length = 0;
                sql.Append("SELECT *                  ");
                sql.Append("FROM Category             ");
                sql.Append("WHERE Status = 1          ");
                if (!String.IsNullOrEmpty(name))
                    sql.Append("AND Name LIKE @Name       ");
                sql.Append("ORDER BY Id  DESC             ");
                if (pageSize > 0 && offset > -1)
                {
                    sql.Append("    OFFSET @Offset ROWS                                 ");
                    sql.Append("    FETCH NEXT @PageSize ROWS ONLY                      ");
                }
                var param = new
                {
                    PageSize = pageSize,
                    Offset = offset,
                    Name = (String.IsNullOrEmpty(name)) ? "" : "%" + name.Trim() + "%"
                };
                return await conn.QueryAsync<Category>(sql.ToString(), param);
            }
        }

        /// <summary>
        /// Check Category
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public async Task<Category> CheckCategoryAsync(Category category)
        {
            using (var conn = OpenDBConnection())
            {
                StringBuilder sql = new StringBuilder();
                sql.Length = 0;
                sql.Append("SELECT *                  ");
                sql.Append("FROM Category              ");
                sql.Append("WHERE Name = @Name         ");
                sql.Append("AND Status = 1             ");
                if (category.Id > 0)
                    sql.Append("AND Id != @Id             ");
                var param = new
                {
                    Name = (String.IsNullOrEmpty(category.Name)) ? "" : category.Name.Trim(),
                    Id = category.Id
                };
                return await conn.QueryFirstOrDefaultAsync<Category>(sql.ToString(), param);
            }
        }

        /// <summary>
        /// Get Category By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Category> FindAsync(int id)
        {
            using (var conn = OpenDBConnection())
            {
                StringBuilder sql = new StringBuilder();
                sql.Length = 0;
                sql.Append("SELECT *                  ");
                sql.Append("FROM Category             ");
                sql.Append("WHERE Id = @Id            ");
                sql.Append("AND Status = 1            ");
                Category result = await conn.QueryFirstOrDefaultAsync<Category>(sql.ToString(), new { Id = id });
                return result;
            }
        }

        /// <summary>
        /// Create Category
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<int> InsertAsync(Category entity)
        {
            using (var conn = OpenDBConnection())
            {
                StringBuilder sql = new StringBuilder();
                sql.Length = 0;
                sql.AppendLine(" INSERT INTO                       ");
                sql.AppendLine("      Category (CreatedBy,         ");
                sql.AppendLine("                UpdatedBy,         ");
                sql.AppendLine("                CreatedAt,         ");
                sql.AppendLine("                UpdatedAt,         ");
                sql.AppendLine("                Name )             ");
                sql.AppendLine("OUTPUT INSERTED.ID                 ");
                sql.AppendLine(" VALUES (@CreatedBy,               ");
                sql.AppendLine("         @UpdatedBy,               ");
                sql.AppendLine("         @CreatedAt,               ");
                sql.AppendLine("         @UpdatedAt,               ");
                sql.AppendLine("         @Name )                   ");
                var param = new
                {
                    CreatedBy = entity.CreatedBy,
                    UpdatedBy = entity.UpdatedBy,
                    CreatedAt = entity.CreatedAt,
                    UpdatedAt = entity.UpdatedAt,
                    Name = (String.IsNullOrEmpty(entity.Name)) ? "" : entity.Name.Trim()
                };
                return await conn.ExecuteScalarAsync<int>(sql.ToString(), param);
            }
        }

        /// <summary>
        /// Update Category
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<int> UpdateAsync(Category entity)
        {
            using (var conn = OpenDBConnection())
            {
                StringBuilder sql = new StringBuilder();
                sql.Length = 0;
                sql.AppendLine(" UPDATE Category                    ");
                sql.AppendLine("    SET Name       = @Name,         ");
                sql.AppendLine("        Status     = @Status,       ");
                sql.AppendLine("        CreatedBy  = @CreatedBy,    ");
                sql.AppendLine("        CreatedAt  = @CreatedAt,    ");
                sql.AppendLine("        UpdatedAt  = @UpdatedAt,    ");
                sql.AppendLine("        UpdatedBy  = @UpdatedBy     ");
                sql.AppendLine(" WHERE Id = @Id                     ");
                var param = new
                {
                    Name = (String.IsNullOrEmpty(entity.Name)) ? "" : entity.Name.Trim(),
                    Status = entity.Status,
                    CreatedBy = entity.CreatedBy,
                    CreatedAt = entity.CreatedAt,
                    UpdatedAt = entity.UpdatedAt,
                    UpdatedBy = entity.UpdatedBy,
                    Id = entity.Id
                };
                return await conn.ExecuteAsync(sql.ToString(), param);
            }
        }

        /// <summary>
        /// Get Total Count
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<int> GetTotalCount(string name)
        {
            using (var conn = OpenDBConnection())
            {
                StringBuilder sql = new StringBuilder();
                sql.AppendLine("SELECT COUNT(*) FROM Category WHERE Status = 1                      ");
                if (!String.IsNullOrEmpty(name))
                    sql.AppendLine("AND Name LIKE @Name                                             ");
                var param = new
                {
                    Name = (String.IsNullOrEmpty(name)) ? "" : "%" + name.Trim() + "%"
                };
                return await conn.ExecuteScalarAsync<int>(sql.ToString(), param);
            }
        }

        public Task<IEnumerable<Category>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<int> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
