using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.ModelBinding;

namespace ExecViewHrk.WebUI.Infrastructure
{
    public class ModelErrorsHelper
    {

        public static void AddErrorsFromResult(ref ModelStateDictionary modelState, IdentityResult result)
        {
            foreach (string error in result.Errors)
            {
                modelState.AddModelError("", error);
            }
        }
    }
}