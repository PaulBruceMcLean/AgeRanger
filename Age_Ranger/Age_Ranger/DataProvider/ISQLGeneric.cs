using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
using System.Data;
using Age_Ranger.Models;

namespace Age_Ranger.DataProvider
{
    interface ISQLGeneric
    {
        void CreateQueryParameter(string ParameterName, DbType ParameterType, object ParameterValue, ParameterDirection Direction = ParameterDirection.Input);
        void CreateCommand(string Command, CommandType Type, bool CommandParameters = false);
        void BindConnectionString(string connectionString);
        void CreateTransaction(bool UseTransActions, IsolationLevel TransactionIsolation = IsolationLevel.Unspecified);
        int ExecuteNonQuery();
        List<PersonModel> ExecuteDataReader(ref string OutPutMessage);
    }
}
