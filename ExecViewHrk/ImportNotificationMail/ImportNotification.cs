using ExecViewHrk.Domain.Helper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinSCP;

namespace ImportNotificationMail
{
    class ImportNotification
    {
        static int[] count = new int[10];
        static void Main(string[] args)
        {

            StringBuilder sbAllResults = new StringBuilder();
            List<string> strResults = new List<string>();
            //var empposreport = EmployeeWithPositionReport();
            //strResults.Add(empposreport);
            var morethanoneposreport = MoreThanOnePrimaryPositionReport();
            strResults.Add(morethanoneposreport);

            var multipleActiveSamePosition = MultipleActiveSamePosition();
            strResults.Add(multipleActiveSamePosition);

            var morethanoneactiveposratesreport = MoreThanOneActivePositionRatesReport();
            strResults.Add(morethanoneactiveposratesreport);

            var skippedBannerRecords = SkippedBannerRecords();
            strResults.Add(skippedBannerRecords);

            //var duplicatepositions = DuplicatePositionsReport();
            //strResults.Add(duplicatepositions);
            //var duplicatepositionid = duplicatepositionidReport();
            //strResults.Add(duplicatepositionid);
            //var duplicatepositionidhavingtime = DuplicatePositionIdhavingTimeReport();
            //strResults.Add(duplicatepositionidhavingtime);
            //var MultiplePrimaryPositionWithNewBannerFile = MultiplePrimaryPositionWithNewBannerFileReport();
            //strResults.Add(MultiplePrimaryPositionWithNewBannerFile);
            //var AllEmployeesCheckedIn = AllEmployeesCheckedInReport();
            //strResults.Add(AllEmployeesCheckedIn);
            var Recordsnotexistinbannerfile = RecordsnotexistinbannerfileReport();
            strResults.Add(Recordsnotexistinbannerfile);

            var ComparisonofactivepositionbetweenBannerandProduction = ComparisonofactivepositionbetweenBannerandProductionReport();
            strResults.Add(ComparisonofactivepositionbetweenBannerandProduction);

            //sbAllResults.AppendLine("Employee With Position Report Count is:"+ count[0]);
            //sbAllResults.Append(Environment.NewLine);
            sbAllResults.AppendLine("More Than One Primary Position Report Count is:" + count[0]);
            sbAllResults.Append(Environment.NewLine);

            sbAllResults.AppendLine("More Than One Active Same Position Report Count is:" + count[1]);
            sbAllResults.Append(Environment.NewLine);

            sbAllResults.AppendLine("More Than One Active Position Rates Report Count is:" + count[2]);
            sbAllResults.Append(Environment.NewLine);

            sbAllResults.AppendLine("Record Skipped in banner to Production Count is:" + count[3]);
            sbAllResults.Append(Environment.NewLine);

            //sbAllResults.AppendLine("Duplicate Positions Report Count is:" + count[3]);
            //sbAllResults.Append(Environment.NewLine);
            //sbAllResults.AppendLine("Duplicate Position Id Report Count is:" + count[4]);
            //sbAllResults.Append(Environment.NewLine);
            //sbAllResults.AppendLine("Duplicate Position Id  having Time Report Count is:" + count[5]);
            //sbAllResults.Append(Environment.NewLine);
            //sbAllResults.AppendLine("Multiple Primary Position With New Banner File Report Count is:" + count[6]);
            //sbAllResults.Append(Environment.NewLine);
            //sbAllResults.AppendLine("All Employees Checked In Report Count is:" + count[7]);
            //sbAllResults.Append(Environment.NewLine);
            sbAllResults.AppendLine("Records Not Exist In Banner File Report Count is:" + count[4]);
            sbAllResults.Append(Environment.NewLine);
            sbAllResults.AppendLine("Priamy position, Pay Rate, Position Start Date, Position End Date Comparison of active position between Banner and Production Report Count is:" + count[5]);
            sbAllResults.Append(Environment.NewLine);
            EmailResults(sbAllResults.ToString(), strResults);
        }

        private static void EmailResults(string strResults, List<string> filePath)
        {
            string strFrom = ConfigurationManager.AppSettings["fromAddress"].ToString();

            string strTo = ConfigurationManager.AppSettings["toAddress"].ToString();
            string strToSecond = ConfigurationManager.AppSettings["toAddressTwo"].ToString();
            string strToThree = ConfigurationManager.AppSettings["toAddressThree"].ToString();

            EmailProcessorCommunity.SendMultipleFileWithAttachment(strFrom, strTo, strToSecond, strToThree, "Drew University Notification Email", "This is an automated message from the Drew University Notification Email Task."
                + Environment.NewLine + Environment.NewLine
                + strResults
                + Environment.NewLine + Environment.NewLine, false, filePath);
        }

