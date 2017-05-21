using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Age_Ranger.CustomExtentionMethods;
using System.Data;
using Age_Ranger.Models;


namespace Age_Ranger.DataProvider
{
    public class SQLDataProvider
    {
        private readonly string SQL_Lite_ConnectionString;

        private readonly string SQL_ConnectionString;

        private readonly string SQL_DataRepository_Type;

        private bool pvt_ConfigurationsSet = true;

        /// <summary>
        /// Propery containing value to ensure the Settings have been applied correctly
        /// </summary>
        public bool ConfigurationsSet { get { return pvt_ConfigurationsSet; } }

        private ISQLGeneric data_provider;

        /// <summary>
        /// Change the Constructor to Public instead of private to Run the Unit test
        /// </summary>
        public SQLDataProvider()
        {
            GetConfigurationSetting(ConfigSetting.AppSetting, "DataRepository", ref SQL_DataRepository_Type);
            GetConfigurationSetting(ConfigSetting.ConnectionString, "SQL_LITE", ref SQL_Lite_ConnectionString);
            GetConfigurationSetting(ConfigSetting.ConnectionString, "SQL", ref SQL_ConnectionString);
            if (!pvt_ConfigurationsSet)
            {
                return;
            }
            if (pvt_ConfigurationsSet)
            {
                switch (SQL_DataRepository_Type)
                {
                    case "SQLite":
                        data_provider = new SQLLITE();
                        break;
                    case "SQL":
                        data_provider = new SQL();
                        break;
                }
            }
        }

        private enum ConfigSetting { AppSetting , ConnectionString};
        /// <summary>
        /// Returns the desired configuration setting from the web.config file
        /// </summary>
        /// <param name="Setting">Eneumarable value which can be selected for either and AppSetting or a ConnectionString</param>
        /// <param name="SettingName">The Key value of the Setting in the web.config file</param>
        /// <param name="SettingObtained">Boolean indicator wether the Setting was successfully Obtained , false indicates a negative result</param>
        /// <returns></returns>
        ///
        private void GetConfigurationSetting(ConfigSetting Setting, string SettingName , ref string ConfigurationSetting)
        {
            if (string.IsNullOrEmpty(SettingName))
            {
                pvt_ConfigurationsSet = false;
                return;
            }
            try
            {
                switch (Setting)
                {
                    case SQLDataProvider.ConfigSetting.AppSetting:
                        ConfigurationSetting = ConfigurationManager.AppSettings[SettingName].ToString().Trim();
                        pvt_ConfigurationsSet = string.IsNullOrEmpty(ConfigurationSetting) ? false : true;                        
                        break;
                    case SQLDataProvider.ConfigSetting.ConnectionString:
                        ConfigurationSetting = ConfigurationManager.ConnectionStrings[SettingName].ToString().Trim();
                        pvt_ConfigurationsSet = string.IsNullOrEmpty(ConfigurationSetting) ? false : true;
                        break;
                }
            }
            catch (NullReferenceException)
            {
                pvt_ConfigurationsSet = false;
            }
        }

        /// <summary>
        /// Creates a static instance of the Dataprovider which will be held in the High frequency heap for faster access
        /// </summary>
        public static SQLDataProvider SQLProvider { get { return new SQLDataProvider(); } }

        /// <summary>
        /// Create a Command Parameter and add to a list of Parameters
        /// </summary>
        /// <param name="ParameterName">Name of Parameret</param>
        /// <param name="ParameterType">Database Parameter value Type</param>
        /// <param name="ParameterValue">The Value of the Parameter</param>
        /// <param name="Direction"></param>
        public void CreateQueryParameter(string ParameterName, DbType ParameterType, object ParameterValue, ParameterDirection Direction = ParameterDirection.Input)
        {
            data_provider.CreateQueryParameter(ParameterName, ParameterType, ParameterValue, Direction);
        }

        /// <summary>
        /// Create a Command to be executed against the Database
        /// </summary>
        /// <param name="Command">The Command String to Execute</param>
        /// <param name="Type">Type of Command(Text,Strored Procedure)</param>
        /// <param name="CommandParameters">Boolean Value indicating if the command is reliant on Command Parameters</param>
        public void CreateCommand(string Command, CommandType Type, bool CommandParameters = false)
        {
            data_provider.CreateCommand(Command, Type, CommandParameters);
        }

        /// <summary>
        /// Decrypts the Encrypted Connection String and Binds to the underlying DataLayer so a connection can be established
        /// </summary>
        public void CreateConnection()
        {
            switch (SQL_DataRepository_Type)
            {
                case "SQLite":
                    data_provider.BindConnectionString(SQL_Lite_ConnectionString.DecryptConnectionString());
                    break;
                case "SQL":
                    data_provider.BindConnectionString(SQL_ConnectionString.DecryptConnectionString());
                    break;
            }
           
        }

        /// <summary>
        /// This is optional but is required if need to perform Transactional Qurries
        /// </summary>
        /// <param name="UseTransActions">Indicates whether or not to use Transactions</param>
        /// <param name="TransactionIsolation">The Type of Transaction Isolation Level</param>
        public void CreateTransaction(bool UseTransActions,IsolationLevel TransactionIsolation)
        {
            data_provider.CreateTransaction(UseTransActions, TransactionIsolation);
        }

        /// <summary>
        /// Executes the Coommand against the database
        /// </summary>
        /// <returns>The number of rows affected by the command execution</returns>
        public int ExecuteNonQuery()
        {
            return data_provider.ExecuteNonQuery();
        }

        /// <summary>
        /// Execute a Datareader against the Database based on the Command
        /// </summary>
        /// <param name="OutPutMessage"></param>
        /// <returns>Returns a List of Person Objects that match the Criteria of the Command</returns>
        public List<PersonModel> ExecuteDataReader(ref string OutPutMessage)
        {
            return data_provider.ExecuteDataReader(ref OutPutMessage);
        }
    }

    
}
