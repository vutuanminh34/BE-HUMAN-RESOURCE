using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Models;
using WebAPI.RequestModel;

namespace WebAPI.Repositories.Skills
{
    public class SkillRepository : RepositoryBase, ISkillRepository
    {
        public SkillRepository(string connectionString) : base(connectionString) { }

        /// <summary>
        /// Get Skill Insert
        /// </summary>
        /// <param name="id"></param>
        /// <param name="idPerson"></param>
        /// <returns></returns>
        public async Task<Category> GetSkillInsertAsync(int id, int idPerson)
        {
            using (var conn = OpenDBConnection())
            {
                StringBuilder sql = new StringBuilder();
                sql.Length = 0;
                sql.AppendLine("SELECT CA.Id, CA.Name, TE.Id, TE.Name, TE.CategoryId,                        ");
                sql.AppendLine("PC.ID, PC.PersonId, PC.CategoryId, PC.OrderIndex                             ");
                sql.AppendLine("FROM PersonTechnology AS PT                                                  ");
                sql.AppendLine("INNER JOIN Technology AS TE                                                  ");
                sql.AppendLine("ON PT.TechnologyId = TE.Id                                                   ");
                sql.AppendLine("INNER JOIN Category AS CA                                                    ");
                sql.AppendLine("ON CA.Id = TE.CategoryId                                                     ");
                sql.AppendLine("INNER JOIN PersonCategory AS PC                                              ");
                sql.AppendLine("ON PC.CategoryId = TE.CategoryId                                             ");
                sql.AppendLine("WHERE  PC.Id = @Id AND  PT.PersonId = @PersonId                              ");
                sql.AppendLine("       AND PT.Status = 1 AND TE.Status = 1                                   ");
                sql.AppendLine("       AND CA.Status = 1 AND PC.Status = 1                                   ");
                sql.AppendLine("GROUP BY CA.Id, CA.Name, TE.Id, TE.Name, TE.CategoryId,                      ");
                sql.AppendLine("PC.ID, PC.PersonId, PC.CategoryId, PC.OrderIndex                             ");
                sql.AppendLine("ORDER BY PC.OrderIndex                                                       ");
                var lookup = new Dictionary<int, Category>();
                var param = new { Id = id, PersonId = idPerson };
                await conn.QueryAsync<Category, Technology, PersonCategory, Category>(sql.ToString(), (m, n, g) =>
                {
                    Category movie = new Category();
                    if (!lookup.TryGetValue(m.Id, out movie))
                        lookup.Add(m.Id, movie = m);
                    if (movie.Technologies == null)
                        movie.Technologies = new List<Technology>();
                    int y = 0;
                    foreach (var item in movie.Technologies)
                    {
                        if (n.Id == item.Id)
                            y++;
                    }
                    if (y <= 0)
                        movie.Technologies.Add(n);

                    if (movie.PersonCategories == null)
                        movie.PersonCategories = new List<PersonCategory>();
                    int x = 0;
                    foreach (var item in movie.PersonCategories)
                    {
                        if (g.Id == item.Id || g.CategoryId == item.CategoryId)
                            x++;
                    }
                    movie.PersonCategories.Add(g);
                    return movie;
                }, param);
                var movies = lookup.Values.FirstOrDefault();
                return movies;
            }
        }

