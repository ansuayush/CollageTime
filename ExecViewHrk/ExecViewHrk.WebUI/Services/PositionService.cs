using System.Web;
using ExecViewHrk.EfClient;
using Kendo.Mvc.Extensions;
using ExecViewHrk.WebUI.Models;
using System.Linq;
using System.Collections.Generic;
using System;
using ExecViewHrk.Models;

namespace ExecViewHrk.WebUI.Services
{
    public class PositionService
    {
        readonly string connString = HttpContext.Current.User.Identity.GetClientConnectionString();
        readonly SalaryGradeService salaryGradeService = new SalaryGradeService();
        readonly PositionBudgetService positionBudgetService = new PositionBudgetService();
        public IEnumerable<PositionDetailsVm> getAllPositions(int? positionID)
        {
            IEnumerable<PositionDetailsVm> positionsList = null;
            IEnumerable<Position> positions = null;
            IEnumerable<Position> allPositions = null;
            //Performance
            IEnumerable<Job> JobList;
            IEnumerable<PositionBusinessLevels> BusinessLevelList;
            IEnumerable<ExecViewHrk.EfClient.PositionHistory> positionhistory = new List<ExecViewHrk.EfClient.PositionHistory>();
            IEnumerable<E_Positions> E_PositionList = new List<E_Positions>();
            PositionDetailsVm positionDetailsDdl = new PositionDetailsVm();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            using (clientDbContext)
            {
                positions = positionID.HasValue ? clientDbContext.Positions.Where(x => x.PositionId == positionID).ToList() : clientDbContext.Positions.ToList();
                JobList = clientDbContext.Jobs.ToList();
                BusinessLevelList = clientDbContext.PositionBusinessLevels.ToList();
                if (positionID.HasValue)
                {
                    allPositions = clientDbContext.Positions.ToList();
                    positionDetailsDdl.performanceProfiles = clientDbContext.PerformanceProfiles.Where(x => x.Active == true).ToList();
                    positionDetailsDdl.performanceProfiles.Insert(0, new PerformanceProfiles { PerProfileID = 0, Description = "Select" });

                    positionDetailsDdl.ddlPositionType = clientDbContext.DdlPositionTypes.Where(x => x.active == true).ToList();
                    positionDetailsDdl.ddlPositionType.Insert(0, new DdlPositionTypes { PositionTypeId = 0, description = "Select" });

                    positionDetailsDdl.ddlPositionCategory = clientDbContext.DdlPositionCategory.Where(x => x.active == true).ToList();
                    positionDetailsDdl.ddlPositionCategory.Insert(0, new DdlPositionCategory { PositionCategoryID = 0, description = "Select" });

                    positionDetailsDdl.positionBusinessLevelsList = clientDbContext.PositionBusinessLevels.Where(x => x.Active == true).Distinct().ToList();
                    positionDetailsDdl.positionBusinessLevelsList.Insert(0, new PositionBusinessLevels { BusinessLevelNbr = 0, BusinessLevelTitle = "Select" });

                    positionDetailsDdl.JobList = clientDbContext.Jobs.Distinct().ToList();
                    positionDetailsDdl.JobList.Insert(0, new Job { JobId = 0, JobDescription = "Select" });

                    positionDetailsDdl.ReportToList = clientDbContext.Positions.Where(x => x.PositionId != positionID).Select(x => new DropDownModel { keyvalue = x.PositionId.ToString(), keydescription = x.PositionDescription + " : " + x.PositionCode }).OrderBy(m => m.keydescription).ToList();
                    positionDetailsDdl.ReportToList.Insert(0, new DropDownModel { keyvalue = "0", keydescription = "Select" });


                    positionDetailsDdl.DepartmentList = clientDbContext.Departments.Where(x => x.IsDepartmentActive == true && x.IsDeleted == false).ToList();
                    positionDetailsDdl.DepartmentList.Insert(0, new Department { DepartmentId = 0, DepartmentDescription = "Select" });

                    positionDetailsDdl.LocationList = clientDbContext.Locations.Where(x => x.Active == true).ToList();
                    positionDetailsDdl.LocationList.Insert(0, new Location { LocationId = 0, LocationDescription = "Select" });

                    positionDetailsDdl.DdlPayFrequency = clientDbContext.DdlPayFrequencies.Where(x => x.Active == true).Select(x => new DropDownModel { keyvalue = x.Code, keydescription = x.Description }).ToList();
                    positionDetailsDdl.DdlPayFrequency.Insert(0, new DropDownModel { keyvalue = "0", keydescription = "--Select--" });
                    positionDetailsDdl.DDEmpType = clientDbContext.DdlEmployeeTypes.Where(x => x.Active == true).Select(x => new DropDownModel { keyvalue = x.EmployeeTypeId.ToString(), keydescription = x.Description }).ToList();
                    positionDetailsDdl.DDEmpType.Insert(0, new DropDownModel { keyvalue = "0", keydescription = "--Select--" });
                    positionDetailsDdl.DropDownSalaryGrade = clientDbContext.DdlSalaryGrades.Select(x => new DropDownModel { keyvalue = x.SalaryGradeID.ToString(), keydescription = x.description }).ToList();
                    positionDetailsDdl.DropDownSalaryGrade.Insert(0, new DropDownModel { keyvalue = "0", keydescription = "--Select--" });
                    positionDetailsDdl.DropDownWorkStatusType = WorkStatusTypeDropDown();
                    positionDetailsDdl.posionFundingSourceVM = new PosionFundingSource();
                    positionDetailsDdl.positionFundingSourceList = positionBudgetService.getFundCodeWithEffectiveDate(positionID).ToList();
                    // positionDetailsDdl.posionFundingSourceVM.posionFundingSourceHistoryList = positionBudgetService.getFundingSourceHistoryList().ToList(); ;
                    E_PositionList = clientDbContext.E_Positions.Where(x=>x.IsDeleted == false).ToList();
                    var Result = (from p in clientDbContext.Persons
                                  join e in clientDbContext.Employees on p.PersonId equals e.PersonId
                                  select new DropDownModel
                                  {
                                      keyvalue = e.PersonId.ToString(),
                                      keydescription = p.Firstname + " " + p.Lastname
                                  }).ToList();
                    positionDetailsDdl.EmployeeList = Result;
                    positionDetailsDdl.EmployeeList.Insert(0, new DropDownModel { keyvalue = "0", keydescription = "Select" });
                    positionDetailsDdl.PayGroupList = clientDbContext.DdlPayGroups.ToList();
                    positionDetailsDdl.PayGroupList.Insert(0, new DdlPayGroup { PayGroupId = 0, Description = "Select" });
                    positionDetailsDdl.PositionHistory = clientDbContext.PositionHistory.ToList();
                }
            }
            positionsList = positions
                .Select(x => new PositionDetailsVm
                {
                    PositionId = x.PositionId,
                    BusinessLevelNbr = x.BusinessLevelNbr,
                    JobId = x.JobId,
                    DepartmentId = x.DepartmentId,
                    LocationId = x.LocationId,
                    // 
                    UserDefinedSegment1Id = x.UserDefinedSegment1Id,
                    UserDefinedSegment2Id = x.UserDefinedSegment2Id,
                    PositionDescription = x.PositionDescription,
                    PositionCode = x.PositionCode + x.Suffix,
                    IsPositionActive = x.IsPositionActive,
                    ReportsToPositionId = x.ReportsToPositionId,
                    Title = x.Title,
                    CreatedDate = x.EnteredDate,
                    FrozenDate = x.FrozenDate,
                    ClosedDate = x.ClosedDate,
                    Status = x.Status,
                    PositionCategoryID = x.PositionCategoryID,
                    PositionTypeID = x.PositionTypeID,
                    PayFrequencyCode = x.PayFrequencyCode,
                    ProbationPeriod = x.ProbationPeriod,
                    UnitsID = x.UnitsID,
                    ScheduledHours = x.ScheduledHours == null ? "40" : x.ScheduledHours,
                    LastModifiedDate = x.LastModifiedDate,
                    AuthorizedByID = x.AuthorizedByID,
                    TotalSlots = x.TotalSlots.HasValue ? x.TotalSlots : 1,
                    EnteredDate = x.EnteredDate,
                    EnteredBy = x.EnteredBy,
                    Requisitno = x.Requisitno,
                    TravPercent = x.TravPercent,
                    Shift = x.Shift,
                    PositReason = x.PositReason == null ? "New" : x.PositReason,
                    //PositLocation = x.PositLocation,
                    PositStatusActive = x.PositStatusActive,
                    PositStatusFrozen = x.PositStatusFrozen,
                    PositStatusClosed = x.PositStatusClosed,
                    PositStatusOpen = x.PositStatusOpen,
                    PositStatusPost = x.PositStatusPost,
                    PerProfileID = x.PerProfileID,
                    StartDate = x.StartDate,
                    ProjectEndDate = x.ProjectEndDate,
                    ActualEnddate = x.ActualEnddate,
                    ThirdShift = x.ThirdShift,
                    Description = x.Description,
                    Code = x.Code,
                    AccountNumber = x.AccountNumber,
                    Group2 = x.Group2,
                    Group3 = x.Group3,
                    Group4 = x.Group4,
                    Group5 = x.Group5,
                    FLSA = x.FLSA,
                    LengthOfContract = x.LengthOfContract,
                    WorkClassificationId = x.WorkClassificationId,
                    AlternateTitle = x.AlternateTitle,
                    BusinessUnitId = x.BusinessUnitId,
                    FTE = x.FTE.HasValue ? x.FTE : 0,
                    IncumbentADPID = x.IncumbentADPID,
                    Division = x.Division,
                    SalaryAmount = x.SalaryAmount,
                    SalaryPayGroup = x.SalaryPayGroup,
                    SalaryPlanCode = x.SalaryPlanCode,
                    SalaryStep = x.SalaryStep,
                    SalaryType = x.SalaryType,
                    CostNumber = x.CostNumber,
                    Suffix=x.Suffix


                }).ToList();

            // }

            //Repair view model

            foreach (PositionDetailsVm item in positionsList)
            {
                item.JobCode = item.JobId.HasValue && item.JobId > 0 ? JobList.FirstOrDefault(x => x.JobId == item.JobId).JobCode : string.Empty;
                item.JobTitle = item.JobId.HasValue && item.JobId > 0 ? JobList.FirstOrDefault(x => x.JobId == item.JobId).JobDescription : string.Empty;
                item.BUCode = item.BusinessLevelNbr.HasValue && item.BusinessLevelNbr > 0 ? BusinessLevelList.FirstOrDefault(x => x.BusinessLevelNbr == item.BusinessLevelNbr.Value).BusinessLevelCode : string.Empty;
                item.BUTitle = item.BusinessLevelNbr.HasValue && item.BusinessLevelNbr > 0 ? BusinessLevelList.FirstOrDefault(x => x.BusinessLevelNbr == item.BusinessLevelNbr.Value).BusinessLevelTitle : string.Empty;
                if (positionID.HasValue)
                {
                    item.PositLocation = item.LocationId.HasValue && item.BusinessLevelNbr > 0 ? positionDetailsDdl.LocationList.FirstOrDefault(x => x.LocationId == item.LocationId.Value).LocationDescription : string.Empty;
                    item.PositionType = item.PositionTypeID.HasValue && item.BusinessLevelNbr > 0 ? positionDetailsDdl.ddlPositionType.FirstOrDefault(x => x.PositionTypeId == item.PositionTypeID.Value).description : string.Empty;
                    item.ReportsTo = item.ReportsToPositionId.HasValue && item.ReportsToPositionId.Value > 0 ? allPositions.FirstOrDefault(x => x.PositionId == item.ReportsToPositionId.Value).PositionDescription : string.Empty;
                    item.PositionCategory = item.PositionCategoryID.HasValue && item.PositionCategoryID > 0 ? positionDetailsDdl.ddlPositionCategory.FirstOrDefault(x => x.PositionCategoryID == item.PositionCategoryID.Value).description : string.Empty;
                    item.CurrentStatus = item.Status.HasValue && item.Status > 0 ? Enum.GetName(typeof(PositionStatus), item.Status) : string.Empty;
                    item.Department = item.DepartmentId.HasValue && item.DepartmentId > 0 ? positionDetailsDdl.DepartmentList.FirstOrDefault(x => x.DepartmentId == item.DepartmentId.Value).DepartmentDescription : string.Empty;
                    item.PerProfile = item.PerProfileID.HasValue && item.PerProfileID > 0 ? positionDetailsDdl.performanceProfiles.FirstOrDefault(x => x.PerProfileID == item.PerProfileID.Value).Description : string.Empty;
                    item.PositLocation = item.LocationId.HasValue ? positionDetailsDdl.LocationList.FirstOrDefault(x => x.LocationId == item.LocationId.Value).LocationDescription : string.Empty;
                    item.PositionType = item.PositionTypeID.HasValue ? positionDetailsDdl.ddlPositionType.FirstOrDefault(x => x.PositionTypeId == item.PositionTypeID.Value).description : string.Empty;
                    item.ReportsTo = item.ReportsToPositionId.HasValue && item.ReportsToPositionId.Value > 0 ? allPositions.FirstOrDefault(x => x.PositionId == item.ReportsToPositionId.Value).PositionDescription + " : " + allPositions.FirstOrDefault(x => x.PositionId == item.ReportsToPositionId.Value).PositionCode : string.Empty;
                    item.CurrentStatus = item.Status.HasValue ? Enum.GetName(typeof(PositionStatus), item.Status) : "Active";
                    item.Department = item.DepartmentId.HasValue ? positionDetailsDdl.DepartmentList.FirstOrDefault(x => x.DepartmentId == item.DepartmentId.Value).DepartmentDescription : string.Empty;
                    item.PerProfile = item.PerProfileID.HasValue ? positionDetailsDdl.performanceProfiles.FirstOrDefault(x => x.PerProfileID == item.PerProfileID.Value).Description : string.Empty;
                    item.SalaryGrade = salaryGradeService.getSalaryGradeForPosition(item.PositionId);
                    item.SalaryGradeHistory = salaryGradeService.getALLHistory().ToList();
                    item.DdlSalaryGrades = salaryGradeService.getALL();
                    item.DdlPayFrequency = getPayFrequencies();
                    item.PositionBudgetList = positionBudgetService.getPositionBudgets(null, item.PositionId);
                    item.FilledSlots = (E_PositionList.Any()) ? E_PositionList.Where(x => x.PositionId == item.PositionId).ToList().Count : 0;
                    item.SalaryGradeHistoryListVm = salaryGradeService.GetSalaryGradHistorylst();
                    item.PayGroupList = positionDetailsDdl.PayGroupList;
                    item.EmployeeList = positionDetailsDdl.EmployeeList;
                    item.posionFundingSourceVM = positionDetailsDdl.posionFundingSourceVM;
                    item.DropDownWorkStatusType = positionDetailsDdl.DropDownWorkStatusType;
                    item.DropDownSalaryGrade = positionDetailsDdl.DropDownSalaryGrade;
                    item.DdlPayFrequency = positionDetailsDdl.DdlPayFrequency;
                    item.LocationList = positionDetailsDdl.LocationList;
                    item.DDEmpType = positionDetailsDdl.DDEmpType;
                    item.DepartmentList = positionDetailsDdl.DepartmentList;
                    item.ReportToList = positionDetailsDdl.ReportToList;
                    item.JobList = positionDetailsDdl.JobList;
                    item.positionBusinessLevelsList = positionDetailsDdl.positionBusinessLevelsList;
                    item.ddlPositionCategory = positionDetailsDdl.ddlPositionCategory;
                    item.ddlPositionType = positionDetailsDdl.ddlPositionType;
                    item.performanceProfiles = positionDetailsDdl.performanceProfiles;
                    item.PositionHistory = positionDetailsDdl.PositionHistory;
                    item.EmpSalaryList = GetEmployeeSalary(item);
                    item.PositionSalaryGradeDetail = string.IsNullOrEmpty(item.SalaryGrade) || item.SalaryGrade == "0" ? new SalaryGradeVm() :
                    item.SalaryGradeHistory.FirstOrDefault(x => x.SalaryGradeID == Convert.ToInt32(item.SalaryGrade));
                    item.positionFundingSourceList = positionDetailsDdl.positionFundingSourceList.ToList();
                }
            }            
            return positionsList;
        }

