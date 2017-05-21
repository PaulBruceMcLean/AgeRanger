using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Age_Ranger.AgeRangerRepository;
using Age_Ranger.Models;
using Age_Ranger.CustomExtentionMethods;
using Age_Ranger.DataProvider;

namespace Age_Ranger.Tests
{
    [TestClass]
    public class InputParameterTests
    {
        #region Input Test
        [TestMethod]
        public void AddNewPersonModelIntegrity()
        {
            // arrange  
            PersonModel newPerson = null;
            IAgeRange<PersonModel> AgeRangeTest = new AgeRange();
            string ExpectedResult = "New Person Object cannot be null";
            // act  
            string ActualResult = AgeRangeTest.AddNewPerson(newPerson);
            // assert  
            Assert.AreEqual(ExpectedResult, ActualResult, "Invalid Details for the person have been supplied");

        }

        [TestMethod]
        public void AddNewPersonModelFirstNameIntegrity()
        {
            // arrange  
            PersonModel newPerson = new PersonModel();
            newPerson.PersonFirstName = "";
            newPerson.PersonLastName = "";
            newPerson.CurrentAge = -1;
            IAgeRange<PersonModel> AgeRangeTest = new AgeRange();
            string ExpectedResult = "Input Value was Null or Empty";
            // act  
            string ActualResult = AgeRangeTest.AddNewPerson(newPerson);
            // assert  
            Assert.AreEqual(ExpectedResult, ActualResult, "Invalid Details for the person have been supplied");
        }

        [TestMethod]
        public void AddNewPersonModelLastNameIntegrity()
        {
            // arrange  
            PersonModel newPerson = new PersonModel();
            newPerson.PersonFirstName = "Tony";
            newPerson.PersonLastName = "";
            newPerson.CurrentAge = -1;
            IAgeRange<PersonModel> AgeRangeTest = new AgeRange();
            string ExpectedResult = "Input Value was Null or Empty";
            // act  
            string ActualResult = AgeRangeTest.AddNewPerson(newPerson);
            // assert  
            Assert.AreEqual(ExpectedResult, ActualResult, "Invalid Details for the person have been supplied");
        }

        [TestMethod]
        public void AddNewPersonModelAgeNameIntegrity()
        {
            // arrange  
            PersonModel newPerson = new PersonModel();
            newPerson.PersonFirstName = "Tony";
            newPerson.PersonLastName = "Hawk";
            newPerson.CurrentAge = -1;
            IAgeRange<PersonModel> AgeRangeTest = new AgeRange();
            string ExpectedResult = "Invalid Input Value";
            // act  
            string ActualResult = AgeRangeTest.AddNewPerson(newPerson);
            // assert  
            Assert.AreEqual(ExpectedResult, ActualResult, "Invalid Details for the person have been supplied");
        }

        [TestMethod]
        public void JavascriptInjectionTest()
        {
            // arrange  
            IAgeRange<PersonModel> AgeRangeTest = new AgeRange();
            string HtmlEncodedString = "&lt;script&gt;alert(&quot;Boo!&quot;)&lt;/script&gt;";

            // assert  
            Assert.IsFalse(AgeRangeTest.CheckHtmlEncodedInput(HtmlEncodedString), "Potential Dangerous Code has been Supplied");
        }

        [TestMethod]
        public void NullorEmptyInputValidation()
        {
            // arrange  
            IAgeRange<PersonModel> AgeRangeTest = new AgeRange();
            string NullorEmptyTestString = string.Empty;

            // assert  
            Assert.IsFalse(AgeRangeTest.CheckInputForNullorEmpty(NullorEmptyTestString), "An input supplied has no value");
        }

        [TestMethod]
        public void AgeConvertionValidation()
        {

            // arrange  
            IAgeRange<PersonModel> AgeRangeTest = new AgeRange();
            int NegativeAge = -32;

            // assert  
            Assert.IsFalse(AgeRangeTest.CheckInputPositiveAge(NegativeAge), "Age Input cannot be a negative number");
        }

        [TestMethod]
        public void DeletePersonWithIDisValidRecordID()
        {
            // arrange  
            IAgeRange<PersonModel> AgeRangeTest = new AgeRange();
            string ExpectedResult = "Person can not have an ID record of Zero";
            long RecordID = 0;
            // act  
            string ActualResult = AgeRangeTest.DeletePerson(RecordID);
            // assert  
            Assert.AreEqual(ExpectedResult, ActualResult, "Invalid Details for the person have been supplied");
        }

        [TestMethod]
        public void DatabaseIDPositiveRange()
        {

            // arrange  
            IAgeRange<PersonModel> AgeRangeTest = new AgeRange();
            long NegativeAge = -32;

            // assert  
            Assert.IsFalse(AgeRangeTest.CheckInputPositiveIDRange(NegativeAge), "database Record ID cannot be a negative number");
        }

