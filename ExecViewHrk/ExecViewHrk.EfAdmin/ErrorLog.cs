namespace ExecViewHrk.EfAdmin
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ErrorLog")]
    public partial class ErrorLog
    {
        public int ErrorLogId { get; set; }

        public DateTime LogDate { get; set; }

        [StringLength(50)]
        public string UserName { get; set; }

        public string ExceptionMessage { get; set; }

        public string ExceptionStackTrace { get; set; }

        [StringLength(250)]
        public string ExceptionTargetSite { get; set; }

        [StringLength(250)]
        public string ExceptionSource { get; set; }

        public bool? IsLocale { get; set; }

        [StringLength(250)]
        public string RequestUrl { get; set; }

        [StringLength(250)]
        public string RequestUrlReferrer { get; set; }

        [StringLength(250)]
        public string RequestUserAgent { get; set; }

        [StringLength(250)]
        public string RequestPhysicalPath { get; set; }

        [StringLength(250)]
        public string ServerVariablesRemoteAddress { get; set; }

        public bool? CookiesEnabled { get; set; }

        public bool? JavaScriptEnabled { get; set; }

        [StringLength(50)]
        public string Browser { get; set; }

        public int? BrowserVersion { get; set; }

        public bool? IsAuthenticated { get; set; }
    }
}
