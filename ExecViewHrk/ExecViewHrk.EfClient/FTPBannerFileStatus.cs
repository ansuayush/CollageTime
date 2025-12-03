using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExecViewHrk.EfClient
{
    public partial class FTPBannerFileStatus
    {
        [Key]
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int NoOfTimeProcessed { get; set; }
        public bool IsStudentBannerStatus { get; set; }
        public bool IsEmailSent { get; set; }
    }
}
