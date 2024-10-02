#if NETSTANDARD1_0_OR_GREATER
using System.Data.SqlClient;
#else
using Microsoft.Data.SqlClient;
#endif
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Dynamic;
using System.Linq;
using System.Diagnostics;
using EasyDataAccess.Enums;

namespace DataAccess.Interfaces
{
    public interface IEasyDataAccessConnection
    {

        #region Connection

        IDbConnection GetConnection();

        #endregion

        #region Transaction

        void ClearTransaction();

        void BeginTransaction();

        void CommitTransaction();

        void RollbackTransaction();

        #endregion

        #region Command

        void SetQuery(string commandText);

        void SetStoredProcedure(string nameStoredProcedure);

        void SetTimeout(int Timeout);

        void ClearParameters();

        void SetParameter(string name, object value);

        void SetParameterOutput(string name, DbType type);

        object GetParameterOuput(string name);

        #endregion

        #region ExecuteReader

        IDataReader ExecuteReader();

        List<T> ExecuteReader<T>();

        Task<IDataReader> ExecuteReaderAsync();

        Task<List<T>> ExecuteReaderAsync<T>();

        #endregion

        #region ExecuteNonQuery

        int ExecuteNonQuery();

        Task<int> ExecuteNonQueryAsync();

        #endregion

        #region ExecuteScalar

        object ExecuteScalar();

        T ExecuteScalar<T>();

        Task<object> ExecuteScalarAsync();

        Task<T> ExecuteScalarAsync<T>();

        #endregion

        #region Connection

        void CreateConnection(string connectionString);

        Task CreateConnectionAsync(string connectionString);

        void CreateConnection(IDbConnection connection);

        Task CreateConnectionAsync(IDbConnection connection);

        void SetEasyDataAccessTypeConnection(EasyDataAccessTypeConnection easyDataAccessTypeConnection);

        void CloseConnection();

        #endregion

        #region Map and Load DataReader To Property/Field Entity

        T SetDataReaderToEntity<T>(IDataReader dr);

        void SetMap(string nameFieldEntity, string nameFieldDbReturn);

        void SetFixedValue(string nameFieldEntity, object fixedValue);

        void ClearMaps();

        void ClearFixedValues();

        #endregion

    }
}
