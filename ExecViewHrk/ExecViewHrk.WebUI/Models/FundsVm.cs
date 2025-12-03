using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExecViewHrk.WebUI.Models
{
    public class FundsVm
    {
        public bool Active { get; internal set; }
        public string code { get; internal set; }
        public string Description { get; internal set; }
        public int ID { get; internal set; }
    }
}