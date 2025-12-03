using ExecViewHrk.Domain.Helper;
using System;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using WinSCP;

namespace FinancialAidJob
{
    class FinancialAidJob
    {
        static void Main(string[] args)
        {
            string strFinancialReportPath = string.Empty;
            string strResults = FinancialReport();
            if (strResults != "No Records Found")
            {
                strFinancialReportPath = strResults;
                int nResult = UploadFiles(strFinancialReportPath);
                if (nResult == 0)
                {
                    EmailResults("The Student Financial Aid Report Uploaded on SFTP and also attached as an attachment.", strFinancialReportPath);
                }
                else
                {
                    EmailResults("Error Uploading Student Financial Aid Report on SFTP and also attached as an attachment.", strFinancialReportPath);
                }
            }
            else
            {
                EmailResults(strResults, strFinancialReportPath);
            }
        }

        private static int UploadFiles(string strFinancialReportPath)
        {
            try
            {
                string strActualsImportLocation = string.Empty;
                string strHosting = ConfigurationManager.AppSettings["Hosting"].ToString();
                SessionOptions sessionOptions = new SessionOptions
                {
                    Protocol = Protocol.Sftp,
                    HostName = "resnavweb.com",
                    UserName = strHosting == "Staging" ? "DrewUniversityStaging" : "DrewUniversity",
                    Password = strHosting == "Staging" ? "UY98sSaf" : "xZH97JaT",
                    PortNumber = 22,
                    SshHostKeyFingerprint = "ssh-rsa 1024 53:05:f6:b9:8a:b1:8e:0b:1d:d9:ab:4b:51:62:b3:92"
                };
                if (strHosting == "Staging")
                {
                    strActualsImportLocation = ConfigurationManager.AppSettings["ImportLocationStaging"].ToString();
                }
                else
                {
                    strActualsImportLocation = ConfigurationManager.AppSettings["ImportLocationProduction"].ToString();
                }
                using (Session session = new Session())
                {
                    session.Open(sessionOptions);
                    TransferOptions transferOptions = new TransferOptions();
                    transferOptions.TransferMode = TransferMode.Binary;
                    TransferOperationResult transferResult = default(TransferOperationResult);
                    transferResult = session.PutFiles(strFinancialReportPath, strActualsImportLocation, false, transferOptions);
                    transferResult.Check();
                }
                return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e);
                InsertLog(System.DateTime.Now, e.ToString());
                return 1;
            }
        }

        private static void EmailResults(string strResults, string filePath)
        {
            string strFrom = ConfigurationManager.AppSettings["fromAddress"].ToString();

            string strTo = ConfigurationManager.AppSettings["toAddress"].ToString();
            string strToSecond = ConfigurationManager.AppSettings["toAddressTwo"].ToString();
            string strToThree = ConfigurationManager.AppSettings["toAddressThree"].ToString();

            EmailProcessorCommunity.SendSingleFileWithAttachment(strFrom, strTo, strToSecond, strToThree, "Drew University Student Financial Aid Report", "This is an automated message from the Drew University Student Financial Aid Report Task."
                + Environment.NewLine + Environment.NewLine
                + strResults
                + Environment.NewLine + Environment.NewLine, false, filePath);
        }

        private static string FinancialReport()
        {
            String sbAllResults = string.Empty;
            var connString = ConfigurationManager.ConnectionStrings["execViewDb"].ConnectionString;
            SqlConnection con = new SqlConnection(connString);
            DataSet ds = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter("GetFinancialAidCustomReport", con);
            da.Fill(ds);
            DataTable dt = ds.Tables[0];
            if (dt.Rows.Count > 0)
            {
                var payperiodnumber = dt.Rows[0]["Pay Period Number"].ToString();
                var Year = DateTime.Now.Year;
                string strAppRoot = ConfigurationManager.AppSettings["appRoot"].ToString();
                string strActualsImportSubFolder = ConfigurationManager.AppSettings["ImportSubFolder"].ToString();
                string strFullPath = strAppRoot + strActualsImportSubFolder + "\\" + "StudentFinancialAidReport" + payperiodnumber + "_" + Year + ".csv";
                ExportToCsv(dt, strFullPath);
                sbAllResults = strFullPath;
            }
            else
            {
                sbAllResults = "No Records Found";
            }
            return sbAllResults;
        }



        public static void InsertLog(DateTime LogDate,  string ExceptionMessage)
        {
            var connString = ConfigurationManager.ConnectionStrings["execViewDb"].ConnectionString;
            SqlConnection mycon = new SqlConnection(connString);
            mycon.Open();
            string strquery = "INSERT INTO errorlog(LogDate, ExceptionMessage) values(@LogDate,@ExceptionMessage)";
           
            SqlCommand cmd = new SqlCommand(strquery, mycon);
            cmd.Parameters.AddWithValue("LogDate", LogDate);
            cmd.Parameters.AddWithValue("ExceptionMessage", ExceptionMessage);           
            cmd.ExecuteNonQuery();           
        }

        public static bool ExportToCsv(DataTable dt, string ExportToCsvPath)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                var columnNames = dt.Columns.Cast<DataColumn>().Select(column => "\"" + column.ColumnName.Replace("\"", "\"\"") + "\"").ToArray();
                sb.AppendLine(string.Join(",", columnNames));
                foreach (DataRow row in dt.Rows)
                {
                    var fields = row.ItemArray.Select(field => "\"" + field.ToString().Replace("\"", "\"\"") + "\"").ToArray();

                    var ssn = fields[5];
                    var insertValueAsSSN = "\t" + ssn;
                    var ssnNew = insertValueAsSSN.TrimStart('"');
                    var ssnEnd = ssnNew.TrimEnd('"');
                    fields[5] = ssnEnd.Replace("\"", "");

                    sb.AppendLine(string.Join(",", fields));
                }
                File.WriteAllText(ExportToCsvPath, sb.ToString(), Encoding.Default);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }




    }
}
