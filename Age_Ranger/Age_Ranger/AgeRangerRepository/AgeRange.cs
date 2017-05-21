using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Age_Ranger.DataProvider;
using Age_Ranger.Models;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;

namespace Age_Ranger.AgeRangerRepository
{
    public class AgeRange : IAgeRange<PersonModel>
    {
        private SQLDataProvider DataAccess;

        /// <summary>
        /// provides a mutual-exclusion lock as the SQLDataProvider is a static object, you dont want multiple threads corrupting each other
        /// </summary>
        private Object thisLock = new Object();

        public AgeRange()
        {
            DataAccess = SQLDataProvider.SQLProvider;
        }
        /// <summary>
        /// Verifies if a string has been HTML Encoded, Specifically looking for ASCII byte Representations of (& , / , ;) and rejects if so, this keeps data clean from strange characters
        /// </summary>
        /// <param name="InputString">HTML Encoded</param>
        /// <returns>Returns True if string has been HTML Encoded with Illegal Characters</returns>
        public bool CheckHtmlEncodedInput(string InputString)
        {
            byte[] inputBytes = Encoding.ASCII.GetBytes(InputString);
            IEnumerable<byte> test = (from xhtmlbytes in inputBytes
                       where xhtmlbytes == 38 || xhtmlbytes == 59 || xhtmlbytes == 47
                                      select xhtmlbytes).ToList();
            if (test.Count() > 0)
            {
                //If the encoded string has been encoded and does contains certain characters, false indicates a positive match
                return false;
            }
            //If the encoded string has been encoded and does not contains certain characters, true indicates no match found
            return true;
        }       
        
        /// <summary>
        /// Verifiy that an Input is not Null or Empty
        /// </summary>
        /// <param name="InputString">String input for the Firtst or Lastname of the Person Object</param>
        /// <returns>Returns false if the input is Null or Empty</returns>
        public bool CheckInputForNullorEmpty(string InputString)
        {
            if (string.IsNullOrEmpty(InputString.Trim()))
            {
                //If the input is Null or Empty then false will be returned
                return false;
            }
            //If the input is not Null nor Empty then true will be returned
            return true;
        }

        /// <summary>
        /// Test the validity of the data type supplied for the age of the person as the age can not be a negative number
        /// </summary>
        /// <param name="InputAge">Age of the person</param>
        /// <returns>Returns false if the age cannot be converted into an integer</returns>
        public bool CheckInputPositiveAge(int InputAge)
        {
            //validate Input is a positive Number
            if (InputAge < 0)
            {
                //If the input cannot be a negative number
                return false;
            }
            //If the input can be converted to an Integer return true
            return true;
        }

        /// <summary>
        /// Validates the Row ID in the Database to be a positive ID as the DB starts at Row ID of 1 for the Person Table
        /// </summary>
        /// <param name="PersonDataBaseID">Database Row ID</param>
        /// <returns>Returns False if the ID is a negative number</returns>
        public bool CheckInputPositiveIDRange(long PersonDataBaseID)
        {
            //validate Input is a positive Number
            if (PersonDataBaseID < 0)
            {
                //If the input cannot be a negative number
                return false;
            }
            //If the input can be converted to an Integer return true
            return true;
        }

        /// <summary>
        /// Will Add a new person to the database 
        /// </summary>
        /// <param name="NewPerson">Object containing the details of the new person to be added</param>
        /// <returns>information message</returns>
        public string AddNewPerson(PersonModel NewPerson)
        {
            lock (thisLock)
            {
                if (NewPerson == null)
                {
                    return "New Person Object cannot be null";
                }
                if (!DataAccess.ConfigurationsSet)
                {
                    return "Access to The Database is Denied";
                }

                PropertyInfo[] properties = NewPerson.GetType().GetProperties();
                bool VerificationStatus;
                foreach (PropertyInfo item in properties)
                {
                    var propvalue = item.GetValue(NewPerson);
                    if (item.PropertyType == typeof(string) && item.Name != "AgeDescription")
                    {
                        VerificationStatus = (CheckInputForNullorEmpty(propvalue.ToString())) ? true : false;
                        if (!VerificationStatus) { return "Input Value was Null or Empty"; }
                        VerificationStatus = (CheckHtmlEncodedInput(propvalue.ToString())) ? true : false;
                        if (!VerificationStatus) { return "Input Value Contained Dangerous Code"; }

                    }
                    if (item.PropertyType == typeof(int))
                    {
                        VerificationStatus = (CheckInputPositiveAge(Convert.ToInt32(propvalue))) ? true : false;
                        if (!VerificationStatus) { return "Invalid Input Value"; }
                    }
                }
                if (!DataAccess.ConfigurationsSet)
                {
                    return "Data Repository can not be accessed at the moment";
                }
                #region Add New Person
                DataAccess.CreateConnection();
                string commandString = "insert into Person(FirstName,LastName,Age) values(@FirstName,@LastName,@Age)";
                DataAccess.CreateCommand(commandString, System.Data.CommandType.Text, true);
                DataAccess.CreateQueryParameter("@FirstName", System.Data.DbType.String, NewPerson.PersonFirstName);
                DataAccess.CreateQueryParameter("@LastName", System.Data.DbType.String, NewPerson.PersonLastName);
                DataAccess.CreateQueryParameter("@Age", System.Data.DbType.Int32, NewPerson.CurrentAge);
                DataAccess.CreateTransaction(true, System.Data.IsolationLevel.ReadCommitted);
                if (DataAccess.ExecuteNonQuery() > 0)
                {
                    return "New Person Added Succesfully";
                }
                return "New Person has not been Added Succesfully";
                #endregion
            }

        }

