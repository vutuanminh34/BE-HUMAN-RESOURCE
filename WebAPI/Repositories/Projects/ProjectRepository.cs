using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Models;
using WebAPI.Helpers;

namespace WebAPI.Repositories.Projects
{
    public class ProjectRepository : RepositoryBase, IProjectRepository
    {
        public ProjectRepository(string connectionString) : base(connectionString)
        {

        }
        /// <summary>
        /// delete project by updating status = 0
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<int> DeleteAsync(int id)
        {
            using (var conn = OpenDBConnection())
            {
                StringBuilder sql = new StringBuilder();
                sql.Length = 0;

                sql.AppendLine("Update dbo.Project                            ");
                sql.AppendLine(" set   UpdatedBy = @UpdatedBy,                ");
                sql.AppendLine("       Status = 0                             ");
                sql.AppendLine("    where Id = @Id and Status = 1             ");

                var param = new
                {
                    UpdatedBy = HttpContext.CurrentUser,
                    Id = id
                };
                return await conn.ExecuteAsync(sql.ToString(), param);
            }
        }

        public async Task<Project> FindAsync(int id)
        {
            using (var conn = OpenDBConnection())
            {
                var sql = "Select * from dbo.Project where Id = @Id and Status = 1";
                var param = new { Id = id };
                return await conn.QueryFirstOrDefaultAsync<Project>(sql, param);
            }
        }

        public async Task<IEnumerable<Project>> GetAllAsync()
        {
            using (var conn = OpenDBConnection())
            {
                var sql = "Select * from dbo.Project where Status = 1";
                return await conn.QueryAsync<Project>(sql);
            }
        }
        /// <summary>
        /// get project by personId
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Project>> GetProjectByPersonIdAsync(int id)
        {
            using (var conn = OpenDBConnection())
            {
                StringBuilder sql = new StringBuilder();
                sql.Length = 0;

                sql.AppendLine("Select PJ.*, TLG.*                                       ");
                sql.AppendLine("from dbo.ProjectTechnology as PT                         ");
                sql.AppendLine("inner join dbo.Project as PJ                             ");
                sql.AppendLine("on PT.ProjectId = PJ.Id                                  ");
                sql.AppendLine("inner join dbo.Technology as TLG                         ");
                sql.AppendLine("on PT.TechnologyId = TLG.Id                              ");
                sql.AppendLine("where PJ.PersonId = @PersonId                            ");
                sql.AppendLine("and PJ.Status = 1 and PT.Status = 1 and TLG.Status = 1   ");
                sql.AppendLine("order by PJ.OrderIndex ASC                               ");

                var param = new { PersonId = id };
                var lookup = new Dictionary<int, Project>();

                await conn.QueryAsync<Project, Technology, Project>(sql.ToString(), (PJ, TLG) =>
                {
                    Project project;
                    if (!lookup.TryGetValue(PJ.Id, out project))
                    {
                        lookup.Add(PJ.Id, project = PJ);
                    }
                    if (project.Technologies == null)
                        project.Technologies = new List<Technology>();
                    project.Technologies.Add(TLG);
                    return project;
                }, param);

                var result = lookup.Values.ToList();
                return result;
            }
        }
        /// <summary>
        /// Count a number of project where ProjectId
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<int> CountProject(int Id)
        {
            using (var conn = OpenDBConnection())
            {
                var sql = "Select Count(*) from dbo.Project where PersonId = '" + Id + "' and Status = 1";
                return await conn.ExecuteScalarAsync<int>(sql);
            }
        }
        /// <summary>
        /// Get max orderIndex in project
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<int> GetMaxOrderIndex(int Id)
        {
            using (var conn = OpenDBConnection())
            {
                var sql = "Select Max(OrderIndex) from dbo.Project where PersonId = '" + Id + "'";
                return await conn.ExecuteScalarAsync<int>(sql);
            }
        }
        /// <summary>
        /// count person exist 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<int> CountPerson(int Id)
        {
            using (var conn = OpenDBConnection())
            {
                var sql = "Select Count(*) from dbo.Person where Id = '" + Id + "' and Status = 1";
                return await conn.ExecuteScalarAsync<int>(sql);
            }
        }

