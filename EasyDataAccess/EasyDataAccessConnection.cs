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
using DataAccess.Interfaces;
using EasyDataAccess.Enums;
using System.Data.Common;

namespace EasyDataAccess
{
    public class EasyDataAccessConnection : IDisposable, IEasyDataAccessConnection
    {
        #region Private Variables

        private bool disposed = false;
        private string connectionString;
        private IDbConnection Connection;
        private IDbCommand Command { get; set; }
        private int CommandTimeout = 0;
        private IDbTransaction Transaction;
        private Dictionary<string, string> dcFieldsDataReader = new Dictionary<string, string>();
        private Dictionary<string, object> dcParameters = new Dictionary<string, object>();
        private Dictionary<string, DbType> dcParametersOutput = new Dictionary<string, DbType>();
        private Dictionary<string, string> dcMap = new Dictionary<string, string>();
        private Dictionary<string, object> dcFixedValues = new Dictionary<string, object>();
        private List<PropertyInfo> lstPropertyInfoEntity = new List<PropertyInfo>();
        private List<FieldInfo> lstFieldInfoEntity = new List<FieldInfo>();
        private EasyDataAccessTypeConnection easyDataAccessTypeConnection = EasyDataAccessTypeConnection.SqlServer;

        #endregion


        #region Constructor

        public EasyDataAccessConnection()
        {
        }

        #endregion

        #region Dispose


        ~EasyDataAccessConnection()
        {
            this.Dispose(false);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    CloseConnection(); ;
                }

