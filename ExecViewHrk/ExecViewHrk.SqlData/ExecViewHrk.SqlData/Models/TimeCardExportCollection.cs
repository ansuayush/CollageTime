using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExecViewHrk.SqlData.Models
{
    public class TimeCardExportCollection
    {
           public short CompanyCode  { get; set;}
           public short BatchId { get; set; }
           public string FileNumber { get; set; }
           public double RegularHours { get; set; }
           public double OverTimeHours { get; set; }
           public string Hours3Code { get; set; }
           public double Hours3Amount { get; set; }
           public string Hours4Code { get; set; }
           public double Hours4Amount { get; set; }
           public string Hours5Code { get; set; }
           public double Hours5Amount { get; set; }
           public string Earnings3Code { get; set; }
           public double Earnings3Amount { get; set; }
           public string Earnings4Code { get; set; }
           public double Earnings4Amount { get; set; }
           public string Earnings5Code { get; set; }
           public double Earnings5Amount { get; set; }

           public override string ToString()
           {
               return  string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\"{8}\",\"{9}\",\"{10}\",\"{11}\",\"{12}\",\"{13}\",\"{14}\",\"{15}\",\"{16}\""
                                , CompanyCode, BatchId, FileNumber, RegularHours,OverTimeHours, Hours3Code,Hours3Amount,
                                Hours4Code, Hours4Amount, Hours5Code, Hours5Amount, Earnings3Code, Earnings3Amount,
                                Earnings4Code, Earnings4Amount, Earnings5Code, Earnings5Amount);

           }
    }
}