        [TestMethod]
        public void TestConnectionEncryption()
        {
            // arrange  
            //Data Source=C:\Users\Paul\Desktop\DebitSuccess\AgeRanger.db;Version=3;
            //Data Source=C:\Users\Paul\Desktop\DebitSuccess\AgeRanger.db;Version=3;
            //Server=Pauls;Database=DebitSuccess;uid=sa;pwd=*********;
            string Encryptiontest = @"Data Source=C:\Users\Paul\Desktop\DebitSuccess\AgeRanger.db;Version=3;";
            string Expected = "blablabla";
            string Actual = Encryptiontest.EncryptConnectionString();
            // assert 
            Assert.AreNotSame(Expected, Actual, "Encryption of an Empty String is Not possible");
        }

        [TestMethod]
        public void TestConnectionDecryption()
        {
            // arrange  
            string Decryptiontest = string.Empty;
            string Expected = "blablabla";
            string Actual = Decryptiontest.DecryptConnectionString();
            // assert 
            Assert.AreNotSame(Expected, Actual, "Decryption of an Empty String is Not possible");
        }
        #endregion


        #region DataProvider Test
        [TestMethod]
        public void DataProviderisNotNull()
        {
            Assert.IsInstanceOfType(SQLDataProvider.SQLProvider, typeof(SQLDataProvider), "Provider Can Be instantiated");
        }

        [TestMethod]
        public void DataProviderConfigurationSettings()
        {
            // arrange  
            SQLDataProvider provider = new SQLDataProvider();
            bool ExpectedResult = true;
            // act  
            bool ActualResult = provider.ConfigurationsSet;
            // assert  
            Assert.AreEqual(ExpectedResult, ActualResult, "Configurations Have not been Correctly Set");
        }


        [TestMethod]
        public void InstantiateProviderWithCorrectSettings()
        {
            // arrange  
            SQLDataProvider provider = new SQLDataProvider();
            Assert.IsFalse(provider.ConfigurationsSet, "Failed to Initialize DataProvider");
           
        }

        [TestMethod]
        public void QuerySQL_LITE_GetPerson()
        {
            // arrange  
            SQLDataProvider provider = new SQLDataProvider();
            provider.CreateConnection();
            string commandString = @"SELECT FirstName,LastName,Age,
                                     CASE WHEN Age <= 2 THEN 'Toddler'
                                          WHEN Age > 4999 THEN 'Kauri Tree'                                     
                                          ELSE(Select Description from AgeGroup where ps.Age >= MinAge and ps.Age <= MaxAge)
                                     END[Description]
                                     ,Id
                                     FROM Person ps
                                     where ps.FirstName like @FirstName";
            provider.CreateCommand(commandString, System.Data.CommandType.Text,true);
            provider.CreateQueryParameter("@FirstName", System.Data.DbType.String, "Corbin%");
            provider.CreateTransaction(true, System.Data.IsolationLevel.ReadCommitted);
            string Message = string.Empty;
            List<PersonModel> People = provider.ExecuteDataReader(ref Message);
        }

        [TestMethod]
        public void QuerySQL_LITE_InsertPerson()
        {
            string Message;

            PersonModel NewPerson = new PersonModel();
            NewPerson.PersonFirstName = "Sean";
            NewPerson.PersonLastName = "Gilmore";
            NewPerson.CurrentAge = 5000;

            SQLDataProvider provider = new SQLDataProvider();
            provider.CreateConnection();
            string commandString = "insert into Person(FirstName,LastName,Age) values(@FirstName,@LastName,@Age)";
            provider.CreateCommand(commandString, System.Data.CommandType.Text,true);
            provider.CreateQueryParameter("@FirstName", System.Data.DbType.String, NewPerson.PersonFirstName);
            provider.CreateQueryParameter("@LastName", System.Data.DbType.String, NewPerson.PersonLastName);
            provider.CreateQueryParameter("@Age", System.Data.DbType.Int32, NewPerson.CurrentAge);
            provider.CreateTransaction(true, System.Data.IsolationLevel.ReadCommitted);
            if (provider.ExecuteNonQuery() > 0)
            {
                Message = "New Person Added Succesfully";
            }
           
        }

        [TestMethod]
        public void QuerySQL_LITE_DeletePerson()
        {
            string Message;
            SQLDataProvider provider = new SQLDataProvider();
            provider.CreateConnection();
            string commandString = "delete from Person where Id = @ID";
            provider.CreateCommand(commandString, System.Data.CommandType.Text, true);
            provider.CreateQueryParameter("@ID", System.Data.DbType.Int64, 2);
            provider.CreateTransaction(true, System.Data.IsolationLevel.ReadCommitted);
            if (provider.ExecuteNonQuery() > 0)
            {
                Message = "Person Deleted";
            }
 }
        #endregion

    }

}
