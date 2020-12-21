using System.Data;
using System.Transactions;

namespace WebAPI.Repositories
{
    public class RepositoryBase 
    {
        private DBConnector DBConnector { get; set; }
        public RepositoryBase(string connectionString) : this(new DBConnector(connectionString))
        {
        }

        public RepositoryBase(DBConnector conn)
        {
            this.DBConnector = conn;
        }

        /// <summary>
        /// Open connection
        /// </summary>
        /// <returns></returns>
        protected IDbConnection OpenDBConnection()
        {
            DbOpen();
            return DBConnection;
        }

        /// <summary>
        /// Get connectiion
        /// </summary>
        protected IDbConnection DBConnection
        {
            get
            {
                return DBConnector.Connection;
            }
        }

        /// <summary>
        /// State connection
        /// </summary>
        protected bool DbOpenState
        {
            get
            {
                return DBConnector.OpenState;
            }
        }

        /// <summary>
        /// Open connection
        /// </summary>
        protected virtual void DbOpen()
        {
            DBConnector.Open();
        }

        /// <summary>
        /// Close connection
        /// </summary>
        protected virtual void DbClose()
        {
            if (DBConnector != null && DBConnector.OpenState)
                DBConnector.Close();
        }

        /// <summary>
        /// Get transactiion
        /// </summary>
        protected virtual IDbTransaction DbTransaction
        {
            get
            {
                return DBConnector.Transaction;
            }
        }

        /// <summary>
        /// Begin transaction
        /// </summary>
        protected virtual void DbBeginTransaction()
        {
            DBConnector.BeginTransaction();
        }

        /// <summary>
        /// Commit
        /// </summary>
        protected virtual void DbCommit()
        {
            DBConnector.Commit();
        }

        /// <summary>
        /// Rollback
        /// </summary>
        protected virtual void DbRollBack()
        {
            DBConnector.RollBack();
        }

        protected virtual TransactionScope DBTransactionScopeAsync
        {
            get
            {
                return DBConnector.TransactionScopeAsync();
            }
        }
    }
}
