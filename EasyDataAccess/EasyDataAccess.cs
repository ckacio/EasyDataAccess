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

        public IDbConnection GetConnection(string connectionString)
        {
            if (!this.dcConnection.ContainsKey(CheckConnectionString(connectionString)))
                return null;

            return dcConnection[connectionString];
        }

        public EasyDataAccessIntance CreateConnection(string connectionString, EasyDataAccessType easyDataAccessType = EasyDataAccessType.SqlServer)
        {
            var con = this.GetConnection(connectionString);
            EasyDataAccessIntance instance = null;

            if (easyDataAccessType == EasyDataAccessType.SqlServer)
            {
                if (con == null)
                    con = CreateConnectionSqlServer(connectionString);

                instance = new EasyDataAccessIntance(con);
            }
            else
            {
                throw new Exception($"EasyDataAccessType {easyDataAccessType} not yet implemented!");
            }

            return instance;
        }

        public EasyDataAccessIntance CreateConnection(IDbConnection connection)
        {
            {
                var con = this.GetConnection(connection.ConnectionString);

                if (con == null)
                {
                    con = connection;
                    this.dcConnection.Add(connection.ConnectionString, connection);
                }

                return new EasyDataAccessIntance(con);

            }
        }

        public async Task<EasyDataAccessIntance> CreateConnectionAsync(string connectionString, EasyDataAccessType easyDataAccessType = EasyDataAccessType.SqlServer)
        {
            var con = this.GetConnection(connectionString);
            EasyDataAccessIntance instance = null;

            if (easyDataAccessType == EasyDataAccessType.SqlServer)
            {
                if (con == null)
                    con = await CreateConnectionSqlServerAsync(connectionString);

                instance = new EasyDataAccessIntance(con);
            }
            else
            {
                throw new Exception($"EasyDataAccessType {easyDataAccessType} not yet implemented!");
            }

            return instance;
        }

        public async Task<EasyDataAccessIntance> CreateConnectionAsync(IDbConnection connection)
        {
            return await Task.Run(() => CreateConnection(connection));
        }

        private IDbConnection CreateConnectionSqlServer(string connectionString)
        {
            SqlConnection sqlConnection = new SqlConnection(CheckConnectionString(connectionString));

            if (sqlConnection.State != ConnectionState.Open)
                sqlConnection.Open();

            this.dcConnection.Add(connectionString, sqlConnection);

            return this.GetConnection(connectionString);
        }

        private async Task<IDbConnection> CreateConnectionSqlServerAsync(string connectionString)
        {
            SqlConnection sqlConnection = new SqlConnection(CheckConnectionString(connectionString));

            if (sqlConnection.State != ConnectionState.Open)
                await sqlConnection.OpenAsync();

            this.dcConnection.Add(connectionString, sqlConnection);

            return this.GetConnection(connectionString);
        }

        private string CheckConnectionString(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new Exception("The ConnectionString needs to be informed!");

            return connectionString;
        }

        #endregion
    }
}
