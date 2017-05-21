using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Age_Ranger.DataProvider;

namespace Age_Ranger.AgeRangerRepository
{
    public interface IAgeRange<T> where T : class
    {
        string AddNewPerson(T NewPerson);
        string DeletePerson(long PersonID);
        List<T> FindPerson(string PersonFirstName, ref string OutPutMessage);
        string UpdatePerson(T NewPerson);
        bool CheckHtmlEncodedInput(string InputString);
        bool CheckInputForNullorEmpty(string InputString);
        bool CheckInputPositiveAge(int InputAge);
        bool CheckInputPositiveIDRange(long InputAge);
    }
}
