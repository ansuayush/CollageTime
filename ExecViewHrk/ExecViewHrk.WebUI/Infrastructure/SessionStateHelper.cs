using ExecViewHrk.EfClient;
using ExecViewHrk.Models;
using System;
using System.Linq;
using System.Web;

namespace ExecViewHrk.WebUI.Infrastructure
{

    public enum SessionStateKeys
    {
        SELECTED_CLIENT_ID,
        CLIENT_DB_CONNECT_STRING,
        PERSON_SELECTED_ID,
        EMPLOYEE_PERSON_SELECTED_ID,
        USER_MULTIPLE_COMPANIES_SELECTED_COMPANY_ID,
        EXTERNAL_USER_SELECTED_ID_CLIENT,
        CLIENT_ADMIN_EMPLOYER_ID,
        REQUEST_TYPE,
        LOGIN_PERSON_ID,
        BROWSER_LOGIN_TIME,   //Client browser time
        LOGGEDIN_USER_ROLE_NAME,
        EMPLOYEE_TIME_CARD_TYPE_CODE,
        LOGIN_PERSON_NAME,
        EMPLOYER_Name
    }

    public static class SessionStateHelper
    {
        public static object Get(SessionStateKeys key)
        {

            string keyString = Enum.GetName(typeof(SessionStateKeys), key);
            return HttpContext.Current.Session[keyString];

        }

        public static object Set(SessionStateKeys key, object value)
        {

            string keyString = Enum.GetName(typeof(SessionStateKeys), key);
            return HttpContext.Current.Session[keyString] = value;
        }

        public static void CheckForEmployeeSelectedValue()
        {
            string connString = HttpContext.Current.User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            if (Get(SessionStateKeys.EMPLOYEE_PERSON_SELECTED_ID) == null)
            {


                var peronsList = clientDbContext.Employees.Where(x => x.TerminationDate == null).Select(c => new PersonVm { PersonId = c.PersonId, PersonName = c.Person.Firstname + " " + c.Person.Lastname }).Distinct().OrderBy(s => s.PersonName).ToList();

                if (Get(SessionStateKeys.PERSON_SELECTED_ID) != null)
                {
                    var row = peronsList.FirstOrDefault(x => x.PersonId == Convert.ToInt32(Get(SessionStateKeys.PERSON_SELECTED_ID)));
                    if (row != null) {
                        Set(SessionStateKeys.EMPLOYEE_PERSON_SELECTED_ID, row.PersonId);
                    }
                }
                if (Get(SessionStateKeys.EMPLOYEE_PERSON_SELECTED_ID) == null)
                {
                    Set(SessionStateKeys.EMPLOYEE_PERSON_SELECTED_ID, peronsList.Any() ? peronsList.FirstOrDefault().PersonId : 0);
                }
            }
        }

        public static void CheckForPersonSelectedValue()
        {

            if (Get(SessionStateKeys.PERSON_SELECTED_ID) == null)
            {
                string connString = HttpContext.Current.User.Identity.GetClientConnectionString();
                ClientDbContext clientDbContext = new ClientDbContext(connString);

                var peronsList = clientDbContext.Persons.Select(c => new PersonVm { PersonId = c.PersonId, PersonName = c.Firstname + " " + c.Lastname }).Distinct().OrderBy(s => s.PersonName).ToList();

                Set(SessionStateKeys.PERSON_SELECTED_ID, peronsList.Any() ? peronsList.FirstOrDefault().PersonId : 0);
            }
        }
    }


}
