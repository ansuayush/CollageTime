using ExecViewHrk.Domain.Helper;
using ExecViewHrk.EfClient;
using ExecViewHrk.WebUI.Models;
using ImportCityOfAlPaso;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using TakeIo.Spreadsheet;
using WinSCP;

namespace ImportAlicePeck
{
    class ImportAlicePeck
    {
        static void Main(string[] args)
        {
            try
            {
                StringBuilder sbAllResults = new StringBuilder();
                List<string> strFileDownLoadList = new List<string>();
                List<string> strRenamedFileList = new List<string>();
                List<string> strAllImportedFiles = new List<string>();
                List<string> strAllImportedFullPaths = new List<string>();
                int nResult = DownloadFiles(ref strFileDownLoadList);
                string strHosting = ConfigurationManager.AppSettings["Hosting"].ToString();
                string ProcessedFileName = string.Empty;
                if (strFileDownLoadList.Count == 0)
                {
                    ProcessedFileName = "Drew University Import Results in " + strHosting;
                    EmailResults("No files found on ftp server.", 1, strAllImportedFiles, ProcessedFileName);
                    //updateSFTPBannerFileStatus(string.Empty);
                    return;
                }
                List<bool> bFileRenamed = new List<bool>();
                StringBuilder sbRenameResults = RenameFiles(strFileDownLoadList, strRenamedFileList, bFileRenamed);
                StringBuilder sbResults = new StringBuilder();
                foreach (string strFileName in strRenamedFileList)
                {
                    strAllImportedFiles.Add(strFileName);
                    string typeOfFile = TypeOfFile(strFileName);
                    if (typeOfFile == "Student Banner File")
                    {
                        ProcessedFileName = "Drew University New Student Banner File Import Results in " + strHosting;
                        sbResults = ImportBannerData(strFileName);
                        creatingAspNetUserNamePassword();
                        //updateSFTPBannerFileStatus(strFileName);
                    }
                    else
                    {
                        sbResults.AppendLine("Results of import for file = " + strFileName);
                        sbResults.AppendLine("");
                        sbResults = sbResults.Append(typeOfFile);
                    }
                    sbAllResults.AppendLine("");
                    sbAllResults.AppendLine("*******************************************************************************************************");
                    sbAllResults.AppendLine(sbResults.ToString());
                    sbAllResults.AppendLine("");
                }
                int nLoopCount = 0;
                sbAllResults.AppendLine("");
                sbAllResults.AppendLine("*******************************************************************************************************");
                sbAllResults.AppendLine("File deleting results follows");
                sbAllResults.AppendLine("");
                foreach (string strFileWithPath in strFileDownLoadList)
                {
                    string[] strFileParts = strFileWithPath.Split('/');
                    string strFileName = strFileParts[strFileParts.Length - 1];
                    if (bFileRenamed[nLoopCount])
                    {
                        int nErr = DeleteFile(strFileName);
                        if (nErr == 0)
                            sbAllResults.AppendLine("File " + strFileName + " deleted from ftp server");
                        else
                            sbAllResults.AppendLine("Could not delete file " + strFileName + " from ftp server");
                    }
                    else
                        sbAllResults.AppendLine("Could not delete file " + strFileName + " from ftp server");
                    nLoopCount++;
                }
                string strFileOut = string.Empty;
                string strResults = sbAllResults.ToString();
                strResults = strResults.Replace("<br />", "");
                writeResultsToFile(strResults, out strFileOut);
                string strAppRoot = ConfigurationManager.AppSettings["appRoot"].ToString();
                string strActualsImportSubFolder = string.Empty;
                if (strHosting == "Staging")
                {
                    strActualsImportSubFolder = ConfigurationManager.AppSettings["ImportSubFolderStaging"].ToString();
                }
                else
                {
                    strActualsImportSubFolder = ConfigurationManager.AppSettings["ImportSubFolderProduction"].ToString();
                }
                foreach (string strFilePath in strAllImportedFiles)
                {
                    string strFullPath = strAppRoot + strActualsImportSubFolder + "\\" + strFilePath;
                    strAllImportedFullPaths.Add(strFullPath);
                }
                EmailResults(strResults, 1, strAllImportedFullPaths, ProcessedFileName);
            }
            catch (Exception err)
            {
                Console.WriteLine("Error importing file.   " + err.Message);
            }
        }