        internal string SaveFundingSourceList(List<PosionFundingSourceListVM> sources, DateTime effectiveDate)
        {
            return positionBudgetService.SaveFundingSourceList(sources, effectiveDate);
        }

        internal string SaveBudgeMonthAmount(PositionBudgetMonths SaveBudgeMonthAmount)
        {
            return positionBudgetService.SaveBudgeMonthAmount(SaveBudgeMonthAmount);
        }

        internal string SavePositionFundHistory(FundHistoryAddVm fundHistoryVm)
        {
            return positionBudgetService.SavePositionFundHistory(fundHistoryVm);
        }

        internal FundHistoryVm GetPositionFundDefinition()
        {
            return positionBudgetService.getNewFundHistory();
        }

        internal string savePositionBudgetFund(PositionFundsVm positionBudgetFundsVM)
        {
            var flag = positionBudgetService.savePositionBudgetFund(positionBudgetFundsVM);
            return flag;
        }

        internal PositionFundsVm GetPositionBudgetFundAllocation(int? positionBudgetID)
        {
            var fund = positionBudgetService.GetNewPositionBudgetFundAllocation();
            fund.PositionBudgetID = positionBudgetID.Value;
            return fund;
        }

        //internal PositionDetailsVm getNewPosition()
        //{
        //    PositionDetailsVm position = new PositionDetailsVm();
        //    using (ClientDbContext clientDbContext = new ClientDbContext(connString))
        //    {
        //        position.positionBusinessLevelsList = clientDbContext.PositionBusinessLevels.Distinct().ToList();
        //        position.positionBusinessLevelsList.Insert(0, new PositionBusinessLevels { BusinessLevelNbr = 0, BusinessLevelTitle = "Select" });
        //        position.JobList = clientDbContext.Jobs.Distinct().ToList();
        //        position.JobList.Insert(0, new Job { JobId = 0, JobDescription = "Select" });
        //    }

