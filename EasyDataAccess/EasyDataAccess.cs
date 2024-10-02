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
using DataAccess.Extentions;

namespace EasyDataAccess
{

    public class EasyDataAccess: IDisposable, IEasyDataAccess
    {
        #region Private Variables

        private static EasyDataAccess instance;

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

        #region Dispose

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        #endregion


        //#region Connection

        public EasyDataAccessConnection CreateConnection(string connectionString)
        {
            var edai = new EasyDataAccessConnection();
            edai.CreateConnection(connectionString);
            return edai;
        }

        public EasyDataAccessConnection CreateConnection(IDbConnection connection)
        {
            var edai = new EasyDataAccessConnection();
            edai.CreateConnection(connection);
            return edai;
        }

        public async Task<EasyDataAccessConnection> CreateConnectionAsync(string connectionString)
        {
            var edai = new EasyDataAccessConnection();
            await edai.CreateConnectionAsync(connectionString);
            return edai;
        }

        public async Task<EasyDataAccessConnection> CreateConnectionAsync(IDbConnection connection)
        {
            var edai = new EasyDataAccessConnection();
            await edai.CreateConnectionAsync(connection);
            return edai;

        }


        //#endregion
    }
}
