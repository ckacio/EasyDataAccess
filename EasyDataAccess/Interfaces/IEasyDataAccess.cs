
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
using EasyDataAccess.Enums;
using EasyDataAccess;

namespace DataAccess.Interfaces
{
    public interface IEasyDataAccess
    {
        #region Connection

        IDbConnection GetConnection(string connectionString);

        EasyDataAccessIntance CreateConnection(string connectionString, EasyDataAccessType easyDataAccessType = EasyDataAccessType.SqlServer);

        EasyDataAccessIntance CreateConnection(IDbConnection connection);

        Task<EasyDataAccessIntance> CreateConnectionAsync(string connectionString, EasyDataAccessType easyDataAccessType = EasyDataAccessType.SqlServer);

        Task<EasyDataAccessIntance> CreateConnectionAsync(IDbConnection connection);

        #endregion
    }
}
