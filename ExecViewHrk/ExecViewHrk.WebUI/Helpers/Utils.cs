using ExecViewHrk.WebUI.Infrastructure;
using ExecViewHrk.WebUI.Models;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Data;
using System.Linq;
using ExecViewHrk.EfClient;
using System;
using System.Web.Mvc;
using System.Data.Entity.Validation;
using ExecViewHrk.WebUI.Helpers;
using ExecViewHrk;
using ExecViewHrk.Models;
using System.ComponentModel;

public static class Utils
{
    public static string GetClientConnectionString(this IIdentity identity)
    {
        var claim = ((ClaimsIdentity)identity).FindFirst(SessionStateKeys.CLIENT_DB_CONNECT_STRING.ToString());
        return claim == null ? null : claim.Value;
    }
    public static string GetBrowserLoginTime(this IIdentity identity)
    {
        var claim = ((ClaimsIdentity)identity).FindFirst(SessionStateKeys.BROWSER_LOGIN_TIME.ToString());
        return claim == null ? null : claim.Value;
    }
    public static string GetSelectedClientID(this IIdentity identity)
    {
        var claim = ((ClaimsIdentity)identity).FindFirst(SessionStateKeys.SELECTED_CLIENT_ID.ToString());
        return claim == null ? null : claim.Value;
    }
    public static string GetUserMultipleCompaniesSelectedCompanyID(this IIdentity identity)
    {
        var claim = ((ClaimsIdentity)identity).FindFirst(SessionStateKeys.USER_MULTIPLE_COMPANIES_SELECTED_COMPANY_ID.ToString());
        return claim == null ? null : claim.Value;
    }
    public static string GetClientAdminEmployerID(this IIdentity identity)
    {
        var claim = ((ClaimsIdentity)identity).FindFirst(SessionStateKeys.CLIENT_ADMIN_EMPLOYER_ID.ToString());
        return claim == null ? null : claim.Value;
    }
    public static string GetExternalUserSelectedClientID(this IIdentity identity)
    {
        var claim = ((ClaimsIdentity)identity).FindFirst(SessionStateKeys.EXTERNAL_USER_SELECTED_ID_CLIENT.ToString());
        return claim == null ? null : claim.Value;
    }
    public static string GetRequestType(this IIdentity identity)
    {
        //identity.GetRequestType
        var claim = ((ClaimsIdentity)identity).FindFirst(SessionStateKeys.REQUEST_TYPE.ToString());
        return claim == null ? "" : claim.Value;
    }
    public static void AddUpdateClaim(this IIdentity identity, string key, string value)
    {

        // check for existing claim and remove it
        var existingClaim = ((ClaimsIdentity)identity).FindFirst(key);
        if (existingClaim != null) ((ClaimsIdentity)identity).RemoveClaim(existingClaim);

        // add new claim
        ((ClaimsIdentity)identity).AddClaim(new Claim(key, value));
        //var authenticationManager = HttpContext.Current.GetOwinContext().Authentication;
        //authenticationManager.AuthenticationResponseGrant = new AuthenticationResponseGrant(new ClaimsPrincipal(identity), new AuthenticationProperties() { IsPersistent = true });
    }

    public static List<DropDownModel> CleanUp(this List<DropDownModel> collection, bool addEmptyItem = true, bool keepWhitespaceValues = false, bool trimValue = true, bool trimDescription = false)
    {
        collection.RemoveAll(i =>
            i.keydescription == null
            || (!keepWhitespaceValues && string.IsNullOrWhiteSpace(i.keyvalue))
        );

        if (addEmptyItem)
        {
            //collection.Insert(0, new DropDownModel { keyvalue = "0", keydescription = "Select" });
            collection.Insert(0, new DropDownModel { keyvalue = "", keydescription = "Select" });
        }

        return collection.Select(i => new DropDownModel
            {
                keyvalue = (!trimValue ? i.keyvalue : i.keyvalue.Trim()),
                keydescription = (!trimDescription ? i.keydescription : i.keydescription.Trim())
            }).ToList();
    }


    public static string GetErrorString(Exception _ex, ClientDbContext clientDbContext, ModelStateDictionary modelState)
    {
        string message = "";
        if (clientDbContext != null)
        {
            IEnumerable<DbEntityValidationResult> errors = clientDbContext.GetValidationErrors();
            if (errors.Count() == 0)
            {
                if (_ex != null)
                {
                    message += _ex.InnerException.InnerException.Message;
                }
                if (message.Contains(" Cannot insert duplicate key"))
                {
                    message = CustomErrorMessages.ERROR_DUPLICATE_RECORD;
                }
            }
            else
            {
                foreach (DbEntityValidationResult error in errors)
                {
                    foreach (var valError in error.ValidationErrors)
                    {
                        if (message != "") message += "<br />";
                        message += valError.ErrorMessage;
                    }
                }
            }
        }
        else
        {
            var modelStateErrors = modelState.Keys.SelectMany(key => modelState[key].Errors);

            foreach (var item in modelStateErrors)
            {
                if (message != "") message += "<br />";
                message += item.ErrorMessage;
            }
        }

        return message;
    }

    public static string GetLoggedInUserRoleName(this IIdentity identity)
    {
        var claim = ((ClaimsIdentity)identity).FindFirst(SessionStateKeys.LOGGEDIN_USER_ROLE_NAME.ToString());
        return claim == null ? null : claim.Value;
    }

    public static string GetEmployeeTimeCardTypeCode(this IIdentity identity)
    {
        var claim = ((ClaimsIdentity)identity).FindFirst(SessionStateKeys.EMPLOYEE_TIME_CARD_TYPE_CODE.ToString());
        return claim == null ? null : claim.Value;
    }

    /// <summary>
    /// This method will convert the given date time in utc to specified timezone date
    /// </summary>
    /// <param name="utcDate">Utc DateTime</param>
    /// <param name="timeZone">TimeZone Name. Ex: Eastern Standard Time</param>
    /// <returns></returns>
    public static DateTime ConvertTimeFromUtc(DateTime utcDate, string timeZone)
    {
        return TimeZoneInfo.ConvertTimeFromUtc(utcDate, TimeZoneInfo.FindSystemTimeZoneById(timeZone));
    }

    public static string GetPersonName(this IIdentity identity)
    {
        var claim = ((ClaimsIdentity)identity).FindFirst(SessionStateKeys.LOGIN_PERSON_NAME.ToString());
        return claim == null ? null : claim.Value;
    }

    public static DataTable AsDataTable<T>(this IEnumerable<T> data)
    {
        PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
        var table = new DataTable();
        foreach (PropertyDescriptor prop in properties)
            table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
        foreach (T item in data)
        {
            DataRow row = table.NewRow();
            foreach (PropertyDescriptor prop in properties)
                row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
            table.Rows.Add(row);
        }
        return table;
    }
    public static string GetEmployerName(this IIdentity identity)
    {
        var claim = ((ClaimsIdentity)identity).FindFirst(SessionStateKeys.EMPLOYER_Name.ToString());
        return claim?.Value;
    }

}