        //    return position;
        //}
        internal PositionDetailsVm getNewPosition()
        {
            PositionDetailsVm position = new PositionDetailsVm();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                position.ddlpositionBusinessLevels = clientDbContext.PositionBusinessLevels.Distinct().OrderBy(x => x.BusinessLevelTitle).Select(x => new DropDownModel
                {
                    keyvalue = x.BusinessLevelCode,
                    keydescription = x.BusinessLevelCode + "-" + x.BusinessLevelTitle
                }).ToList().CleanUp();
                position.ddlJobs = clientDbContext.Jobs.Distinct().OrderBy(x => x.JobDescription).Select(x => new DropDownModel
                {
                    keyvalue = x.JobCode,
                    keydescription = x.JobCode + "-" + x.JobDescription
                }).ToList().CleanUp();
            }

            return position;
        }

        internal IEnumerable<PositionBudgetFundAllocationVm> GetPositionBudgetAllocation(int? positionBudgetID)
        {
            return positionBudgetService.getPositionBudgetFundAllocation(positionBudgetID);
        }

        internal IEnumerable<PositionBudgetsVM> GetPositionBudgets(int? positionID)
        {
            return positionBudgetService.getPositionBudgets(null, positionID.Value).ToList();
        }


        public PositionDetailsVm getPositionDetails(int positionID)
        {
            var positionDetail = getAllPositions(positionID).SingleOrDefault(a => a.PositionId == positionID);
            return positionDetail;
        }