        private static string GetPassword(string password)
        {
            var userStore = new UserStore<IdentityUser>();
            var userManager = new UserManager<IdentityUser>(userStore);
            var hashPassword = userManager.PasswordHasher.HashPassword(password);
            return hashPassword;
        }
        private static DataTable PersonTable()
        {
            var dt = new DataTable();
            dt.Columns.Add("AspNetUserId", typeof(string));
            dt.Columns.Add("PersonId", typeof(Int32));
            dt.Columns.Add("LastName", typeof(string));
            dt.Columns.Add("FisrtName", typeof(string));
            dt.Columns.Add("PasswordHash", typeof(string));
            dt.Columns.Add("SecurityStamp", typeof(string));
            dt.Columns.Add("EMail", typeof(string));
            return dt;
        }
        private static void creatingAspNetUserNamePassword()
        {
            try
            {
                string connString = ConfigurationManager.ConnectionStrings["execViewDb"].ConnectionString;
                var clientDbContext = new ClientDbContext(connString);
                var personList = clientDbContext.Persons.ToList();
                var dtPerson = PersonTable();
                string ssn = null, last4DigitSSN = "", passwordHash = "";
                foreach (var person in personList)
                {
                    ssn = person.SSN;
                    if (string.IsNullOrEmpty(ssn))
                    {
                        continue;
                    }
                    last4DigitSSN = ssn.Substring(ssn.Length - 4, 4);
                    passwordHash = GetPassword(last4DigitSSN);
                    dtPerson.Rows.Add(Guid.NewGuid().ToString(), person.PersonId, person.Lastname, person.Firstname, passwordHash, last4DigitSSN, person.eMail);
                }
                if (dtPerson.Rows.Count > 0)
                {
                    ImportEmployeeManager(connString, dtPerson);
                    GenerateEmployeeManager(connString);
                }
            }
            catch (Exception)
            {
            }
        }
        private static void ImportEmployeeManager(string _connectionString, DataTable dtPersonInfo)
        {
            var con = new SqlConnection(_connectionString);
            var cmd = con.CreateCommand();
            cmd.CommandText = ("dbo.spImportEmployeeManager");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@PersonInfo", dtPersonInfo);
            cmd.CommandTimeout = 0;
            try
            {
                con.Open();
                cmd.ExecuteNonQuery();
            }
            finally
            {
                con.Close();
            }
        }
        private static void GenerateEmployeeManager(string connString)
        {
            var con = new SqlConnection(connString);
            var cmd = con.CreateCommand();
            cmd.CommandText = ("dbo.spGenerateEmployee");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 0;
            try
            {
                con.Open();
                cmd.ExecuteNonQuery();
            }
            finally
            {
                con.Close();
            }
        }
        private static string TypeOfFile(string strFile)
        {
            string strFullPath = string.Empty;
            try
            {
                var strAppRoot = ConfigurationManager.AppSettings["appRoot"].ToString();
                string strHosting = ConfigurationManager.AppSettings["Hosting"].ToString();
                string strActualsImportSubFolder = string.Empty;
                if (strHosting == "Staging")
                {
                    strActualsImportSubFolder = ConfigurationManager.AppSettings["ImportSubFolderStaging"].ToString();
                }
                else
                {
                    strActualsImportSubFolder = ConfigurationManager.AppSettings["ImportSubFolderProduction"].ToString();
                }
                strFullPath = strAppRoot + strActualsImportSubFolder + "\\" + strFile;
                var inputFile = new FileInfo(strFullPath);
                var sheet = Spreadsheet.Read(inputFile);
                foreach (var row in sheet)
                {
                    if (row[0].ToLower().Trim() == "nzrmast_pidm")
                    {
                        return "Student Banner File";
                    }
                    else
                        return "Cannot determine type of import file";
                }
            }
            catch (Exception)
            {
            }
            return "Cannot determine type of import file";
        }
        private static StringBuilder RenameFiles(List<string> strFileDownLoadList, List<string> strRenamedFileList, List<bool> bFileRenamed)
        {
            StringBuilder sbResults = new StringBuilder();
            string strNewFileName = string.Empty;
            string strPath = string.Empty;
            string strNewPathAndFile = string.Empty;
            try
            {
                string strClientId = ConfigurationManager.AppSettings["ClientID"].ToString();
                string strAppRoot = ConfigurationManager.AppSettings["appRoot"].ToString();
                string strHosting = ConfigurationManager.AppSettings["Hosting"].ToString();
                string strActualsImportSubFolder = string.Empty;
                if (strHosting == "Staging")
                {
                    strActualsImportSubFolder = ConfigurationManager.AppSettings["ImportSubFolderStaging"].ToString();
                }
                else
                {
                    strActualsImportSubFolder = ConfigurationManager.AppSettings["ImportSubFolderProduction"].ToString();
                }
                strPath = strAppRoot + strActualsImportSubFolder;
                for (int i = 0; i < strFileDownLoadList.Count; i++)
                {
                    string[] strFileParts = strFileDownLoadList[i].Split('/');
                    string strInitialFileName = strFileParts[strFileParts.Length - 1];
                    strNewFileName = strClientId + "Import_" + DateTime.Now + "-" + DateTime.Now.Ticks + strInitialFileName;
                    strNewFileName = strNewFileName.Replace("/", "-");
                    strNewFileName = strNewFileName.Replace(":", "-");
                    strNewPathAndFile = strPath + "\\" + strNewFileName;
                    string strOldPathAndFileName = strPath + "\\" + strInitialFileName;
                    bFileRenamed.Add(false);
                    if (File.Exists(strOldPathAndFileName))
                    {
                        File.Move(strOldPathAndFileName, strNewPathAndFile);
                        bFileRenamed[i] = true;
                    }
                    strRenamedFileList.Add(strNewFileName);
                }
                sbResults.AppendLine("Renaming of files completed successfully.");
                sbResults.AppendLine("");
                return sbResults;
            }
            catch (Exception err)
            {
                sbResults.AppendLine("An error(s) was detected renaming the file(s) as follows:");
                sbResults.AppendLine(err.Message);
                sbResults.AppendLine("");
                return sbResults;
            }
        }
        private static void EmailResults(string strResults, int numberOfAttempts, List<string> strAllImportedFullPaths, string processedFileName)
        {
            string strFrom = ConfigurationManager.AppSettings["fromAddress"].ToString();
            string strTo = ConfigurationManager.AppSettings["toAddress"].ToString();
            string strToSecond = ConfigurationManager.AppSettings["toAddressTwo"].ToString();
            string strToThree = ConfigurationManager.AppSettings["toAddressThree"].ToString();
            string strToFour = ConfigurationManager.AppSettings["toAddressFour"].ToString();
            string strToFive = ConfigurationManager.AppSettings["toAddressFive"].ToString();
            EmailProcessorCommunity.SendWithAttachment(strFrom, strTo, strToSecond, strToThree, strToFour, strToFive, processedFileName, "This is an automated message from the Drew University Import Task. Results of import are as follows: "
                + Environment.NewLine + Environment.NewLine
                + strResults
                + Environment.NewLine + Environment.NewLine
                + "Number Of Attempts = " + numberOfAttempts.ToString(), false, strAllImportedFullPaths);
        }
        private static int DownloadFiles(ref List<string> strFileDownLoadList)
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
                string strAppRoot = ConfigurationManager.AppSettings["appRoot"].ToString();
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
                    TransferOperationResult transferResult;
                    transferResult = session.GetFiles(strActualsImportLocation, strAppRoot + @"\ImportCsv\*", false, transferOptions);
                    transferResult.Check();
                    foreach (TransferEventArgs transfer in transferResult.Transfers)
                    {
                        strFileDownLoadList.Add(transfer.FileName);
                        Console.WriteLine("Upload of {0} succeeded", transfer.FileName);
                    }
                }
                return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e);
                return 1;
            }
        }
        private static int DeleteFile(string strFile)
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
                string strAppRoot = ConfigurationManager.AppSettings["appRoot"].ToString();
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
                    RemovalOperationResult opResult = session.RemoveFiles(strActualsImportLocation + "/" + strFile);
                    opResult.Check();
                    foreach (RemovalEventArgs removal in opResult.Removals)
                    {
                        Console.WriteLine("Removal of {0} succeeded", removal.FileName);
                    }
                }
                return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e);
                return 1;
            }
        }
        private static StringBuilder ImportBannerData(string strFile)
        {
            var sbErrors = new StringBuilder();
            int nLoopCount = 0;
            string strFullPath = string.Empty;
            try
            {
                sbErrors.AppendLine("Results of import for file = " + strFile);
                sbErrors.AppendLine("");
                string strAppRoot = ConfigurationManager.AppSettings["appRoot"].ToString();
                string strConnection = ConfigurationManager.ConnectionStrings["execViewDb"].ConnectionString;
                var sqlConnection = new SqlConnection(strConnection);
                PositionCategories positionCategories = new PositionCategories(sqlConnection);
                DataRow drPosCat = positionCategories.GetRecordForCode("DEFAULT");
                int nPosCat_ID = 0;
                if (drPosCat == null)
                    nPosCat_ID = positionCategories.Insert("DEFAULT", "DEFAULT", true);
                else
                    nPosCat_ID = Convert.ToInt32(drPosCat["PositionCategoryID"]);

                string strHosting = ConfigurationManager.AppSettings["Hosting"].ToString();
                string strActualsImportSubFolder = string.Empty;
                if (strHosting == "Staging")
                {
                    strActualsImportSubFolder = ConfigurationManager.AppSettings["ImportSubFolderStaging"].ToString();
                }
                else
                {
                    strActualsImportSubFolder = ConfigurationManager.AppSettings["ImportSubFolderProduction"].ToString();
                }
                strFullPath = strAppRoot + strActualsImportSubFolder + "\\" + strFile;
                var inputFile = new FileInfo(strFullPath);
                var sheet = Spreadsheet.Read(inputFile);
                var newBannerImportFileList = GenerateNewBannerImportList(sheet);
                BulkCopyData(strConnection, "NewBannerImportFile", newBannerImportFileList.AsDataTable());
                bool bErrorFound = false;
                var clientDbContext = new ClientDbContext(strConnection);
                foreach (var row in sheet)
                {
                    nLoopCount++;
                    if (nLoopCount == 1)
                    {
                        if (LowerTrimFunction(row[0]) != "nzrmast_pidm")
                        {
                            sbErrors.AppendLine("Column A is named: " + row[0] + " and should be named: nzrmast_pidm.");
                            bErrorFound = true;
                        }
                        if (LowerTrimFunction(row[1]) != "nzrmast_id")
                        {
                            sbErrors.AppendLine("Column B is named: " + row[1] + " and should be named: nzrmast_id.");
                            bErrorFound = true;
                        }
                        if (LowerTrimFunction(row[2]) != "nzrmast_adpid")
                        {
                            sbErrors.AppendLine("Column C is named: " + row[2] + " and should be named: nzrmast_adpid");
                            bErrorFound = true;
                        }
                        if (LowerTrimFunction(row[3]) != "nzrmast_employee_last_name")
                        {
                            sbErrors.AppendLine("Column D is named: " + row[3] + " and should be named: nzrmast_employee_last_name");
                            bErrorFound = true;
                        }
                        if (LowerTrimFunction(row[4]) != "nzrmast_employee_first_name")
                        {
                            sbErrors.AppendLine("Column E is named: " + row[4] + " and should be named: nzrmast_employee_first_name.");
                            bErrorFound = true;
                        }
                        if (LowerTrimFunction(row[5]) != "nzrmast_employee_mi")
                        {
                            sbErrors.AppendLine("Column F is named: " + row[5] + " and should be named: nzrmast_employee_mi.");
                            bErrorFound = true;
                        }
                        if (LowerTrimFunction(row[6]) != "nzrmast_taxid")
                        {
                            sbErrors.AppendLine("Column G is named: " + row[6] + " and should be named: nzrmast_taxid.");
                            bErrorFound = true;
                        }
                        if (LowerTrimFunction(row[7]) != "nzrmast_birth_date")
                        {
                            sbErrors.AppendLine("Column H is named: " + row[7] + " and should be named: nzrmast_birth_date.");
                            bErrorFound = true;
                        }
                        if (LowerTrimFunction(row[8]) != "nzrmast_empr_code")
                        {
                            sbErrors.AppendLine("Column I is named: " + row[8] + " and should be named: nzrmast_empr_code.");
                            bErrorFound = true;
                        }
                        if (LowerTrimFunction(row[9]) != "nzrmast_email_address")
                        {
                            sbErrors.AppendLine("Column J is named: " + row[9] + " and should be named: nzrmast_email_address.");
                            bErrorFound = true;
                        }
                        if (LowerTrimFunction(row[10]) != "nzrmast_home_phone")
                        {
                            sbErrors.AppendLine("Column K is named: " + row[10] + " and should be named: nzrmast_home_phone.");
                            bErrorFound = true;
                        }
                        if (LowerTrimFunction(row[11]) != "nzrmast_hire_status")
                        {
                            sbErrors.AppendLine("Column L is named: " + row[11] + " and should be named: nzrmast_hire_status.");
                            bErrorFound = true;
                        }
                        if (LowerTrimFunction(row[12]) != "nzrmast_current_hire_date")
                        {
                            sbErrors.AppendLine("Column M is named: " + row[12] + " and should be named: nzrmast_current_hire_date.");
                            bErrorFound = true;
                        }
                        if (LowerTrimFunction(row[13]) != "nzrmast_termination_date")
                        {
                            sbErrors.AppendLine("Column N is named: " + row[13] + " and should be named: nzrmast_termination_date.");
                            bErrorFound = true;
                        }
                        if (LowerTrimFunction(row[14]) != "nzrmast_home_orgn_code")
                        {
                            sbErrors.AppendLine("Column O is named: " + row[14] + " and should be named: nzrmast_home_orgn_code.");
                            bErrorFound = true;
                        }
                        if (LowerTrimFunction(row[15]) != "nzrmast_home_orgn_code_desc")
                        {
                            sbErrors.AppendLine("Column P is named: " + row[15] + " and should be named: nzrmast_home_orgn_code_desc.");
                            bErrorFound = true;
                        }
                        if (LowerTrimFunction(row[16]) != "nzrmast_pay_frequency_code")
                        {
                            sbErrors.AppendLine("Column Q is named: " + row[16] + " and should be named: nzrmast_pay_frequency_code.");
                            bErrorFound = true;
                        }
                        if (LowerTrimFunction(row[17]) != "nzrmast_ecls_code")
                        {
                            sbErrors.AppendLine("Column R is named: " + row[17] + " and should be named: nzrmast_ecls_code.");
                            bErrorFound = true;
                        }
                        if (LowerTrimFunction(row[18]) != "nzrmast_position")
                        {
                            sbErrors.AppendLine("Column S is named: " + row[18] + " and should be named: nzrmast_position.");
                            bErrorFound = true;
                        }
                        if (LowerTrimFunction(row[19]) != "nzrmast_suff")
                        {
                            sbErrors.AppendLine("Column T is named: " + row[19] + " and should be named: nzrmast_suff.");
                            bErrorFound = true;
                        }
                        if (LowerTrimFunction(row[20]) != "nzrmast_job_orgn_code")
                        {
                            sbErrors.AppendLine("Column U is named: " + row[20] + " and should be named: nzrmast_job_orgn_code.");
                            bErrorFound = true;
                        }
                        if (LowerTrimFunction(row[21]) != "nzrmast_job_orgn_code_desc")
                        {
                            sbErrors.AppendLine("Column V is named: " + row[21] + " and should be named: nzrmast_job_orgn_code_desc.");
                            bErrorFound = true;
                        }
                        if (LowerTrimFunction(row[22]) != "nzrmast_job_status")
                        {
                            sbErrors.AppendLine("Column W is named: " + row[22] + " and should be named: nzrmast_job_status.");
                            bErrorFound = true;
                        }
                        if (LowerTrimFunction(row[23]) != "nzrmast_primary_ind")
                        {
                            sbErrors.AppendLine("Column X is named: " + row[23] + " and should be named: nzrmast_job_status.");
                            bErrorFound = true;
                        }
                        if (LowerTrimFunction(row[24]) != "nzrmast_job_desc")
                        {
                            sbErrors.AppendLine("Column Y is named: " + row[24] + " and should be named: nzrmast_job_desc");
                            bErrorFound = true;
                        }
                        if (LowerTrimFunction(row[25]) != "nzrmast_start_date")
                        {
                            sbErrors.AppendLine("Column Z is named: " + row[25] + " and should be named: nzrmast_start_date");
                            bErrorFound = true;
                        }
                        if (LowerTrimFunction(row[26]) != "nzrmast_end_date")
                        {
                            sbErrors.AppendLine("Column AA is named: " + row[26] + " and should be named:nzrmast_end_date.");
                            bErrorFound = true;
                        }
                        if (LowerTrimFunction(row[27]) != "nzrmast_eff_date")
                        {
                            sbErrors.AppendLine("Column AB is named: " + row[27] + " and should be named:nzrmast_eff_date");
                            bErrorFound = true;
                        }
                        if (LowerTrimFunction(row[28]) != "nzrmast_rate_amount")
                        {
                            sbErrors.AppendLine("Column AC is named: " + row[28] + " and should be named:nzrmast_rate_amount");
                            bErrorFound = true;
                        }
                        if (LowerTrimFunction(row[29]) != "nzrmast_rate_type")
                        {
                            sbErrors.AppendLine("Column AD is named: " + row[29] + " and should be named:nzrmast_rate_type");
                            bErrorFound = true;
                        }
                        if (LowerTrimFunction(row[30]) != "nzrmast_pay_group_code")
                        {
                            sbErrors.AppendLine("Column AE is named: " + row[30] + " and should be named:nzrmast_pay_group_code");
                            bErrorFound = true;
                        }
                        if (LowerTrimFunction(row[31]) != "nzrmast_pay_group_desc")
                        {
                            sbErrors.AppendLine("Column AF is named: " + row[31] + " and should be named: nzrmast_pay_group_desc.");
                            bErrorFound = true;
                        }
                        if (LowerTrimFunction(row[32]) != "nzrmast_fact")
                        {
                            sbErrors.AppendLine("Column AG is named: " + row[32] + " and should be named: nzrmast_fact.");
                            bErrorFound = true;
                        }
                        if (LowerTrimFunction(row[33]) != "nzrmast_pays")
                        {
                            sbErrors.AppendLine("Column AH is named: " + row[33] + " and should be named: nzrmast_pays.");
                            bErrorFound = true;
                        }
                        if (LowerTrimFunction(row[34]) != "nzrmast_status_flag_1_code")
                        {
                            sbErrors.AppendLine("Column AI is named: " + row[34] + " and should be named: nzrmast_status_flag_1_code.");
                            bErrorFound = true;
                        }
                        if (LowerTrimFunction(row[35]) != "nzrmast_status_flag_1_desc")
                        {
                            sbErrors.AppendLine("Column AJ is named: " + row[35] + " and should be named: nzrmast_status_flag_1_desc.");
                            bErrorFound = true;
                        }
                        if (LowerTrimFunction(row[36]) != "nzrmast_appr_pidm")
                        {
                            sbErrors.AppendLine("Column AK is named: " + row[36] + " and should be named: nzrmast_appr_pidm.");
                            bErrorFound = true;
                        }
                        if (LowerTrimFunction(row[37]) != "nzrmast_app_posn")
                        {
                            sbErrors.AppendLine("Column AL is named: " + row[37] + " and should be named: nzrmast_app_posn.");
                            bErrorFound = true;
                        }
                        if (LowerTrimFunction(row[38]) != "nzrmast_appr_adpid")
                        {
                            sbErrors.AppendLine("Column AM is named: " + row[38] + " and should be named: nzrmast_appr_adpid.");
                            bErrorFound = true;
                        }
                        if (LowerTrimFunction(row[39]) != "nzrmast_appr_last_name")
                        {
                            sbErrors.AppendLine("Column AN is named: " + row[39] + " and should be named: nzrmast_appr_last_name.");
                            bErrorFound = true;
                        }
                        if (LowerTrimFunction(row[40]) != "nzrmast_appr_first_name")
                        {
                            sbErrors.AppendLine("Column AO is named: " + row[40] + " and should be named: nzrmast_appr_first_name.");
                            bErrorFound = true;
                        }
                        if (LowerTrimFunction(row[41]) != "nzrmast_appr_mi")
                        {
                            sbErrors.AppendLine("Column AP is named: " + row[41] + " and should be named: nzrmast_appr_mi.");
                            bErrorFound = true;
                        }
                        if (LowerTrimFunction(row[42]) != "nzrmast_treaty_limit")
                        {
                            sbErrors.AppendLine("Column AQ is named: " + row[42] + " and should be named: nzrmast_treaty_limit.");
                            bErrorFound = true;
                        }
                        if (LowerTrimFunction(row[43]) != "nzrmast_academic_period")
                        {
                            sbErrors.AppendLine("Column AR is named: " + row[43] + " and should be named: nzrmast_academic_period.");
                            bErrorFound = true;
                        }
                        if (LowerTrimFunction(row[44]) != "nzrmast_ws_limit")
                        {
                            sbErrors.AppendLine("Column AS is named: " + row[44] + " and should be named: nzrmast_ws_limit.");
                            bErrorFound = true;
                        }
                        if (LowerTrimFunction(row[45]) != "nzrmast_jlbd_eff_date")
                        {
                            sbErrors.AppendLine("Column AT is named: " + row[45] + " and should be named: nzrmast_jlbd_eff_date.");
                            bErrorFound = true;
                        }
                        if (LowerTrimFunction(row[46]) != "nzrmast_account1")
                        {
                            sbErrors.AppendLine("Column AU is named: " + row[46] + " and should be named: nzrmast_account1.");
                            bErrorFound = true;
                        }
                        if (LowerTrimFunction(row[47]) != "nzrmast_percent1")
                        {
                            sbErrors.AppendLine("Column AV is named: " + row[47] + " and should be named: nzrmast_percent1.");
                            bErrorFound = true;
                        }
                        if (LowerTrimFunction(row[48]) != "nzrmast_account2")
                        {
                            sbErrors.AppendLine("Column AW is named: " + row[48] + " and should be named: nzrmast_account2.");
                            bErrorFound = true;
                        }
                        if (LowerTrimFunction(row[49]) != "nzrmast_percent2")
                        {
                            sbErrors.AppendLine("Column AX is named: " + row[49] + " and should be named: nzrmast_percent2.");
                            bErrorFound = true;
                        }
                        if (LowerTrimFunction(row[50]) != "nzrmast_account3")
                        {
                            sbErrors.AppendLine("Column AY is named: " + row[50] + " and should be named: nzrmast_account3.");
                            bErrorFound = true;
                        }
                        if (LowerTrimFunction(row[51]) != "nzrmast_percent3")
                        {
                            sbErrors.AppendLine("Column AZ is named: " + row[51] + " and should be named: nzrmast_percent3.");
                            bErrorFound = true;
                        }
                        if (LowerTrimFunction(row[52]) != "nzrmast_account4")
                        {
                            sbErrors.AppendLine("Column BA is named: " + row[52] + " and should be named: nzrmast_account4.");
                            bErrorFound = true;
                        }
                        if (LowerTrimFunction(row[53]) != "nzrmast_percent4")
                        {
                            sbErrors.AppendLine("Column BB is named: " + row[53] + " and should be named: nzrmast_percent4.");
                            bErrorFound = true;
                        }
                        if (LowerTrimFunction(row[54]) != "nzrmast_account5")
                        {
                            sbErrors.AppendLine("Column BC is named: " + row[54] + " and should be named: nzrmast_account5.");
                            bErrorFound = true;
                        }
                        if (LowerTrimFunction(row[55]) != "nzrmast_percent5")
                        {
                            sbErrors.AppendLine("Column BD is named: " + row[55] + " and should be named: nzrmast_percent5.");
                            bErrorFound = true;
                        }
                        if (LowerTrimFunction(row[56]) != "nzrmast_account6")
                        {
                            sbErrors.AppendLine("Column BE is named: " + row[56] + " and should be named: nzrmast_account6.");
                            bErrorFound = true;
                        }
                        if (LowerTrimFunction(row[57]) != "nzrmast_percent6")
                        {
                            sbErrors.AppendLine("Column BF is named: " + row[57] + " and should be named: nzrmast_percent6.");
                            bErrorFound = true;
                        }
                        if (bErrorFound) break;
                    }
                    else
                    {
                        string fileNumber = ConvertToString(row[2]);
                        if (fileNumber.Length == 0)
                        {
                            sbErrors.AppendLine(Environment.NewLine + "nzrmast_adpid is blank.  Record skipped. Record number = " + nLoopCount);
                            continue;
                        }

                        string lastName = ConvertToString(row[3]);
                        if (lastName.Length == 0)
                        {
                            sbErrors.AppendLine(Environment.NewLine + "nzrmast_employee_last_name is blank.  Record skipped. Record number = " + nLoopCount);
                            continue;
                        }
                        string firstName = ConvertToString(row[4]);
                        if (firstName.Length == 0)
                        {
                            sbErrors.AppendLine(Environment.NewLine + "nzrmast_employee_first_name is blank.  Record skipped. Record number = " + nLoopCount);
                            continue;
                        }
                        string middleName = ConvertToString(row[5]);
                        string ssn = ConvertToString(row[6]);
                        if (ssn.Length == 0)
                        {
                            sbErrors.AppendLine(Environment.NewLine + "nzrmast_taxid is blank.  Record skipped. Record number = " + nLoopCount);
                            continue;
                        }
                        ssn = ssn.Replace("-", "").Trim();
                        ssn = ssn.PadLeft(9, '0');
                        if (ssn.Length != 9)
                        {
                            sbErrors.AppendLine(Environment.NewLine + "SSN must be nine digits.  Record skipped. Record number = " + nLoopCount);
                            continue;
                        }
                        string strBirthDate = ConvertToString(row[7]);
                        DateTime? birthDate = ConvertToDateTime(strBirthDate);
                        if (birthDate == null)
                        {
                            sbErrors.AppendLine(Environment.NewLine + "nzrmast_birth_date is not in the correct format. Record skipped. Record number =" + nLoopCount);
                            continue;
                        }
                        string companyCode = ConvertToString(row[8]);
                        if (companyCode.Length == 0)
                        {
                            sbErrors.AppendLine(Environment.NewLine + "nzrmast_empr_code is blank.  Record skipped. Record number = " + nLoopCount);
                            continue;
                        }
                        string eMail = ConvertToString(row[9]);
                        if (string.IsNullOrEmpty(eMail))
                        {
                            eMail = lastName + firstName + fileNumber + "@resnav.com";
                        }
                        string homePhone = ConvertToString(row[10]);
                        homePhone = homePhone.TrimStart('+');
                        string employeeStatus = ConvertToString(row[11]);
                        string employeeStatusCode = employeeStatus;
                        if (employeeStatus.Length == 0)
                        {
                            sbErrors.AppendLine(Environment.NewLine + "nzrmast_hire_status is blank.  Record skipped. Record number = " + nLoopCount);
                            continue;
                        }
                        else if (employeeStatus == "O")
                        {
                            employeeStatus = "OnLeave";
                        }
                        else if (employeeStatus == "A")
                        {
                            employeeStatus = "Active";
                        }
                        else if (employeeStatus == "T")
                        {
                            employeeStatus = "Terminated";
                        }
                        string strHireDate = ConvertToString(row[12]);
                        DateTime? hireDate = ConvertToDateTime(strHireDate);
                        if (hireDate == null)
                        {
                            sbErrors.AppendLine(Environment.NewLine + "nzrmast_current_hire_date is not in the correct format. Record skipped. Record number =" + nLoopCount);
                            continue;
                        }
                        DateTime? terminationDate = null;
                        string strTerminationDate = ConvertToString(row[13]);
                        if (!string.IsNullOrEmpty(strTerminationDate))
                        {
                            terminationDate = ConvertToDateTime(strTerminationDate);
                        }
                        if (employeeStatus.ToLower().Trim() == "terminated" || employeeStatus.ToLower().Trim() == "retired")
                        {
                            if (terminationDate == null && strTerminationDate.Length == 0)
                            {
                                sbErrors.AppendLine(Environment.NewLine + "Termination Date is Empty for " + employeeStatus + " Employee. Record skipped. Record number =" + nLoopCount);
                                continue;
                            }
                        }
                        string strBuCode = ConvertToString(row[14]);
                        if (strBuCode.Length == 0)
                        {
                            sbErrors.AppendLine(Environment.NewLine + "nzrmast_home_orgn_code is blank.  Record skipped. Record number = " + nLoopCount);
                            continue;
                        }
                        string strBuDesc = ConvertToString(row[15]);
                        if (strBuDesc.Length == 0)
                        {
                            sbErrors.AppendLine(Environment.NewLine + "nzrmast_home_orgn_code_desc is blank.  Record skipped. Record number = " + nLoopCount);
                            continue;
                        }
                        string payFrequency = ConvertToString(row[16]);
                        if (payFrequency.Length == 0)
                        {
                            sbErrors.AppendLine(Environment.NewLine + "nzrmast_pay_frequency_code is blank.  Record skipped. Record number = " + nLoopCount);
                            continue;
                        }
                        if (payFrequency.ToUpper() == "B")
                        {
                            payFrequency = "BiWeekly";
                        }
                        var payFrequencyId = clientDbContext.DdlPayFrequencies.Where(x => x.Description.ToLower().Trim() == payFrequency.ToLower()).Select(x => x.PayFrequencyId).FirstOrDefault();
                        string empType = ConvertToString(row[17]);
                        if (empType.Length == 0)
                        {
                            sbErrors.AppendLine(Environment.NewLine + "nzrmast_ecls_code is blank.  Record skipped. Record number = " + nLoopCount);
                            continue;
                        }
                        var isStudent = (empType.ToUpper() == "ST" || empType.ToUpper() == "SW");
                        string jobCode = ConvertToString(row[18]);
                        if (jobCode.Length == 0)
                        {
                            sbErrors.AppendLine(Environment.NewLine + "nzrmast_position is blank.  Record skipped. Record number = " + nLoopCount);
                            continue;
                        }
                        string suffix = ConvertToString(row[19]);
                        if (suffix.Length == 0)
                        {
                            sbErrors.AppendLine(Environment.NewLine + "nzrmast_suff is blank.  Record skipped. Record number = " + nLoopCount);
                            continue;
                        }
                        else if (suffix.Length > 2)
                        {
                            sbErrors.AppendLine(Environment.NewLine + "nzrmast_suff max length is 2.   Record skipped. Record number = " + nLoopCount);
                            continue;
                        }
                        var isSuffixNumeric = suffix.All(char.IsDigit);
                        if (isSuffixNumeric)
                        {
                            suffix = suffix.PadLeft(2, '0');
                        }
                        string departmentCode = ConvertToString(row[20]);
                        if (departmentCode.Length == 0)
                        {
                            sbErrors.AppendLine(Environment.NewLine + "nzrmast_job_orgn_code is blank.  Record skipped. Record number = " + nLoopCount);
                            continue;
                        }
                        string departmentDesc = ConvertToString(row[21]);
                        if (departmentDesc.Length == 0)
                        {
                            sbErrors.AppendLine(Environment.NewLine + "nzrmast_job_orgn_code_desc is blank.  Record skipped. Record number = " + nLoopCount);
                            continue;
                        }
                        string primaryPosition = ConvertToString(row[23]);
                        if (primaryPosition.Length == 0)
                        {
                            sbErrors.AppendLine(Environment.NewLine + "nzrmast_primary_ind is blank.  Record skipped. Record number = " + nLoopCount);
                            continue;
                        }
                        var isPrimaryPosition = (primaryPosition.ToUpper() == "Y");
                        string jobDesc = ConvertToString(row[24]);
                        if (jobDesc.Length == 0)
                        {
                            sbErrors.AppendLine(Environment.NewLine + "nzrmast_job_desc is blank.  Record skipped. Record number = " + nLoopCount);
                            continue;
                        }
                        DateTime? startDate = null;
                        string strStartDate = ConvertToString(row[25]);
                        if (strStartDate.Length == 0)
                        {
                            sbErrors.AppendLine(Environment.NewLine + "nzrmast_start_date is blank.  Record skipped. Record number = " + nLoopCount);
                            continue;
                        }
                        startDate = Convert.ToDateTime(strStartDate);
                        DateTime? endDate = null;
                        string strEndDate = ConvertToString(row[26]);
                        if (!string.IsNullOrEmpty(strEndDate))
                        {
                            endDate = Convert.ToDateTime(strEndDate);
                        }
                        DateTime? rateEffectiveDate = null;
                        string strRateEffectiveDate = ConvertToString(row[27]);
                        if (strRateEffectiveDate.Length == 0)
                        {
                            sbErrors.AppendLine(Environment.NewLine + "nzrmast_eff_date is blank.  Record skipped. Record number = " + nLoopCount);
                            continue;
                        }
                        rateEffectiveDate = Convert.ToDateTime(strRateEffectiveDate);
                        string strPayRate = ConvertToString(row[28]);
                        strPayRate = strPayRate.Replace("$", "").Trim();
                        strPayRate = strPayRate.Replace(",", "").Trim();
                        Decimal? dPayRate = ConvertToDecimal(strPayRate);
                        if (strPayRate.Length > 0 && dPayRate == null)
                        {
                            sbErrors.AppendLine(Environment.NewLine + "nzrmast_rate_amount is not in the correct format.  Record skipped. Record number = " + nLoopCount);
                            continue;
                        }
                        string rateType = ConvertToString(row[29]);
                        string payGroupCode = ConvertToString(row[30]);
                        string payGroupDesc = ConvertToString(row[31]);
                        string payDays = ConvertToString(row[33]);
                        decimal? dPayDays = ConvertToDecimal(payDays);
                        if (rateType.ToUpper().Trim() == "S")
                        {
                            dPayRate = dPayRate / dPayDays;
                            dPayRate = TruncateDecimal(Convert.ToDecimal(dPayRate), 2);
                        }
                        string statusFlag1Code = ConvertToString(row[34]);
                        string statusFlag1Description = ConvertToString(row[35]);
                        string appFileNumber = ConvertToString(row[38]);
                        string appLastName = ConvertToString(row[39]);
                        string appFirstName = ConvertToString(row[40]);
                        string appMiddleName = ConvertToString(row[41]);
                        Decimal? dTreatyLimit = null, dAdpWSLimit = null;
                        string adpYear = string.Empty;
                        if (row.Count > 44)
                        {
                            string treatyLimit = ConvertToString(row[42]);
                            adpYear = ConvertToString(row[43]);
                            string adpWSLimit = ConvertToString(row[44]);
                            dTreatyLimit = ConvertToDecimal(treatyLimit);
                            dAdpWSLimit = ConvertToDecimal(adpWSLimit);
                        }
                        DateTime? costNumberEffectiveDate = row.Count > 45 ? ConvertToDateTime(row[45]) : null;
                        string costNumber1Account = row.Count > 46 ? (ConvertToString(row[46])) : string.Empty;
                        Decimal? costNumber1Percent = row.Count > 47 ? ConvertToDecimal(row[47]) : null;
                        string costNumber2Account = row.Count > 48 ? (ConvertToString(row[48])) : string.Empty;
                        Decimal? costNumber2Percent = row.Count > 49 ? ConvertToDecimal(row[49]) : null;
                        string costNumber3Account = row.Count > 50 ? (ConvertToString(row[50])) : string.Empty;
                        Decimal? costNumber3Percent = row.Count > 51 ? ConvertToDecimal(row[51]) : null;
                        string costNumber4Account = row.Count > 52 ? (ConvertToString(row[52])) : string.Empty;
                        Decimal? costNumber4Percent = row.Count > 53 ? ConvertToDecimal(row[53]) : null;
                        string costNumber5Account = row.Count > 54 ? (ConvertToString(row[54])) : string.Empty;
                        Decimal? costNumber5Percent = row.Count > 55 ? ConvertToDecimal(row[55]) : null;
                        string costNumber6Account = row.Count > 56 ? (ConvertToString(row[56])) : string.Empty;
                        Decimal? costNumber6Percent = row.Count > 57 ? ConvertToDecimal(row[57]) : null;
                                     

                        int empClassId = 0;
                        var objEmpClass = clientDbContext.EmployeeClass.Where(x => x.ClassName == empType).ToList();
                        if (!objEmpClass.Any())
                        {
                            var empClass = new EmployeeClass();
                            empClass.ClassName = empType;
                            empClass.IsActive = true;
                            empClass.CreatedBy = "ftpauto";
                            empClass.CreatedDate = DateTime.Now;
                            clientDbContext.EmployeeClass.Add(empClass);
                            clientDbContext.SaveChanges();
                            empClassId = empClass.EmployeeClassId;
                        }
                        else
                        {
                            empClassId = objEmpClass.Select(x => x.EmployeeClassId).FirstOrDefault();
                        }
                        int payGroupId = 0, employeeTypeId = 0;
                        if (!string.IsNullOrEmpty(statusFlag1Code) && !string.IsNullOrEmpty(statusFlag1Description))
                        {
                            var drEmployeeTypes = clientDbContext.DdlEmployeeTypes.Where(x => x.Code == statusFlag1Code).ToList();
                            if (drEmployeeTypes.Count() == 0)
                            {
                                var ddlEmployeeType = new DdlEmployeeType();
                                ddlEmployeeType.Code = statusFlag1Code;
                                ddlEmployeeType.Description = statusFlag1Description;
                                ddlEmployeeType.Active = true;
                                clientDbContext.DdlEmployeeTypes.Add(ddlEmployeeType);
                                clientDbContext.SaveChanges();
                                employeeTypeId = ddlEmployeeType.EmployeeTypeId;
                            }
                            else
                            {
                                employeeTypeId = drEmployeeTypes.Select(x => x.EmployeeTypeId).FirstOrDefault();
                            }
                        }
                        if (!string.IsNullOrEmpty(payGroupCode) && !string.IsNullOrEmpty(payGroupDesc))
                        {
                            var drPayGroup = clientDbContext.DdlPayGroups.Where(x => x.Code == payGroupCode).ToList();
                            if (drPayGroup.Count == 0)
                            {
                                var ddlPayGroups = new DdlPayGroup();
                                ddlPayGroups.Code = payGroupCode;
                                ddlPayGroups.Description = payGroupDesc;
                                ddlPayGroups.Active = true;
                                clientDbContext.DdlPayGroups.Add(ddlPayGroups);
                                clientDbContext.SaveChanges();
                                payGroupId = ddlPayGroups.PayGroupId;
                            }
                            else
                            {
                                payGroupId = drPayGroup.Select(x => x.PayGroupId).FirstOrDefault();
                            }
                        }
                        int nPersonId = 0;
                        int departmentId = 0;
                        int nmPersonId = 0;
                        int companyCodeId = 0;
                        var drCompanyCode = clientDbContext.CompanyCodes.Where(x => x.CompanyCodeCode == companyCode).ToList();
                        if (drCompanyCode.Count() == 0)
                        {
                            var objCompanyCode = new CompanyCode();
                            objCompanyCode.CompanyCodeCode = companyCode;
                            objCompanyCode.CompanyCodeDescription = companyCode;
                            objCompanyCode.IsCompanyCodeActive = true;
                            clientDbContext.CompanyCodes.Add(objCompanyCode);
                            clientDbContext.SaveChanges();
                            companyCodeId = objCompanyCode.CompanyCodeId;
                        }
                        else
                        {
                            companyCodeId = clientDbContext.CompanyCodes.Where(x => x.CompanyCodeCode == companyCode).Select(x => x.CompanyCodeId).FirstOrDefault();
                        }
                        int nEmploymentStatusId = 0;
                        var drStatusCode = clientDbContext.DdlEmploymentStatuses.Where(x => x.Code == employeeStatusCode).Select(x => x.EmploymentStatusId).FirstOrDefault();
                        if (drStatusCode == 0)
                        {
                            var ddlEmploymentStatus = new DdlEmploymentStatus();
                            ddlEmploymentStatus.Code = employeeStatusCode;
                            ddlEmploymentStatus.Description = employeeStatus;
                            ddlEmploymentStatus.Active = true;
                            clientDbContext.DdlEmploymentStatuses.Add(ddlEmploymentStatus);
                            clientDbContext.SaveChanges();
                        }
                        else
                        {
                            nEmploymentStatusId = Convert.ToInt32(drStatusCode);
                        }
                        var drJob = clientDbContext.Departments.Where(x => x.DepartmentCode == departmentCode && x.IsDeleted == false && x.CompanyCodeId == companyCodeId).Count();
                        if (drJob == 0)
                        {
                            var department = new Department();
                            department.CompanyCodeId = companyCodeId;
                            department.DepartmentDescription = departmentDesc;
                            department.DepartmentCode = departmentCode;
                            department.IsDepartmentActive = true;
                            department.IsDeleted = false;
                            clientDbContext.Departments.Add(department);
                            clientDbContext.SaveChanges();
                            departmentId = department.DepartmentId;
                        }
                        else
                        {
                            Department objdepartment = (from x in clientDbContext.Departments where x.DepartmentCode == departmentCode && x.CompanyCodeId == companyCodeId select x).FirstOrDefault();
                            objdepartment.CompanyCodeId = companyCodeId;
                            objdepartment.DepartmentDescription = departmentDesc;
                            objdepartment.DepartmentCode = departmentCode;
                            objdepartment.IsDepartmentActive = true;
                            clientDbContext.SaveChanges();
                            departmentId = objdepartment.DepartmentId;
                        }

                   var EarningsCodeId = clientDbContext.EarningsCodes.Where(x => x.CompanyCodeId == companyCodeId && x.IsDefault == true).Select(x => x.EarningsCodeId).FirstOrDefault();
                        var EarningsCodeIdnew = (from ea in clientDbContext.EarningsCodes
                                                 join emp in clientDbContext.Employees on ea.EarningsCodeId equals emp.EarningsCodeId
                                                 where ea.CompanyCodeId == companyCodeId  && emp.EarningsCodeId != EarningsCodeId
                                                 select ea.EarningsCodeId
                                                   ).ToList();
                        
                        //person Supervisor Checking
                        var dtPersonsManager = (from pers in clientDbContext.Persons
                                                join emp in clientDbContext.Employees
                                                on pers.PersonId equals emp.PersonId
                                                where emp.FileNumber == appFileNumber
                                                select new { id = pers.PersonId }).ToList();
                        if (dtPersonsManager.Count() > 1)
                        {
                            sbErrors.AppendLine(" More than one employee record found for " + lastName + ", " + firstName + ".  Record skipped. Record number = " + nLoopCount + ".");
                            continue;
                        }
                        else if (dtPersonsManager.Count() == 0)
                        {
                            if (string.IsNullOrEmpty(appFirstName) && string.IsNullOrEmpty(appLastName))
                            {
                                sbErrors.AppendLine(" Manager not Exist in Master File " + appLastName + ", " + appFirstName + "," + fileNumber + ". Record number = " + nLoopCount + ".");
                            }
                            else
                            {
                                var dtPersonManagerManual = (from pers in clientDbContext.Persons
                                                             where pers.Firstname == appFirstName && pers.Lastname == appLastName
                                                             select new { id = pers.PersonId }).ToList();
                                if (dtPersonManagerManual.Count() > 1)
                                {
                                    sbErrors.AppendLine(" More than one employee record found for " + lastName + ", " + firstName + ".  Record skipped. Record number = " + nLoopCount + ".");
                                    continue;
                                }
                                else if (dtPersonManagerManual.Count() == 0)
                                {
                                    sbErrors.AppendLine(" Manager not Exist in Master File " + appLastName + ", " + appFirstName + "," + fileNumber + ". Record number = " + nLoopCount + ".");
                                }
                                else if (dtPersonManagerManual.Count() == 1)
                                {
                                    nmPersonId = Convert.ToInt32(dtPersonManagerManual.FirstOrDefault().id);
                                }
                            }
                        }
                        else
                        {
                            nmPersonId = Convert.ToInt32(dtPersonsManager.FirstOrDefault().id);
                        }

                        int nmPreviousPersonId = nmPersonId;
                        var designatedSupervisors = (from x in clientDbContext.DesignatedSupervisors where x.ManagerPersonId == nmPersonId select x).FirstOrDefault();
                        if (designatedSupervisors != null)
                        {
                            nmPersonId = designatedSupervisors.DesignatedManagerPersonId;
                        }

                        //Perosns Checking
                        var dtPersons = (from pers in clientDbContext.Persons
                                         join emp in clientDbContext.Employees
                                         on pers.PersonId equals emp.PersonId
                                         where emp.FileNumber == fileNumber
                                         select new { id = pers.PersonId }).ToList();
                        if (dtPersons.Count() == 0)
                        {
                            var person = new Person();
                            person.Firstname = firstName;
                            person.Lastname = lastName;
                            person.MiddleName = middleName;
                            person.eMail = eMail;
                            person.SSN = ssn;
                            person.DOB = birthDate;
                            person.EnteredDate = DateTime.Now;
                            person.EnteredBy = "ftpauto";
                            clientDbContext.Persons.Add(person);
                            clientDbContext.SaveChanges();
                            nPersonId = person.PersonId;
                        }
                        else
                        {
                            nPersonId = Convert.ToInt32(dtPersons.FirstOrDefault().id);
                            Person objperson = (from x in clientDbContext.Persons where x.PersonId == nPersonId select x).FirstOrDefault();
                            objperson.Firstname = firstName;
                            objperson.Lastname = lastName;
                            objperson.MiddleName = middleName;
                            objperson.eMail = eMail;
                            objperson.SSN = ssn;
                            objperson.DOB = birthDate;
                            objperson.ModifiedDate = DateTime.Now;
                            objperson.ModifiedBy = "ftpauto";
                            clientDbContext.SaveChanges();
                        }
                        if (!string.IsNullOrEmpty(homePhone))
                        {
                            if (homePhone.Length > 15)
                            {
                                sbErrors.AppendLine(" Personal Contact Home Phone Length is greater than 15. " + lastName + ", " + firstName + ". Record number = " + nLoopCount + ".");
                            }
                            else
                            {
                                var drPersonPhoneNumbers = clientDbContext.PersonPhoneNumbers.Where(x => x.PhoneNumber == homePhone).Count();
                                if (drPersonPhoneNumbers == 0)
                                {
                                    var personPhoneNumber = new PersonPhoneNumber();
                                    personPhoneNumber.PersonId = nPersonId;
                                    personPhoneNumber.PhoneTypeId = 1;
                                    personPhoneNumber.PhoneNumber = homePhone;
                                    personPhoneNumber.EnteredBy = "ftpauto";
                                    personPhoneNumber.EnteredDate = DateTime.Now;
                                    personPhoneNumber.IsPrimaryPhone = true;
                                    clientDbContext.PersonPhoneNumbers.Add(personPhoneNumber);
                                    clientDbContext.SaveChanges();
                                }
                                else
                                {
                                    var objPersonPhoneNumber = (from x in clientDbContext.PersonPhoneNumbers where x.PhoneNumber == homePhone select x).FirstOrDefault();
                                    if (objPersonPhoneNumber != null)
                                    {
                                        objPersonPhoneNumber.PersonId = nPersonId;
                                        objPersonPhoneNumber.PhoneTypeId = 1;
                                        objPersonPhoneNumber.PhoneNumber = homePhone;
                                        objPersonPhoneNumber.ModifiedBy = "ftpauto";
                                        objPersonPhoneNumber.ModifiedDate = DateTime.Now;
                                        objPersonPhoneNumber.IsPrimaryPhone = true;
                                        clientDbContext.SaveChanges();
                                    }
                                }
                            }
                        }
                        var businessUnit = new PositionBusinessLevels();
                        int nbuNbr = 0;
                        var drBu = clientDbContext.PositionBusinessLevels.Where(x => x.BusinessLevelCode == strBuCode).Select(x => x.BusinessLevelNbr).FirstOrDefault();
                        if (drBu == 0)
                        {
                            businessUnit.BusinessLevelCode = strBuCode;
                            businessUnit.BusinessLevelTitle = strBuDesc;
                            businessUnit.ParentBULevelNbr = departmentId;
                            businessUnit.EnteredBy = "ftpauto";
                            businessUnit.EnteredDate = DateTime.Now;
                            clientDbContext.PositionBusinessLevels.Add(businessUnit);
                            clientDbContext.SaveChanges();
                            nbuNbr = businessUnit.BusinessLevelNbr;
                        }
                        else
                        {
                            nbuNbr = Convert.ToInt32(drBu);
                            var buUnit = (from x in clientDbContext.PositionBusinessLevels where x.BusinessLevelNbr == nbuNbr select x).FirstOrDefault();
                            buUnit.BusinessLevelCode = strBuCode;
                            buUnit.BusinessLevelTitle = strBuDesc;
                            buUnit.ParentBULevelNbr = departmentId;
                            buUnit.ModifiedBy = "ftpauto";
                            buUnit.ModifiedDate = DateTime.Now;
                            clientDbContext.SaveChanges();
                        }

                        int nEmployeeId = 0;
                        int nmEmployeeId = 0;
                        var dtEmployees = (from pers in clientDbContext.Employees
                                           join emp in clientDbContext.Persons
                                           on pers.PersonId equals emp.PersonId
                                           join empstat in clientDbContext.DdlEmploymentStatuses on pers.EmploymentStatusId equals empstat.EmploymentStatusId
                                           where pers.FileNumber == fileNumber && pers.CompanyCodeId == companyCodeId
                                           select new { id = pers.EmployeeId }).ToList();
                        if (dtEmployees.Count() > 1)
                        {
                            sbErrors.AppendLine(Environment.NewLine + "More than one employee record found for " + lastName + ", " + firstName + ".  Record skipped. Record number = " + nLoopCount);
                            continue;
                        }
                        else if (dtEmployees.Count() == 0)
                        {
                            int nEmploymentNumber = 1;
                            var drMaxEmploymentNumber = clientDbContext.Employees.Where(x => x.PersonId == nPersonId).ToList();
                            if (drMaxEmploymentNumber.Count > 0)
                            {
                                var MaxEmpNumber = clientDbContext.Employees.Where(x => x.PersonId == nPersonId).Max(x => x.EmploymentNumber);
                                nEmploymentNumber = MaxEmpNumber + 1;
                            }
                            var employee = new Employee();
                            employee.CompanyCode = Convert.ToString(nbuNbr);
                            employee.FileNumber = fileNumber;
                            employee.HireDate = (DateTime)hireDate;
                            employee.PersonId = nPersonId;
                            if (terminationDate != null)
                            {
                                if (DateTime.Compare(Convert.ToDateTime(DateTime.Now.ToShortDateString()), Convert.ToDateTime(terminationDate.Value.Date.ToShortDateString())) >= 0)
                                {
                                    employee.EmploymentStatusId = drStatusCode;
                                }
                                else
                                {
                                    employee.EmploymentStatusId = clientDbContext.DdlEmploymentStatuses.Where(x => x.Code == "A").Select(x => x.EmploymentStatusId).FirstOrDefault();
                                }
                            }
                            else
                            {
                                employee.EmploymentStatusId = drStatusCode;
                            }
                            if (terminationDate != null)
                            {
                                employee.TerminationDate = terminationDate;
                            }
                            employee.DepartmentId = nbuNbr;
                            employee.EnteredBy = "ftpauto";
                            employee.EnteredDate = DateTime.Now;
                            employee.EmploymentNumber = nEmploymentNumber;
                            employee.CompanyCodeId = companyCodeId;
                            employee.TimeCardTypeId = 1;
                            employee.Rate = dPayRate;
                            employee.PayFrequencyId = payFrequencyId;
                            employee.IsStudent = isStudent;
                            employee.TreatyLimit = dTreatyLimit;
                            employee.AdpWSLimit = dAdpWSLimit;
                            employee.EarningsCodeId = EarningsCodeId == 0 ? 0 : EarningsCodeId; 
                            clientDbContext.Employees.Add(employee);
                            clientDbContext.SaveChanges();
                            nEmployeeId = employee.EmployeeId;
                        }
                        else if (dtEmployees.Count() == 1)
                        {
                            nEmployeeId = Convert.ToInt32(dtEmployees.FirstOrDefault().id);
                            Employee objemployee = (from x in clientDbContext.Employees where x.EmployeeId == nEmployeeId select x).FirstOrDefault();
                            objemployee.ModifiedBy = "ftpauto";
                            objemployee.ModifiedDate = DateTime.Now;
                            if (terminationDate != null)
                            {
                                if (DateTime.Compare(Convert.ToDateTime(DateTime.Now.ToShortDateString()), Convert.ToDateTime(terminationDate.Value.Date.ToShortDateString())) >= 0)
                                {
                                    objemployee.EmploymentStatusId = drStatusCode;
                                }
                                else
                                {
                                    objemployee.EmploymentStatusId = clientDbContext.DdlEmploymentStatuses.Where(x => x.Code == "A").Select(x => x.EmploymentStatusId).FirstOrDefault();
                                }
                            }
                            else
                            {
                                objemployee.EmploymentStatusId = drStatusCode;
                            }
                            if (terminationDate != null)
                            {
                                objemployee.TerminationDate = terminationDate;
                            }
                            objemployee.ReportToPersonId = nmPersonId == 0 ? (int?)null : nmPersonId;
                            objemployee.IsStudent = isStudent;
                            objemployee.HireDate = (DateTime)hireDate;
                            objemployee.DepartmentId = nbuNbr;
                            objemployee.Rate = dPayRate;
                            objemployee.PayFrequencyId = payFrequencyId;
                            objemployee.TreatyLimit = dTreatyLimit;
                            objemployee.AdpYear = string.IsNullOrEmpty(adpYear) ? (DateTime?)null : Convert.ToDateTime(adpYear);
                            objemployee.AdpWSLimit = dAdpWSLimit;
                            foreach (var item in EarningsCodeIdnew)
                            {
                                if (objemployee.EarningsCodeId != item)
                                {
                                    objemployee.EarningsCodeId = EarningsCodeId == 0 ? 0 : EarningsCodeId;
                                }
                            }
                            clientDbContext.SaveChanges();
                        }

                        var dtEmployeesManagers = (from pers in clientDbContext.Employees
                                                   join emp in clientDbContext.Persons
                                                   on pers.PersonId equals emp.PersonId
                                                   join empstat in clientDbContext.DdlEmploymentStatuses on pers.EmploymentStatusId equals empstat.EmploymentStatusId
                                                   where pers.FileNumber == appFileNumber && pers.CompanyCodeId == companyCodeId
                                                   select new { id = pers.EmployeeId }).ToList();
                        if (dtEmployeesManagers.Count() > 1)
                        {
                            sbErrors.AppendLine(Environment.NewLine + "More than one employee record found for " + appLastName + ", " + appFirstName + ".  Record skipped. Record number = " + nLoopCount);
                            continue;
                        }
                        else if (dtEmployeesManagers.Count() == 1)
                        {
                            if (nmPersonId > 0)
                            {
                                nmEmployeeId = Convert.ToInt32(dtEmployeesManagers.FirstOrDefault().id);
                                Employee objemployee = (from x in clientDbContext.Employees where x.EmployeeId == nmEmployeeId select x).FirstOrDefault();
                                objemployee.ModifiedBy = "ftpauto";
                                objemployee.ModifiedDate = DateTime.Now;
                                objemployee.ReportToPersonId = null;
                                objemployee.IsStudent = false;
                                objemployee.DepartmentId = nbuNbr;
                                var drWorkedInStateTaxCode = 0;
                                if (drWorkedInStateTaxCode == 0)
                                {
                                    objemployee.WorkedStateTaxCodeId = null;
                                }
                                objemployee.PayFrequencyId = 2;
                                objemployee.AdpYear = string.IsNullOrEmpty(adpYear) ? (DateTime?)null : Convert.ToDateTime(adpYear);
                                objemployee.AdpWSLimit = dAdpWSLimit;
                                clientDbContext.SaveChanges();
                            }
                        }

                        int nJobID = 0;
                        var job = new Job();
                        var drJobs = clientDbContext.Jobs.Where(x => x.JobCode == jobCode && x.CompanyCodeId == companyCodeId).Select(x => x.JobId).ToList();
                        job.JobCode = jobCode;
                        job.CompanyCodeId = companyCodeId;
                        job.JobDescription = jobDesc;
                        job.title = jobDesc;
                        job.enteredBy = "ftpauto";
                        job.createdDate = DateTime.Now;
                        job.enteredDate = DateTime.Now;
                        job.IsJobActive = true;
                        if (drJobs.Count == 0)
                        {
                            clientDbContext.Jobs.Add(job);
                            clientDbContext.SaveChanges();
                            nJobID = job.JobId;
                        }
                        else if (drJobs.Count == 1)
                        {
                            nJobID = Convert.ToInt32(drJobs.FirstOrDefault());
                            Job objjob = (from x in clientDbContext.Jobs where x.JobId == nJobID select x).FirstOrDefault();
                            objjob.JobCode = jobCode;
                            objjob.CompanyCodeId = companyCodeId;
                            objjob.JobDescription = jobDesc;
                            objjob.title = jobDesc;
                            objjob.enteredBy = "ftpauto";
                            objjob.enteredDate = DateTime.Now;
                            objjob.IsJobActive = true;
                            clientDbContext.SaveChanges();
                        }

                        int nPositionID = 0;
                        string positionCode = departmentCode + jobCode;
                        var drPosition = clientDbContext.Positions.Where(x => x.PositionCode == positionCode && x.Suffix == suffix).ToList();
                        if (drPosition.Count == 0)
                        {
                            var position = new Position();
                            position.JobId = nJobID;
                            position.Title = jobDesc;
                            position.Code = positionCode;
                            position.StartDate = null;
                            position.PositionCode = positionCode;
                            position.BusinessLevelNbr = nbuNbr;
                            position.PositionDescription = jobDesc;
                            position.PayFrequencyCode = string.Empty;
                            position.Status = 1;
                            position.Suffix = suffix;
                            if (departmentId > 0)
                            {
                                position.DepartmentId = departmentId;
                            }
                            position.IsPositionActive = true;
                            clientDbContext.Positions.Add(position);
                            clientDbContext.SaveChanges();
                            nPositionID = position.PositionId;
                        }
                        else if (drPosition.Count == 1)
                        {
                            nPositionID = drPosition.FirstOrDefault().PositionId;
                            Positions objposition = (from x in clientDbContext.Positions where x.PositionId == nPositionID select x).FirstOrDefault();
                            objposition.JobId = nJobID;
                            objposition.Title = jobDesc;
                            objposition.PayFrequencyCode = string.Empty;
                            objposition.PositionDescription = jobDesc;
                            objposition.StartDate = null;
                            objposition.Code = positionCode;
                            objposition.PositionCode = positionCode;
                            objposition.TotalSlots = null;
                            objposition.PositionId = nPositionID;
                            objposition.Status = 1;
                            objposition.Suffix = suffix;
                            if (departmentId > 0)
                            {
                                objposition.DepartmentId = departmentId;
                            }
                            objposition.IsPositionActive = true;
                            clientDbContext.SaveChanges();
                        }
                        else
                        {
                            sbErrors.AppendLine(Environment.NewLine + "More than one Position Found for the same business unit, job, and position title " + lastName + ", " + firstName + ".  Record skipped. Record number = " + nLoopCount);
                            continue;
                        }

                        var dtEmpPositions = (from ep in clientDbContext.E_Positions
                                              join e in clientDbContext.Employees on ep.EmployeeId equals e.EmployeeId
                                              join p in clientDbContext.Positions on ep.PositionId equals p.PositionId
                                              join b in clientDbContext.PositionBusinessLevels on p.BusinessLevelNbr equals b.BusinessLevelNbr
                                              join j in clientDbContext.Jobs on p.JobId equals j.JobId
                                              //where ((!ep.actualEndDate.HasValue) || (ep.actualEndDate.HasValue && DateTime.Compare(ep.actualEndDate.Value, DateTime.Now) >= 0))
                                              select new
                                              {
                                                  ep.E_PositionId,
                                                  p.PositionId,
                                                  ep.StartDate,
                                                  ep.EmployeeId
                                              }).ToList();
                        int nEPositionID = 0;
                        var rateId = clientDbContext.DdlRateTypes.Where(x => x.Description == rateType).Select(x => x.RateTypeId).FirstOrDefault();
                        var dra_ePositions = dtEmpPositions.Where(x => x.PositionId == nPositionID && x.EmployeeId == nEmployeeId && x.StartDate == startDate).Select(x => x.E_PositionId).ToList();
                        if (dra_ePositions.Count == 1)
                        {
                            nEPositionID = Convert.ToInt32(dra_ePositions[0]);
                        }
                        else if (dra_ePositions.Count() > 1)
                        {
                            if (endDate == null)
                            {
                                sbErrors.AppendLine(Environment.NewLine + "More than one ePosition found for  Employee = " + lastName + ", " + firstName + " - Employee File Number = " + fileNumber + ". Job processing skipped.  Record number = " + nLoopCount);
                                continue;
                            }
                            else
                            {
                                nEPositionID = dra_ePositions.Max();
                            }
                        }

                        //costcenter default sending if not having any cost center 
                        if (String.IsNullOrEmpty(costNumber1Account) && String.IsNullOrEmpty(costNumber2Account) && String.IsNullOrEmpty(costNumber3Account) && String.IsNullOrEmpty(costNumber4Account) && String.IsNullOrEmpty(costNumber5Account) && String.IsNullOrEmpty(costNumber6Account))
                           {                               
                                costNumber1Account = "000000000000000000";                              
                                costNumber1Percent = 100;                           
                          }
                        if (dra_ePositions.Count() == 0)
                        {
                            var lastEndDate = clientDbContext.E_Positions.Where(m => m.EmployeeId == nEmployeeId && m.E_PositionId != nEPositionID && m.PositionId == nPositionID).OrderByDescending(m => m.E_PositionId).FirstOrDefault();

                            var ePositions = new E_Positions();
                            ePositions.EmployeeId = nEmployeeId;
                            ePositions.PositionId = nPositionID;
                            ePositions.PrimaryPosition = isPrimaryPosition;
                            ePositions.StartDate = startDate;
                            ePositions.actualEndDate = endDate;
                            ePositions.RateTypeId = rateId == 0 ? (int?)null : rateId;
                            ePositions.PayFrequencyId = payFrequencyId;
                            ePositions.EnteredBy = "ftpauto";
                            ePositions.EnteredDate = DateTime.Now;
                            ePositions.PositionTypeID = 1;
                            ePositions.EmployeeTypeId = employeeTypeId;
                            ePositions.PayGroupId = payGroupId;
                            ePositions.ReportsToID = nmPersonId == 0 ? (int?)null : nmPersonId;
                            ePositions.FileNumber = fileNumber;
                            ePositions.DepartmentId = departmentId;
                            ePositions.EmployeeClassId = empClassId;
                            ePositions.IsDeleted = false;
                            ePositions.companyID = companyCodeId;
                            ePositions.AdpYear = string.IsNullOrEmpty(adpYear) ? (int?)null : Convert.ToInt32(adpYear);
                            ePositions.AdpWSLimit = dAdpWSLimit;
                            ePositions.IsDesignated = designatedSupervisors != null ? true : false;
                            ePositions.CostNumberEffectiveDate = costNumberEffectiveDate;
                            ePositions.CostNumber = costNumber1Account;
                            ePositions.CostNumber1Percent = costNumber1Percent;
                            ePositions.CostNumber2Account = costNumber2Account;
                            ePositions.CostNumber2Percent = costNumber2Percent;
                            ePositions.CostNumber3Account = costNumber3Account;
                            ePositions.CostNumber3Percent = costNumber3Percent;
                            ePositions.CostNumber4Account = costNumber4Account;
                            ePositions.CostNumber4Percent = costNumber4Percent;
                            ePositions.CostNumber5Account = costNumber5Account;
                            ePositions.CostNumber5Percent = costNumber5Percent;
                            ePositions.CostNumber6Account = costNumber6Account;
                            ePositions.CostNumber6Percent = costNumber6Percent;
                            clientDbContext.E_Positions.Add(ePositions);
                            clientDbContext.SaveChanges();
                            nEPositionID = ePositions.E_PositionId;

                            if (lastEndDate != null)
                            {
                                if (startDate > lastEndDate.actualEndDate)
                                {
                                }
                                else
                                {
                                    clientDbContext.Database.ExecuteSqlCommand("update E_Positions set actualEndDate=@actualEndDate,PrimaryPosition=@PrimaryPosition where Employeeid=@Employeeid and E_PositionId != @EPositionId  and PositionId = @PositionId and convert(date,isnull(actualenddate,getdate()))>=convert(date,getdate())", new SqlParameter("@actualEndDate", DateTime.Now.AddDays(-1)), new SqlParameter("@PrimaryPosition", false), new SqlParameter("@Employeeid", nEmployeeId), new SqlParameter("@EPositionId", nEPositionID), new SqlParameter("@PositionId", nPositionID));
                                    clientDbContext.SaveChanges();

                                    clientDbContext.Database.ExecuteSqlCommand("update ep set ep.actualEndDate=@actualEndDate ,ep.PrimaryPosition=@PrimaryPosition from E_Positions ep join Employees emp on ep.EmployeeId=emp.EmployeeId where ep.Employeeid != @Employeeid  and emp.Personid= @PersonId and E_PositionId != @EPositionId and PositionId = @PositionId and convert(date,isnull(actualenddate,getdate()))>=convert(date,getdate())", new SqlParameter("@actualEndDate", DateTime.Now.AddDays(-1)), new SqlParameter("@PrimaryPosition", false), new SqlParameter("@Employeeid", nEmployeeId), new SqlParameter("@PersonId", nPersonId), new SqlParameter("@EPositionId", nEPositionID), new SqlParameter("@PositionId", nPositionID));
                                    clientDbContext.SaveChanges();

                                    clientDbContext.Database.ExecuteSqlCommand("update E_Positions set actualEndDate=@actualEndDate,PrimaryPosition=@PrimaryPosition where Employeeid=@Employeeid and E_PositionId != @EPositionId  and  isnull(CONVERT(date, ModifiedDate),CONVERT(date,EnteredDate)) != CONVERT(date, GETDATE()) and convert(date,isnull(actualenddate,getdate()))>=convert(date,getdate())", new SqlParameter("@actualEndDate", DateTime.Now.AddDays(-1)), new SqlParameter("@PrimaryPosition", false), new SqlParameter("@Employeeid", nEmployeeId), new SqlParameter("@EPositionId", nEPositionID));
                                    clientDbContext.SaveChanges();

                                    clientDbContext.Database.ExecuteSqlCommand("update ep set ep.actualEndDate=@actualEndDate ,ep.PrimaryPosition=@PrimaryPosition from E_Positions ep join Employees emp on ep.EmployeeId=emp.EmployeeId where ep.Employeeid != @Employeeid and emp.Personid= @PersonId and E_PositionId != @EPositionId  and isnull(CONVERT(date, ep.ModifiedDate),CONVERT(date,ep.EnteredDate)) != CONVERT(date, GETDATE()) and convert(date,isnull(actualenddate,getdate()))>=convert(date,getdate())", new SqlParameter("@actualEndDate", DateTime.Now.AddDays(-1)), new SqlParameter("@PrimaryPosition", false), new SqlParameter("@Employeeid", nEmployeeId), new SqlParameter("@PersonId", nPersonId), new SqlParameter("@EPositionId", nEPositionID));
                                    clientDbContext.SaveChanges();
                                }
                            }
                            else
                            {
                                clientDbContext.Database.ExecuteSqlCommand("update E_Positions set actualEndDate=@actualEndDate,PrimaryPosition=@PrimaryPosition where Employeeid=@Employeeid and E_PositionId != @EPositionId  and PositionId = @PositionId and convert(date,isnull(actualenddate,getdate()))>=convert(date,getdate())", new SqlParameter("@actualEndDate", DateTime.Now.AddDays(-1)), new SqlParameter("@PrimaryPosition", false), new SqlParameter("@Employeeid", nEmployeeId), new SqlParameter("@EPositionId", nEPositionID), new SqlParameter("@PositionId", nPositionID));
                                clientDbContext.SaveChanges();

                                clientDbContext.Database.ExecuteSqlCommand("update ep set ep.actualEndDate=@actualEndDate ,ep.PrimaryPosition=@PrimaryPosition from E_Positions ep join Employees emp on ep.EmployeeId=emp.EmployeeId where ep.Employeeid != @Employeeid  and emp.Personid= @PersonId and E_PositionId != @EPositionId and PositionId = @PositionId and convert(date,isnull(actualenddate,getdate()))>=convert(date,getdate())", new SqlParameter("@actualEndDate", DateTime.Now.AddDays(-1)), new SqlParameter("@PrimaryPosition", false), new SqlParameter("@Employeeid", nEmployeeId), new SqlParameter("@PersonId", nPersonId), new SqlParameter("@EPositionId", nEPositionID), new SqlParameter("@PositionId", nPositionID));
                                clientDbContext.SaveChanges();

                                clientDbContext.Database.ExecuteSqlCommand("update E_Positions set actualEndDate=@actualEndDate,PrimaryPosition=@PrimaryPosition where Employeeid=@Employeeid and E_PositionId != @EPositionId  and isnull(CONVERT(date, ModifiedDate),CONVERT(date,EnteredDate)) != CONVERT(date, GETDATE()) and convert(date,isnull(actualenddate,getdate()))>=convert(date,getdate())", new SqlParameter("@actualEndDate", DateTime.Now.AddDays(-1)), new SqlParameter("@PrimaryPosition", false), new SqlParameter("@Employeeid", nEmployeeId), new SqlParameter("@EPositionId", nEPositionID));
                                clientDbContext.SaveChanges();

                                clientDbContext.Database.ExecuteSqlCommand("update ep set ep.actualEndDate=@actualEndDate ,ep.PrimaryPosition=@PrimaryPosition from E_Positions ep join Employees emp on ep.EmployeeId=emp.EmployeeId where ep.Employeeid != @Employeeid  and emp.Personid= @PersonId and E_PositionId != @EPositionId and isnull(CONVERT(date, ep.ModifiedDate),CONVERT(date,ep.EnteredDate)) != CONVERT(date, GETDATE()) and convert(date,isnull(actualenddate,getdate()))>=convert(date,getdate())", new SqlParameter("@actualEndDate", DateTime.Now.AddDays(-1)), new SqlParameter("@PrimaryPosition", false), new SqlParameter("@Employeeid", nEmployeeId), new SqlParameter("@PersonId", nPersonId), new SqlParameter("@EPositionId", nEPositionID));
                                clientDbContext.SaveChanges();
                            }
                        }
                        else
                        {
                            
                            var objEpositions = (from x in clientDbContext.E_Positions where x.E_PositionId == nEPositionID select x).FirstOrDefault();
                            objEpositions.EmployeeId = nEmployeeId;
                            objEpositions.PositionId = nPositionID;
                            objEpositions.StartDate = startDate;
                            objEpositions.actualEndDate = endDate;
                            objEpositions.PrimaryPosition = isPrimaryPosition;
                            objEpositions.RateTypeId = rateId == 0 ? (int?)null : rateId;
                            objEpositions.PayFrequencyId = 2;
                            objEpositions.ModifiedBy = "ftpauto";
                            objEpositions.ModifiedDate = DateTime.Now;
                            objEpositions.PositionTypeID = 1;
                            objEpositions.EmployeeTypeId = employeeTypeId;
                            objEpositions.PayGroupId = payGroupId;
                            objEpositions.ReportsToID = nmPersonId == 0 ? (int?)null : nmPersonId;
                            objEpositions.FileNumber = fileNumber;
                            objEpositions.E_PositionId = nEPositionID;
                            objEpositions.DepartmentId = departmentId;
                            objEpositions.EmployeeClassId = empClassId;
                            objEpositions.companyID = companyCodeId;
                            objEpositions.AdpYear = string.IsNullOrEmpty(adpYear) ? (int?)null : Convert.ToInt32(adpYear);
                            objEpositions.AdpWSLimit = dAdpWSLimit;
                            objEpositions.IsDesignated = designatedSupervisors != null ? true : false;
                            objEpositions.CostNumberEffectiveDate = costNumberEffectiveDate;
                            objEpositions.CostNumber = costNumber1Account;
                            objEpositions.CostNumber1Percent = costNumber1Percent;
                            objEpositions.CostNumber2Account = costNumber2Account;
                            objEpositions.CostNumber2Percent = costNumber2Percent;
                            objEpositions.CostNumber3Account = costNumber3Account;
                            objEpositions.CostNumber3Percent = costNumber3Percent;
                            objEpositions.CostNumber4Account = costNumber4Account;
                            objEpositions.CostNumber4Percent = costNumber4Percent;
                            objEpositions.CostNumber5Account = costNumber5Account;
                            objEpositions.CostNumber5Percent = costNumber5Percent;
                            objEpositions.CostNumber6Account = costNumber6Account;
                            objEpositions.CostNumber6Percent = costNumber6Percent;
                            clientDbContext.SaveChanges();

                           
                                    clientDbContext.Database.ExecuteSqlCommand("update E_Positions set actualEndDate=@actualEndDate,PrimaryPosition=@PrimaryPosition where Employeeid=@Employeeid and E_PositionId != @EPositionId and PositionId != @PositionId  and isnull(CONVERT(date, ModifiedDate),CONVERT(date,EnteredDate)) <> CONVERT(date, GETDATE()) and convert(date,isnull(actualenddate,getdate()))>=convert(date,getdate())", new SqlParameter("@actualEndDate", DateTime.Now.AddDays(-1)), new SqlParameter("@PrimaryPosition", false), new SqlParameter("@Employeeid", nEmployeeId), new SqlParameter("@EPositionId", nEPositionID), new SqlParameter("@PositionId", nPositionID));
                                    clientDbContext.SaveChanges();

                                    clientDbContext.Database.ExecuteSqlCommand("update ep set ep.actualEndDate=@actualEndDate ,ep.PrimaryPosition=@PrimaryPosition from E_Positions ep join Employees emp on ep.EmployeeId=emp.EmployeeId where ep.Employeeid != @Employeeid and emp.Personid= @PersonId and E_PositionId != @EPositionId and PositionId != @PositionId  and isnull(CONVERT(date, ep.ModifiedDate),CONVERT(date,ep.EnteredDate)) != CONVERT(date, GETDATE()) and convert(date,isnull(actualenddate,getdate()))>=convert(date,getdate())", new SqlParameter("@actualEndDate", DateTime.Now.AddDays(-1)), new SqlParameter("@PrimaryPosition", false), new SqlParameter("@Employeeid", nEmployeeId), new SqlParameter("@PersonId", nPersonId), new SqlParameter("@EPositionId", nEPositionID), new SqlParameter("@PositionId", nPositionID));
                                    clientDbContext.SaveChanges();
                              
                        }
                        var currentdate = DateTime.Now.Date;
                        if (employeeStatus.ToLower().Trim() == "terminated" || terminationDate != null)
                        {
                            var epositionCount = clientDbContext.E_Positions.Where(x => x.EmployeeId == nEmployeeId && x.companyID == companyCodeId).Count();
                            if (epositionCount > 0)
                            {
                                if (DateTime.Compare(Convert.ToDateTime(DateTime.Now.ToShortDateString()), Convert.ToDateTime(terminationDate.Value.Date.ToShortDateString())) >= 0)
                                {
                                    clientDbContext.Database.ExecuteSqlCommand("update E_Positions set actualEndDate=@actualEndDate,PrimaryPosition=@PrimaryPosition where Employeeid=@Employeeid", new SqlParameter("@actualEndDate", terminationDate), new SqlParameter("@PrimaryPosition", false), new SqlParameter("@Employeeid", nEmployeeId));
                                    clientDbContext.SaveChanges();
                                    clientDbContext.Database.ExecuteSqlCommand("update E_Positions set PrimaryPosition=@PrimaryPosition where Employeeid=@Employeeid and actualEndDate is not null", new SqlParameter("@PrimaryPosition", false), new SqlParameter("@Employeeid", nEmployeeId));
                                    clientDbContext.SaveChanges();
                                    clientDbContext.Database.ExecuteSqlCommand("update Employees set TerminationDate=@TerminationDate,EmploymentStatusId=@EmploymentStatusId where Employeeid=@Employeeid", new SqlParameter("@TerminationDate", terminationDate), new SqlParameter("@EmploymentStatusId", nEmploymentStatusId), new SqlParameter("@Employeeid", nEmployeeId));
                                    clientDbContext.SaveChanges();
                                }
                            }

                            var ePositionSalaryHistory = clientDbContext.E_PositionSalaryHistories.Where(m => m.E_PositionId == nEPositionID && m.EndDate == null).OrderByDescending(m => m.EffectiveDate).FirstOrDefault();
                            if (ePositionSalaryHistory != null)
                            {
                                ePositionSalaryHistory.EndDate = terminationDate;
                                clientDbContext.SaveChanges();
                            }
                        }
                        if (terminationDate ==null || terminationDate >= currentdate)
                        {
                            var epositionactive = clientDbContext.E_Positions.Where(x => x.EmployeeId == nEmployeeId && (x.actualEndDate == null || x.actualEndDate >= currentdate)).FirstOrDefault();
                            if (epositionactive != null)
                            {
                                if (epositionactive.actualEndDate >= currentdate)
                                {
                                    if (terminationDate == null)
                                    {
                                        clientDbContext.Database.ExecuteSqlCommand("update Employees set TerminationDate=null,EmploymentStatusId=@EmploymentStatusId where Employeeid=@Employeeid", new SqlParameter("@EmploymentStatusId", nEmploymentStatusId), new SqlParameter("@Employeeid", nEmployeeId));
                                        var latestSalaryHistoryId = clientDbContext.E_PositionSalaryHistories.Where(x => x.E_PositionId == epositionactive.E_PositionId).OrderByDescending(x => x.E_PositionSalaryHistoryId).Select(x => x.E_PositionSalaryHistoryId).FirstOrDefault();
                                        if (endDate != null)
                                        {
                                            //updated enddate those having null for latestSalaryHistoryId--06/04/2019
                                            clientDbContext.Database.ExecuteSqlCommand("update E_PositionSalaryHistories set EndDate=@endDate where E_PositionSalaryHistoryId=@Epositionsalaryhistoryid and EndDate is null ", new SqlParameter("@endDate", endDate), new SqlParameter("@Epositionsalaryhistoryid", latestSalaryHistoryId));
                                        }
                                    }
                                    else
                                    {
                                        clientDbContext.Database.ExecuteSqlCommand("update Employees set TerminationDate=@TerminationDate,EmploymentStatusId=@EmploymentStatusId where Employeeid=@Employeeid", new SqlParameter("@TerminationDate", terminationDate), new SqlParameter("@EmploymentStatusId", nEmploymentStatusId), new SqlParameter("@Employeeid", nEmployeeId));
                                        clientDbContext.Database.ExecuteSqlCommand("update E_PositionSalaryHistories set EndDate=@TerminationDate where E_PositionId=@EpositionId and EndDate is null", new SqlParameter("@TerminationDate", terminationDate), new SqlParameter("@EpositionId", epositionactive.E_PositionId));
                                    }

                                    clientDbContext.SaveChanges();
                                }
                            }
                            else
                            {
                                if (terminationDate == null)
                                {
                                    clientDbContext.Database.ExecuteSqlCommand("update Employees set TerminationDate=null,EmploymentStatusId=@EmploymentStatusId where Employeeid=@Employeeid", new SqlParameter("@EmploymentStatusId", nEmploymentStatusId), new SqlParameter("@Employeeid", nEmployeeId));
                                    
                                }
                                else
                                {
                                    clientDbContext.Database.ExecuteSqlCommand("update Employees set TerminationDate=@TerminationDate,EmploymentStatusId=@EmploymentStatusId where Employeeid=@Employeeid", new SqlParameter("@TerminationDate", terminationDate), new SqlParameter("@EmploymentStatusId", nEmploymentStatusId), new SqlParameter("@Employeeid", nEmployeeId));
                                    
                                }
                                clientDbContext.SaveChanges();
                            }
                        }
                        if (designatedSupervisors != null)
                        {
                            var designatedPositions = new DesignatedPositions();
                            designatedPositions.ManagerPersonId = nmPreviousPersonId;
                            designatedPositions.E_PositionId = nEPositionID;
                            clientDbContext.DesignatedPositions.Add(designatedPositions);
                            clientDbContext.SaveChanges();
                        }

                        var drEffective = (from c in clientDbContext.E_PositionSalaryHistories
                                           join ep in clientDbContext.E_Positions on c.E_PositionId equals ep.E_PositionId
                                           join po in clientDbContext.Positions on ep.PositionId equals po.PositionId
                                           where c.E_PositionId == nEPositionID && c.PayRate == dPayRate
                                           select c.E_PositionSalaryHistoryId).ToList();
                        if (drEffective.Count() == 0)
                        {
                            var lasteffectivedate = clientDbContext.E_PositionSalaryHistories.Where(m => m.E_PositionId == nEPositionID).OrderByDescending(m => m.E_PositionSalaryHistoryId).FirstOrDefault();
                            var ePositionsSalHistory = new E_PositionSalaryHistories();
                            ePositionsSalHistory.E_PositionId = nEPositionID;
                            ePositionsSalHistory.EffectiveDate = rateEffectiveDate;
                            ePositionsSalHistory.RateTypeId = rateId;
                            ePositionsSalHistory.EnteredBy = "ftpauto";
                            ePositionsSalHistory.EnteredDate = DateTime.Now;
                            ePositionsSalHistory.HoursPerPayPeriod = 40.0m;
                            ePositionsSalHistory.IsDeleted = false;
                            ePositionsSalHistory.PayRate = dPayRate;
                            if (rateType.ToUpper().Trim() == "H")
                            {
                                ePositionsSalHistory.AnnualSalary = 26 * 40.0m * dPayRate;
                            }
                            else if (rateType.ToUpper().Trim() == "S")
                            {
                                ePositionsSalHistory.AnnualSalary = 26 * dPayRate;
                            }
                            clientDbContext.E_PositionSalaryHistories.Add(ePositionsSalHistory);
                            clientDbContext.SaveChanges();
                            if (lasteffectivedate != null)
                            {
                                var salaryEndDate = rateEffectiveDate.Value.AddDays(-1);
                                var ePosSalHisId = lasteffectivedate.E_PositionSalaryHistoryId;
                                clientDbContext.Database.ExecuteSqlCommand("Update [E_PositionSalaryHistories] SET EndDate = @EndDate WHERE E_PositionSalaryHistoryId = @E_PositionSalaryHistoryId", new SqlParameter("@EndDate", salaryEndDate), new SqlParameter("@E_PositionSalaryHistoryId", ePosSalHisId));
                                clientDbContext.SaveChanges();

                                var ePosSalaryHistory = (from x in clientDbContext.E_PositionSalaryHistories where x.E_PositionSalaryHistoryId == ePosSalHisId select x).FirstOrDefault();
                                if (endDate < ePosSalaryHistory.EffectiveDate)
                                {
                                    ePosSalaryHistory.EndDate = lasteffectivedate.EffectiveDate;
                                    clientDbContext.SaveChanges();
                                }
                            }
                        }
                        else
                        {
                            var drEffectiveWithRate = (from c in clientDbContext.E_PositionSalaryHistories
                                                       join ep in clientDbContext.E_Positions on c.E_PositionId equals ep.E_PositionId
                                                       join po in clientDbContext.Positions on ep.PositionId equals po.PositionId
                                                       where c.E_PositionId == nEPositionID && c.PayRate == dPayRate && c.EffectiveDate == rateEffectiveDate
                                                       select c.E_PositionSalaryHistoryId).ToList();
                            if (drEffectiveWithRate.Count == 0)
                            {
                                var lasteffectivedatenew = clientDbContext.E_PositionSalaryHistories.Where(m => m.E_PositionId == nEPositionID).OrderByDescending(m => m.E_PositionSalaryHistoryId).FirstOrDefault();
                                var ePositionsSalHistory = new E_PositionSalaryHistories();
                                ePositionsSalHistory.E_PositionId = nEPositionID;
                                ePositionsSalHistory.EffectiveDate = rateEffectiveDate;
                                ePositionsSalHistory.RateTypeId = rateId;
                                ePositionsSalHistory.EnteredBy = "ftpauto";
                                ePositionsSalHistory.EnteredDate = DateTime.Now;
                                ePositionsSalHistory.HoursPerPayPeriod = 40.0m;
                                ePositionsSalHistory.IsDeleted = false;
                                ePositionsSalHistory.PayRate = dPayRate;
                                if (rateType.ToUpper().Trim() == "H")
                                {
                                    ePositionsSalHistory.AnnualSalary = 26 * 40.0m * dPayRate;
                                }
                                else if (rateType.ToUpper().Trim() == "S")
                                {
                                    ePositionsSalHistory.AnnualSalary = 26 * dPayRate;
                                }
                                clientDbContext.E_PositionSalaryHistories.Add(ePositionsSalHistory);
                                clientDbContext.SaveChanges();
                                if (lasteffectivedatenew != null)
                                {
                                    var salaryEndDate = DateTime.Now.AddDays(-1);
                                    
                                    //Updated EndDate with EffectiveDate-1 if previous record EndDate is future date otherwise EndDate will be Imported-1--06/04/2019
                                    if (lasteffectivedatenew.EndDate != null && lasteffectivedatenew.EndDate >= currentdate)
                                    {
                                        salaryEndDate = rateEffectiveDate.Value.AddDays(-1);
                                    }
                                    var ePosSalHisId = lasteffectivedatenew.E_PositionSalaryHistoryId;
                                    clientDbContext.Database.ExecuteSqlCommand("Update [E_PositionSalaryHistories] SET EndDate = @EndDate WHERE E_PositionSalaryHistoryId = @E_PositionSalaryHistoryId", new SqlParameter("@EndDate", salaryEndDate), new SqlParameter("@E_PositionSalaryHistoryId", ePosSalHisId));
                                    clientDbContext.SaveChanges();

                                    var ePosSalaryHistory = (from x in clientDbContext.E_PositionSalaryHistories where x.E_PositionSalaryHistoryId == ePosSalHisId select x).FirstOrDefault();
                                    if (endDate < ePosSalaryHistory.EffectiveDate)
                                    {
                                        ePosSalaryHistory.EndDate = lasteffectivedatenew.EffectiveDate;
                                        clientDbContext.SaveChanges();
                                    }
                                }
                            }
                            else
                            {
                                var EPosSalHistoryId = Convert.ToInt32(drEffectiveWithRate[0]);
                                var objEPosSalHistory = (from x in clientDbContext.E_PositionSalaryHistories where x.E_PositionSalaryHistoryId == EPosSalHistoryId select x).FirstOrDefault();
                                objEPosSalHistory.E_PositionId = nEPositionID;
                                objEPosSalHistory.EffectiveDate = rateEffectiveDate;
                                objEPosSalHistory.RateTypeId = rateId;
                                objEPosSalHistory.ModifiedBy = "ftpauto";
                                objEPosSalHistory.ModifiedDate = DateTime.Now;
                                objEPosSalHistory.HoursPerPayPeriod = 40.0m;
                                objEPosSalHistory.IsDeleted = false;
                                objEPosSalHistory.PayRate = dPayRate;
                                objEPosSalHistory.EndDate = endDate;
                                if (rateType.ToUpper().Trim() == "H")
                                {
                                    objEPosSalHistory.AnnualSalary = 26 * 40.0m * dPayRate;
                                }
                                else if (rateType.ToUpper().Trim() == "S")
                                {
                                    objEPosSalHistory.AnnualSalary = 26 * dPayRate;
                                }
                                clientDbContext.SaveChanges();

                                clientDbContext.Database.ExecuteSqlCommand("update epsh set epsh.EndDate = ep.actualEndDate from E_PositionSalaryHistories epsh inner join E_Positions ep on ep.E_PositionId = epsh.E_PositionId inner join Employees ee on ee.EmployeeId = ep.EmployeeId where ep.EmployeeId = @EmployeeId and epsh.E_PositionSalaryHistoryId = @EPositionSalaryHistoryId and isnull(CONVERT(date, epsh.ModifiedDate),CONVERT(date,epsh.EnteredDate)) != CONVERT(date, GETDATE())", new SqlParameter("@EmployeeId", nEmployeeId), new SqlParameter("@EPositionSalaryHistoryId", EPosSalHistoryId));
                                clientDbContext.SaveChanges();

                                clientDbContext.Database.ExecuteSqlCommand("update epsh set epsh.EndDate = ep.actualEndDate from E_PositionSalaryHistories epsh inner join E_Positions ep on ep.E_PositionId = epsh.E_PositionId inner join Employees ee on ee.EmployeeId = ep.EmployeeId where ep.Employeeid != @EmployeeId and ee.Personid = @PersonId and ep.E_PositionId != @EPositionId and epsh.E_PositionSalaryHistoryId != @EPositionSalaryHistoryId and isnull(CONVERT(date, epsh.ModifiedDate),CONVERT(date,epsh.EnteredDate)) != CONVERT(date, GETDATE())", new SqlParameter("@EmployeeId", nEmployeeId), new SqlParameter("@PersonId", nPersonId), new SqlParameter("@EPositionId", nEPositionID), new SqlParameter("@EPositionSalaryHistoryId", EPosSalHistoryId));
                                clientDbContext.SaveChanges();
                            }
                        }
                        var getActualEndDate = clientDbContext.E_Positions.Where(x => x.E_PositionId == nEPositionID).Select(x => x.actualEndDate).FirstOrDefault();
                        if (getActualEndDate != null)
                        {
                            var ePositionSalaryHistory = clientDbContext.E_PositionSalaryHistories.Where(m => m.E_PositionId == nEPositionID && m.EndDate == null).FirstOrDefault();
                            if (ePositionSalaryHistory != null)
                            {
                                ePositionSalaryHistory.EndDate = getActualEndDate;
                                clientDbContext.SaveChanges();
                            }
                        }

                        clientDbContext.Database.ExecuteSqlCommand("update epsh set epsh.EndDate = ep.actualEndDate from E_PositionSalaryHistories epsh inner join E_Positions ep on ep.E_PositionId = epsh.E_PositionId inner join Employees ee on ee.EmployeeId = ep.EmployeeId where ep.EmployeeId = @EmployeeId and ep.E_PositionId != @EPositionId and ep.actualEndDate is not null and epsh.EndDate is null", new SqlParameter("@EmployeeId", nEmployeeId), new SqlParameter("@EPositionId", nEPositionID));
                        clientDbContext.SaveChanges();

                        clientDbContext.Database.ExecuteSqlCommand("update epsh set epsh.EndDate = ep.actualEndDate from E_PositionSalaryHistories epsh inner join E_Positions ep on ep.E_PositionId = epsh.E_PositionId inner join Employees ee on ee.EmployeeId = ep.EmployeeId where ep.Employeeid != @EmployeeId and ee.Personid = @PersonId and ep.E_PositionId != @EPositionId and ep.actualEndDate is not null and epsh.EndDate is null", new SqlParameter("@EmployeeId", nEmployeeId), new SqlParameter("@PersonId", nPersonId), new SqlParameter("@EPositionId", nEPositionID));
                        clientDbContext.SaveChanges();
                    }
                }
                clientDbContext.Database.ExecuteSqlCommand("update E_Positions set ModifiedDate = GETDATE()-1  where CONVERT(date, ModifiedDate) = CONVERT(date, GETDATE())");
                clientDbContext.SaveChanges();

                clientDbContext.Database.ExecuteSqlCommand("update E_Positions set EnteredDate = GETDATE()-1 where CONVERT(date,EnteredDate) = CONVERT(date, GETDATE())");
                clientDbContext.SaveChanges();

                clientDbContext.Database.ExecuteSqlCommand("update E_Positionsalaryhistories set ModifiedDate = GETDATE()-1 where CONVERT(date, ModifiedDate) = CONVERT(date, GETDATE())");
                clientDbContext.SaveChanges();

                clientDbContext.Database.ExecuteSqlCommand("update E_Positionsalaryhistories set EnteredDate = GETDATE()-1 where CONVERT(date,EnteredDate) = CONVERT(date, GETDATE())");
                clientDbContext.SaveChanges();
                if (sheet.Count == 1)
                {
                    sbErrors.AppendLine("There were no records to process");
                }
            }
            catch (Exception err)
            {
                sbErrors.AppendLine("Error trying to parse file = " + strFullPath);
                sbErrors.AppendLine(err.Message + System.Environment.NewLine + err.StackTrace);
            }
            return sbErrors;
        }

        private static List<NewBannerImportFileVM> GenerateNewBannerImportList(IList<IList<string>> sheet)
        {
            var newBannerImportFileList = new List<NewBannerImportFileVM>();
            try
            {
                int nLoopCount = 0;
                foreach (var row in sheet)
                {
                    nLoopCount++;
                    if (nLoopCount == 1) continue;
                    else
                    {
                        var newBannerImportFile = new NewBannerImportFileVM();
                        newBannerImportFile.nzrmast_pidm = ConvertToFloat(row[0]);
                        newBannerImportFile.nzrmast_id = ConvertToFloat(row[1]);
                        newBannerImportFile.nzrmast_adpid = ConvertToFloat(row[2]);
                        newBannerImportFile.nzrmast_employee_last_name = ConvertToString(row[3]);
                        newBannerImportFile.nzrmast_employee_first_name = ConvertToString(row[4]);
                        newBannerImportFile.nzrmast_employee_mi = ConvertToString(row[5]);
                        newBannerImportFile.nzrmast_taxid = ConvertToFloat(row[6]);
                        newBannerImportFile.nzrmast_birth_date = ConvertToDateTime(row[7]);
                        newBannerImportFile.nzrmast_empr_code = ConvertToString(row[8]);
                        newBannerImportFile.nzrmast_email_address = ConvertToString(row[9]);
                        newBannerImportFile.nzrmast_home_phone = ConvertToFloat(row[10]);
                        newBannerImportFile.nzrmast_hire_status = ConvertToString(row[11]);
                        newBannerImportFile.nzrmast_current_hire_date = ConvertToDateTime(row[12]);
                        newBannerImportFile.nzrmast_termination_date = ConvertToString(row[13]);
                        newBannerImportFile.nzrmast_home_orgn_code = ConvertToString(row[14]);
                        newBannerImportFile.nzrmast_home_orgn_code_desc = ConvertToString(row[15]);
                        newBannerImportFile.nzrmast_pay_frequency_code = ConvertToString(row[16]);
                        newBannerImportFile.nzrmast_ecls_code = ConvertToString(row[17]);
                        newBannerImportFile.nzrmast_position = ConvertToString(row[18]);
                        newBannerImportFile.nzrmast_suff = ConvertToString(row[19]).PadLeft(2, '0');
                        newBannerImportFile.nzrmast_job_orgn_code = ConvertToString(row[20]);
                        newBannerImportFile.nzrmast_job_orgn_code_desc = ConvertToString(row[21]);
                        newBannerImportFile.nzrmast_job_status = ConvertToString(row[22]);
                        newBannerImportFile.nzrmast_primary_ind = ConvertToString(row[23]);
                        newBannerImportFile.nzrmast_job_desc = ConvertToString(row[24]);
                        newBannerImportFile.nzrmast_start_date = ConvertToDateTime(row[25]);
                        newBannerImportFile.nzrmast_end_date = ConvertToDateTime(row[26]);
                        newBannerImportFile.nzrmast_eff_date = ConvertToDateTime(row[27]);
                        newBannerImportFile.nzrmast_rate_amount = ConvertToFloat(row[28]);
                        newBannerImportFile.nzrmast_rate_type = ConvertToString(row[29]);
                        newBannerImportFile.nzrmast_pay_group_code = ConvertToFloat(row[30]);
                        newBannerImportFile.nzrmast_pay_group_desc = ConvertToString(row[31]);
                        newBannerImportFile.nzrmast_fact = ConvertToFloat(row[32]);
                        newBannerImportFile.nzrmast_pays = ConvertToFloat(row[33]);
                        newBannerImportFile.nzrmast_status_flag_1_code = ConvertToFloat(row[34]);
                        newBannerImportFile.nzrmast_status_flag_1_desc = ConvertToString(row[35]);
                        newBannerImportFile.nzrmast_appr_pidm = ConvertToFloat(row[36]);
                        newBannerImportFile.nzrmast_app_posn = ConvertToString(row[37]);
                        newBannerImportFile.nzrmast_appr_adpid = ConvertToFloat(row[38]);
                        newBannerImportFile.nzrmast_appr_last_name = ConvertToString(row[39]);
                        newBannerImportFile.nzrmast_appr_first_name = ConvertToString(row[40]);
                        newBannerImportFile.nzrmast_appr_mi = ConvertToString(row[41]);
                        newBannerImportFile.nzrmast_treaty_limit = ConvertToString(row[42]);
                        newBannerImportFile.nzrmast_academic_period = ConvertToString(row[43]);
                        newBannerImportFile.nzrmast_ws_limit = ConvertToString(row[44]);
                        newBannerImportFile.nzrmast_jlbd_eff_date = row.Count > 45 ? ConvertToDateTime(row[45]) : null;
                        newBannerImportFile.nzrmast_account1 = row.Count > 46 ? (ConvertToString(row[46])) : "000000000000000000";
                        newBannerImportFile.nzrmast_percent1 = row.Count > 47 ? ConvertToFloat(row[47]) : 100;
                        newBannerImportFile.nzrmast_account2 = row.Count > 48 ? (ConvertToString(row[48])) : string.Empty;
                        newBannerImportFile.nzrmast_percent2 = row.Count > 49 ? (ConvertToString(row[49])) : string.Empty;
                        newBannerImportFile.nzrmast_account3 = row.Count > 50 ? (ConvertToString(row[50])) : string.Empty;
                        newBannerImportFile.nzrmast_percent3 = row.Count > 51 ? (ConvertToString(row[51])) : string.Empty;
                        newBannerImportFile.nzrmast_account4 = row.Count > 52 ? (ConvertToString(row[52])) : string.Empty;
                        newBannerImportFile.nzrmast_percent4 = row.Count > 53 ? (ConvertToString(row[53])) : string.Empty;
                        newBannerImportFile.nzrmast_account5 = row.Count > 54 ? (ConvertToString(row[54])) : string.Empty;
                        newBannerImportFile.nzrmast_percent5 = row.Count > 55 ? (ConvertToString(row[55])) : string.Empty;
                        newBannerImportFile.nzrmast_account6 = row.Count > 56 ? (ConvertToString(row[56])) : string.Empty;
                        newBannerImportFile.nzrmast_percent6 = row.Count > 57 ? (ConvertToString(row[57])) : string.Empty;
                        newBannerImportFileList.Add(newBannerImportFile);
                    }
                }
                return newBannerImportFileList;
            }
            catch (Exception ex)
            {
                return newBannerImportFileList;
            }
        }
        private static void updateSFTPBannerFileStatus(string FileName)
        {
            string strConnection = ConfigurationManager.ConnectionStrings["execViewDb"].ConnectionString;
            ClientDbContext clientDbContext = new ClientDbContext(strConnection);
            var todaysDate = DateTime.Now.Date;
            var objFtpBannerFileStatus = (from x in clientDbContext.FTPBannerFileStatus where x.Date == todaysDate select x).FirstOrDefault();
            if (objFtpBannerFileStatus == null)
            {
                var ftpBannerFileStatus = new FTPBannerFileStatus();
                ftpBannerFileStatus.Date = DateTime.Now.Date;
                ftpBannerFileStatus.NoOfTimeProcessed = 1;
                if (string.IsNullOrEmpty(FileName))
                {
                    ftpBannerFileStatus.IsStudentBannerStatus = false;
                }
                else
                {
                    ftpBannerFileStatus.IsStudentBannerStatus = true;
                }
                ftpBannerFileStatus.IsEmailSent = false;
                clientDbContext.FTPBannerFileStatus.Add(ftpBannerFileStatus);
                clientDbContext.SaveChanges();
            }
            else
            {
                if (objFtpBannerFileStatus.NoOfTimeProcessed < 4)
                {
                    var numberOfProcessing = objFtpBannerFileStatus.NoOfTimeProcessed + 1;
                    objFtpBannerFileStatus.Date = DateTime.Now.Date;
                    objFtpBannerFileStatus.NoOfTimeProcessed = numberOfProcessing;
                    if (objFtpBannerFileStatus.IsStudentBannerStatus == false && !string.IsNullOrEmpty(FileName))
                    {
                        objFtpBannerFileStatus.IsStudentBannerStatus = true;
                    }
                    objFtpBannerFileStatus.IsEmailSent = false;
                    clientDbContext.SaveChanges();
                }
            }

            var ftpBannerFileStatusInADay = (from x in clientDbContext.FTPBannerFileStatus where x.Date == todaysDate select x).FirstOrDefault();
            if (ftpBannerFileStatusInADay.NoOfTimeProcessed == 4 && ftpBannerFileStatusInADay.IsStudentBannerStatus == false && ftpBannerFileStatusInADay.IsEmailSent == false)
            {
                sendEmailWhenNoFileAttached();
                ftpBannerFileStatusInADay.IsEmailSent = true;
                clientDbContext.SaveChanges();
            }
        }
        private static void sendEmailWhenNoFileAttached()
        {
            string strFrom = ConfigurationManager.AppSettings["fromAddress"].ToString();
            string strTo = ConfigurationManager.AppSettings["toAddWhenNoFileAttached"].ToString();
            string strToSecond = ConfigurationManager.AppSettings["toAddress"].ToString();
            string strToThree = ConfigurationManager.AppSettings["toAddressTwo"].ToString();
            EmailProcessorCommunity.sendEmailForSFTPBanner(strFrom, strTo, strToSecond, strToThree, "Drew University Email When No File is Received to the SFTP Server in a day.", "This is an automated message from the Drew University."
                + Environment.NewLine + Environment.NewLine
                + "No Student Banner File Found In a Day."
                + Environment.NewLine, false);
        }

        private static void writeResultsToFile(string strResults, out string strFileName)
        {
            DirectoryInfo di = null;
            FileStream fsOut = null;
            StreamWriter sw = null;
            string strMapPath = string.Empty;
            strFileName = string.Empty;
            string strPath = string.Empty;
            string strPathAndFile = string.Empty;
            try
            {
                string strClientId = System.Configuration.ConfigurationManager.AppSettings["ClientID"].ToString();
                strMapPath = System.Configuration.ConfigurationManager.AppSettings["appRoot"].ToString();
                strPath = strMapPath + "\\ResultsFiles";
                if (!Directory.Exists(strPath))
                    di = Directory.CreateDirectory(strPath);
                strFileName = strClientId + "Import" + DateTime.Now.Ticks + ".txt";
                strPathAndFile = strMapPath + "\\ResultsFiles\\" + strFileName;
                if (File.Exists(strPathAndFile))
                    File.Delete(strPathAndFile);

                fsOut = File.OpenWrite(strPathAndFile);
                sw = new StreamWriter(fsOut);
                sw.WriteLine("Results of import - " + DateTime.Now);
                sw.WriteLine();
                sw.WriteLine(strResults);
                if (sw != null)
                {
                    sw.Flush();
                    sw.Close();
                    sw = null;
                }
                if (fsOut != null)
                {
                    fsOut.Close();
                    fsOut = null;
                }
            }
            catch (Exception err)
            {
            }
            finally
            {
                if (sw != null)
                {
                    sw.Flush();
                    sw.Close();
                }
                if (fsOut != null)
                    fsOut.Close();
            }
        }

        private static Decimal? ConvertToDecimal(string strDecimalIn)
        {
            Decimal dTemp = -1111.456m;
            if (Decimal.TryParse(strDecimalIn, out dTemp))
                return dTemp;
            else
                return null;
        }

        private static float? ConvertToFloat(string strFloatVal)
        {
            float temp_value = 0;
            if (float.TryParse(strFloatVal, out temp_value))
                return temp_value;
            else
                return null;
        }

        private static DateTime? ConvertToDateTime(string strDateIn)
        {
            DateTime tempDate = DateTime.MaxValue;
            if (DateTime.TryParse(strDateIn, out tempDate))
                return tempDate;
            else
                return null;
        }

        private static string LowerTrimFunction(string strString)
        {
            return strString.ToLower().Trim();
        }

        private static string ConvertToString(string strString)
        {
            return strString == null ? string.Empty : strString.ToString().Trim();
        }

        private static decimal TruncateDecimal(decimal value, int precision)
        {
            decimal step = (decimal)Math.Pow(10, precision);
            decimal tmp = Math.Truncate(step * value);
            return tmp / step;
        }

        private static void BulkCopyData(string _connectionString, string _destinationTableName, DataTable _dt)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.TableLock | SqlBulkCopyOptions.FireTriggers | SqlBulkCopyOptions.UseInternalTransaction, null))
                {
                    using (SqlCommand cmdTruncate = new SqlCommand("truncate table NewBannerImportFile", connection))
                    {
                        connection.Open();
                        cmdTruncate.CommandTimeout = 300;
                        cmdTruncate.ExecuteNonQuery();

                        bulkCopy.DestinationTableName = _destinationTableName;
                        bulkCopy.BulkCopyTimeout = 300;
                        bulkCopy.BatchSize = 10000;
                        try
                        {
                            bulkCopy.WriteToServer(_dt);
                            connection.Close();
                        }
                        catch (Exception ex)
                        {
                            connection.Close();
                        }
                    }
                }
            }
        }
    }
}