        //private static string EmployeeWithPositionReport()
        //{
        //    String sbAllResults = string.Empty;
        //    var connString = ConfigurationManager.ConnectionStrings["execViewDb"].ConnectionString;
        //    SqlConnection con = new SqlConnection(connString);
        //    DataSet ds = new DataSet();
        //    SqlDataAdapter da = new SqlDataAdapter("EmployeeWithPosition", con);
        //    da.Fill(ds);
        //    DataTable dt = ds.Tables[0];
        //    count[0] = dt.Rows.Count;
        //    if (dt.Rows.Count > 0)
        //    {

        //        string strAppRoot = ConfigurationManager.AppSettings["appRoot"].ToString();
        //        string strActualsImportSubFolder = ConfigurationManager.AppSettings["ImportSubFolder"].ToString();
        //        string strFullPath = strAppRoot + strActualsImportSubFolder + "\\" + "EmployeeWithPositionReport" + ".csv";
        //        ExportToCsv(dt, strFullPath);
        //        sbAllResults = strFullPath;
        //    }
        //    else
        //    {
        //        sbAllResults = "No Records Found";
        //    }
        //    return sbAllResults;
        //}

        private static string MultipleActiveSamePosition()
        {
            String sbAllResults = string.Empty;
            var connString = ConfigurationManager.ConnectionStrings["execViewDb"].ConnectionString;
            SqlConnection con = new SqlConnection(connString);
            DataSet ds = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter("MultipleActiveSamePosition", con);
            da.Fill(ds);
            DataTable dt = ds.Tables[0];
            count[1] = dt.Rows.Count;
            if (dt.Rows.Count > 0)
            {

                string strAppRoot = ConfigurationManager.AppSettings["appRoot"].ToString();
                string strActualsImportSubFolder = ConfigurationManager.AppSettings["ImportSubFolder"].ToString();
                string strFullPath = strAppRoot + strActualsImportSubFolder + "\\" + "MultipleActiveSamePosition" + ".csv";
                ExportToCsv(dt, strFullPath);
                sbAllResults = strFullPath;
            }
            else
            {
                sbAllResults = "No Records Found";
            }
            return sbAllResults;
        }


        private static string SkippedBannerRecords()
        {
            String sbAllResults = string.Empty;
            var connString = ConfigurationManager.ConnectionStrings["execViewDb"].ConnectionString;
            SqlConnection con = new SqlConnection(connString);
            DataSet ds = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter("SkippedBannerRecords", con);
            da.Fill(ds);
            DataTable dt = ds.Tables[0];
            count[3] = dt.Rows.Count;
            if (dt.Rows.Count > 0)
            {

                string strAppRoot = ConfigurationManager.AppSettings["appRoot"].ToString();
                string strActualsImportSubFolder = ConfigurationManager.AppSettings["ImportSubFolder"].ToString();
                string strFullPath = strAppRoot + strActualsImportSubFolder + "\\" + "SkippedBannerRecords" + ".csv";
                ExportToCsv(dt, strFullPath);
                sbAllResults = strFullPath;
            }
            else
            {
                sbAllResults = "No Records Found";
            }
            return sbAllResults;
        }

        private static string MoreThanOnePrimaryPositionReport()
        {
            String sbAllResults = string.Empty;
            var connString = ConfigurationManager.ConnectionStrings["execViewDb"].ConnectionString;
            SqlConnection con = new SqlConnection(connString);
            DataSet ds = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter("MoreThanOnePrimaryPosition", con);
            da.Fill(ds);
            DataTable dt = ds.Tables[0];
            count[0] = dt.Rows.Count;
            if (dt.Rows.Count > 0)
            {

                string strAppRoot = ConfigurationManager.AppSettings["appRoot"].ToString();
                string strActualsImportSubFolder = ConfigurationManager.AppSettings["ImportSubFolder"].ToString();
                string strFullPath = strAppRoot + strActualsImportSubFolder + "\\" + "MoreThanOnePrimaryPositionReport" + ".csv";
                ExportToCsv(dt, strFullPath);
                sbAllResults = strFullPath;
            }
            else
            {
                sbAllResults = "No Records Found";
            }
            return sbAllResults;
        }