                this.disposed = true;
            }
        }

        #endregion

        #region Connection

        public IDbConnection GetConnection()
        {
            return this.Connection;
        }

        #endregion

        #region Transaction

        public void BeginTransaction()
        {
            if (this.Transaction == null)
            {
                this.Transaction = this.Connection.BeginTransaction();

                if (this.Command == null)
                    this.CreateCommand();

                this.Command.Transaction = this.Transaction;
            }
        }

        public void CommitTransaction()
        {
            if (this.Command.Transaction != null)
            {
                this.Command.Transaction.Commit();
                ClearTransaction();
            }
        }

        public void RollbackTransaction()
        {
            if (this.Command.Transaction != null)
            {
                this.Command.Transaction.Rollback();
                ClearTransaction();
            }
        }

        public void ClearTransaction()
        {
            if (this.Command.Transaction != null)
            {
                this.Command.Transaction.Dispose();
                this.Command.Transaction = null;
            }

            if (this.Transaction != null)
            {
                this.Transaction.Dispose();
                this.Transaction = null;
            }
        }

        #endregion

        #region Command

        private IDbCommand CreateCommand()
        {
            if (this.Connection == null || this.Connection.State == ConnectionState.Closed)
                throw new Exception("To CreateCommand a Open Connection is mandatory");

            this.Command = this.Connection.CreateCommand();

            return this.Command;
        }

        public void SetQuery(string commandText)
        {
            if (this.Command == null)
                CreateCommand();

            this.Command.CommandText = commandText;
            CommandType(System.Data.CommandType.Text);
        }

        public void SetStoredProcedure(string nameStoredProcedure)
        {
            if (this.Command == null)
                CreateCommand();

            this.Command.CommandText = nameStoredProcedure;
            CommandType(System.Data.CommandType.StoredProcedure);
        }

        private void CommandType(CommandType commandType)
        {
            this.Command.CommandType = commandType;
        }

        public void SetTimeout(int Timeout)
        {
            this.CommandTimeout = Timeout;
        }

        private void ConfigureCommand()
        {
            if (this.Command == null)
                CreateCommand();

            if (this.CommandTimeout > 0)
            {
                this.Command.CommandTimeout = this.CommandTimeout;
            }

            Command.Parameters.Clear();
            foreach (KeyValuePair<string, object> item in dcParameters)
            {
                var parameterNameClean = ClearParameterKey(item.Key);

                if (string.IsNullOrEmpty(parameterNameClean))
                    throw new Exception("Please enter Parameter.Name this information is mandatory");

                var parameterName = "@" + parameterNameClean;
                var par = Command.CreateParameter();
                par.ParameterName = parameterName;
                par.Value = item.Value;
                this.Command.Parameters.Add(par);
            }

            foreach (KeyValuePair<string, DbType> item in dcParametersOutput)
            {
                var parameterNameClean = ClearParameterKey(item.Key);

                if (string.IsNullOrEmpty(parameterNameClean))
                    throw new Exception("Please enter ParameterOutput.Name this information is mandatory");

                var parameterName = "@" + parameterNameClean;
                var par = this.Command.CreateParameter();
                par.ParameterName = parameterName;
                par.Size = int.MaxValue;
                par.DbType = (DbType)item.Value;
                par.Direction = ParameterDirection.Output;
                this.Command.Parameters.Add(par);
            }
        }

        public void ClearParameters()
        {
            dcParameters.Clear();
            dcParametersOutput.Clear();
        }

        private string ClearParameterKey(string parameter)
        {
            return parameter.Replace("@", string.Empty);
        }

        public void SetParameter(string name, object value)
        {
            if (!dcParameters.ContainsKey(name))
                dcParameters.Add(name, value);
        }

        public void SetParameterOutput(string name, DbType type)
        {
            if (!dcParametersOutput.ContainsKey(name))
                dcParametersOutput.Add(name, type);
        }

        public object GetParameterOuput(string name)
        {
            object instance = null;
            this.ClearParameterKey(name);
            if (this.Command != null && !string.IsNullOrEmpty(name))
            {
                var parName = "@" + name;
                if (this.Command.Parameters.Contains(parName))
                    instance = this.Command.Parameters[parName];
            }
            return ((IDataParameter)instance).Value;
        }

        #endregion

        #region ExecuteReader

        public IDataReader ExecuteReader()
        {
            ConfigureCommand();
            return this.Command.ExecuteReader();
        }

        public List<T> ExecuteReader<T>()
        {
            List<T> lst = new List<T>();
            ConfigureCommand();
            using (IDataReader dr = this.Command.ExecuteReader())
            {
                while (dr.Read())
                {
                    var entity = SetDataReaderToEntity<T>(dr);
                    lst.Add(entity);
                }
            }
            return lst;
        }

        public async Task<IDataReader> ExecuteReaderAsync()
        {
            return await Task.Run(() => ExecuteReader());
        }

        public async Task<List<T>> ExecuteReaderAsync<T>()
        {
            return await Task.Run(() => ExecuteReader<T>());
        }

        #endregion

        #region ExecuteNonQuery

        public int ExecuteNonQuery()
        {
            ConfigureCommand();
            return this.Command.ExecuteNonQuery();
        }

        public async Task<int> ExecuteNonQueryAsync()
        {
            return await Task.Run(() => ExecuteNonQuery());
        }

        #endregion

        #region ExecuteScalar

        public object ExecuteScalar()
        {
            ConfigureCommand();
            return this.Command.ExecuteScalar();
        }

        public T ExecuteScalar<T>()
        {
            ConfigureCommand();
            return (T)this.Command.ExecuteScalar();
        }

        public async Task<object> ExecuteScalarAsync()
        {
            return await Task.Run(() => ExecuteScalar());
        }

        public async Task<T> ExecuteScalarAsync<T>()
        {
            return await Task.Run(() => ExecuteScalar<T>());
        }

        #endregion

        #region Connection

        public void CreateConnection(string connectionString)
        {
            if (easyDataAccessTypeConnection == EasyDataAccessTypeConnection.SqlServer)
            {
                if (this.Connection == null)
                    this.Connection = CreateConnectionSqlServer(connectionString);

            }
            else
            {
                throw new Exception($"EasyDataAccessType {easyDataAccessTypeConnection} not yet implemented!");
            }
        }

        public async Task CreateConnectionAsync(string connectionString)
        {
            if (this.easyDataAccessTypeConnection == EasyDataAccessTypeConnection.SqlServer)
            {
                if (this.Connection == null)
                    this.Connection = await CreateConnectionSqlServerAsync(connectionString);
            }
            else
            {
                throw new Exception($"EasyDataAccessType {this.easyDataAccessTypeConnection} not yet implemented!");
            }
        }

        public void CreateConnection(IDbConnection connection)
        {
            if (connection != null)
            {
                this.Connection = connection;
            }
        }

        public async Task CreateConnectionAsync(IDbConnection connection)
        {
            await Task.Run(() => CreateConnection(connection));
        }

        private IDbConnection CreateConnectionSqlServer(string connectionString)
        {
            SqlConnection sqlConnection = new SqlConnection(CheckConnectionString(connectionString));

            if (sqlConnection.State != ConnectionState.Open)
                sqlConnection.Open();

            this.Connection = sqlConnection;

            return this.Connection;
        }

        private async Task<IDbConnection> CreateConnectionSqlServerAsync(string connectionString)
        {
            SqlConnection sqlConnection = new SqlConnection(CheckConnectionString(connectionString));

            if (sqlConnection.State != ConnectionState.Open)
                await sqlConnection.OpenAsync();

            this.Connection = sqlConnection;

            return this.Connection;
        }

        private string CheckConnectionString(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new Exception("The ConnectionString needs to be informed!");

            return connectionString;
        }

        public void SetEasyDataAccessTypeConnection(EasyDataAccessTypeConnection easyDataAccessTypeConnection)
        {
            this.easyDataAccessTypeConnection = easyDataAccessTypeConnection;
        }

        public void CloseConnection()
        {
            if (this.Connection != null && this.Connection.State == ConnectionState.Open)
            {
                this.Connection.Close();
            }
        }

        #endregion

        #region Map and Load DataReader To Property/Field Entity

        public T SetDataReaderToEntity<T>(IDataReader dr)
        {
            object instance = CreateInstance<T>();

            ClearFieldsDataReader();
            LoadFieldsDataReader(dr);

            LoadDataReaderToPropertyEntity<T>(instance, dr);
            LoadDataReaderToFieldEntity<T>(instance, dr);

            return (T)instance;
        }

        private T LoadDataReaderToPropertyEntity<T>(object instance, IDataReader dr)
        {
            ClearListPropertyInfoEntity();
            LoadListPropertyInfoEntity<T>();

            foreach (PropertyInfo info in lstPropertyInfoEntity)
            {
                var _namePropertyEntity = string.Empty;

                _namePropertyEntity = info.Name.Replace("_", "").ToLower();

                if (dcMap.ContainsKey(_namePropertyEntity))
                {
                    _namePropertyEntity = dcMap[_namePropertyEntity];
                }

                if (dcFieldsDataReader.ContainsKey(_namePropertyEntity))
                {
                    object valorDr = dr[dcFieldsDataReader[_namePropertyEntity]];

                    if (dcFixedValues.ContainsKey(_namePropertyEntity))
                    {
                        valorDr = dcFixedValues[_namePropertyEntity];
                    }

                    if (valorDr != DBNull.Value)
                    {
                        if (!info.PropertyType.IsEnum)
                        {
                            if (Nullable.GetUnderlyingType(info.PropertyType) != null)
                            {
                                if (Nullable.GetUnderlyingType(info.PropertyType).IsEnum == false)
                                {
                                    info.SetValue(instance, Convert.ChangeType(valorDr, Nullable.GetUnderlyingType(info.PropertyType)), null);
                                }
                            }
                            else
                            {
                                object valorConvertido = Convert.ChangeType(valorDr, info.PropertyType);
                                info.SetValue(instance, valorConvertido, null);
                            }
                        }
                        else
                        {
                            info.SetValue(instance, Enum.Parse(info.PropertyType, valorDr.ToString()), null);
                        }
                    }
                }
            }

            return (T)instance;
        }

        private T LoadDataReaderToFieldEntity<T>(object instance, IDataReader dr)
        {
            ClearListFieldInfoEntity();
            LoadListFieldInfoEntity(instance);

            foreach (FieldInfo info in lstFieldInfoEntity)
            {
                var _nameAttributeEntity = string.Empty;

                _nameAttributeEntity = info.Name.Replace("_", "").ToLower();

                if (dcMap.ContainsKey(_nameAttributeEntity))
                {
                    _nameAttributeEntity = dcMap[_nameAttributeEntity];
                }

                if (dcFieldsDataReader.ContainsKey(_nameAttributeEntity))
                {
                    object valorDr = dr[dcFieldsDataReader[_nameAttributeEntity]];

                    if (dcFixedValues.ContainsKey(_nameAttributeEntity))
                    {
                        valorDr = dcFixedValues[_nameAttributeEntity];
                    }

                    if (valorDr != DBNull.Value)
                    {
                        if (!info.FieldType.IsEnum)
                        {
                            if (Nullable.GetUnderlyingType(info.FieldType) != null)
                            {
                                if (Nullable.GetUnderlyingType(info.FieldType).IsEnum == false)
                                {
                                    info.SetValue(instance, Convert.ChangeType(valorDr, Nullable.GetUnderlyingType(info.FieldType)));
                                }
                            }
                            else
                            {
                                object valorConvertido = Convert.ChangeType(valorDr, info.FieldType);
                                info.SetValue(instance, valorConvertido);
                            }
                        }
                        else
                        {
                            info.SetValue(instance, Enum.Parse(info.FieldType, valorDr.ToString()));
                        }
                    }
                }
            }

            return (T)instance;
        }

        private void ClearFieldsDataReader()
        {
            dcFieldsDataReader.Clear();
        }

        private void ClearListPropertyInfoEntity()
        {
            lstPropertyInfoEntity.Clear();
        }

        private void ClearListFieldInfoEntity()
        {
            lstFieldInfoEntity.Clear();
        }

        private void LoadFieldsDataReader(IDataReader dr)
        {
            int numCampos = dr.FieldCount;
            for (int i = 0; i < numCampos; i++)
            {
                dcFieldsDataReader.Add(dr.GetName(i).Replace("_", "").ToLower(), dr.GetName(i).ToLower());
            }
        }

        private void LoadListPropertyInfoEntity<T>()
        {
            PropertyInfo[] propertiesEntity = typeof(T).GetProperties();
            foreach (PropertyInfo property in propertiesEntity)
            {
                lstPropertyInfoEntity.Add(property);
            }
        }

        private void LoadListFieldInfoEntity(object obj)
        {
            string key = string.Empty;
            foreach (FieldInfo info in obj.GetType().GetFields(BindingFlags.Public |
                                                                BindingFlags.NonPublic |
                                                                BindingFlags.Instance |
                                                                BindingFlags.Static))
            {
                if (info.GetValue(obj) == null)
                {
                    key = GetClearFieldInfoName(info);

                    if (!string.IsNullOrEmpty(key))
                    {
                        if (!lstFieldInfoEntity.Contains(info))
                        {
                            lstFieldInfoEntity.Add(info);
                        }
                    }
                }
            }
        }

        private string GetClearFieldInfoName(FieldInfo attribute)
        {
            var name = attribute.Name;
            if (name.IndexOf(">") != -1)
            {
                name = name.Remove(name.IndexOf(">"), name.Length - name.IndexOf(">")).Replace("<", "");
                name = name.Replace("_", "").ToLower();
            }
            return name;
        }

        private object CreateInstance<T>()
        {
            Assembly objAssembly = Assembly.GetAssembly(typeof(T));
            return objAssembly.CreateInstance(typeof(T).FullName, false);
        }

        public void SetMap(string nameFieldEntity, string nameFieldDbReturn)
        {
            nameFieldEntity = nameFieldEntity.Replace("_", "").ToLower();
            nameFieldDbReturn = nameFieldDbReturn.Replace("_", "").ToLower();

            if (!dcMap.ContainsKey(nameFieldEntity))
                dcMap.Add(nameFieldEntity, nameFieldDbReturn);
        }

        public void SetFixedValue(string nameFieldEntity, object fixedValue)
        {
            nameFieldEntity = nameFieldEntity.Replace("_", "").ToLower();

            if (!dcFixedValues.ContainsKey(nameFieldEntity))
                dcFixedValues.Add(nameFieldEntity, fixedValue);
        }

        public void ClearMaps()
        {
            dcMap.Clear();
        }

        public void ClearFixedValues()
        {
            dcFixedValues.Clear();
        }

        #endregion

    }
}
