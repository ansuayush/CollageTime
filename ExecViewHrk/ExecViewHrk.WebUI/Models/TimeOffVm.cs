using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ExecViewHrk.WebUI.Models
{
    public class TimeOffVm
    {
        public IList<TimeOffEmpDetailsVm> TimeOffEmpDetailsList { get; set; }
        public IList<TimeOffTotalRequests> TimeOffRequestsList { get; set; }
    }       
}