        private static string MoreThanOneActivePositionRatesReport()
        {
            String sbAllResults = string.Empty;
            var connString = ConfigurationManager.ConnectionStrings["execViewDb"].ConnectionString;
            SqlConnection con = new SqlConnection(connString);
            DataSet ds = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter("MoreThanOneActivePositionRates", con);
            da.Fill(ds);
            DataTable dt = ds.Tables[0];
            count[2] = dt.Rows.Count;
            if (dt.Rows.Count > 0)
            {

                string strAppRoot = ConfigurationManager.AppSettings["appRoot"].ToString();
                string strActualsImportSubFolder = ConfigurationManager.AppSettings["ImportSubFolder"].ToString();
                string strFullPath = strAppRoot + strActualsImportSubFolder + "\\" + "MoreThanOneActivePositionRates" + ".csv";
                ExportToCsv(dt, strFullPath);
                sbAllResults = strFullPath;
            }
            else
            {
                sbAllResults = "No Records Found";
            }
            return sbAllResults;
        }

        //private static string DuplicatePositionsReport()
        //{
        //    String sbAllResults = string.Empty;
        //    var connString = ConfigurationManager.ConnectionStrings["execViewDb"].ConnectionString;
        //    SqlConnection con = new SqlConnection(connString);
        //    DataSet ds = new DataSet();
        //    SqlDataAdapter da = new SqlDataAdapter("DuplicatePositions", con);
        //    da.Fill(ds);
        //    DataTable dt = ds.Tables[0];
        //    count[3] = dt.Rows.Count;
        //    if (dt.Rows.Count > 0)
        //    {

        //        string strAppRoot = ConfigurationManager.AppSettings["appRoot"].ToString();
        //        string strActualsImportSubFolder = ConfigurationManager.AppSettings["ImportSubFolder"].ToString();
        //        string strFullPath = strAppRoot + strActualsImportSubFolder + "\\" + "DuplicatePositions" + ".csv";
        //        ExportToCsv(dt, strFullPath);
        //        sbAllResults = strFullPath;
        //    }
        //    else
        //    {
        //        sbAllResults = "No Records Found";
        //    }
        //    return sbAllResults;
        //}


        //private static string duplicatepositionidReport()
        //{
        //    String sbAllResults = string.Empty;
        //    var connString = ConfigurationManager.ConnectionStrings["execViewDb"].ConnectionString;
        //    SqlConnection con = new SqlConnection(connString);
        //    DataSet ds = new DataSet();
        //    SqlDataAdapter da = new SqlDataAdapter("duplicatepositionid", con);
        //    da.Fill(ds);
        //    DataTable dt = ds.Tables[0];
        //    count[4] = dt.Rows.Count;
        //    if (dt.Rows.Count > 0)
        //    {

        //        string strAppRoot = ConfigurationManager.AppSettings["appRoot"].ToString();
        //        string strActualsImportSubFolder = ConfigurationManager.AppSettings["ImportSubFolder"].ToString();
        //        string strFullPath = strAppRoot + strActualsImportSubFolder + "\\" + "DuplicatePositionId" + ".csv";
        //        ExportToCsv(dt, strFullPath);
        //        sbAllResults = strFullPath;
        //    }
        //    else
        //    {
        //        sbAllResults = "No Records Found";
        //    }
        //    return sbAllResults;
        //}

        //private static string DuplicatePositionIdhavingTimeReport()
        //{
        //    String sbAllResults = string.Empty;
        //    var connString = ConfigurationManager.ConnectionStrings["execViewDb"].ConnectionString;
        //    SqlConnection con = new SqlConnection(connString);
        //    DataSet ds = new DataSet();
        //    SqlDataAdapter da = new SqlDataAdapter("DuplicatePositionIdhavingTime", con);
        //    da.Fill(ds);
        //    DataTable dt = ds.Tables[0];
        //    count[5] = dt.Rows.Count;
        //    if (dt.Rows.Count > 0)
        //    {

        //        string strAppRoot = ConfigurationManager.AppSettings["appRoot"].ToString();
        //        string strActualsImportSubFolder = ConfigurationManager.AppSettings["ImportSubFolder"].ToString();
        //        string strFullPath = strAppRoot + strActualsImportSubFolder + "\\" + "DuplicatePositionIdhavingTime" + ".csv";
        //        ExportToCsv(dt, strFullPath);
        //        sbAllResults = strFullPath;
        //    }
        //    else
        //    {
        //        sbAllResults = "No Records Found";
        //    }
        //    return sbAllResults;
        //}

