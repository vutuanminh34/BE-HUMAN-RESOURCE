using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Models;

namespace WebAPI.Repositories.Technologies
{
    public class TechnologyRepository : RepositoryBase, ITechnologyRepository
    {
        public TechnologyRepository(string connectionString) : base(connectionString) { }

        /// <summary>
        /// Get Technology By Person
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Technology>> GetTechnologyByPersonAsync(int id)
        {
            using (var conn = OpenDBConnection())
            {
                StringBuilder sql = new StringBuilder();
                sql.Length = 0;
                sql.Append("SELECT TE.Id, TE.Name, TE.CreatedAt, CA.Id, CA.Name              ");
                sql.Append("FROM PersonTechnology AS PT                                      ");
                sql.Append("INNER JOIN Technology AS TE                                      ");
                sql.Append("ON PT.TechnologyId = TE.Id                                       ");
                sql.Append("INNER JOIN Category AS CA                                        ");
                sql.Append("ON CA.Id = TE.CategoryId                                         ");
                sql.Append("INNER JOIN PersonCategory AS PC                                  ");
                sql.Append("ON CA.Id = PC.CategoryId                                         ");
                sql.Append("WHERE PT.PersonId = @PersonId  AND PC.PersonId = PT.PersonId     ");
                sql.Append("AND PT.Status = 1 AND TE.Status = 1                              ");
                sql.Append("AND CA.Status = 1 AND PC.Status = 1                              ");
                sql.Append("ORDER BY TE.Id  DESC                                             ");
                var lookup = new Dictionary<int, Technology>();
                var param = new
                {
                    PersonId = id
                };
                await conn.QueryAsync<Technology, Category, Technology>(sql.ToString(), (m, n) =>
                {
                    Technology mode = new Technology();
                    if (!lookup.TryGetValue(m.Id, out mode))
                        lookup.Add(m.Id, mode = m);
                    if (mode.Category == null)
                        mode.Category = new Category();
                    mode.Category = n;
                    return mode;
                }, param);
                var models = lookup.Values.ToList();
                return models;
            }
        }

        /// <summary>
        /// Get Technology By Category
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Technology>> GetTechnologyByCategoryAsync(int id)
        {
            using (var conn = OpenDBConnection())
            {
                StringBuilder sql = new StringBuilder();
                sql.Length = 0;
                sql.Append("SELECT TE.ID, TE.NAME, TE.CategoryId, TE.CreatedAt, CA.Id, CA.Name    ");
                sql.Append("FROM Technology AS TE                                   ");
                sql.Append("INNER JOIN Category AS CA                               ");
                sql.Append("ON TE.CategoryID = CA.ID                                ");
                sql.Append("WHERE TE.Status = 1 AND CA.Id = @Id                     ");
                sql.Append("AND CA.Status = 1                                       ");
                sql.Append("ORDER BY TE.Id  DESC                                    ");
                var lookup = new Dictionary<int, Technology>();
                var param = new
                {
                    Id = id
                };
                await conn.QueryAsync<Technology, Category, Technology>(sql.ToString(), (m, n) =>
                {
                    Technology mode = new Technology();
                    if (!lookup.TryGetValue(m.Id, out mode))
                        lookup.Add(m.Id, mode = m);
                    if (mode.Category == null)
                        mode.Category = new Category();
                    mode.Category = n;
                    return mode;
                }, param);
                var models = lookup.Values.ToList();
                return models;
            }
        }