        public string getPositionTitle(int positionId)
        {
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            string title = clientDbContext.Positions.SingleOrDefault(x => x.PositionId == positionId).Description;
            return title;
        }

        public PositionBudgetsVM getPositionBudget(int? positionBudgetID, short? positionID)
        {
            if (positionBudgetID.HasValue)
            {
                return positionBudgetService.getPositionBudget(positionBudgetID.Value);
            }
            else
            {
                return positionBudgetService.getNewPositionBudget(positionID.Value);
            }
        }

        public string saveSalaryGrade(AddSalaryGradeitem addSalaryitem)
        {
            var flag = salaryGradeService.saveSalaryItem(addSalaryitem);
            return flag;
        }
        public string savePositionBudget(PositionBudgetsVM positionBudgetsVM)
        {
            var flag = positionBudgetService.saveBudgetEntity(positionBudgetsVM);
            return flag;
        }
        public string savePositionFundingSourceitem(AddPositionFundSourceitem addfundingSourceitem)
        {
            var flag = salaryGradeService.savefundingSourceItem(addfundingSourceitem);
            return flag;
        }

        public string checkPercentage(DateTime EffDate)
        {
            var flag = salaryGradeService.PercentageonEffDate(EffDate);
            return flag;
        }
        //public string UpdatePositionBudget(PositionBudgetUpdate positionBudgetUpdate)
        //{
        //    var flag = salaryGradeService.updatePositionBudget(positionBudgetUpdate);
        //    return flag;
        //}
        public string saveSalaryHistory(int E_PositionId, int RateType, int PayRate, int HoursPerPayPeriod, DateTime? EffectiveDate)
        {
            var flag = salaryGradeService.saveSalaryHistoryItem(E_PositionId, RateType, PayRate, HoursPerPayPeriod, EffectiveDate);
            return flag;
        }
        public List<PositionBudgetsVM> GetPositionBudgetList(int PositionID)
        {
            List<PositionBudgetsVM> budgetlst = positionBudgetService.getPositionBudgets(null, PositionID).ToList();
            return budgetlst;
        }
        public List<PositionHistory> getpositionhistory()
        {
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            {
                var PositionBudgetList = clientDbContext.PositionHistory
                                         .ToList();


                return PositionBudgetList;
            }


        }
        #region private Methods

        public enum PositionStatus
        {
            Active = 1,
            Frozen = 2,
            Closed = 3,
            Open = 4,
            Posted = 5
        }
        private string getJobDescription(int jobID)
        {
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            string code = "";
            if (jobID > 0)
            {
                code = clientDbContext.Jobs.Where(x => x.JobId == jobID).Select(x => x.JobDescription).SingleOrDefault().ToString();
            }
            return code;
        }
        private string getJobCode(int jobID)
        {
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            string code = "";
            if (jobID > 0)
            {
                code = clientDbContext.Jobs.Where(x => x.JobId == jobID).Select(x => x.JobCode).SingleOrDefault().ToString();
            }
            return code;
        }

        private string getBUCode(int businessUnitID)
        {
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            string code = "";
            if (businessUnitID > 0)
            {
                code = clientDbContext.PositionBusinessLevels.Where(x => x.BusinessLevelNbr == businessUnitID).Select(x => x.BusinessLevelCode).SingleOrDefault().ToString();
            }
            return code;
        }

        private string getBUTitle(int businessUnitID)
        {
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            string code = "";
            if (businessUnitID > 0)
            {
                code = clientDbContext.PositionBusinessLevels.Where(x => x.BusinessLevelNbr == businessUnitID).Select(x => x.BusinessLevelTitle).SingleOrDefault();
            }
            return code;
        }

        //private string getBusinessLevel(int busineeLeveID)
        // {
        //   return string.Empty;
        // }


        private string getLocationName(short locationID)
        {
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            string code = "";
            if (locationID > 0)
            {
                code = clientDbContext.Locations.Where(x => x.LocationId == locationID).Select(x => x.LocationDescription).SingleOrDefault();
            }
            return code;
        }

        private string getPositionType(int positionTypeId)
        {
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            string code = "";
            if (positionTypeId > 0)
            {
                code = clientDbContext.DdlPositionTypes.Where(x => x.PositionTypeId == positionTypeId).Select(x => x.description).SingleOrDefault();
            }
            return code;
        }

        private string getReportToPosition(short positionId)
        {
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            string code = "";
            if (positionId > 0)
            {
                code = clientDbContext.Positions.Where(x => x.PositionId == positionId).Select(x => x.Title).FirstOrDefault();
            }
            return code;
        }
        private string getPositionCategory(int PositionCategoryID)
        {
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            string description = "";
            if (PositionCategoryID > 0)
            {
                description = clientDbContext.DdlPositionCategory.Where(x => x.PositionCategoryID == PositionCategoryID).Select(x => x.description).SingleOrDefault().ToString();
            }

            return description;
        }

