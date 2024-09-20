using System;
using System.Data;
using System.Threading.Tasks;

namespace EasyDataAccess
{

    public class EasyDataAccess
    {
        #region Constructor

        public EasyDataAccess()
        {

        }

        public EasyDataAccess(string connectionString)
        {
            this.ConnectionString = SetConnectionString(connectionString);
        }

        public EasyDataAccess(IDbConnection connection)
        {
            this.Connection = connection;
        }

        #endregion

        #region Private Variables

        private string ConnectionString;

        private IDbConnection Connection;

        #endregion

        #region Private Sets

        private string SetConnectionStringApplicationName(string connectionString)
        {
            if (!connectionString.Contains("Application Name"))
            {
                return connectionString = "Application Name = 'EasyDataAccess Micro ORM';" + connectionString;
            }
            else
            {
                return connectionString;
            }
        }

        private string SetConnectionString(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new Exception("The ConnectionString needs to be informed!");

            this.ConnectionString = SetConnectionStringApplicationName(connectionString);
            return this.ConnectionString;
        }

        #endregion

        #region Public Methods

        public EasyDataAccessIntance CreateConnection()
        {
            EasyDataAccessIntance con = new EasyDataAccessIntance();

            if (string.IsNullOrEmpty(this.ConnectionString) && this.Connection == null)
                throw new Exception("The ConnectionString needs to be informed on constructor! Example: new DataAccess(connectionString) or new DataAccess(connection)");

            if (!string.IsNullOrEmpty(this.ConnectionString))
            {
                con = this.CreateConnection(this.ConnectionString);
            }

            if (this.Connection != null)
            {
                con = this.CreateConnection(this.Connection);
            }

            return con;
        }


        public async Task<EasyDataAccessIntance> CreateConnectionAsync()
        {
            EasyDataAccessIntance con = new EasyDataAccessIntance();

            if (string.IsNullOrEmpty(this.ConnectionString) && this.Connection == null)
                throw new Exception("The ConnectionString needs to be informed on constructor! Example: new DataAccess(connectionString) or new DataAccess(connection)");

            if (!string.IsNullOrEmpty(this.ConnectionString))
            {
                con = await this.CreateConnectionAsync(this.ConnectionString);
            }

            if (this.Connection != null)
            {
                con = this.CreateConnection(this.Connection);
            }

            return con;
        }

        public EasyDataAccessIntance CreateConnection(string connectionString)
        {
            this.ConnectionString = SetConnectionString(connectionString);
            var con = new EasyDataAccessIntance(this.ConnectionString);
            con.CreateConnection();

            return con;
        }

        public EasyDataAccessIntance CreateConnection(IDbConnection connection)
        {
            this.Connection = connection;
            var con = new EasyDataAccessIntance(connection);

            return con;
        }

        public async Task<EasyDataAccessIntance> CreateConnectionAsync(string connectionString)
        {
            this.ConnectionString = SetConnectionString(connectionString); ;
            var con = new EasyDataAccessIntance(this.ConnectionString);
            await con.CreateConnectionAsync();

            return con;
        }

        #endregion
    }
}