        /// <summary>
        /// Get Technology
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Technology>> GetAllTechnologyAsync(int pageSize, int offset, string name, int categoryId)
        {
            using (var conn = OpenDBConnection())
            {
                StringBuilder sql = new StringBuilder();
                sql.Length = 0;
                sql.Append("SELECT TE.ID, TE.NAME, TE.CategoryId, TE.CreatedAt, CA.Id, CA.Name    ");
                sql.Append("FROM Technology AS TE                                   ");
                sql.Append("INNER JOIN Category AS CA                               ");
                sql.Append("ON TE.CategoryID = CA.ID                                ");
                sql.Append("WHERE TE.Status = 1                                     ");
                sql.Append("AND CA.Status = 1                                       ");
                if (!String.IsNullOrEmpty(name))
                    sql.Append("AND TE.Name LIKE @Name                                  ");
                if (categoryId > 0)
                    sql.Append("AND TE.CategoryId = @CategoryId                         ");
                sql.Append("ORDER BY TE.Id  DESC                                        ");
                if (pageSize > 0 && offset > -1)
                {
                    sql.Append("    OFFSET @Offset ROWS                                 ");
                    sql.Append("    FETCH NEXT @PageSize ROWS ONLY                      ");
                }
                var lookup = new Dictionary<int, Technology>();
                var param = new
                {
                    PageSize = pageSize,
                    Offset = offset,
                    Name = (String.IsNullOrEmpty(name)) ? null : "%" + name.Trim() + "%",
                    CategoryId = categoryId
                };
                await conn.QueryAsync<Technology, Category, Technology>(sql.ToString(), (m, n) =>
                {
                    Technology mode = new Technology();
                    if (!lookup.TryGetValue(m.Id, out mode))
                        lookup.Add(m.Id, mode = m);
                    if (mode.Category == null)
                        mode.Category = new Category();
                    mode.Category = n;
                    return mode;
                }, param);
                var models = lookup.Values.ToList();
                return models;
            }
        }

        /// <summary>
        /// Check Technology
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<Technology> CheckTechnologyAsync(Technology technology)
        {
            using (var conn = OpenDBConnection())
            {
                StringBuilder sql = new StringBuilder();
                sql.Length = 0;
                sql.Append("SELECT *                           ");
                sql.Append("FROM Technology as TE                     ");
                sql.Append("WHERE TE.CategoryId = @CategoryId         ");
                sql.Append("AND TE.Name = @Name AND TE.Status = 1     ");
                if (technology.Id > 0)
                    sql.Append("AND TE.Id != @Id                          ");
                var param = new
                {
                    CategoryId = technology.CategoryId,
                    Name = (String.IsNullOrEmpty(technology.Name)) ? "" : technology.Name.Trim(),
                    Id = technology.Id
                };
                return await conn.QueryFirstOrDefaultAsync<Technology>(sql.ToString(), param);
            }
        }

        /// <summary>
        /// Create Technology
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<int> InsertAsync(Technology entity)
        {
            using (var conn = OpenDBConnection())
            {
                StringBuilder sql = new StringBuilder();
                sql.Length = 0;
                sql.Append("INSERT INTO                          ");
                sql.Append("       Technology(CreatedBy,         ");
                sql.Append("                  UpdatedBy,         ");
                sql.Append("                CreatedAt,           ");
                sql.Append("                UpdatedAt,           ");
                sql.Append("                  Name,              ");
                sql.Append("                  CategoryId)        ");
                sql.Append("OUTPUT INSERTED.ID                   ");
                sql.Append("VALUES (@CreatedBy,                  ");
                sql.Append("        @UpdatedBy,                  ");
                sql.Append("                @CreatedAt,          ");
                sql.Append("                @UpdatedAt,          ");
                sql.Append("        @Name,                       ");
                sql.Append("        @CategoryId)                 ");
                var param = new
                {
                    CreatedBy = entity.CreatedBy,
                    UpdatedBy = entity.UpdatedBy,
                    CreatedAt = entity.CreatedAt,
                    UpdatedAt = entity.UpdatedAt,
                    Name = (String.IsNullOrEmpty(entity.Name)) ? "" : entity.Name.Trim(),
                    CategoryId = entity.CategoryId
                };
                return await conn.ExecuteScalarAsync<int>(sql.ToString(), param);
            }
        }

