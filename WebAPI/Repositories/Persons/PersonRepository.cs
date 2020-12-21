using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Models;

namespace WebAPI.Repositories.Persons
{
    public class PersonRepository : RepositoryBase, IPersonRepository
    {
        public PersonRepository(string connectionString) : base(connectionString) { }


        /// <summary>
        /// Get all Person
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Person>> GetAllAsync()
        {
            using (var conn = OpenDBConnection())
            {
                var sql = "SELECT * FROM dbo.Person where  Status = 1";
                return await conn.QueryAsync<Person>(sql);
            }
        }


        /// <summary>
        /// CHECK PERSON
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool CheckPerson(IDbConnection conn, int id)
        {
            StringBuilder sql = new StringBuilder();
            sql.Length = 0;
            sql.AppendLine("    SELECT SUM(CNT)                       ");
            sql.AppendLine("    FROM (                                        ");
            sql.AppendLine("        SELECT                                     ");
            sql.AppendLine("           COUNT(*) AS CNT                ");
            sql.AppendLine("        FROM dbo.Person                   ");
            sql.AppendLine("        WHERE Id = @Id                     ");
            sql.AppendLine("     )                                                  ");

            int count = conn.ExecuteScalar<int>(sql.ToString(), new { Id = id });
            return count > 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Person> FindAsync(int id)
        {
            using (var conn = OpenDBConnection())
            {
                var sql = "SELECT * FROM dbo.Person WHERE Id = @Id AND STATUS = 1";
                var param = new { Id = id };
                return await conn.QueryFirstOrDefaultAsync<Person>(sql.ToString(), param);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        public async Task<int> CheckPhoneExisting(string phone)
        {
            using (var conn = OpenDBConnection())
            {
                var sql = "SELECT count(*) FROM dbo.Person WHERE Phone = @Phone AND STATUS=1";
                var param = new { Phone = phone };

                var result = await conn.ExecuteScalarAsync<int>(sql, param);
                return result;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="phone"></param>
        /// <returns></returns>
        public async Task<int> CheckPhoneToUpdate(int id, string phone)
        {
            using (var conn = OpenDBConnection())
            {
                var sql = "SELECT count(*) FROM dbo.Person WHERE Phone = @Phone AND Id <> @Id AND STATUS=1";
                var param = new
                {
                    Phone = phone,
                    Id = id
                };

                var result = await conn.ExecuteScalarAsync<int>(sql, param);
                return result;
            }
        }
        /*
         * All method handling for person
         */
        #region Handling insert info Person
        /// <summary>
        /// Check email when insert person
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public async Task<int> CheckEmailExist(string email)
        {
            using (var conn = OpenDBConnection())
            {
                var sql = "SELECT count(*) FROM dbo.Person WHERE Email = @Email AND STATUS=1";
                var param = new { Email = email };

                var result = await conn.ExecuteScalarAsync<int>(sql, param);
                return result;
            }
        }


        public async Task<int> CheckEmailUpdate(int id, string email)
        {
            using (var conn = OpenDBConnection())
            {
                var sql = "SELECT count(*) FROM dbo.Person WHERE Email = @Email AND Id <> @Id AND STATUS=1";
                var param = new
                { 
                    Email = email,
                    Id = id
                };

                var result = await conn.ExecuteScalarAsync<int>(sql, param);
                return result;
            }
        }

        public async Task<int> AmountOfPerson()
        {
            using (var conn = OpenDBConnection())
            {
                var sql = "SELECT count(*) FROM Person";
                return await conn.ExecuteScalarAsync<int>(sql);
            }
        }

        public async Task<int> CheckPersonExisting(int id)
        {
            using (var conn = OpenDBConnection())
            {
                var sql = "SELECT count(*) FROM dbo.Person WHERE Id = @Id AND STATUS=1";
                var param = new { Id = id };

                var result = await conn.ExecuteScalarAsync<int>(sql, param);
                return result;
            }
        }

        public async Task<string> GetDescription(int id)
        {
            using (var conn = OpenDBConnection())
            {
                var sql = "SELECT Description FROM dbo.Person WHERE Id = @Id AND STATUS=1";
                var param = new { Id = id };

                string result = await conn.ExecuteScalarAsync<string>(sql, param);
                return result;
            }
        }


        public async Task<string> GetStaffIdPerson(int id)
        {
            using (var conn = OpenDBConnection())
            {
                var sql = "SELECT StaffId FROM dbo.Person where Id = @Id and Status=1";
                var param = new { Id = id };
                string staffId = await conn.ExecuteScalarAsync<string>(sql, param);
                return staffId;
            }
        }

        public async Task<int> GetMaxIdPerson()
        {
            using (var conn = OpenDBConnection())
            {
                var sql = "SELECT max(id) FROM Person";
                return await conn.ExecuteScalarAsync<int>(sql);
            }
        }

        public async Task<int> TotalPersonNotSkill()
        {
            using (var conn = OpenDBConnection())
            {
                var sql = "SELECT count(*) FROM dbo.Person WHERE  Status = 1";
                return await conn.ExecuteScalarAsync<int>(sql);
            }
        }

        public async Task<int> TotalPersonAndSkill(string fullName, int location, List<int> listTechnologyId)
        {
            using (var conn = OpenDBConnection())
            {
                StringBuilder sql = new StringBuilder();
                sql.Length = 0;

                sql.AppendLine("    SELECT pe.*, ca.* , te.*                                                                                                                                                                      ");
                sql.AppendLine("    FROM (SELECT Person.* FROM Person                      ");
                if (listTechnologyId.Count > 0)
                {
                    sql.AppendLine("    INNER JOIN PersonTechnology as pt                     ");
                    sql.AppendLine("    on Person.Id = pt.PersonId                    ");
                    sql.AppendLine("    INNER JOIN (Select * from technology where Id IN @listTechnologyId) as te                   ");
                    sql.AppendLine("    on pt.TechnologyId = te.Id                  ");
                }
                sql.AppendLine(" WHERE Person.Status=1                                   ");
                if (listTechnologyId.Count > 0)
                {
                    sql.AppendLine("    AND pt.Status=1       AND te.Status = 1                 ");
                }
                if (!String.IsNullOrEmpty(fullName))
                    sql.AppendLine("    AND FullName LIKE N'%" + fullName + "%' ");
                if (location >= 0)
                    sql.AppendLine("AND Location = @location   ");
                sql.AppendLine("    ) AS pe Left JOIN persontechnology AS pt                                                                                                                                              ");
                sql.AppendLine("    ON pe.id = pt.PersonId         AND pt.Status=1                                                                                                                                ");
                sql.AppendLine("    Left JOIN technology AS te                                                                                   ");
                sql.AppendLine("    ON te.id = pt.TechnologyId               AND te.Status = 1                                                                                                                 ");
                sql.AppendLine("    Left JOIN category AS ca                                                                                                                                                             ");
                sql.AppendLine("    ON ca.Id = te.CategoryId                  AND ca.Status=1                                                                                                                    ");

                var lookup = new Dictionary<int, Person>();
                var param = new
                {
                    location = location,
                    listTechnologyId = listTechnologyId
                };

                await conn.QueryAsync<Person, Category, Technology, Person>(sql.ToString(), map: (pe, ca, te) =>
                {
                    Person person;
                    if (!lookup.TryGetValue(pe.Id, out person))
                        lookup.Add(pe.Id, person = pe);

                    if (person.Categories == null)
                        person.Categories = new List<Category>();
                    int i = 0;

                    foreach (var item in person.Categories)
                    {
                        if (ca != null)
                        {
                            if (item.Id == ca.Id)
                                i++;
                        }
                    }

                    if (i <= 0 && ca != null)
                        person.Categories.Add(ca);
                    if (person.Categories.Count > 0)
                    {
                        foreach (var item in person.Categories)
                        {
                            if (item.Technologies == null)
                                item.Technologies = new List<Technology>();
                            if (te != null)
                            {
                                if (item.Id == te.CategoryId)
                                {
                                    item.Technologies.Add(te);
                                }
                            }

                        }
                    }

                    return person;
                }, param);
                var list = lookup.Values.ToList();
                return list.Count();
            }
        }
        #endregion


        /// <summary>
        /// Create Person
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<int> InsertAsync(Person entity)
        {
            using (var conn = OpenDBConnection())
            {
                StringBuilder sql = new StringBuilder();
                sql.Length = 0;

                sql.AppendLine(" INSERT INTO                             ");
                sql.AppendLine("      dbo.Person (StaffId,             ");
                sql.AppendLine("                    FullName,               ");
                sql.AppendLine("                    Email,                     ");
                sql.AppendLine("                    Location,                ");
                sql.AppendLine("                    Avatar,                   ");
                sql.AppendLine("                    Description,           ");
                sql.AppendLine("                    Phone,                   ");
                sql.AppendLine("                    YearOfBirth,           ");
                sql.AppendLine("                    Gender,                  ");
                sql.AppendLine("                    CreatedAt,             ");
                sql.AppendLine("                    CreatedBy,             ");
                sql.AppendLine("                    UpdatedAt,            ");
                sql.AppendLine("                    UpdatedBy)           ");
                sql.AppendLine("OUTPUT INSERTED.ID                 ");
                sql.AppendLine(" VALUES (@StaffId,                   ");
                sql.AppendLine("         @FullName,                     ");
                sql.AppendLine("         @Email,                            ");
                sql.AppendLine("         @Location,                       ");
                sql.AppendLine("         @Avatar,                          ");
                sql.AppendLine("         @Description,                  ");
                sql.AppendLine("         @Phone,                          ");
                sql.AppendLine("         @YearOfBirth,                  ");
                sql.AppendLine("         @Gender,                         ");
                sql.AppendLine("         getdate(),                          ");
                sql.AppendLine("         @CreatedBy,                    ");
                sql.AppendLine("         getdate(),                          ");
                sql.AppendLine("         @UpdatedBy)                   ");
                entity.Avatar = "";
                if (String.IsNullOrEmpty(entity.Description))
                    entity.Description = "";
                var param = new
                {
                    staffId = entity.StaffId,
                    fullName = entity.FullName,
                    email = entity.Email.ToString(),
                    location = entity.Location,
                    avatar = entity.Avatar,
                    description = entity.Description,
                    phone = entity.Phone,
                    yearOfBirth = entity.YearOfBirth.ToString(),
                    gender = entity.Gender,
                    createdBy = WebAPI.Helpers.HttpContext.CurrentUser,
                    updatedBy = entity.UpdatedBy
                };
                return await conn.ExecuteScalarAsync<int>(sql.ToString(), param);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<int> UpdateAsync(Person entity)
        {
            using (var conn = OpenDBConnection())
            {
                StringBuilder sql = new StringBuilder();
                sql.Length = 0;

                sql.AppendLine(" UPDATE dbo.Person                                       ");
                sql.AppendLine("      SET  FullName        = @FullName,            ");
                sql.AppendLine("              Email           = @Email,                      ");
                sql.AppendLine("              Location        = @Location,               ");
                sql.AppendLine("              Phone        = @Phone,                      ");
                sql.AppendLine("              YearOfBirth     = @YearOfBirth,        ");
                sql.AppendLine("              Gender          = @Gender,                ");
                sql.AppendLine("              UpdatedAt       = @UpdatedAt,        ");
                sql.AppendLine("              UpdatedBy       = @UpdatedBy        ");
                sql.AppendLine("        WHERE Id = @Id       and Status = 1      ");
                var param = new
                {
                    fullName = entity.FullName.ToString(),
                    email = entity.Email.ToString(),
                    location = entity.Location,
                    phone = entity.Phone,
                    yearOfBirth = Convert.ToDateTime(entity.YearOfBirth),
                    gender = entity.Gender,
                    updatedAt = DateTime.Now.ToString(),
                    updatedBy = WebAPI.Helpers.HttpContext.CurrentUser,
                    id = entity.Id
                };
                return await conn.ExecuteAsync(sql.ToString(), param);
            }
        }
        /// <summary>
        /// Update description
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<int> UpdateOverview(Person entity)
        {
            using (var conn = OpenDBConnection())
            {
                StringBuilder sql = new StringBuilder();
                sql.Length = 0;

                sql.AppendLine(" UPDATE dbo.Person                                         ");
                sql.AppendLine("      SET  Description        = @description,         ");
                sql.AppendLine("        updatedAt        = getdate(),                      ");
                sql.AppendLine("        updatedBy        = @updatedBy                ");
                sql.AppendLine("        WHERE Id = @id            and Status = 1   ");

                var param = new
                {
                    description = entity.Description,
                    updatedBy = WebAPI.Helpers.HttpContext.CurrentUser,
                    id = entity.Id
                };
                return await conn.ExecuteAsync(sql.ToString(), param);
            }
        }

        /// <summary>
        /// Delete Person
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<int> DeleteAsync(int id)
        {
            using (var conn = OpenDBConnection())
            {
                StringBuilder sql = new StringBuilder();
                sql.Length = 0;

                sql.AppendLine("    UPDATE dbo.Person                           ");
                sql.AppendLine("    SET UpdatedAt = @date,                   ");
                sql.AppendLine("           UpdatedBy = @UpdatedBy,        ");
                sql.AppendLine("           Status = 0                                    ");
                sql.AppendLine("    WHERE Id = @Id      and Status = 1   ");

                var param = new
                {
                    UpdatedBy = WebAPI.Helpers.HttpContext.CurrentUser,
                    date = DateTime.Now,
                    id = id
                };
                return await conn.ExecuteAsync(sql.ToString(), param);
            }
        }

        /// <summary>
        ///  Screen List CV and Search
        /// </summary>
        /// <param name="limit"></param>
        /// <param name="checkOffset"></param>
        /// <param name="fullName"></param>
        /// <param name="location"></param>
        /// <param name="listTechnologyId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Person>> SearchPersonAndSkillAsync(int limit, int checkOffset, string fullName, int location, List<int> listTechnologyId)
        {
            using (var conn = OpenDBConnection())
            {
                StringBuilder sql = new StringBuilder();
                sql.Length = 0;

                sql.AppendLine("    SELECT pe.*, ca.* , te.*                                                                                                                                                                      ");
                sql.AppendLine("    FROM (SELECT Person.* FROM Person                                                                                                    ");
                if (listTechnologyId.Count > 0)
                {
                    sql.AppendLine("    INNER JOIN PersonTechnology as pt                                                                                 ");
                    sql.AppendLine("    on Person.Id = pt.PersonId                                                                                                       ");
                    sql.AppendLine("    INNER JOIN (Select * from technology where Id IN @listTechnologyId) as te                   ");
                    sql.AppendLine("    on pt.TechnologyId = te.Id                                                                                                           ");
                }
                sql.AppendLine(" WHERE Person.Status=1                                                                                                                  ");
                if (listTechnologyId.Count > 0)
                {
                    sql.AppendLine("    AND pt.Status=1       AND te.Status = 1                                                                                          ");
                }
                if (!String.IsNullOrEmpty(fullName))
                    sql.AppendLine("    AND FullName LIKE N'%" + fullName + "%'                                                                                                   ");
                if (location >= 0)
                    sql.AppendLine("AND Location = @location                                                                                                                                 ");
                sql.AppendLine("     ORDER BY Person.Id DESC                                                                                                                                          ");
                sql.AppendLine("    	 OFFSET @Offset ROWS                                                                                    ");
                sql.AppendLine("     FETCH NEXT @PageSize ROWS ONLY  ");
                sql.AppendLine("    ) AS pe Left JOIN persontechnology AS pt                                                                                                                                              ");
                sql.AppendLine("    ON pe.id = pt.PersonId         AND pt.Status=1                                                                                                                                ");
                sql.AppendLine("    Left JOIN technology AS te                                                                                                                                   ");
                sql.AppendLine("    ON te.id = pt.TechnologyId               AND te.Status = 1                                                                                                                 ");
                sql.AppendLine("    Left JOIN category AS ca                                                                                                                                                             ");
                sql.AppendLine("    ON ca.Id = te.CategoryId                  AND ca.Status=1                                                                                                                    ");

                var lookup = new Dictionary<int, Person>();
                var param = new
                {
                    PageSize = limit,
                    Offset = checkOffset,
                    location = location,
                    listTechnologyId = listTechnologyId
                };

                await conn.QueryAsync<Person, Category, Technology, Person>(sql.ToString(), map: (pe, ca, te) =>
                {
                    Person person;
                    if (!lookup.TryGetValue(pe.Id, out person))
                        lookup.Add(pe.Id, person = pe);

                    if (person.Categories == null)
                        person.Categories = new List<Category>();
                    int i = 0;

                    foreach (var item in person.Categories)
                    {
                        if (ca != null)
                        {
                            if (item.Id == ca.Id)
                                i++;
                        }
                    }

                    if (i <= 0 && ca != null)
                        person.Categories.Add(ca);
                    if (person.Categories.Count > 0)
                    {
                        foreach (var item in person.Categories)
                        {
                            if (item.Technologies == null)
                                item.Technologies = new List<Technology>();
                            if (te != null)
                            {
                                if (item.Id == te.CategoryId)
                                {
                                    int y = 0;
                                    foreach (var index in item.Technologies)
                                    {
                                        if (index.Id == te.Id)
                                            y++;
                                    }
                                    if(y<=0)
                                      item.Technologies.Add(te);
                                }
                            }

                        }
                    }
                    return person;
                }, param);
                var list = lookup.Values.ToList();
                return list;
            }
        }
    }
}

