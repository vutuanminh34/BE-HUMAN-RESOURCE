using Dapper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Models;

namespace WebAPI.Repositories.Persons
{
    public class UploadRepository : RepositoryBase, IUploadRepository
    {
        public UploadRepository(string connectionString) : base(connectionString) { }


        public Task<int> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<FileUpload> FindAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<FileUpload>> GetAllAsync()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Create Person
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public Task<int> InsertAsync(Person entity)
        {
            throw new NotImplementedException();
        }


        public Task<int> UpdateAsync(Person entity)
        {
            throw new NotImplementedException();
        }

        public Task<int> InsertAsync(FileUpload entity)
        {
            throw new NotImplementedException();
        }

        public Task<int> UpdateAsync(FileUpload entity)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Upload insert Image
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public async Task<int> UploadImage(string fileName, string path)
        {
            using (var conn = OpenDBConnection())
            {
                StringBuilder sql = new StringBuilder();
                sql.Length = 0;

                sql.AppendLine("    UPDATE dbo.Person        ");
                sql.AppendLine("    SET  Avatar = @Avatar   ");
                sql.AppendLine("    WHERE Id = (SELECT MAX(Person.Id) FROM Person)             ");

                var param = new
                {
                    avatar = fileName,
                };
                return await conn.ExecuteAsync(sql.ToString(), param);
            }
        }
        /// <summary>
        /// Update Image
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<int> UpdateImage(string fileName, int id)
        {
            using (var conn = OpenDBConnection())
            {
                StringBuilder sql = new StringBuilder();
                sql.Length = 0;

                sql.AppendLine("    UPDATE dbo.Person                                                                ");
                sql.AppendLine("    SET  Avatar = @Avatar                                                            ");
                sql.AppendLine("    WHERE Id = @Id and Status=1                                       ");

                var param = new
                {
                    Id = id,
                    avatar = fileName,
                };
                return await conn.ExecuteAsync(sql.ToString(), param);
            }
        }
    }
}