        /// <summary>
        /// Get Skill By PersonId
        /// </summary>
        /// <param name="personId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Category>> GetSkillByPersonAsync(int personId)
        {
            using (var conn = OpenDBConnection())
            {

                StringBuilder sql = new StringBuilder();
                sql.Length = 0;
                sql.AppendLine("SELECT CA.Id, CA.Name, TE.Id, TE.Name, TE.CategoryId,                        ");
                sql.AppendLine("PC.ID, PC.PersonId, PC.CategoryId, PC.OrderIndex                             ");
                sql.AppendLine("FROM PersonTechnology AS PT                                                  ");
                sql.AppendLine("INNER JOIN Technology AS TE                                                  ");
                sql.AppendLine("ON PT.TechnologyId = TE.Id                                                   ");
                sql.AppendLine("INNER JOIN Category AS CA                                                    ");
                sql.AppendLine("ON CA.Id = TE.CategoryId                                                     ");
                sql.AppendLine("INNER JOIN PersonCategory AS PC                                              ");
                sql.AppendLine("ON PC.CategoryId = TE.CategoryId                                             ");
                sql.AppendLine("WHERE PT.PersonId = @PersonId AND PC.PersonId = @PersonId                    ");
                sql.AppendLine("       AND PT.Status = 1 AND TE.Status = 1                                   ");
                sql.AppendLine("       AND CA.Status = 1 AND PC.Status = 1                                   ");
                sql.AppendLine("ORDER BY PC.OrderIndex DESC                                                  ");
                var lookup = new Dictionary<int, Category>();
                var param = new { personId = personId };
                await conn.QueryAsync<Category, Technology, PersonCategory, Category>(sql.ToString(), (m, n, g) =>
                {
                    Category movie = new Category();
                    if (!lookup.TryGetValue(m.Id, out movie))
                        lookup.Add(m.Id, movie = m);
                    if (movie.Technologies == null)
                        movie.Technologies = new List<Technology>();
                    int y = 0;
                    foreach (var item in movie.Technologies)
                    {
                        if (n.Id == item.Id)
                            y++;
                    }
                    if (y <= 0)
                        movie.Technologies.Add(n);

                    if (movie.PersonCategories == null)
                        movie.PersonCategories = new List<PersonCategory>();
                    int x = 0;
                    foreach (var item in movie.PersonCategories)
                    {
                        if (g.Id == item.Id || g.CategoryId == item.CategoryId)
                            x++;
                    }
                    movie.PersonCategories.Add(g);
                    return movie;
                }, param);
                var movies = lookup.Values.ToList();
                return movies;
            }
        }

        /// <summary>
        /// Check Number Item PersonCategory
        /// </summary>
        /// <returns></returns>
        public async Task<int> GetNumberItemPersonCategory()
        {
            using (var conn = OpenDBConnection())
            {
                StringBuilder sql = new StringBuilder();
                sql.Length = 0;
                sql.AppendLine("SELECT COUNT(*)                                 ");
                sql.AppendLine("FROM PersonCategory AS PC                       ");
                sql.AppendLine("INNER JOIN Category AS CA                       ");
                sql.AppendLine("ON CA.Id = PC.CategoryId                        ");
                sql.AppendLine("WHERE PC.Status = 1 AND CA.Status = 1           ");
                return await conn.ExecuteScalarAsync<int>(sql.ToString());
            }
        }

        /// <summary>
        /// Get Max OrderIndex
        /// </summary>
        /// <returns></returns>
        public async Task<int> GetMaxOrderIndex()
        {
            using (var conn = OpenDBConnection())
            {
                var sql = "SELECT MAX(OrderIndex) FROM PersonCategory";
                return await conn.ExecuteScalarAsync<int>(sql);
            }
        }

        /// <summary>
        /// Check PersonCategory
        /// </summary>
        /// <param name="personCategory"></param>
        /// <returns></returns>
        public async Task<bool> CheckPersonCategoryAsync(PersonCategory personCategory)
        {
            using (var conn = OpenDBConnection())
            {
                StringBuilder sql = new StringBuilder();
                sql.Length = 0;
                sql.Append("SELECT COUNT(*)                                  ");
                sql.Append("FROM PersonCategory AS PC                        ");
                sql.Append("INNER JOIN Category AS CA                        ");
                sql.Append("ON PC.CategoryId = CA.Id                         ");
                sql.Append("WHERE PC.PersonId = @PersonId                    ");
                sql.Append("      AND PC.CategoryId = @CategoryId            ");
                sql.Append("      AND PC.Status = 1 AND CA.Status = 1        ");
                if (personCategory.Id > 0)
                    sql.Append("      AND PC.Id != @Id                  ");
                var param = new
                {
                    PersonId = personCategory.PersonId,
                    CategoryId = personCategory.CategoryId,
                    Id = personCategory.Id
                };
                int count = await conn.ExecuteScalarAsync<int>(sql.ToString(), param);
                return count > 0;
            }
        }