        private string getDepartment(int departmentID)
        {
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            string description = "";
            if (departmentID > 0)
            {
                description = clientDbContext.Departments.Where(x => x.DepartmentId == departmentID && x.IsDeleted == false).Select(x => x.DepartmentDescription).SingleOrDefault().ToString();
            }

            return description;
        }

        private string getPerformanceProfile(int perProfileId)
        {
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            string description = "";
            if (perProfileId > 0)
            {
                description = clientDbContext.PerformanceProfiles.Where(x => x.PerProfileID == perProfileId).Select(x => x.Description).SingleOrDefault().ToString();
            }
            return description;
        }

        private List<DropDownModel> getPayFrequencies()
        {
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            var PayFrequncies = clientDbContext.DdlPayFrequencies.Where(x => x.Active == true).Select(x => new DropDownModel { keyvalue = x.Code, keydescription = x.Description }).ToList();
            PayFrequncies.Insert(0, new DropDownModel { keyvalue = "0", keydescription = "--Select--" });
            return PayFrequncies;
        }


        private IEnumerable<DropDownModel> getEmployeeTypes()
        {
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            var EmployeeType = clientDbContext.DdlEmployeeTypes.Where(x => x.Active == true).Select(x => new DropDownModel { keyvalue = x.EmployeeTypeId.ToString(), keydescription = x.Description }).ToList();
            EmployeeType.Insert(0, new DropDownModel { keyvalue = "0", keydescription = "--Select--" });
            return EmployeeType;
        }

