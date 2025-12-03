using ExecViewHrk.EfAdmin;
using ExecViewHrk.EfClient;
using ExecViewHrk.WebUI.Infrastructure;
using ExecViewHrk.WebUI.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace ExecViewHrk.WebUI.Controllers
{
    public class TestController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(HttpPostedFileBase postedFile)// Import Employee
        {
            List<EmployeeImport> customers = new List<EmployeeImport>();
            string filePath = string.Empty;
            if (postedFile != null)
            {
                try
                {
                    string path = Server.MapPath("~/Uploads/");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    filePath = path + Path.GetFileName(postedFile.FileName);
                    string extension = Path.GetExtension(postedFile.FileName);
                    postedFile.SaveAs(filePath);

                    //Read the contents of CSV file.
                    string csvData = System.IO.File.ReadAllText(filePath);

                    //Execute a loop over the rows.
                    foreach (string row in csvData.Split('\r'))
                    {
                        if (!string.IsNullOrEmpty(row))
                        {
                            if (row.Split(',')[0].ToString() != "Last Name")
                            {
                                customers.Add(new EmployeeImport
                                {
                                    LastName = row.Split(',')[0],////////////
                                    FirstName = row.Split(',')[1],////////////
                                    Birthdate_PersonalDta = row.Split(',')[2],////////////
                                    HireDate = row.Split(',')[3],/////////////
                                    TerminationDate = row.Split(',')[4],/////////////
                                    PayGroup_JobDta = row.Split(',')[5],
                                    AutolinkFileNumber_JobDta = row.Split(',')[6],///////////
                                    EmployeeStatus_JobDta = row.Split(',')[7],
                                    CompensationRate_JobDta = row.Split(',')[8],
                                    EffectiveDate_JobDta = row.Split(',')[9],
                                    CompensationFrequency_JobDta = row.Split(',')[10],
                                    StandardHours_JobDta = row.Split(',')[11],
                                    PayFrequency_PayGroupInfo = row.Split(',')[12],
                                    LocationCode_JobDta = row.Split(',')[13],
                                    JobCode_JobDta = row.Split(',')[14],
                                    Description_JobCodeInfo = row.Split(',')[15],
                                });
                            }
                        }
                    }

                    foreach (var row in customers)
                    {
                        SqlConnection con = new SqlConnection(User.Identity.GetClientConnectionString());

                        string sql = "INSERT INTO [dbo].[EmployeeMaster] " +
                        "([LastName],[FirstName],[Birthdate_PersonalDta],[HireDate],[TerminationDate],[PayGroup_JobDta],[AutolinkFileNumber_JobDta], " +
                        "[EmployeeStatus_JobDta],[CompensationRate_JobDta],[EffectiveDate_JobDta],[CompensationFrequency_JobDta],[StandardHours_JobDta], " +
                        "[PayFrequency_PayGroupInfo],[LocationCode_JobDta],[JobCode_JobDta],[Description_JobCodeInfo])  " +

                        "VALUES " +
                        "('" + row.LastName.Replace("'", "''") + "','" + row.FirstName.Replace("'", "''") + "','" + row.Birthdate_PersonalDta + "','" + row.HireDate + "', " +
                        "'" + row.TerminationDate + "','" + row.PayGroup_JobDta + "','" + row.AutolinkFileNumber_JobDta + "', " +
                        "'" + row.EmployeeStatus_JobDta + "','" + row.CompensationRate_JobDta + "','" + row.EffectiveDate_JobDta + "', " +
                        "'" + row.CompensationFrequency_JobDta + "','" + row.StandardHours_JobDta + "','" + row.PayFrequency_PayGroupInfo + "', " +
                        "'" + row.LocationCode_JobDta + "','" + row.JobCode_JobDta.Replace("'", "''") + "','" + row.Description_JobCodeInfo.Replace("'", "''") + "')";

                        con.Open();
                        SqlCommand cmd = new SqlCommand(sql, con);

                        cmd.CommandType = CommandType.Text;
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
                catch (Exception ex)
                {
                    return Content(ex.Message);
                }
            }
            return View();
        }

        [HttpPost]
        public ActionResult UpdateDB(FormCollection frm)
        {
            try
            {
                List<EmployeeImport> customers = new List<EmployeeImport>();
                string filePath = string.Empty;
                string count = frm["count"].ToString();
                SqlConnection con1 = new SqlConnection(User.Identity.GetClientConnectionString());
                string sql = "Select TOP 500 * from EmployeeMaster WHERE ID > '" + count + "' ORDER BY ID ASC";
                //string sql = "Select TOP 300 * from EmployeeMaster WHERE ID = 529 ORDER BY ID ASC";
                con1.Open();
                SqlCommand cmd1 = new SqlCommand(sql, con1);
                SqlDataAdapter da = new SqlDataAdapter(cmd1);
                DataSet ds = new DataSet();
                da.Fill(ds, "EmployeeMaster");
                con1.Close();
                //for (int i = 0; i < 1; i++)
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    customers.Add(new EmployeeImport
                    {
                        ID = Convert.ToInt32(ds.Tables[0].Rows[i]["ID"]),
                        LastName = ds.Tables[0].Rows[i]["LastName"].ToString(),
                        FirstName = ds.Tables[0].Rows[i]["FirstName"].ToString(),
                        Birthdate_PersonalDta = ds.Tables[0].Rows[i]["Birthdate_PersonalDta"].ToString(),
                        HireDate = ds.Tables[0].Rows[i]["HireDate"].ToString(),
                        TerminationDate = ds.Tables[0].Rows[i]["TerminationDate"].ToString(),
                        PayGroup_JobDta = ds.Tables[0].Rows[i]["PayGroup_JobDta"].ToString(),
                        AutolinkFileNumber_JobDta = ds.Tables[0].Rows[i]["AutolinkFileNumber_JobDta"].ToString(),
                        EmployeeStatus_JobDta = ds.Tables[0].Rows[i]["EmployeeStatus_JobDta"].ToString(),
                        CompensationRate_JobDta = ds.Tables[0].Rows[i]["CompensationRate_JobDta"].ToString(),
                        EffectiveDate_JobDta = ds.Tables[0].Rows[i]["EffectiveDate_JobDta"].ToString(),
                        CompensationFrequency_JobDta = ds.Tables[0].Rows[i]["CompensationFrequency_JobDta"].ToString(),
                        StandardHours_JobDta = ds.Tables[0].Rows[i]["StandardHours_JobDta"].ToString(),
                        PayFrequency_PayGroupInfo = ds.Tables[0].Rows[i]["PayFrequency_PayGroupInfo"].ToString(),
                        LocationCode_JobDta = ds.Tables[0].Rows[i]["LocationCode_JobDta"].ToString(),
                        JobCode_JobDta = ds.Tables[0].Rows[i]["JobCode_JobDta"].ToString(),
                        Description_JobCodeInfo = ds.Tables[0].Rows[i]["Description_JobCodeInfo"].ToString(),
                    });
                }

                //#region "Employment Info"
                ///Last Name
                ///Employment Info > Identity

                ///First Name
                ///Employment Info > Identity

                ///Birthdate - Personal Dta
                ///Employment Info > Identity

                ///Hire Date
                ///Employment Info > Employee

                ///Termination Date
                ///Employment Info > Employee

                ///Employee Status -Job Dta
                ///Employment Info > Employee

                ///Autolink File Number -Job Dta
                ///Employment Info > Employee

                ///Pay Frequency - Pay Group Info
                ///Employment Info > Employee > Pay Frequency
                //#endregion

                //#region "Job Setup"
                ///Job Code -Job Dta
                ///Job Setup > Job Code

                ///Description - Job Code Info
                ///Job Setup > Job Title
                //#endregion

                //#region "Position Setup"

                ///Location Code - Job Dta
                ///Position Setup > Location

                ///Standard Hours -Job Dta
                ///Position Setup > Standard Hours
                //#endregion

                /////Pay Group -Job Dta
                /////Salary Details > Pay Group
                ////Compensation Rate - Job Dta  ->Not On Screen
                ////Effective Date - Job Dta ->Not On Screen
                ////Compensation Frequency - Job Dta ->Not On Screen

                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                string connString = User.Identity.GetClientConnectionString();

                ClientDbContext clientDbContext = new ClientDbContext(connString);

                foreach (var row in customers)
                {

                    var _newssn = ("000000000" + row.ID);
                    _newssn = _newssn.Substring(_newssn.Length - 9);
                    var _newEmail = (row.FirstName + "" + row.LastName + "" + row.AutolinkFileNumber_JobDta + "@execview.com").ToLower();

                    if (clientDbContext.Persons.Where(x => x.SSN == _newssn || x.eMail == _newEmail).Count() == 0)
                    {
                        DateTime? dateBirthDate = null;
                        if (!string.IsNullOrEmpty(row.Birthdate_PersonalDta))
                        {
                            dateBirthDate = Convert.ToDateTime(row.Birthdate_PersonalDta);
                        }

                        var _person = new Person
                        {
                            SSN = _newssn,
                            Lastname = row.LastName,
                            Firstname = row.FirstName,
                            PreferredName = row.FirstName + " " + row.LastName,
                            DOB = dateBirthDate,
                            eMail = _newEmail,
                            EnteredDate = DateTime.UtcNow,
                            EnteredBy = User.Identity.Name
                        };


                        clientDbContext.Persons.Add(_person);
                        clientDbContext.SaveChanges();

                        //-------Employee

                        DateTime? dateTerminationDate = null;
                        if (!string.IsNullOrEmpty(row.TerminationDate))
                        {
                            dateTerminationDate = Convert.ToDateTime(row.TerminationDate);
                        }

                        int _EmploymentStatusId, _PayFrequencyId;
                        if (clientDbContext.DdlEmploymentStatuses.Where(x => x.Code == row.EmployeeStatus_JobDta).Count() > 0)
                        {
                            _EmploymentStatusId = clientDbContext.DdlEmploymentStatuses.Where(x => x.Code == row.EmployeeStatus_JobDta).FirstOrDefault().EmploymentStatusId;
                        }
                        else
                        {
                            var _newEmploymentStatus = new DdlEmploymentStatus { Description = row.EmployeeStatus_JobDta, Code = row.EmployeeStatus_JobDta, Active = true };
                            clientDbContext.DdlEmploymentStatuses.Add(_newEmploymentStatus);
                            clientDbContext.SaveChanges();
                            _EmploymentStatusId = _newEmploymentStatus.EmploymentStatusId;
                        }

                        if (clientDbContext.DdlPayFrequencies.Where(x => x.Code == row.PayFrequency_PayGroupInfo).Count() > 0)
                        {
                            _PayFrequencyId = clientDbContext.DdlPayFrequencies.Where(x => x.Code == row.PayFrequency_PayGroupInfo).FirstOrDefault().PayFrequencyId;
                        }
                        else
                        {
                            var _newPayFrequency = new DdlPayFrequency { Description = row.PayFrequency_PayGroupInfo, Code = row.PayFrequency_PayGroupInfo, Active = true };
                            clientDbContext.DdlPayFrequencies.Add(_newPayFrequency);
                            clientDbContext.SaveChanges();
                            _PayFrequencyId = _newPayFrequency.PayFrequencyId;
                        }
                        var _companyCodeId = clientDbContext.CompanyCodes.Where(x => x.IsCompanyCodeActive == true).FirstOrDefault().CompanyCodeId;

                        var _employee = clientDbContext.Employees.Where(x => x.FileNumber == row.AutolinkFileNumber_JobDta).FirstOrDefault();
                        if (_employee != null)
                        {
                            _employee.HireDate = Convert.ToDateTime(row.HireDate);
                            _employee.TerminationDate = dateTerminationDate;
                            _employee.FileNumber = row.AutolinkFileNumber_JobDta;
                            _employee.EmploymentStatusId = _EmploymentStatusId;
                            _employee.PayFrequencyId = _PayFrequencyId;
                            _employee.PersonId = _person.PersonId;
                            _employee.ModifiedDate = DateTime.UtcNow;
                            _employee.ModifiedBy = User.Identity.Name;
                        }
                        else
                        {
                            _employee = new Employee
                            {
                                HireDate = Convert.ToDateTime(row.HireDate),
                                TerminationDate = dateTerminationDate,
                                FileNumber = row.AutolinkFileNumber_JobDta,
                                EmploymentStatusId = _EmploymentStatusId,
                                PayFrequencyId = _PayFrequencyId,
                                PersonId = _person.PersonId,
                                CompanyCodeId = _companyCodeId,
                                EnteredDate = DateTime.UtcNow,
                                EnteredBy = User.Identity.Name
                            };
                            clientDbContext.Employees.Add(_employee);
                        }

                        clientDbContext.SaveChanges(); // For Employee

                        var _job = clientDbContext.Jobs.Where(x => x.JobCode == row.JobCode_JobDta.Trim()).FirstOrDefault();
                        if (_job != null)
                        {
                            _job.JobCode = row.JobCode_JobDta;
                            _job.JobDescription = row.Description_JobCodeInfo;
                            _job.createdDate = DateTime.UtcNow;
                            _job.CompanyCodeId = _companyCodeId;
                            _job.IsJobActive = true;
                        }
                        else
                        {
                            _job = new Job
                            {
                                JobCode = row.JobCode_JobDta,
                                title = row.Description_JobCodeInfo,
                                JobDescription = row.Description_JobCodeInfo,
                                createdDate = DateTime.UtcNow,
                                CompanyCodeId = _companyCodeId,
                                IsJobActive = true,
                                enteredDate = DateTime.UtcNow,
                                enteredBy = User.Identity.Name
                            };
                            clientDbContext.Jobs.Add(_job);
                        }
                        clientDbContext.SaveChanges(); // Save Job

                        int _LocationId;
                        if (clientDbContext.Locations.Where(x => x.LocationCode == row.LocationCode_JobDta).Count() > 0)
                        {
                            _LocationId = clientDbContext.Locations.Where(x => x.LocationCode == row.LocationCode_JobDta).FirstOrDefault().LocationId;
                        }
                        else
                        {
                            var _newLocation = new Location { LocationDescription = row.LocationCode_JobDta, LocationCode = row.LocationCode_JobDta, Active = true };
                            clientDbContext.Locations.Add(_newLocation);
                            clientDbContext.SaveChanges();
                            _LocationId = _newLocation.LocationId;
                        }

                        int _PayGroupId;
                        if (clientDbContext.DdlPayGroups.Where(x => x.Code == row.PayGroup_JobDta).Count() > 0)
                        {
                            _PayGroupId = clientDbContext.DdlPayGroups.Where(x => x.Code == row.PayGroup_JobDta).FirstOrDefault().PayGroupId;
                        }
                        else
                        {
                            var _newPayGroup = new DdlPayGroup { Description = row.PayGroup_JobDta, Code = row.PayGroup_JobDta, Active = true };
                            clientDbContext.DdlPayGroups.Add(_newPayGroup);
                            clientDbContext.SaveChanges();
                            _PayGroupId = _newPayGroup.PayGroupId;
                        }

                        var _PositionCodeNew = row.LocationCode_JobDta.Trim() + "-" + row.JobCode_JobDta.Trim();
                        var _position = clientDbContext.Positions.Where(x => x.JobId == _job.JobId && x.PositionCode == _PositionCodeNew).FirstOrDefault();
                        if (_position != null)
                        {
                            _position.ScheduledHours = row.StandardHours_JobDta;
                            _position.LocationId = _LocationId;
                            _position.SalaryPayGroup = _PayGroupId.ToString();
                            _position.LastModifiedDate = DateTime.UtcNow;
                            _position.IsPositionActive = true;
                        }
                        else
                        {
                            _position = new Position();
                            _position.ScheduledHours = row.StandardHours_JobDta;
                            _position.LocationId = _LocationId;
                            _position.JobId = _job.JobId;
                            _position.BusinessLevelNbr = clientDbContext.PositionBusinessLevels.FirstOrDefault().BusinessLevelNbr;

                            _position.PositionCode = _PositionCodeNew;
                            _position.Title = _job.JobDescription;
                            _position.PositionDescription = _job.JobDescription;
                            _position.SalaryPayGroup = _PayGroupId.ToString();
                            _position.IsPositionActive = true;
                            _position.EnteredDate = DateTime.UtcNow;
                            _position.EnteredBy = User.Identity.Name;

                            clientDbContext.Positions.Add(_position);
                        }
                        clientDbContext.SaveChanges();

                        int _RateTypeId;
                        if (clientDbContext.DdlRateTypes.Where(x => x.Code == row.CompensationFrequency_JobDta).Count() > 0)
                        {
                            _RateTypeId = clientDbContext.DdlRateTypes.Where(x => x.Code == row.CompensationFrequency_JobDta).FirstOrDefault().RateTypeId;
                        }
                        else
                        {
                            var _newPayGroup = new DdlRateType { Description = row.CompensationFrequency_JobDta, Code = row.CompensationFrequency_JobDta, Active = true };
                            clientDbContext.DdlRateTypes.Add(_newPayGroup);
                            clientDbContext.SaveChanges();
                            _RateTypeId = _newPayGroup.RateTypeId;
                        }

                        var e_Position = new E_Positions();
                        e_Position.EnteredBy = User.Identity.Name;
                        e_Position.EnteredDate = DateTime.Now;
                        e_Position.EmployeeId = _employee.EmployeeId;
                        e_Position.PayFrequencyId = _PayFrequencyId;
                        e_Position.RateTypeId = _RateTypeId;
                        e_Position.PositionId = _position.PositionId;
                        e_Position.Notes = "";
                        e_Position.EnteredDate = DateTime.UtcNow;
                        e_Position.EnteredBy = User.Identity.Name;
                        e_Position.PrimaryPosition = true;

                        clientDbContext.E_Positions.Add(e_Position);
                        clientDbContext.SaveChanges();

                        DateTime? dateEffectiveDate = null;
                        if (!string.IsNullOrEmpty(row.EffectiveDate_JobDta))
                        {
                            dateEffectiveDate = Convert.ToDateTime(row.EffectiveDate_JobDta);
                        }

                        var _ePositionSalaryHistory = new E_PositionSalaryHistories
                        {
                            E_PositionId = e_Position.E_PositionId,
                            EffectiveDate = dateEffectiveDate,
                            PayRate = Convert.ToDecimal(row.CompensationRate_JobDta),
                            HoursPerPayPeriod = string.IsNullOrEmpty(row.StandardHours_JobDta) == true ? 0 : Convert.ToDecimal(row.StandardHours_JobDta),
                            EnteredDate = DateTime.UtcNow,
                            EnteredBy = User.Identity.Name

                        };

                        clientDbContext.E_PositionSalaryHistories.Add(_ePositionSalaryHistory);
                        clientDbContext.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
            return View("Index");
        }

        [HttpPost]
        public ActionResult Budget(HttpPostedFileBase postedFile)// Import Employee
        {
            List<BudgetImport> _budgetImportList = new List<BudgetImport>();
            string filePath = string.Empty;
            if (postedFile != null)
            {
                try
                {
                    string path = Server.MapPath("~/Uploads/");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    filePath = path + Path.GetFileName(postedFile.FileName);
                    string extension = Path.GetExtension(postedFile.FileName);
                    postedFile.SaveAs(filePath);

                    //Read the contents of CSV file.
                    string csvData = System.IO.File.ReadAllText(filePath);

                    //Execute a loop over the rows.
                    foreach (string row in csvData.Split('\r'))
                    {
                        if (!string.IsNullOrEmpty(row))
                        {
                            if (!row.Split(',')[0].Contains("Position Code"))
                            {
                                try
                                {
                                    _budgetImportList.Add(new BudgetImport
                                    {
                                        PositionCode = row.Split(',')[0].Replace(@"""", string.Empty),
                                        Year = row.Split(',')[1].Replace(@"""", string.Empty),////////////
                                        RevisionNumber = row.Split(',')[2].Replace(@"""", string.Empty),////////////
                                        StartDate = row.Split(',')[3].Replace(@"""", string.Empty),/////////////
                                        EndDate = row.Split(',')[4].Replace(@"""", string.Empty),/////////////
                                        Fte = row.Split(',')[5].Replace(@"""", string.Empty),
                                        BudgetAmount = row.Split(',')[6].Replace(@"""", string.Empty),///////////
                                        RevisionRank = row.Split(',')[7].Replace(@"""", string.Empty)
                                    });
                                }
                                catch (Exception ex)
                                {

                                }
                            }
                        }
                    }

                    foreach (var row in _budgetImportList)
                    {
                        SqlConnection con = new SqlConnection(User.Identity.GetClientConnectionString());

                        string sql = "INSERT INTO [dbo].[BudgetImport] " +
                        "([PositionCode],[Year],[RevisionNumber],[StartDate],[EndDate],[Fte],[BudgetAmount],[RevisionRank])  " +

                        "VALUES " +
                        "('" + row.PositionCode.Replace("'", "''").Trim() + "','" + row.Year.Replace("'", "''").Trim() + "','" + row.RevisionNumber + "','" + row.StartDate + "', " +
                        "'" + row.EndDate + "','" + row.Fte.Trim() + "','" + row.BudgetAmount.Trim() + "', " +
                        "'" + row.RevisionRank + "')";

                        con.Open();
                        SqlCommand cmd = new SqlCommand(sql, con);

                        cmd.CommandType = CommandType.Text;
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
                catch (Exception ex)
                {
                    return Content(ex.Message);
                }
            }
            return View();
        }

        [HttpPost]
        public ActionResult UpdateBudgetToDB(FormCollection frm)
        {
            try
            {
                List<BudgetImport> _budgetImportList = new List<BudgetImport>();
                string filePath = string.Empty;
                string count = frm["count"].ToString();
                SqlConnection con1 = new SqlConnection(User.Identity.GetClientConnectionString());
                string sql = "Select * from BudgetImport WHERE ID > '" + count + "' ORDER BY ID ASC";
                //string sql = "Select * from BudgetImport WHERE PositionCode = 'GA001-6918' ORDER BY ID ASC";
                con1.Open();
                SqlCommand cmd1 = new SqlCommand(sql, con1);
                SqlDataAdapter da = new SqlDataAdapter(cmd1);
                DataSet ds = new DataSet();
                da.Fill(ds, "EmployeeMaster");
                con1.Close();
                //for (int i = 0; i < 1; i++)
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    _budgetImportList.Add(new BudgetImport
                    {
                        ID = Convert.ToInt32(ds.Tables[0].Rows[i]["ID"]),

                        PositionCode = ds.Tables[0].Rows[i]["PositionCode"].ToString(),
                        Year = ds.Tables[0].Rows[i]["Year"].ToString(),
                        RevisionNumber = ds.Tables[0].Rows[i]["RevisionNumber"].ToString(),
                        StartDate = ds.Tables[0].Rows[i]["StartDate"].ToString(),
                        EndDate = ds.Tables[0].Rows[i]["EndDate"].ToString(),
                        Fte = ds.Tables[0].Rows[i]["Fte"].ToString(),
                        BudgetAmount = ds.Tables[0].Rows[i]["BudgetAmount"].ToString(),
                        RevisionRank = ds.Tables[0].Rows[i]["RevisionRank"].ToString(),


                    });
                }

                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                string connString = User.Identity.GetClientConnectionString();

                ClientDbContext clientDbContext = new ClientDbContext(connString);




                foreach (var row in _budgetImportList)
                {

                    var _position = clientDbContext.Positions.Where(x => x.PositionCode == row.PositionCode.Trim()).FirstOrDefault();


                    if (_position != null)
                    {
                        if (clientDbContext.PositionsBudgets.Where(x => x.PositionID == _position.PositionId).Count() == 0)
                        {
                            decimal _amount = Convert.ToDecimal(row.BudgetAmount) / 12;

                            var _positionBudgts = new PositionBudgets();
                            _positionBudgts.PositionID = _position.PositionId;
                            _positionBudgts.BudgetMonth = 1;
                            _positionBudgts.BudgetYear = Convert.ToInt32(row.Year);
                            _positionBudgts.BudgetAmount = Convert.ToDecimal(Math.Round(_amount, 2));
                            _positionBudgts.FTE = Convert.ToDecimal(row.Fte);
                            clientDbContext.PositionsBudgets.Add(_positionBudgts);


                            _positionBudgts = new PositionBudgets();
                            _positionBudgts.PositionID = _position.PositionId;
                            _positionBudgts.BudgetMonth = 2;
                            _positionBudgts.BudgetYear = Convert.ToInt32(row.Year);
                            _positionBudgts.BudgetAmount = Convert.ToDecimal(Math.Round(_amount, 2));
                            _positionBudgts.FTE = Convert.ToDecimal(row.Fte);
                            clientDbContext.PositionsBudgets.Add(_positionBudgts);

                            _positionBudgts = new PositionBudgets();
                            _positionBudgts.PositionID = _position.PositionId;
                            _positionBudgts.BudgetMonth = 3;
                            _positionBudgts.BudgetYear = Convert.ToInt32(row.Year);
                            _positionBudgts.BudgetAmount = Convert.ToDecimal(Math.Round(_amount, 2));
                            _positionBudgts.FTE = Convert.ToDecimal(row.Fte);
                            clientDbContext.PositionsBudgets.Add(_positionBudgts);

                            _positionBudgts = new PositionBudgets();
                            _positionBudgts.PositionID = _position.PositionId;
                            _positionBudgts.BudgetMonth = 4;
                            _positionBudgts.BudgetYear = Convert.ToInt32(row.Year);
                            _positionBudgts.BudgetAmount = Convert.ToDecimal(Math.Round(_amount, 2));
                            _positionBudgts.FTE = Convert.ToDecimal(row.Fte);
                            clientDbContext.PositionsBudgets.Add(_positionBudgts);

                            _positionBudgts = new PositionBudgets();
                            _positionBudgts.PositionID = _position.PositionId;
                            _positionBudgts.BudgetMonth = 5;
                            _positionBudgts.BudgetYear = Convert.ToInt32(row.Year);
                            _positionBudgts.BudgetAmount = Convert.ToDecimal(Math.Round(_amount, 2));
                            _positionBudgts.FTE = Convert.ToDecimal(row.Fte);
                            clientDbContext.PositionsBudgets.Add(_positionBudgts);

                            _positionBudgts = new PositionBudgets();
                            _positionBudgts.PositionID = _position.PositionId;
                            _positionBudgts.BudgetMonth = 6;
                            _positionBudgts.BudgetYear = Convert.ToInt32(row.Year);
                            _positionBudgts.BudgetAmount = Convert.ToDecimal(Math.Round(_amount, 2));
                            _positionBudgts.FTE = Convert.ToDecimal(row.Fte);
                            clientDbContext.PositionsBudgets.Add(_positionBudgts);

                            _positionBudgts = new PositionBudgets();
                            _positionBudgts.PositionID = _position.PositionId;
                            _positionBudgts.BudgetMonth = 7;
                            _positionBudgts.BudgetYear = Convert.ToInt32(row.Year);
                            _positionBudgts.BudgetAmount = Convert.ToDecimal(Math.Round(_amount, 2));
                            _positionBudgts.FTE = Convert.ToDecimal(row.Fte);
                            clientDbContext.PositionsBudgets.Add(_positionBudgts);

                            _positionBudgts = new PositionBudgets();
                            _positionBudgts.PositionID = _position.PositionId;
                            _positionBudgts.BudgetMonth = 8;
                            _positionBudgts.BudgetYear = Convert.ToInt32(row.Year);
                            _positionBudgts.BudgetAmount = Convert.ToDecimal(Math.Round(_amount, 2));
                            _positionBudgts.FTE = Convert.ToDecimal(row.Fte);
                            clientDbContext.PositionsBudgets.Add(_positionBudgts);

                            _positionBudgts = new PositionBudgets();
                            _positionBudgts.PositionID = _position.PositionId;
                            _positionBudgts.BudgetMonth = 9;
                            _positionBudgts.BudgetYear = Convert.ToInt32(row.Year);
                            _positionBudgts.BudgetAmount = Convert.ToDecimal(Math.Round(_amount, 2));
                            _positionBudgts.FTE = Convert.ToDecimal(row.Fte);
                            clientDbContext.PositionsBudgets.Add(_positionBudgts);

                            _positionBudgts = new PositionBudgets();
                            _positionBudgts.PositionID = _position.PositionId;
                            _positionBudgts.BudgetMonth = 10;
                            _positionBudgts.BudgetYear = Convert.ToInt32(row.Year);
                            _positionBudgts.BudgetAmount = Convert.ToDecimal(Math.Round(_amount, 2));
                            _positionBudgts.FTE = Convert.ToDecimal(row.Fte);
                            clientDbContext.PositionsBudgets.Add(_positionBudgts);

                            _positionBudgts = new PositionBudgets();
                            _positionBudgts.PositionID = _position.PositionId;
                            _positionBudgts.BudgetMonth = 11;
                            _positionBudgts.BudgetYear = Convert.ToInt32(row.Year);
                            _positionBudgts.BudgetAmount = Convert.ToDecimal(Math.Round(_amount, 2));
                            _positionBudgts.FTE = Convert.ToDecimal(row.Fte);
                            clientDbContext.PositionsBudgets.Add(_positionBudgts);

                            _positionBudgts = new PositionBudgets();
                            _positionBudgts.PositionID = _position.PositionId;
                            _positionBudgts.BudgetMonth = 12;
                            _positionBudgts.BudgetYear = Convert.ToInt32(row.Year);
                            _positionBudgts.BudgetAmount = Convert.ToDecimal(Math.Round(_amount, 2));
                            _positionBudgts.FTE = Convert.ToDecimal(row.Fte);
                            clientDbContext.PositionsBudgets.Add(_positionBudgts);

                            clientDbContext.SaveChanges();
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
            return View("Index");
        }

        [HttpPost]
        public ActionResult GenerateUserLogins()
        {
            string message = "";
            string connString = User.Identity.GetClientConnectionString();

            ClientDbContext _clientDbContext = new ClientDbContext(connString);
            var _personsList = _clientDbContext.Persons.Where(x => x.eMail != null).ToList();
            AdminDbContext adminDbContext = new AdminDbContext();

            foreach (var row in _personsList)
            {

                var aspNetUser = adminDbContext.AspNetUsers.FirstOrDefault(x => x.UserName == row.eMail || x.Email == row.eMail);
                if (aspNetUser == null)
                {


                    var employerId = Convert.ToInt32(User.Identity.GetSelectedClientID());

                   
                    AppUser user = new AppUser
                    {
                        UserName = row.eMail,
                        Email = row.eMail,
                        LastPasswordChangeDate = DateTime.Today,
                        EmployerId = employerId,
                        FirstName = row.Firstname,
                        LastName = row.Lastname,

                    };

                    IdentityResult result = HttpContext.GetOwinContext().GetUserManager<AppUserManager>().Create(user, "11111111");



                    if (result.Succeeded)
                    {
                        UserManager.AddToRole(user.Id, "ClientEmployees");
                       
                        adminDbContext.UserCompanies.Add(new EfAdmin.UserCompany { UserId = user.Id, EmployerId = employerId });
                        adminDbContext.SaveChanges();
                        try
                        {
                            _clientDbContext.UserNamesPersons.Add(new UserNamesPerson
                            {
                                PersonID = row.PersonId,
                                UserName = user.UserName,
                                EnteredBy = User.Identity.GetUserName(),
                                CreationDate = DateTime.UtcNow
                            });
                            _clientDbContext.SaveChanges();
                        }
                        catch(Exception ex) {

                            message += row.PersonId + " - " + row.eMail + " - Person Not Added To UserNamesPersons - " + ex.Message + " <br /> ";
                        }
                    }
                    else
                    {
                        foreach (string error in result.Errors)
                        {
                            message += row.PersonId + " - " + row.eMail + " - " + error + " <br /> ";
                        }

                    }
                }
                else
                {
                    message += row.PersonId + " - " + row.eMail + " - Email ID Duplicate <br />";


                }
            }
            return Content(message);
        }

        private AppUserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<AppUserManager>();
            }
        }

    }
}

public class EmployeeImport
{
    public int ID { get; set; }
    public string LastName { get; set; }
    public string FirstName { get; set; }
    public string Birthdate_PersonalDta { get; set; }
    public string HireDate { get; set; }

    public string TerminationDate { get; set; }
    public string PayGroup_JobDta { get; set; }
    public string AutolinkFileNumber_JobDta { get; set; }
    public string EmployeeStatus_JobDta { get; set; }
    public string CompensationRate_JobDta { get; set; }
    public string EffectiveDate_JobDta { get; set; }
    public string CompensationFrequency_JobDta { get; set; }

    public string StandardHours_JobDta { get; set; }
    public string PayFrequency_PayGroupInfo { get; set; }
    public string LocationCode_JobDta { get; set; }
    public string JobCode_JobDta { get; set; }
    public string Description_JobCodeInfo { get; set; }
}

public class BudgetImport
{
    public int ID { get; set; }
    public string PositionCode { get; set; }
    public string Year { get; set; }
    public string RevisionNumber { get; set; }
    public string StartDate { get; set; }
    public string EndDate { get; set; }
    public string Fte { get; set; }
    public string BudgetAmount { get; set; }
    public string RevisionRank { get; set; }

}