using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Models;
using WebAPI.Helpers;

namespace WebAPI.Repositories.ProjectTechnologies
{
    public class ProjectTechnologyRepository : RepositoryBase, IProjectTechnologyRepository
    {
        public ProjectTechnologyRepository(string connectionString) : base(connectionString)
        {

        }
        public async Task<int> DeleteAsync(int id)
        {
            using (var conn = OpenDBConnection())
            {
                var sql = "Delete from dbo.ProjectTechnology WHERE ProjectId = @ProjectId";
                return await conn.ExecuteAsync(sql.ToString(), new { ProjectId = id });
            }
        }

        public Task<ProjectTechnology> FindAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ProjectTechnology>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<int> InsertAsync(ProjectTechnology entity)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// get projectId has been inserted lastest
        /// </summary>
        /// <returns></returns>
        public async Task<int> GetMaxId()
        {
            using(var conn = OpenDBConnection())
            {
                var sql = "select max(id) from dbo.Project";
                return await conn.ExecuteScalarAsync<int>(sql);
            }
        }

        /// <summary>
        /// insert value into ProjectTechnology table
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public async Task<int> InsertListTechnologyAsync(List<ProjectTechnology> entities)
        {
            var projectId = entities.First().ProjectId;
            for (int i = 0; i < entities.Count; i++)
            {
                entities[i].CreatedBy = HttpContext.CurrentUser;
                entities[i].CreatedAt = DateTime.Now;
                entities[i].UpdatedBy = HttpContext.CurrentUser;
                entities[i].UpdatedAt = DateTime.Now;
                entities[i].ProjectId = projectId;
            }

            using(var conn = OpenDBConnection())
            {
                var transaction = conn.BeginTransaction();
                StringBuilder sql = new StringBuilder();
                sql.Length = 0;
                sql.AppendLine("  INSERT INTO dbo.ProjectTechnology(CreatedBy, ");
                sql.AppendLine("       CreatedAt,                              ");
                sql.AppendLine("       UpdatedBy,                              ");
                sql.AppendLine("       UpdatedAt,                              ");
                sql.AppendLine("       ProjectId,                              ");
                sql.AppendLine("       TechnologyId)                           ");
                sql.AppendLine("  VALUES(@CreatedBy,                           ");
                sql.AppendLine("       @CreatedAt,                             ");
                sql.AppendLine("       @UpdatedBy,                             ");
                sql.AppendLine("       @UpdatedAt,                             ");
                sql.AppendLine("       @ProjectId,                             ");
                sql.AppendLine("       @TechnologyId)                          ");

                var result = await conn.ExecuteAsync(sql.ToString(), entities, transaction: transaction);
                transaction.Commit();
                return result;
            }
        }

        public Task<int> UpdateAsync(ProjectTechnology entity)
        {
            throw new NotImplementedException();
        }

        public async Task<int> UpdateListTechnologyAsync(int id, List<ProjectTechnology> entities)
        {
            await DeleteAsync(id);
            for (int i = 0; i < entities.Count; i++)
            {
                entities[i].CreatedBy = HttpContext.CurrentUser;
                entities[i].UpdatedBy = HttpContext.CurrentUser;
                entities[i].CreatedAt = DateTime.Now;
                entities[i].UpdatedAt = DateTime.Now;
                entities[i].ProjectId = id;
            }

            using (var conn = OpenDBConnection())
            {
                var transaction = conn.BeginTransaction();
                StringBuilder sql = new StringBuilder();
                sql.Length = 0;
                sql.AppendLine("  INSERT INTO dbo.ProjectTechnology(CreatedBy, ");
                sql.AppendLine("       CreatedAt,                              ");
                sql.AppendLine("       UpdatedBy,                              ");
                sql.AppendLine("       UpdatedAt,                              ");
                sql.AppendLine("       ProjectId,                              ");
                sql.AppendLine("       TechnologyId)                           ");
                sql.AppendLine("  VALUES(@CreatedBy,                           ");
                sql.AppendLine("       @CreatedAt,                             ");
                sql.AppendLine("       @UpdatedBy,                             ");
                sql.AppendLine("       @UpdatedAt,                             ");
                sql.AppendLine("       @ProjectId,                             ");
                sql.AppendLine("       @TechnologyId)                          ");
                var result = await conn.ExecuteAsync(sql.ToString(), entities, transaction: transaction);
                transaction.Commit();
                return result;
            }
        }

        public async Task<IEnumerable<Technology>> GetListTechnology(int id)
        {
            using(var conn = OpenDBConnection())
            {
                StringBuilder sql = new StringBuilder();
                sql.Length = 0;
                sql.AppendLine("select te.*                            ");
                sql.AppendLine("from ProjectTechnology as pt           ");
                sql.AppendLine("inner join Technology as te            ");
                sql.AppendLine("on pt.TechnologyId = te.Id             ");
                sql.AppendLine("where ProjectId = @Id                  ");
                return await conn.QueryAsync<Technology>(sql.ToString(), new { Id = id });
            }
        }

        public async Task<int> CountProject(int id)
        {
            using (var conn = OpenDBConnection())
            {
                var sql = "Select Count(*) from dbo.Project where Id = '" + id + "' and Status = 1";
                return await conn.ExecuteScalarAsync<int>(sql);
            }
        }

        public async Task<int> CountTechnology(int id)
        {
            using (var conn = OpenDBConnection())
            {
                var sql = "Select Count(*) from dbo.Technology where Id = '" + id + "' and Status = 1";
                return await conn.ExecuteScalarAsync<int>(sql);
            }
        }

        public async Task<int> GetPersonId(int id)
        {
            using (var conn = OpenDBConnection())
            {
                var sql = "Select PersonId from dbo.Project where Id = '" + id + "' ";
                return await conn.ExecuteScalarAsync<int>(sql);
            }
        }

        public async Task<int> CheckTechnologyInPerson(int PersonId, int TechnologyId)
        {
            using (var conn = OpenDBConnection())
            {
                var sql = "Select Count(*) from dbo.PersonTechnology where PersonId = '" + PersonId + "' and TechnologyId = '" + TechnologyId + "' ";
                return await conn.ExecuteScalarAsync<int>(sql);
            }
        }
    }
}