        public async Task<int> InsertAsync(Project entity)
        {
            int OrderIndex = 0;
            if (CountProject(entity.PersonId).Result <= 0)
            {
                OrderIndex = 1;
            }
            else
            {
                OrderIndex = GetMaxOrderIndex(entity.PersonId).Result + 1;
            }
            using (var conn = OpenDBConnection())
            {
                StringBuilder sql = new StringBuilder();
                sql.Length = 0;

                sql.AppendLine("  INSERT INTO              ");
                sql.AppendLine("       dbo.Project(        ");
                sql.AppendLine("       CreatedBy,          ");
                sql.AppendLine("       CreatedAt,          ");
                sql.AppendLine("       StartDate,          ");
                sql.AppendLine("       EndDate,            ");
                sql.AppendLine("       Name,               ");
                sql.AppendLine("       Description,        ");
                sql.AppendLine("       Position,           ");
                sql.AppendLine("       Responsibilities,   ");
                sql.AppendLine("       TeamSize,           ");
                sql.AppendLine("       OrderIndex,         ");
                sql.AppendLine("       PersonId)           ");
                sql.AppendLine("  VALUES(@CreatedBy,       ");
                sql.AppendLine("       @CreatedAt,         ");
                sql.AppendLine("       @StartDate,         ");
                sql.AppendLine("       @EndDate,           ");
                sql.AppendLine("       @Name,              ");
                sql.AppendLine("       @Description,       ");
                sql.AppendLine("       @Position,          ");
                sql.AppendLine("       @Responsibilities,  ");
                sql.AppendLine("       @TeamSize,          ");
                sql.AppendLine("       @OrderIndex,        ");
                sql.AppendLine("       @PersonId)          ");
                sql.AppendLine("  SELECT SCOPE_IDENTITY()  ");


                var param = new
                {
                    CreatedBy = entity.CreatedBy,
                    CreatedAt = entity.CreatedAt,
                    StartDate = entity.StartDate,
                    EndDate = entity.EndDate,
                    Name = entity.Name,
                    Description = entity.Description,
                    Position = entity.Position,
                    Responsibilities = entity.Responsibilities,
                    TeamSize = entity.TeamSize,
                    OrderIndex = OrderIndex,
                    PersonId = entity.PersonId
                };
                return await conn.ExecuteScalarAsync<int>(sql.ToString(), param);
            }
        }

        public async Task<int> UpdateAsync(Project entity)
        {
            using (var conn = OpenDBConnection())
            {
                StringBuilder sql = new StringBuilder();
                sql.Length = 0;

                sql.AppendLine("Update dbo.Project                            ");
                sql.AppendLine(" set   UpdatedBy = @UpdatedBy,                ");
                sql.AppendLine("       UpdatedAt = @UpdatedAt,                ");
                sql.AppendLine("       StartDate = @StartDate,                ");
                sql.AppendLine("       EndDate = @EndDate,                    ");
                sql.AppendLine("       Name = @Name,                          ");
                sql.AppendLine("       Description = @Description,            ");
                sql.AppendLine("       Position = @Position,                  ");
                sql.AppendLine("       Responsibilities = @Responsibilities,  ");
                sql.AppendLine("       TeamSize = @TeamSize,                  ");
                sql.AppendLine("       OrderIndex = @OrderIndex               ");
                sql.AppendLine("    where Id = @Id                            ");
                var param = new
                {
                    UpdatedBy = entity.UpdatedBy,
                    UpdatedAt = entity.UpdatedAt,
                    StartDate = entity.StartDate,
                    EndDate = entity.EndDate,
                    Name = entity.Name,
                    Description = entity.Description,
                    Position = entity.Position,
                    Responsibilities = entity.Responsibilities,
                    TeamSize = entity.TeamSize,
                    OrderIndex = entity.OrderIndex,
                    Id = entity.Id
                };


                return await conn.ExecuteAsync(sql.ToString(), param);
            }
        }

        public async Task<int> UpdateProjectWithIdAsync(Project entity)
        {
            using (var conn = OpenDBConnection())
            {
                StringBuilder sql = new StringBuilder();
                sql.Length = 0;

                sql.AppendLine("Update dbo.Project                            ");
                sql.AppendLine(" set   UpdatedBy = @UpdatedBy,                ");
                sql.AppendLine("       UpdatedAt = @UpdatedAt,                ");
                sql.AppendLine("       StartDate = @StartDate,                ");
                sql.AppendLine("       EndDate = @EndDate,                    ");
                sql.AppendLine("       Name = @Name,                          ");
                sql.AppendLine("       Description = @Description,            ");
                sql.AppendLine("       Position = @Position,                  ");
                sql.AppendLine("       Responsibilities = @Responsibilities,  ");
                sql.AppendLine("       TeamSize = @TeamSize                   ");
                sql.AppendLine("    where Id = @Id and Status = 1             ");
                var param = new
                {
                    UpdatedBy = entity.UpdatedBy,
                    UpdatedAt = entity.UpdatedAt,
                    StartDate = entity.StartDate,
                    EndDate = entity.EndDate,
                    Name = entity.Name,
                    Description = entity.Description,
                    Position = entity.Position,
                    Responsibilities = entity.Responsibilities,
                    TeamSize = entity.TeamSize,
                    Id = entity.Id
                };

                return await conn.ExecuteAsync(sql.ToString(), param);
            }
        }
    }
}