        /// <summary>
        /// Check PersonCategory On Delete
        /// </summary>
        /// <param name="personCategory"></param>
        /// <returns></returns>
        public async Task<PersonCategory> CheckPersonCategoryOnDeleteAsync(PersonCategory personCategory)
        {
            using (var conn = OpenDBConnection())
            {
                StringBuilder sql = new StringBuilder();
                sql.Length = 0;
                sql.Append("SELECT PC.*                                   ");
                sql.Append("FROM PersonCategory AS PC                     ");
                sql.Append("INNER JOIN Category AS CA                     ");
                sql.Append("ON PC.CategoryId = CA.Id                      ");
                sql.Append("WHERE PersonId = @PersonId                    ");
                sql.Append("AND CategoryId = @CategoryId                  ");
                sql.Append("AND PC.Status = 0 AND CA.Status = 1           ");
                var param = new
                {
                    PersonId = personCategory.PersonId,
                    CategoryId = personCategory.CategoryId
                };
                return await conn.QueryFirstOrDefaultAsync<PersonCategory>(sql.ToString(), param);
            }
        }

        /// <summary>
        /// Update PersonCategory to Insert
        /// </summary>
        /// <param name="personCategory"></param>
        /// <returns></returns>
        public async Task<int> UpdatePersonCategoryToInsertAsync(PersonCategory personCategory)
        {
            using (var conn = OpenDBConnection())
            {
                StringBuilder sql = new StringBuilder();
                sql.Length = 0;
                sql.Append("UPDATE PersonCategory                ");
                sql.Append("SET Status      = 1,                 ");
                sql.Append("OrderIndex      = @OrderIndex,       ");
                sql.Append("UpdatedBy       = @UpdatedBy,        ");
                sql.Append("UpdatedAt       = @UpdatedAt         ");
                sql.Append("FROM PersonCategory AS PC            ");
                sql.Append("INNER JOIN Category AS CA            ");
                sql.Append("ON PC.CategoryId = CA.Id             ");
                sql.Append("WHERE PC.PersonId  = @PersonId       ");
                sql.Append("AND PC.CategoryId  = @CategoryId     ");
                sql.Append("AND CA.Status = 1                    ");
                var param = new
                {
                    OrderIndex = personCategory.OrderIndex,
                    UpdatedBy = personCategory.UpdatedBy,
                    UpdatedAt = personCategory.UpdatedAt,
                    PersonId = personCategory.PersonId,
                    CategoryId = personCategory.CategoryId,
                };
                return await conn.ExecuteAsync(sql.ToString(), param);
            }
        }

        /// <summary>
        /// Insert PersonCategory
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<int> InsertPersonCategoryAsync(PersonCategory entity)
        {
            using (var conn = OpenDBConnection())
            {
                StringBuilder sql = new StringBuilder();
                sql.Length = 0;
                sql.Append("INSERT INTO                                ");
                sql.Append("        PersonCategory (PersonId,          ");
                sql.Append("                        CategoryId,        ");
                sql.Append("                        OrderIndex,        ");
                sql.Append("                         CreatedAt,        ");
                sql.Append("                        UpdatedAt,         ");
                sql.Append("                        CreatedBy,         ");
                sql.Append("                        UpdatedBy)         ");
                sql.Append("OUTPUT INSERTED.ID                         ");
                sql.Append("VALUES (@PersonId,                         ");
                sql.Append("        @CategoryId,                       ");
                sql.Append("        @OrderIndex,                       ");
                sql.Append("        @CreatedAt,                        ");
                sql.Append("        @UpdatedAt,                        ");
                sql.Append("        @CreatedBy,                        ");
                sql.Append("        @UpdatedBy)                        ");
                var param = new
                {
                    PersonId = entity.PersonId,
                    CategoryId = entity.CategoryId,
                    OrderIndex = entity.OrderIndex,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    CreatedBy = entity.CreatedBy,
                    UpdatedBy = entity.UpdatedBy
                };
                return await conn.ExecuteScalarAsync<int>(sql.ToString(), param);
            }
        }