        public PositionDetailsVm SavePosition(PositionDetailsVm positionDetailsVm)
        {
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            Position position = new Position();
            Positions PositionRecord = clientDbContext.Positions.Where(x => x.PositionId == positionDetailsVm.PositionId).SingleOrDefault();

            if (PositionRecord == null)
            {

                position.BusinessLevelNbr = positionDetailsVm.BusinessLevelNbr == 0 ? null : positionDetailsVm.BusinessLevelNbr;
                position.IsPositionActive = positionDetailsVm.IsPositionActive;
                position.Code = positionDetailsVm.Code;
                position.PositionCode = positionDetailsVm.PositionCode;
                position.Title = positionDetailsVm.PositionDescription;
                position.PositionDescription = positionDetailsVm.PositionDescription;
                position.CostNumber = positionDetailsVm.CostNumber;
                position.Status = 1;
                position.PositReason = "New";
                position.ScheduledHours = "40";
                position.TotalSlots = 1;
                position.ProbationPeriod = "0";
                position.EnteredDate = DateTime.UtcNow;
                position.JobId = positionDetailsVm.JobId == 0 ? null : positionDetailsVm.JobId;
                position.Suffix = positionDetailsVm.Suffix;
                if (clientDbContext.DdlEmployeeTypes.Where(x => x.Active).Count() > 0)
                {
                    position.WorkClassificationId = clientDbContext.DdlEmployeeTypes.Where(x => x.Active).FirstOrDefault().EmployeeTypeId;
                }
                if (clientDbContext.DdlPositionCategory.Where(x => x.active).Count() > 0)
                {
                    position.PositionCategoryID = clientDbContext.DdlPositionCategory.Where(x => x.active).FirstOrDefault().PositionCategoryID;
                }
                if (clientDbContext.DdlPositionTypes.Where(x => x.active).Count() > 0)
                {
                    position.PositionTypeID = clientDbContext.DdlPositionTypes.Where(x => x.active).FirstOrDefault().PositionTypeId;
                }
                //if (clientDbContext.Departments.Where(x => x.IsDepartmentActive).Count() > 0)
                //{
                //    position.DepartmentId = clientDbContext.Departments.Where(x => x.IsDepartmentActive).FirstOrDefault().DepartmentId;
                //}
                //if (clientDbContext.Locations.Where(x => x.Active).Count() > 0)
                //{
                //    position.LocationId = clientDbContext.Locations.Where(x => x.Active).FirstOrDefault().LocationId;
                //}
                if (clientDbContext.PerformanceProfiles.Where(x => x.Active).Count() > 0)
                {
                    position.PerProfileID = clientDbContext.PerformanceProfiles.Where(x => x.Active).FirstOrDefault().PerProfileID;
                }
                clientDbContext.Positions.Add(position);
            }
            else
            {
                var PositionGradeID = SavePositionSalaryGrade(Convert.ToByte(positionDetailsVm.SalaryGrade), positionDetailsVm.PositionId, positionDetailsVm.EnteredBy);
                PositionRecord.PositionId = positionDetailsVm.PositionId;
                PositionRecord.ActualEnddate = positionDetailsVm.ActualEnddate;
                PositionRecord.AuthorizedByID = positionDetailsVm.AuthorizedByID;
                PositionRecord.BusinessLevelNbr = positionDetailsVm.BusinessLevelNbr == 0 ? null : positionDetailsVm.BusinessLevelNbr;
                PositionRecord.BusinessUnitId = positionDetailsVm.BusinessUnitId;
                PositionRecord.ClosedDate = positionDetailsVm.ClosedDate;
                PositionRecord.Code = positionDetailsVm.Code;
                PositionRecord.DepartmentId = positionDetailsVm.DepartmentId == 0 ? null : positionDetailsVm.DepartmentId;
                PositionRecord.Description = positionDetailsVm.Description;
                PositionRecord.Division = positionDetailsVm.Division;
                PositionRecord.EnteredBy = positionDetailsVm.EnteredBy;
                PositionRecord.FrozenDate = positionDetailsVm.FrozenDate;
                PositionRecord.FTE = positionDetailsVm.FTE;
                PositionRecord.Group2 = positionDetailsVm.Group2;
                PositionRecord.Group3 = positionDetailsVm.Group3;
                PositionRecord.Group4 = positionDetailsVm.Group4;
                PositionRecord.Group5 = positionDetailsVm.Group5;
                PositionRecord.IsPositionActive = true;
                PositionRecord.LocationId = positionDetailsVm.LocationId == 0 ? null : positionDetailsVm.LocationId;
                PositionRecord.PayFrequencyCode = positionDetailsVm.PayFrequencyCode;
                PositionRecord.PerProfileID = positionDetailsVm.PerProfileID == 0 ? null : positionDetailsVm.PerProfileID;
                PositionRecord.PositionCategoryID = positionDetailsVm.PositionCategoryID;
                PositionRecord.PositionCode = positionDetailsVm.PositionCode;
                PositionRecord.PositionDescription = positionDetailsVm.PositionDescription;
                PositionRecord.PositionGradeID = positionDetailsVm.PositionGradeID;
                PositionRecord.PositionTypeID = positionDetailsVm.PositionTypeID;
                PositionRecord.PositLocation = positionDetailsVm.PositLocation;
                PositionRecord.PositReason = positionDetailsVm.PositReason;
                PositionRecord.PositStatusActive = positionDetailsVm.PositStatusActive;
                PositionRecord.PositStatusClosed = positionDetailsVm.PositStatusClosed;
                try
                {
                    PositionRecord.Status = (int)((PositionStatus)Enum.Parse(typeof(PositionStatus), positionDetailsVm.CurrentStatus));
                }
                catch { PositionRecord.Status = 1; }
                PositionRecord.PositStatusFrozen = positionDetailsVm.PositStatusFrozen;
                PositionRecord.PositStatusPost = positionDetailsVm.PositStatusPost;
                PositionRecord.ProbationPeriod = positionDetailsVm.ProbationPeriod;
                PositionRecord.ReportsToPositionId = positionDetailsVm.ReportsToPositionId;
                PositionRecord.PositLocation = positionDetailsVm.PositLocation;
                PositionRecord.Shift = positionDetailsVm.Shift;
                PositionRecord.Title = positionDetailsVm.PositionDescription;
                PositionRecord.PositionDescription = positionDetailsVm.PositionDescription;
                PositionRecord.TotalSlots = positionDetailsVm.TotalSlots;
                PositionRecord.SalaryPayGroup = positionDetailsVm.SalaryPayGroup;
                //******************************Position salary Grade*****************
                PositionRecord.ScheduledHours = positionDetailsVm.ScheduledHours;
                PositionRecord.PayFrequencyCode = positionDetailsVm.PayFrequencyCode;
                PositionRecord.SalaryAmount = positionDetailsVm.SalaryAmount;
                PositionRecord.SalaryPlanCode = positionDetailsVm.SalaryPlanCode;
                PositionRecord.PositionGradeID = PositionGradeID;
                PositionRecord.WorkClassificationId = positionDetailsVm.WorkClassificationId;
                PositionRecord.UnitsID = positionDetailsVm.UnitsID;
                PositionRecord.SalaryType = positionDetailsVm.SalaryType;
                PositionRecord.ProjectEndDate = positionDetailsVm.ProjectEndDate;
                PositionRecord.StartDate = positionDetailsVm.StartDate;
                PositionRecord.LastModifiedDate = positionDetailsVm.LastModifiedDate;
                PositionRecord.TravPercent = positionDetailsVm.TravPercent;
                PositionRecord.IncumbentADPID = positionDetailsVm.IncumbentADPID;
                PositionRecord.Requisitno = positionDetailsVm.Requisitno;
                PositionRecord.AccountNumber = positionDetailsVm.AccountNumber;
                PositionRecord.SalaryStep = positionDetailsVm.SalaryStep;
                PositionRecord.CostNumber = positionDetailsVm.CostNumber;
                PositionRecord.Suffix = positionDetailsVm.Suffix;
                //********************************************************************

                PositionRecord.WorkClassificationId = clientDbContext.DdlEmployeeTypes.Where(x => x.Active).FirstOrDefault().EmployeeTypeId;
                PositionRecord.PositionCategoryID = clientDbContext.DdlPositionCategory.Where(x => x.active).FirstOrDefault().PositionCategoryID;
                PositionRecord.PositionTypeID = clientDbContext.DdlPositionTypes.Where(x => x.active).FirstOrDefault().PositionTypeId;
                PositionRecord.PerProfileID = clientDbContext.PerformanceProfiles.Where(x => x.Active).FirstOrDefault().PerProfileID;
            }
            clientDbContext.SaveChanges();
            if (PositionRecord == null)
            {
                positionDetailsVm.PositionId = position.PositionId != 0 ? position.PositionId : positionDetailsVm.PositionId;
            }
            if (positionDetailsVm != null && PositionRecord != null)
            {
                if (positionDetailsVm.oldval != PositionRecord.Division)
                {
                    var positionhistory = new PositionHistory();
                    positionhistory.PositionId = positionDetailsVm.PositionId.ToString();
                    positionhistory.Field = "Division";
                    positionhistory.OldValue = positionDetailsVm.oldval;
                    positionhistory.NewValue = positionDetailsVm.Division;
                    positionhistory.StartDate = DateTime.Now;
                    positionhistory.EndDate = DateTime.Now;
                    clientDbContext.PositionHistory.Add(positionhistory);
                    clientDbContext.SaveChanges();
                }
            }


            if (positionDetailsVm.DepartmentId != null && positionDetailsVm.olddepartment != positionDetailsVm.DepartmentId.ToString() && positionDetailsVm.DepartmentId != 0)
            {
                var positionhistory = new PositionHistory();
                int dept = 0;
                if (positionDetailsVm.olddepartment != "")
                {
                    dept = Convert.ToInt32(positionDetailsVm.olddepartment);
                }
                var deptdesc = clientDbContext.Departments.Where(x => x.DepartmentId == positionDetailsVm.DepartmentId && x.IsDeleted == false).Select(x => x.DepartmentDescription).FirstOrDefault();
                var deptolddesc = clientDbContext.Departments.Where(x => x.DepartmentId == dept && x.IsDeleted == false).Select(x => x.DepartmentDescription).FirstOrDefault();
                positionhistory.PositionId = positionDetailsVm.PositionId.ToString();
                positionhistory.Field = "Department";
                positionhistory.OldValue = deptolddesc;
                positionhistory.NewValue = deptdesc.ToString();
                positionhistory.StartDate = DateTime.Now;
                positionhistory.EndDate = DateTime.Now;
                clientDbContext.PositionHistory.Add(positionhistory);
                clientDbContext.SaveChanges();
            }

            if (positionDetailsVm.SalaryGrade != null && positionDetailsVm.oldsalarygrade != positionDetailsVm.SalaryGrade.ToString())
            {
                var positionhistory = new PositionHistory();
                int dept = Convert.ToInt32(positionDetailsVm.SalaryGrade);
                int olddept = Convert.ToInt32(positionDetailsVm.oldsalarygrade);
                var deptdesc = clientDbContext.DdlSalaryGrades.Where(x => x.SalaryGradeID == dept).Select(x => x.description).FirstOrDefault();
                var deptolddesc = clientDbContext.DdlSalaryGrades.Where(x => x.SalaryGradeID == olddept).Select(x => x.description).FirstOrDefault();
                positionhistory.PositionId = positionDetailsVm.PositionId.ToString();
                positionhistory.Field = "SalaryGrade";
                positionhistory.OldValue = deptolddesc;
                positionhistory.NewValue = deptdesc.ToString();
                positionhistory.StartDate = DateTime.Now;
                positionhistory.EndDate = DateTime.Now;
                clientDbContext.PositionHistory.Add(positionhistory);
                clientDbContext.SaveChanges();
            }
            if (positionDetailsVm.PayFrequencyCode != null && positionDetailsVm.oldpayfrequency != positionDetailsVm.PayFrequencyCode.ToString() && positionDetailsVm.PayFrequencyCode != "0")
            {
                var positionhistory = new PositionHistory();

                var deptdesc = clientDbContext.DdlPayFrequencies.Where(x => x.Code == positionDetailsVm.PayFrequencyCode).Select(x => x.Description).FirstOrDefault();
                var deptolddesc = clientDbContext.DdlPayFrequencies.Where(x => x.Code == positionDetailsVm.oldpayfrequency).Select(x => x.Description).FirstOrDefault();
                positionhistory.PositionId = positionDetailsVm.PositionId.ToString();
                positionhistory.Field = "PayFrequency";
                positionhistory.OldValue = deptolddesc;
                positionhistory.NewValue = deptdesc.ToString();
                positionhistory.StartDate = DateTime.Now;
                positionhistory.EndDate = DateTime.Now;
                clientDbContext.PositionHistory.Add(positionhistory);
                clientDbContext.SaveChanges();
            }
            if (positionDetailsVm.oldsalaryplancode != positionDetailsVm.SalaryPlanCode && positionDetailsVm.SalaryPlanCode != null)
            {
                var positionhistory = new PositionHistory();
                positionhistory.PositionId = positionDetailsVm.PositionId.ToString();
                positionhistory.Field = "SalaryPlanCode";
                positionhistory.OldValue = positionDetailsVm.oldsalaryplancode;
                positionhistory.NewValue = positionDetailsVm.SalaryPlanCode;
                positionhistory.StartDate = DateTime.Now;
                positionhistory.EndDate = DateTime.Now;
                clientDbContext.PositionHistory.Add(positionhistory);
                clientDbContext.SaveChanges();
            }
            if (positionDetailsVm.oldsalarystep != positionDetailsVm.SalaryStep && positionDetailsVm.SalaryStep != null)
            {
                var positionhistory = new PositionHistory();
                positionhistory.PositionId = positionDetailsVm.PositionId.ToString();
                positionhistory.Field = "SalaryStep";
                positionhistory.OldValue = positionDetailsVm.oldsalarystep;
                positionhistory.NewValue = positionDetailsVm.SalaryStep;
                positionhistory.StartDate = DateTime.Now;
                positionhistory.EndDate = DateTime.Now;
                clientDbContext.PositionHistory.Add(positionhistory);
                clientDbContext.SaveChanges();
            }
            return positionDetailsVm;
        }

