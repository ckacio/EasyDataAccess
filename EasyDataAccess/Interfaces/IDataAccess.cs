
#if NETSTANDARD1_0_OR_GREATER
using System.Data.SqlClient;
#else
using Microsoft.Data.SqlClient;
#endif
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Data.Common;
using DataAccess.Enums;

namespace DataAccess.Interfaces
{
    public interface IDataAccess
    {
        #region Public Gets

        object GetParameterOuput(string name);

        #endregion

        #region Public Sets
        void SetProcName(string procName);

        void SetParameter(string name, object value);

        void SetParameterOutput(string name, DbType type);

        void SetNick(string entityPropertyName, string nameFieldReturnedFromProc);

        void SetFixedValue(string entityPropertyName, object fixedValue);

        void SetTimeout(int timeout);
                
        void SetConnectionString(string connectionString);

        T SetDataReaderToEntity<T>(IDataReader dr);

        #endregion

        #region ExecuteReader
        IDataReader ExecuteReader();
        List<T> ExecuteReader<T>();

        #endregion

        #region ExecuteReaderAsync

        Task<IDataReader> ExecuteReaderAsync();

        Task<List<T>> ExecuteReaderAsync<T>();
                
        #endregion

        #region ExecuteScalar

        object ExecuteScalar();

        T ExecuteScalar<T>();

        #endregion

        #region ExecuteScalarAsync

        Task<object> ExecuteScalarAsync();

        Task<T> ExecuteScalarAsync<T>();

        #endregion

        #region ExecuteNonQuery

        int ExecuteNonQuery();

        #endregion

        #region ExecuteNonQueryAsync

        Task<int> ExecuteNonQueryAsync();

        #endregion

        #region Public Methods
        IDbConnection CreateConnection(string connectionString);

        IDbConnection CreateConnection();

        void CloseConnection();

        IDbCommand CreateCommand();

        void RunSingleton();

        void ClearProcName();

        void ClearParameters();

        void ClearNicks();

        void ClearFixedValues();

        void ClearTransaction();

        void BeginTransaction();

        void CommitTransaction();

        void RollbackTransaction();

        #endregion

        #region Pulic Methods Async

        Task<IDbConnection> CreateConnectionAsync();

        Task<IDbConnection> CreateConnectionAsync(string connectionString);
        
        Task<IDbConnection> CreateConnectionAsync(DataAccessType dataAccessType);
        
        Task<IDbCommand> CreateCommandAsync();

        #endregion

    }
}
