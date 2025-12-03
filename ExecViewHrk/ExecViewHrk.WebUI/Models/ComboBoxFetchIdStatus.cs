using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExecViewHrk.WebUI.Models
{
    public class ComboBoxFetchIdStatus
    {
        public bool succeed;
        public string Message;
        public int DbObject;

        public ComboBoxFetchIdStatus()
        {
            succeed = false;
            Message = string.Empty;
        }
    }
}