        internal PositionFundingSourceGroupVM getNewPositionFundingSource()
        {
            return positionBudgetService.getNewPositionFundingSource();
        }

        private IEnumerable<DropDownModel> getSalaryGrade()
        {
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            var salarydescription = clientDbContext.DdlSalaryGrades.Select(x => new DropDownModel { keyvalue = x.SalaryGradeID.ToString(), keydescription = x.description }).ToList();
            salarydescription.Insert(0, new DropDownModel { keyvalue = "0", keydescription = "--Select--" });
            return salarydescription;
        }

        public IEnumerable<DropDownModel> WorkStatusTypeDropDown()
        {
            List<DropDownModel> WorkStatusDropDown = new List<DropDownModel>();
            WorkStatusDropDown.Add(new DropDownModel { keyvalue = "Reg", keydescription = "Regular" });
            WorkStatusDropDown.Add(new DropDownModel { keyvalue = "Temp", keydescription = "Tempory" });
            return WorkStatusDropDown;
        }
        public int SavePositionSalaryGrade(short salaryGrade, int PositionId, string Enterby)
        {
            int _positionsalaryGradeID = 0;
            if (salaryGrade > 0)
            {
                ClientDbContext clientDbContext = new ClientDbContext(connString);
                var PositionsalaryGradeInDb = clientDbContext.PositionSalaryGrades.Where(x => (x.PositionId == PositionId) && (x.salaryGradeID == salaryGrade)).SingleOrDefault();
                if (PositionsalaryGradeInDb == null)
                {
                    var newePositionSalaryGrade = new PositionSalaryGrades
                    {
                        salaryGradeID = Convert.ToInt16(salaryGrade),
                        PositionId = Convert.ToInt16(PositionId),
                        enteredBy = Enterby,
                        enteredDate = System.DateTime.Now
                    };
                    try
                    {
                        clientDbContext.PositionSalaryGrades.Add(newePositionSalaryGrade);
                        clientDbContext.SaveChanges();
                        _positionsalaryGradeID = newePositionSalaryGrade.id;
                    }
                    catch { }
                }
                else
                {
                    _positionsalaryGradeID = PositionsalaryGradeInDb.id;
                }
            }
            return _positionsalaryGradeID;
        }
        private int getSlotFilled(int? PositionId)
        {
            int count = 0;
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            try
            {
                count = clientDbContext.E_Positions.Where(x => x.PositionId == PositionId).Count();
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(ex.Message))
                {
                    return count;
                }
            }
            return count;

        }

        public List<E_PositionSalaryHistoryVm> GetEmployeeSalary(PositionDetailsVm positionDetailsVm)
        {
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            var data = clientDbContext.E_Positions.Where(x => x.PositionId == positionDetailsVm.PositionId).ToList();

            List<E_PositionSalaryHistoryVm> EmployeeSalary = new List<E_PositionSalaryHistoryVm>();
            var AverageSalary = clientDbContext.Positions.Where(x => x.PositionId == positionDetailsVm.PositionId).FirstOrDefault().SalaryAmount;
            decimal? BudgetAmount = GetPositionBudget(positionDetailsVm.PositionId);
            System.Text.StringBuilder ValidationMessage = new System.Text.StringBuilder();
            if (positionDetailsVm.TotalSlots > 0)
            {
                foreach (var item in data)
                {

                    var ResultData = clientDbContext.E_Positions.Where(m => m.E_PositionId == item.E_PositionId).FirstOrDefault();
                    int personid = clientDbContext.Employees.Where(m => m.EmployeeId == ResultData.EmployeeId).Select(x => x.PersonId).SingleOrDefault();
                    string EmpName = clientDbContext.Persons.Where(m => m.PersonId == personid).Select(x => x.Firstname + " " + x.Lastname).SingleOrDefault();
                    List<E_PositionSalaryHistoryVm> Emp = clientDbContext.E_PositionSalaryHistories
                        .Include("E_Positions.DdlPayFrequency")
                        .OrderByDescending(e => e.EffectiveDate)
                        .Where(x => x.E_PositionId == item.E_PositionId).ToList()
                        .Select(e => new E_PositionSalaryHistoryVm
                        {
                            RateTypeId = e.E_Positions.RateTypeId,
                            PayRate = e.PayRate,
                            HoursPerPayPeriod = e.HoursPerPayPeriod,
                            BudgetSalary = BudgetAmount / positionDetailsVm.TotalSlots,
                            EffectiveDate = e.EffectiveDate,
                            EnteredDate = (e.EffectiveDate == null ? (DateTime?)null : e.EffectiveDate.Value.AddDays(-1)),
                            EmpName = EmpName,
                            AverageSalary = Convert.ToDecimal(AverageSalary),

                            SalaryVariance = e.E_Positions.DdlPayFrequency == null ? 0 : (BudgetAmount / positionDetailsVm.TotalSlots) - (e.E_Positions.DdlPayFrequency.Description.ToLower().StartsWith("Bi", StringComparison.InvariantCultureIgnoreCase) ? (26 * e.HoursPerPayPeriod * e.PayRate) :
                                    (e.E_Positions.DdlPayFrequency.Description.ToLower().StartsWith("Semi", StringComparison.InvariantCultureIgnoreCase) ? (24 * e.HoursPerPayPeriod * e.PayRate) :
                                        (e.E_Positions.DdlPayFrequency.Description.ToLower().StartsWith("Mon", StringComparison.InvariantCultureIgnoreCase) ? (12 * e.HoursPerPayPeriod * e.PayRate) :
                                            (e.E_Positions.DdlPayFrequency.Description.ToLower().StartsWith("Week", StringComparison.InvariantCultureIgnoreCase) ? (52 * e.HoursPerPayPeriod * e.PayRate) : 0)
                                        )
                                    )
                                 ),

                            AnnualSalary = e.E_Positions.DdlPayFrequency == null ? 0 : (e.E_Positions.DdlPayFrequency.Description.ToLower().StartsWith("Bi", StringComparison.InvariantCultureIgnoreCase) ? (26 * e.HoursPerPayPeriod * e.PayRate) :
                                    (e.E_Positions.DdlPayFrequency.Description.ToLower().StartsWith("Semi", StringComparison.InvariantCultureIgnoreCase) ? (24 * e.HoursPerPayPeriod * e.PayRate) :
                                        (e.E_Positions.DdlPayFrequency.Description.ToLower().StartsWith("Mon", StringComparison.InvariantCultureIgnoreCase) ? (12 * e.HoursPerPayPeriod * e.PayRate) :
                                            (e.E_Positions.DdlPayFrequency.Description.ToLower().StartsWith("Week", StringComparison.InvariantCultureIgnoreCase) ? (52 * e.HoursPerPayPeriod * e.PayRate) : 0)
                                        )
                                    )
                                 ),

                        }).ToList();
                    EmployeeSalary.AddRange(Emp);

                }

            }
            var DistinctItems = EmployeeSalary.GroupBy(x => x.EmpName).Select(y => y.First());
            EmployeeSalary = DistinctItems.ToList();
            foreach (var item in EmployeeSalary)
            {
                if (item.SalaryVariance == null) { item.SalaryVariance = BudgetAmount / positionDetailsVm.TotalSlots; }
                ValidationMessage.Append(item.AnnualSalary == null ? item.EmpName + ", " : "");
            }
            if (positionDetailsVm.TotalSlots > EmployeeSalary.Count)
            {
                for (int i = EmployeeSalary.Count; i <= positionDetailsVm.TotalSlots - 1; i++)
                {
                    E_PositionSalaryHistoryVm SetData = new E_PositionSalaryHistoryVm();
                    SetData.EmpName = "unassigned";
                    SetData.AnnualSalary = null;
                    SetData.SalaryVariance = BudgetAmount / positionDetailsVm.TotalSlots;
                    SetData.AverageSalary = Convert.ToDecimal(AverageSalary);
                    SetData.BudgetSalary = BudgetAmount / positionDetailsVm.TotalSlots;
                    EmployeeSalary.Add(SetData);
                }
            }
            return EmployeeSalary;
        }

        public decimal? GetPositionBudget(int PositionId)
        {
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            {
                var PositionBudgetList = (from pb in clientDbContext.PositionsBudgets
                                          join p in clientDbContext.Positions on pb.PositionID equals p.PositionId
                                          where pb.PositionID == PositionId
                                          select new PositionBudgetsVM
                                          {
                                              ID = pb.ID,
                                              PositionID = pb.PositionID,
                                              BudgetYear = pb.BudgetYear,
                                              BudgetMonth = pb.BudgetMonth,
                                              BudgetAmount = pb.BudgetAmount,
                                              FTE = pb.FTE,
                                              PositionTitle = p.Title,
                                          }).ToList();
                decimal? Amount = 0;
                if (PositionBudgetList.Count() > 0)
                {



                    Amount = PositionBudgetList[PositionBudgetList.Count - 1].BudgetAmount;
                }

                return Amount;
            }
        }


        #endregion
    }
}