        //private static string MultiplePrimaryPositionWithNewBannerFileReport()
        //{
        //    String sbAllResults = string.Empty;
        //    var connString = ConfigurationManager.ConnectionStrings["execViewDb"].ConnectionString;
        //    SqlConnection con = new SqlConnection(connString);
        //    DataSet ds = new DataSet();
        //    SqlDataAdapter da = new SqlDataAdapter("MultiplePrimaryPositionWithNewBannerFile", con);
        //    da.Fill(ds);
        //    DataTable dt = ds.Tables[0];
        //    count[6] = dt.Rows.Count;
        //    if (dt.Rows.Count > 0)
        //    {

        //        string strAppRoot = ConfigurationManager.AppSettings["appRoot"].ToString();
        //        string strActualsImportSubFolder = ConfigurationManager.AppSettings["ImportSubFolder"].ToString();
        //        string strFullPath = strAppRoot + strActualsImportSubFolder + "\\" + "MultiplePrimaryPositionWithNewBannerFile" + ".csv";
        //        ExportToCsv(dt, strFullPath);
        //        sbAllResults = strFullPath;
        //    }
        //    else
        //    {
        //        sbAllResults = "No Records Found";
        //    }
        //    return sbAllResults;
        //}

        //private static string AllEmployeesCheckedInReport()
        //{
        //    String sbAllResults = string.Empty;
        //    var connString = ConfigurationManager.ConnectionStrings["execViewDb"].ConnectionString;
        //    SqlConnection con = new SqlConnection(connString);
        //    DataSet ds = new DataSet();
        //    SqlDataAdapter da = new SqlDataAdapter("AllEmployeesCheckedIn", con);
        //    da.Fill(ds);
        //    DataTable dt = ds.Tables[0];
        //    count[7] = dt.Rows.Count;
        //    if (dt.Rows.Count > 0)
        //    {

        //        string strAppRoot = ConfigurationManager.AppSettings["appRoot"].ToString();
        //        string strActualsImportSubFolder = ConfigurationManager.AppSettings["ImportSubFolder"].ToString();
        //        string strFullPath = strAppRoot + strActualsImportSubFolder + "\\" + "AllEmployeesCheckedInReport" + ".csv";
        //        ExportToCsv(dt, strFullPath);
        //        sbAllResults = strFullPath;
        //    }
        //    else
        //    {
        //        sbAllResults = "No Records Found";
        //    }
        //    return sbAllResults;
        //}

        private static string RecordsnotexistinbannerfileReport()
        {
            String sbAllResults = string.Empty;
            var connString = ConfigurationManager.ConnectionStrings["execViewDb"].ConnectionString;
            SqlConnection con = new SqlConnection(connString);
            DataSet ds = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter("Recordsnotexistinbannerfile", con);
            da.Fill(ds);
            DataTable dt = ds.Tables[0];
            count[4] = dt.Rows.Count;
            if (dt.Rows.Count > 0)
            {

                string strAppRoot = ConfigurationManager.AppSettings["appRoot"].ToString();
                string strActualsImportSubFolder = ConfigurationManager.AppSettings["ImportSubFolder"].ToString();
                string strFullPath = strAppRoot + strActualsImportSubFolder + "\\" + "RecordsNotExistInBannerFile" + ".csv";
                ExportToCsv(dt, strFullPath);
                sbAllResults = strFullPath;
            }
            else
            {
                sbAllResults = "No Records Found";
            }
            return sbAllResults;
        }

        private static string ComparisonofactivepositionbetweenBannerandProductionReport()
        {
            String sbAllResults = string.Empty;
            var connString = ConfigurationManager.ConnectionStrings["execViewDb"].ConnectionString;
            SqlConnection con = new SqlConnection(connString);
            DataSet ds = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter("ComparisonofactivepositionbetweenBannerandProduction", con);
            da.Fill(ds);
            DataTable dt = ds.Tables[0];
            count[5] = dt.Rows.Count;
            if (dt.Rows.Count > 0)
            {

                string strAppRoot = ConfigurationManager.AppSettings["appRoot"].ToString();
                string strActualsImportSubFolder = ConfigurationManager.AppSettings["ImportSubFolder"].ToString();
                string strFullPath = strAppRoot + strActualsImportSubFolder + "\\" + "ComparisonofactivepositionbetweenBannerandProduction" + ".csv";
                ExportToCsv(dt, strFullPath);
                sbAllResults = strFullPath;
            }
            else
            {
                sbAllResults = "No Records Found";
            }
            return sbAllResults;
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

                    //var ssn = fields[5];
                    //var insertValueAsSSN = "\t" + ssn;
                    //var ssnNew = insertValueAsSSN.TrimStart('"');
                    //var ssnEnd = ssnNew.TrimEnd('"');
                    //fields[5] = ssnEnd.Replace("\"", "");

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