        /// <summary>
        /// Check PersonTechnology On Delete
        /// </summary>
        /// <param name="personTechnology"></param>
        /// <returns></returns>
        public async Task<bool> CheckPersonTechnologyOnDeleteAsync(PersonTechnology personTechnology)
        {
            using (var conn = OpenDBConnection())
            {
                StringBuilder sql = new StringBuilder();
                sql.Length = 0;
                sql.Append("SELECT COUNT(*)                                       ");
                sql.Append("FROM PersonTechnology AS PT                           ");
                sql.Append("INNER JOIN Technology AS TE                           ");
                sql.Append("ON PT.TechnologyId = TE.Id                            ");
                sql.Append("WHERE PT.Status = 0 AND PersonId = @PersonId          ");
                sql.Append("AND TechnologyId = @TechnologyId                      ");
                sql.Append("AND TE.Status = 1                                     ");
                var param = new
                {
                    PersonId = personTechnology.PersonId,
                    TechnologyId = personTechnology.TechnologyId
                };
                var count = await conn.ExecuteScalarAsync<int>(sql.ToString(), param);
                return count > 0;
            }
        }

        /// <summary>
        /// Check PersonTechnology
        /// </summary>
        /// <param name="personTechnology"></param>
        /// <returns></returns>
        public async Task<bool> CheckPersonTechnologyAsync(PersonTechnology personTechnology)
        {
            using (var conn = OpenDBConnection())
            {
                StringBuilder sql = new StringBuilder();
                sql.Length = 0;
                sql.Append("SELECT COUNT(*)                                                 ");
                sql.Append("FROM PersonTechnology AS PT                                     ");
                sql.Append("INNER JOIN Technology AS TE                                     ");
                sql.Append("ON PT.TechnologyId = TE.Id                                      ");
                sql.Append("WHERE PT.PersonId = @PersonId                                   ");
                sql.Append("AND PT.TechnologyId = @TechnologyId                             ");
                sql.Append("AND PT.Status = 1 AND TE.Status = 1                             ");
                var param = new
                {
                    PersonId = personTechnology.PersonId,
                    TechnologyId = personTechnology.TechnologyId
                };
                var count = await conn.ExecuteScalarAsync<int>(sql.ToString(), param);
                return count > 0;
            }
        }

        /// <summary>
        /// Update PersonTechnology to Insert
        /// </summary>
        /// <param name="personTechnologies"></param>
        /// <returns></returns>
        public async Task<int> UpdatePersonTechnologyToInsertAsync(List<PersonTechnology> personTechnologies)
        {
            using (var conn = OpenDBConnection())
            {
                var trans = conn.BeginTransaction();
                StringBuilder sql = new StringBuilder();
                sql.Length = 0;
                sql.Append("UPDATE PersonTechnology                  ");
                sql.Append("SET Status        = 1,                   ");
                sql.Append("UpdatedBy         = @UpdatedBy,          ");
                sql.Append("UpdatedAt         = @UpdatedAt           ");
                sql.Append("FROM PersonTechnology AS PT              ");
                sql.Append("INNER JOIN Technology AS TE              ");
                sql.Append("ON PT.TechnologyId = TE.Id               ");
                sql.Append("WHERE PT.PersonId    = @PersonId         ");
                sql.Append("AND PT.TechnologyId  = @TechnologyId     ");
                sql.Append("AND TE.Status = 1                        ");
                sql.Append("AND PT.Status = 0                        ");
                var result = await conn.ExecuteAsync(sql.ToString(), personTechnologies, transaction: trans);
                trans.Commit();
                return result;
            }
        }

        /// <summary>
        /// Create List PersonTechnology
        /// </summary>
        /// <param name="personTechnologies"></param>
        /// <returns></returns>
        public async Task<int> InsertListPersonTechnologyAsync(List<PersonTechnology> personTechnologies)
        {
            using (var conn = OpenDBConnection())
            {
                var trans = conn.BeginTransaction();
                StringBuilder sql = new StringBuilder();
                sql.Length = 0;
                sql.Append("INSERT INTO ");
                sql.Append("        PersonTechnology (CreatedBy,            ");
                sql.Append("                          UpdatedBy,            ");
                sql.Append("                          CreatedAt,             ");
                sql.Append("                          UpdatedAt,            ");
                sql.Append("                          PersonId,             ");
                sql.Append("                          TechnologyId)         ");
                sql.Append("VALUES (@CreatedBy,                             ");
                sql.Append("        @UpdatedBy,                             ");
                sql.Append("        @CreatedAt,                              ");
                sql.Append("        @UpdatedAt,                             ");
                sql.Append("        @PersonId,                              ");
                sql.Append("        @TechnologyId)                          ");
                var result = await conn.ExecuteAsync(sql.ToString(), personTechnologies, transaction: trans);
                trans.Commit();
                return result;
            }
        }

