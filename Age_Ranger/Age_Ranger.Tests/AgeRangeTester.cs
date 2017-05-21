using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Age_Ranger.AgeRangerRepository;
using Age_Ranger.Models;

namespace Age_Ranger.Tests
{
    [TestClass]
    public class UnitTest1
    {
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
            Assert.AreEqual(ExpectedResult, ActualResult,"Invalid Details for the person have been supplied");          
           
        }

        [TestMethod]
        public void AddNewPersonModelFirstNameIntegrity()
        {
            // arrange  
            PersonModel newPerson = new PersonModel();
            newPerson.PersonFirstName = "";
            newPerson.PersonLastName = "";
            newPerson.CurrentAge = "";
            IAgeRange<PersonModel> AgeRangeTest = new AgeRange();
            string ExpectedResult = "A First Name Must be Supplied";
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
            newPerson.CurrentAge = "";
            IAgeRange<PersonModel> AgeRangeTest = new AgeRange();
            string ExpectedResult = "A Last Name Must be Supplied";
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
            newPerson.CurrentAge = "";
            IAgeRange<PersonModel> AgeRangeTest = new AgeRange();
            string ExpectedResult = "The person's age must be a valid number";
            // act  
            string ActualResult = AgeRangeTest.AddNewPerson(newPerson);
            // assert  
            Assert.AreEqual(ExpectedResult, ActualResult, "Invalid Details for the person have been supplied");
        }
    }
}