        /// <summary>
        /// this will delete the Person from the Database
        /// </summary>
        /// <param name="PersonID">The Database ID of the Person Record</param>
        /// <returns>Message that describes the result of the action</returns>
        public string DeletePerson(long PersonID)
        {
            lock (thisLock)
            {
                if (PersonID == 0)
                {
                    return "Person can not have an ID record of Zero";
                }
                if (!DataAccess.ConfigurationsSet)
                {
                    return "Access to The Database is Denied";
                }
                DataAccess.CreateConnection();
                string commandString = "delete from Person where Id = @ID";
                DataAccess.CreateCommand(commandString, System.Data.CommandType.Text, true);
                DataAccess.CreateQueryParameter("@ID", System.Data.DbType.Int64, PersonID);
                DataAccess.CreateTransaction(true, System.Data.IsolationLevel.ReadCommitted);
                if (DataAccess.ExecuteNonQuery() > 0)
                {
                    return "Person has been Succesfully removed from Database";
                }
                return "Person has not been Succesfully removed from Database";
            }
        }

        /// <summary>
        /// Searches for a Record or Records in the Database by the First Name of the Person
        /// </summary>
        /// <param name="PersonFirstName">First Name of the Person in the Database</param>
        /// <param name="OutPutMessage">Return Message for the Search</param>
        /// <returns>List of Person Objects that have been found based on the search Criteria</returns>
        public List<PersonModel> FindPerson(string PersonFirstName , ref string OutPutMessage)
        {
            lock (thisLock)
            {
                bool VerificationStatus;
                VerificationStatus = (CheckInputForNullorEmpty(PersonFirstName)) ? true : false;
                if (!VerificationStatus) { OutPutMessage = "Input Value was Null or Empty"; return new List<PersonModel>(); }
                VerificationStatus = (CheckHtmlEncodedInput(PersonFirstName)) ? true : false;
                if (!VerificationStatus) { OutPutMessage = "Input Value Contained Dangerous Code"; return new List<PersonModel>(); }
                if (!DataAccess.ConfigurationsSet) { OutPutMessage = "Access to The Database is Denied"; return new List<PersonModel>(); }

                //Go and get the persons with the name
                DataAccess.CreateConnection();
                string commandString = @"SELECT FirstName,LastName,Age,
                                     CASE WHEN Age <= 2 THEN 'Toddler'
                                          WHEN Age > 4999 THEN 'Kauri Tree'                                     
                                          ELSE(Select Description from AgeGroup where ps.Age >= MinAge and ps.Age <= MaxAge)
                                     END[Description]
                                     ,Id
                                     FROM Person ps
                                     where ps.FirstName like @FirstName";
                //TODO : Dont for get the inner join on the query
                DataAccess.CreateCommand(commandString, System.Data.CommandType.Text, true);
                DataAccess.CreateQueryParameter("@FirstName", System.Data.DbType.String, string.Concat(PersonFirstName, "%"));
                DataAccess.CreateTransaction(true, System.Data.IsolationLevel.ReadCommitted);
                return DataAccess.ExecuteDataReader(ref OutPutMessage);
            }
        }

        /// <summary>
        /// Updatedates a Specicfic Person Record in the Database
        /// </summary>
        /// <param name="NewPerson">Object containing the details of the new person to be updated</param>
        /// <returns>Message that describes the result of the action</returns>
        public string UpdatePerson(PersonModel NewPerson)
        {
            lock (thisLock)
            {
                if (NewPerson == null)
                {
                    return "New Person Object cannot be null";
                }
                if (!DataAccess.ConfigurationsSet)
                {
                    return "Access to The Database is Denied";
                }

                PropertyInfo[] properties = NewPerson.GetType().GetProperties();
                bool VerificationStatus;
                foreach (PropertyInfo item in properties)
                {
                    var propvalue = item.GetValue(NewPerson);
                    if (item.PropertyType == typeof(string) && item.Name != "AgeDescription")
                    {
                        VerificationStatus = (CheckInputForNullorEmpty(propvalue.ToString())) ? true : false;
                        if (!VerificationStatus) { return "Input Value was Null or Empty"; }
                        VerificationStatus = (CheckHtmlEncodedInput(propvalue.ToString())) ? true : false;
                        if (!VerificationStatus) { return "Input Value Contained Dangerous Code"; }

                    }
                    if (item.PropertyType == typeof(int))
                    {
                        VerificationStatus = (CheckInputPositiveAge(Convert.ToInt32(propvalue))) ? true : false;
                        if (!VerificationStatus) { return "Invalid Input Value"; }
                    }

                    if (item.PropertyType == typeof(long))
                    {
                        VerificationStatus = (CheckInputPositiveIDRange(Convert.ToInt64(propvalue))) ? true : false;
                        if (!VerificationStatus) { return "Invalid Input Value"; }
                    }
                }
                #region Update
                DataAccess.CreateConnection();
                string commandString = "update Person set FirstName = @FirstName ,LastName = @LastName ,Age = @Age where Id = @Id";
                DataAccess.CreateCommand(commandString, System.Data.CommandType.Text, true);
                DataAccess.CreateQueryParameter("@FirstName", System.Data.DbType.String, NewPerson.PersonFirstName);
                DataAccess.CreateQueryParameter("@LastName", System.Data.DbType.String, NewPerson.PersonLastName);
                DataAccess.CreateQueryParameter("@Age", System.Data.DbType.Int32, NewPerson.CurrentAge);
                DataAccess.CreateQueryParameter("@Id", System.Data.DbType.Int64, NewPerson.PersonID);
                DataAccess.CreateTransaction(true, System.Data.IsolationLevel.ReadUncommitted);
                if (DataAccess.ExecuteNonQuery() > 0)
                {
                    return "Person has been Succesfully Updated";
                }
                return "Person has not been Succesfully Updated";
                #endregion
            }
        }
    }
}