        /// <summary>
        /// Delete PersonTechnology to Update
        /// </summary>
        /// <param name="personCategory"></param>
        /// <returns></returns>
        public async Task<int> DeletePersonTechnologyToUpdateAsync(PersonCategory personCategory)
        {
            using (var conn = OpenDBConnection())
            {
                StringBuilder sql = new StringBuilder();
                sql.Length = 0;
                sql.Append("DELETE PT                                                     ");
                sql.Append("FROM PersonTechnology  AS PT                                  ");
                sql.Append("INNER JOIN Technology AS TE                                   ");
                sql.Append("ON PT.TechnologyId = TE.Id                                    ");
                sql.Append("WHERE PersonId = @PersonId AND TE.CategoryId = @CategoryId    ");
                var param = new
                {
                    PersonId = personCategory.PersonId,
                    CategoryId = personCategory.CategoryId
                };
                return await conn.ExecuteAsync(sql.ToString(), param);
            }
        }

        /// <summary>
        /// Delete PersonCategory
        /// </summary>
        /// <param name="personCategory"></param>
        /// <returns></returns>
        public async Task<int> DeletePersonCategoryAsync(PersonCategory personCategory)
        {
            using (var conn = OpenDBConnection())
            {
                StringBuilder sql = new StringBuilder();
                sql.Length = 0;
                sql.Append("UPDATE PersonCategory                       ");
                sql.Append("SET Status      = 0,                        ");
                sql.Append("UpdatedBy       = @UpdatedBy,               ");
                sql.Append("UpdatedAt       = @UpdatedAt                ");
                sql.Append("FROM PersonCategory AS PC                   ");
                sql.Append("INNER JOIN Category AS CA                   ");
                sql.Append("ON PC.CategoryId = CA.Id                    ");
                sql.Append("WHERE PC.PersonId  = @PersonId              ");
                sql.Append("AND PC.CategoryId  = @CategoryId            ");
                sql.Append("AND PC.Status = 1 AND CA.Status = 1         ");
                var param = new
                {
                    UpdatedBy = personCategory.UpdatedBy,
                    UpdatedAt = personCategory.UpdatedAt,
                    PersonId = personCategory.PersonId,
                    CategoryId = personCategory.CategoryId,
                };
                return await conn.ExecuteAsync(sql.ToString(), param);
            }
        }

        /// <summary>
        /// Delete PersonTechnology
        /// </summary>
        /// <param name="personCategory"></param>
        /// <returns></returns>
        public async Task<int> DeletePersonTechnologyAsync(PersonCategory personCategory)
        {
            using (var conn = OpenDBConnection())
            {
                StringBuilder sql = new StringBuilder();
                sql.Length = 0;
                sql.Append("UPDATE PersonTechnology                   ");
                sql.Append("SET Status        = 0,                    ");
                sql.Append("UpdatedBy         = @UpdatedBy,           ");
                sql.Append("UpdatedAt         = @UpdatedAt            ");
                sql.Append("FROM PersonTechnology AS PT               ");
                sql.Append("INNER JOIN Technology AS TE               ");
                sql.Append("ON PT.TechnologyId = TE.Id                ");
                sql.Append("WHERE PT.PersonId    = @PersonId          ");
                sql.Append("AND TE.CategoryId = @CategoryId           ");
                sql.Append("ANd PT.Status = 1                         ");
                var param = new
                {
                    UpdatedBy = personCategory.UpdatedBy,
                    UpdatedAt = personCategory.UpdatedAt,
                    PersonId = personCategory.PersonId,
                    CategoryId = personCategory.CategoryId
                };
                var result = await conn.ExecuteAsync(sql.ToString(), param);
                return result;
            }
        }

