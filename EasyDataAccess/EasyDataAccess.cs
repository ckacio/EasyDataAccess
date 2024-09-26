#if NETSTANDARD1_0_OR_GREATER
using System.Data.SqlClient;
#else
using Microsoft.Data.SqlClient;
#endif
using EasyDataAccess.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using DataAccess.Interfaces;

namespace EasyDataAccess
{

    public class EasyDataAccess: IEasyDataAccess
    {
        #region Private Variables

        private static EasyDataAccess instance;

        private Dictionary<string, IDbConnection> dcConnection = new Dictionary<string, IDbConnection>();

        #endregion

        #region Constructor Singleton

        public static EasyDataAccess Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new EasyDataAccess();
                }
                return instance;
            }
        }

        #endregion

        #region Connection

        public IDbConnection GetConnection(string connectionName)
        {
            if (string.IsNullOrEmpty(connectionName))
                throw new Exception("The ConnectionName needs to be informed!");

            if (!this.dcConnection.ContainsKey(connectionName))
                return null;

            return dcConnection[connectionName];
        }

        public EasyDataAccessIntance CreateConnection(string connectionName, string connectionString, EasyDataAccessType easyDataAccessType = EasyDataAccessType.SqlServer)
        {
            var con = this.GetConnection(connectionName);
            EasyDataAccessIntance instance = null;

            if (easyDataAccessType == EasyDataAccessType.SqlServer)
            {
                if (con == null)
                    con = CreateConnectionSqlServer(connectionName, connectionString);

                instance = new EasyDataAccessIntance(con);
            }
            else
            {
                throw new Exception($"EasyDataAccessType {easyDataAccessType} not yet implemented!");
            }

            return instance;
        }

        public EasyDataAccessIntance CreateConnection(string connectionName, IDbConnection connection)
        {
            {
                var con = this.GetConnection(connectionName);

                if (con == null)
                {
                    con = connection;
                    this.dcConnection.Add(connectionName, connection);
                }

                return new EasyDataAccessIntance(con);

            }
        }

        public async Task<EasyDataAccessIntance> CreateConnectionAsync(string connectionName, string connectionString, EasyDataAccessType easyDataAccessType = EasyDataAccessType.SqlServer)
        {
            var con = this.GetConnection(connectionName);
            EasyDataAccessIntance instance = null;

            if (easyDataAccessType == EasyDataAccessType.SqlServer)
            {
                if (con == null)
                    con = await CreateConnectionSqlServerAsync(connectionName, connectionString);

                instance = new EasyDataAccessIntance(con);
            }
            else
            {
                throw new Exception($"EasyDataAccessType {easyDataAccessType} not yet implemented!");
            }

            return instance;
        }

        public async Task<EasyDataAccessIntance> CreateConnectionAsync(string connectionName, IDbConnection connection)
        {
            return await Task.Run(() => CreateConnection(connectionName,connection));
        }

        private IDbConnection CreateConnectionSqlServer(string connectionName, string connectionString)
        {
            SqlConnection sqlConnection = new SqlConnection(SetConnectionString(connectionString));

            if (sqlConnection.State != ConnectionState.Open)
                sqlConnection.Open();

            this.dcConnection.Add(connectionName, sqlConnection);

            return this.GetConnection(connectionName);
        }

        private async Task<IDbConnection> CreateConnectionSqlServerAsync(string connectionName, string connectionString)
        {
            SqlConnection sqlConnection = new SqlConnection(SetConnectionString(connectionString));

            if (sqlConnection.State != ConnectionState.Open)
                await sqlConnection.OpenAsync();

            this.dcConnection.Add(connectionName, sqlConnection);

            return this.GetConnection(connectionName);
        }

        public void CloseConnection(string connectionName)
        {
            if (this.dcConnection.ContainsKey(connectionName) && this.dcConnection[connectionName].State == ConnectionState.Open)
            {
                this.dcConnection[connectionName].Close();
            }
        }

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

            return SetConnectionStringApplicationName(connectionString);
        }

        #endregion
    }
}
