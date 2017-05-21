using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Age_Ranger.AgeRangerRepository;
using Age_Ranger.Models;
using Newtonsoft.Json;

namespace Age_Ranger.Controllers
{
    public class PersonController : Controller
    {
        IAgeRange<PersonModel> gv_Age_Ranger;
        public PersonController()
        {
            gv_Age_Ranger = new AgeRange();
        }

        [Route("Person/OverView")]
        public ActionResult PersonOverView()
        {
            return View();
        }

        [HttpPost]
        [Route("Person/Add")]
        public string AddPerson(PersonModel NewPserson)
        {
            if (ModelState.IsValid)
            {
                return gv_Age_Ranger.AddNewPerson(NewPserson);
            }
            return "Data provide for the New Person was Invalid and could not be used";
        }

        [HttpPost]
        [Route("Person/Delete/{mv_PersonID}")]
        public string DeletePerson(long mv_PersonID)
        {
            if (ModelState.IsValid)
            {
                return gv_Age_Ranger.DeletePerson(mv_PersonID);
            }
            return "The ID for the Person you wish to Delete is invalid";
        }

        [HttpPost]
        [Route("api/Person/Update/")]
        public string UpdatePerson(PersonModel NewPserson)
        {
            if (ModelState.IsValid)
            {
                return gv_Age_Ranger.UpdatePerson(NewPserson);
            }

            return "Data provided for Person to be updated is Invalid and could not be used";
        }

        [HttpGet]
        [Route("Person/Find/{mv_PersonFirstName}")]
        public string FindPerson(string mv_PersonFirstName)
        {
            Dictionary<string, List<PersonModel>> Results = new Dictionary<string, List<PersonModel>>(); ;
            if (ModelState.IsValid)
            {               
                string OutPutMessage = string.Empty;
                var resultsSet = gv_Age_Ranger.FindPerson(mv_PersonFirstName, ref OutPutMessage);
                Results.Add(OutPutMessage, resultsSet);
                return JsonConvert.SerializeObject(Results, Formatting.Indented);
                
            }
            Results.Add("Invalid Search Criteria", new List<PersonModel>());
            return JsonConvert.SerializeObject(Results, Formatting.Indented);
        }
    }
}