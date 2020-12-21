using WebAPI.Constants;
using System.Data;
using System.Data.SqlClient;
using System.Transactions;

namespace WebAPI.Repositories
{
    public class DBConnector
    {
        private readonly string _connectionString = string.Empty;
        private IDbConnection conn;
        private IDbTransaction trans;

        public DBConnector(string connectionString)
        {
            this._connectionString = connectionString;
        }

        /// <summary>
        /// State connection
        /// </summary>
        public bool OpenState
        {
            get
            {
                if (conn == null)
                    return false;

                ConnectionState state = conn.State;
                if (state == ConnectionState.Open)
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// Open connection
        /// </summary>
        public void Open()
        {
            if (string.IsNullOrEmpty(_connectionString))
                throw new System.Exception(Constant.CONNECT_ERROR);

            if (!OpenState)
            {
                conn = new SqlConnection(this._connectionString);
                conn.Open();
            }
        }

        /// <summary>
        /// Close connection
        /// </summary>
        public void Close()
        {
            if (conn != null)
                conn.Close();
        }

        /// <summary>
        /// Get connectiion
        /// </summary>
        public IDbConnection Connection
        {
            get { return conn; }
        }

        /// <summary>
        /// Begin transaction
        /// </summary>
        public void BeginTransaction()
        {
            trans = conn.BeginTransaction();
        }

        /// <summary>
        /// Get transactiion
        /// </summary>
        public IDbTransaction Transaction
        {
            get { return trans; }
        }

        /// <summary>
        /// Commit
        /// </summary>
        public void Commit()
        {
            if (trans != null)
                trans.Commit();
            trans = null;
        }

        /// <summary>
        /// Rollback
        /// </summary>
        public virtual void RollBack()
        {
            if (trans != null)
                trans.Rollback();
        }

        public TransactionScope TransactionScopeAsync() {
            return new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        }

    }
}
