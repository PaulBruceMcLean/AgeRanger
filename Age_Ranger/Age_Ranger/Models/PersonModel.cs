using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Age_Ranger.Models
{
    public class PersonModel
    {
        public long PersonID { get; set; }

        public string PersonFirstName { get; set; }

        public string PersonLastName { get; set; }

        public int CurrentAge { get; set; }

        public string AgeDescription { get; set; }
    }
}