        /// <summary>
        /// Update Technology
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<int> UpdateAsync(Technology entity)
        {
            using (var conn = OpenDBConnection())
            {
                StringBuilder sql = new StringBuilder();
                sql.Length = 0;
                sql.Append("UPDATE Technology                        ");
                sql.Append("      SET UpdatedAt   = @UpdatedAt,      ");
                sql.Append("          UpdatedBy   = @UpdatedBy,      ");
                sql.Append("          CreatedAt   = @CreatedAt,      ");
                sql.Append("          CreatedBy   = @CreatedBy,      ");
                sql.Append("          CategoryId  = @CategoryId,     ");
                sql.Append("          Name        = @Name,           ");
                sql.Append("          Status      = @Status          ");
                sql.Append("FROM Technology AS TE                    ");
                sql.Append("INNER JOIN Category AS CA                ");
                sql.Append("ON TE.CategoryId = CA.Id                 ");
                sql.Append("WHERE TE.Id = @Id                        ");
                sql.Append("AND CA.Status = 1                        ");
                entity.UpdatedAt = DateTime.Now;
                var param = new
                {
                    CreatedAt = entity.CreatedAt,
                    CreatedBy = entity.CreatedBy,
                    UpdatedAt = entity.UpdatedAt,
                    UpdatedBy = entity.UpdatedBy,
                    Name = (String.IsNullOrEmpty(entity.Name)) ? "" : entity.Name.Trim(),
                    CategoryId = entity.CategoryId,
                    Id = entity.Id,
                    Status = entity.Status
                };
                return await conn.ExecuteAsync(sql.ToString(), param);
            }
        }

        /// <summary>
        /// Get Technology By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Technology> FindAsync(int id)
        {
            using (var conn = OpenDBConnection())
            {
                StringBuilder sql = new StringBuilder();
                sql.Length = 0;
                sql.Append("SELECT TE.*, CA.*                                        ");
                sql.Append("FROM Technology AS TE                                    ");
                sql.Append("INNER JOIN Category AS CA                                ");
                sql.Append("ON TE.CategoryId = CA.Id                                 ");
                sql.Append("WHERE TE.Id = @Id                                        ");
                sql.Append("AND TE.Status = 1 AND CA.Status = 1                      ");
                var lookup = new Dictionary<int, Technology>();
                var param = new
                {
                    Id = id
                };
                await conn.QueryAsync<Technology, Category, Technology>(sql.ToString(), (m, n) =>
                {
                    Technology mode = new Technology();
                    if (!lookup.TryGetValue(m.Id, out mode))
                        lookup.Add(m.Id, mode = m);
                    if (mode.Category == null)
                        mode.Category = new Category();
                    mode.Category = n;
                    return mode;
                }, param);
                var models = lookup.Values.FirstOrDefault();
                return models;
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
                sql.Length = 0;
                sql.Append("SELECT COUNT(*)                                         ");
                sql.Append("FROM Technology AS TE                                   ");
                sql.Append("INNER JOIN Category AS CA                               ");
                sql.Append("ON TE.CategoryID = CA.ID                                ");
                sql.Append("WHERE TE.Status = 1                                     ");
                sql.Append("AND CA.Status = 1                                       ");
                if (!String.IsNullOrEmpty(name))
                    sql.Append("AND TE.Name LIKE @Name                                  ");
                var param = new
                {
                    Name = (String.IsNullOrEmpty(name)) ? "" : "%" + name.Trim() + "%"
                };
                return await conn.ExecuteScalarAsync<int>(sql.ToString(), param);
            }
        }

        /// <summary>
        /// Get Technology By PersonId And Name Technology
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<int> GetTechnologyByPersonAndNameAsync(int personId, string name)
        {
            using (var conn = OpenDBConnection())
            {
                StringBuilder sql = new StringBuilder();
                sql.Length = 0;
                sql.Append("SELECT TE.Id                                             ");
                sql.Append("FROM Technology AS TE                                   ");
                sql.Append("INNER JOIN PersonTechnology AS PT                       ");
                sql.Append("ON TE.Id = PT.TechnologyId                              ");
                sql.Append("WHERE PT.PersonId = @PersonId AND TE.Name = @Name       ");
                sql.Append("AND TE.Status = 1 AND PT.Status = 1                     ");
                var param = new
                {
                    PersonId = personId,
                    Name = (String.IsNullOrEmpty(name)) ? "" : name.Trim()
                };
                return await conn.ExecuteScalarAsync<int>(sql.ToString(), param);
            }
        }

        public Task<IEnumerable<Technology>> GetAllAsync()
        {
            throw new NotImplementedException();
        }
        public Task<int> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
