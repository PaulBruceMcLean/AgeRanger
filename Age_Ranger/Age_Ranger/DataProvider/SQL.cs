using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using Age_Ranger.Models;

namespace Age_Ranger.DataProvider
{
    public class SQL : ISQLGeneric
    {
        private SqlCommand command;
        private SqlConnection connection;
        private SqlTransaction transaction;
        private IsolationLevel transactionIsolation;
        private List<SqlParameter> QueryParameters;
        private string ConnectionString = string.Empty;
        private bool TransActional = false;
        private bool BindParametersToQuery = false;

        public SQL()
        {
            QueryParameters = new List<SqlParameter>();
        }

        public void CreateQueryParameter(string ParameterName, DbType ParameterType, object ParameterValue, ParameterDirection Direction = ParameterDirection.Input)
        {
            SqlParameter newParameter = new SqlParameter() { ParameterName = ParameterName, DbType = ParameterType, Value = ParameterValue, Direction = Direction };
            QueryParameters.Add(newParameter);
        }

        public void CreateCommand(string Command, CommandType Type, bool CommandParameters = false)
        {
            command = new SqlCommand();
            command.CommandText = Command;
            command.CommandType = Type;
            if (CommandParameters)
            {
                if (CommandParameters)
                {
                    BindParametersToQuery = CommandParameters;
                }
            }
        }

        public void BindConnectionString(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public void CreateTransaction(bool UseTransActions, IsolationLevel TransactionIsolation = IsolationLevel.Unspecified)
        {
            TransActional = UseTransActions;
            transactionIsolation = TransactionIsolation;
        }

        private void CleanProvider()
        {
            command = null;
            transactionIsolation = IsolationLevel.Unspecified;
            QueryParameters.Clear();
            TransActional = false;
            BindParametersToQuery = false;
        }

        public int ExecuteNonQuery()
        {
            int returnResult;
            try
            {
                using (connection = new SqlConnection(ConnectionString))
                {
                    if (connection.State == ConnectionState.Closed)
                    {
                        connection.Open();
                    }
                    if (command == null)
                    {
                        connection.Close();
                        return 0;
                    }
                    if (QueryParameters.Count > 0 && BindParametersToQuery == true)
                    {
                        command.Parameters.AddRange(QueryParameters.ToArray());
                    }
                    if (TransActional)
                    {
                        transaction = connection.BeginTransaction(transactionIsolation);
                    }
                    command.Connection = connection;
                    command.Transaction = transaction;
                    returnResult = command.ExecuteNonQuery();
                    transaction.Commit();
                }
            }
            catch (SqlException)
            {
                //Will be a good idea to add fault logging here;
                returnResult = 0;
                try
                {
                    // This catch block will handle any errors that may have occurred
                    // on the server that would cause the rollback to fail, such as
                    // a closed connection.
                    transaction.Rollback();
                }
                catch (Exception)
                {

                }
            }
            finally
            {
                CleanProvider();
            }

            return returnResult;
        }

        public List<PersonModel> ExecuteDataReader(ref string OutPutMessage)
        {
            List<PersonModel> PersonResults = new List<PersonModel>();
            try
            {
                using (connection = new SqlConnection(ConnectionString))
                {
                    if (connection.State == ConnectionState.Closed)
                    {
                        connection.Open();
                    }
                    if (command == null)
                    {
                        connection.Close();
                        OutPutMessage = "Select Instructions is Blank and not data will be returned";
                        return PersonResults;
                    }
                    if (QueryParameters.Count > 0 && BindParametersToQuery == true)
                    {
                        command.Parameters.AddRange(QueryParameters.ToArray());
                    }
                    if (TransActional)
                    {
                        transaction = connection.BeginTransaction(transactionIsolation);
                    }
                    command.Connection = connection;
                    command.Transaction = transaction;
                    SqlDataReader resultsTableReader = command.ExecuteReader();
                    while (resultsTableReader.Read())
                    {
                        PersonModel Result = new PersonModel();
                        Result.PersonFirstName = resultsTableReader["FirstName"].ToString().Trim();
                        Result.PersonLastName = resultsTableReader["LastName"].ToString().Trim();
                        Result.CurrentAge = Convert.ToInt32(resultsTableReader["Age"].ToString().Trim());
                        Result.AgeDescription = (resultsTableReader.FieldCount == 5) ? resultsTableReader["Description"].ToString().Trim() : "";
                        Result.PersonID = Convert.ToInt64(resultsTableReader["Id"].ToString().Trim());
                        PersonResults.Add(Result);
                    }
                    OutPutMessage = string.Format("Total Records Found is {0}", PersonResults.Count);
                    resultsTableReader.Close();
                }
            }
            catch (SqlException)
            {
                OutPutMessage = "There was an Error Retrieving Data from the DataBase";
                //Will be a good idea to add fault logging here;
                try
                {
                    // This catch block will handle any errors that may have occurred
                    // on the server that would cause the rollback to fail, such as
                    // a closed connection.
                    transaction.Rollback();
                }
                catch (Exception)
                {

                }
            }
            finally
            {
                CleanProvider();
            }
            return PersonResults;
        }
    }
}