        /// <summary>
        /// Get Skill By Category
        /// </summary>
        /// <param name="personCategory"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Technology>> GetSkilByCategoryAsync(PersonCategory personCategory)
        {
            using (var conn = OpenDBConnection())
            {
                StringBuilder sql = new StringBuilder();
                sql.Length = 0;
                sql.Append("SELECT TE.Id, TE.Name, TE.CategoryId, CA.Id, Ca.Name       ");
                sql.Append("FROM Technology AS TE                                      ");
                sql.Append("INNER JOIN PersonTechnology AS PT                          ");
                sql.Append("ON TE.Id = PT.TechnologyId                                 ");
                sql.Append("INNER JOIN Category AS CA                                  ");
                sql.Append("ON CA.Id = Te.CategoryId                                   ");
                sql.Append("WHERE PT.PersonId = @PersonId                              ");
                sql.Append("AND TE.CategoryId = @CategoryId                            ");
                sql.Append("AND TE.Status = 1 AND PT.Status = 1                        ");
                sql.Append("GROUP BY TE.Id, TE.Name, TE.CategoryId, CA.Id, Ca.Name     ");
                var lookup = new Dictionary<int, Technology>();
                var param = new
                {
                    PersonId = personCategory.PersonId,
                    CategoryId = personCategory.CategoryId,
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
        /// Update OrderIndex PersonCateogry
        /// </summary>
        /// <param name="personCategories"></param>
        /// <returns></returns>
        public async Task<int> UpdateOrderIndexPersonCategory(List<PersonCategory> personCategories)
        {
            using (var conn = OpenDBConnection())
            {
                var trans = conn.BeginTransaction();
                StringBuilder sql = new StringBuilder();
                sql.Length = 0;
                sql.Append("UPDATE PersonCategory                ");
                sql.Append("SET OrderIndex  = @OrderIndex,       ");
                sql.Append("UpdatedBy       = @UpdatedBy,        ");
                sql.Append("UpdatedAt       = @UpdatedAt         ");
                sql.Append("FROM PersonCategory AS PC            ");
                sql.Append("INNER JOIN Category AS CA            ");
                sql.Append("ON PC.CategoryId = CA.Id             ");
                sql.Append("WHERE PC.Id        = @Id             ");
                sql.Append("AND PC.Status = 1 AND CA.Status = 1  ");
                var result = await conn.ExecuteAsync(sql.ToString(), personCategories, transaction: trans);
                trans.Commit();
                return result;
            }
        }

        /// <summary>
        /// Get Person Category
        /// </summary>
        /// <param name="id"></param>
        /// <param name="personId"></param>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public async Task<PersonCategory> GetPersonCategoryAsync(int? id, int? personId, int? categoryId)
        {
            using (var conn = OpenDBConnection())
            {
                StringBuilder sql = new StringBuilder();
                sql.Length = 0;
                sql.Append("SELECT PC.*                            ");
                sql.Append("FROM PersonCategory AS PC              ");
                sql.Append("INNER JOIN Category AS CA              ");
                sql.Append("ON PC.CategoryId = CA.Id               ");
                sql.Append("WHERE PC.Status = 1 AND CA.Status = 1  ");
                if (id != null)
                    sql.Append("AND PC.Id = @Id                  ");
                else
                    sql.Append("AND PC.CategoryId = @CategoryId AND PC.PersonId = @PersonId              ");
                var param = new
                {
                    ID = id,
                    CategoryId = categoryId,
                    PersonId = personId
                };
                return await conn.QueryFirstOrDefaultAsync<PersonCategory>(sql.ToString(), param);
            }
        }

        /// <summary>
        /// Check number person category
        /// </summary>
        /// <param name="personCategory"></param>
        /// <returns></returns>
        public async Task<bool> CheckNumberTechnology(PersonCategory personCategory)
        {
            using (var conn = OpenDBConnection())
            {
                StringBuilder sql = new StringBuilder();
                sql.Length = 0;
                sql.Append("SELECT COUNT(PT.Id)                                  ");
                sql.Append("FROM PersonTechnology AS PT                          ");
                sql.Append("INNER JOIN Technology AS TE                          ");
                sql.Append("ON PT.TechnologyId = TE.Id                           ");
                sql.Append("WHERE PT.Status = 1 AND TE.Status = 1                ");
                sql.Append("AND TE.CategoryId = @CategoryId                      ");
                sql.Append("AND PT.PersonId = @PersonId                          ");
                var param = new
                {
                    CategoryId = personCategory.CategoryId,
                    PersonId = personCategory.PersonId
                };
                var result = await conn.ExecuteScalarAsync<int>(sql.ToString(), param);
                return result > 0;
            }
        }
    }
}
