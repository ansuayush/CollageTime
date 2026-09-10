using ExecViewHrk.EfClient;
using System.Linq;

namespace ExecViewHrk.WebUI.Helpers
{
    public static class StFormMetadataSchemaHelper
    {
        public static void EnsureSchema(ClientDbContext db)
        {
            db.Database.ExecuteSqlCommand(@"
IF OBJECT_ID(N'[dbo].[stModules]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[stModules] (
        [Id]         INT IDENTITY(1,1) NOT NULL,
        [ModuleName] VARCHAR(50) NULL,
        [ControlId]  VARCHAR(50) NULL,
        CONSTRAINT [PK_stModules] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
END

IF OBJECT_ID(N'[dbo].[stForms]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[stForms] (
        [Id]         INT NOT NULL,
        [FormName]   VARCHAR(50) NULL,
        [ModuleId]   INT NULL,
        [ControlId]  VARCHAR(50) NULL,
        [IsUsed]     BIT NULL,
        [TableName]  VARCHAR(25) NULL,
        CONSTRAINT [PK_stForms] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
END

IF OBJECT_ID(N'[dbo].[stFormSections]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[stFormSections] (
        [Id]                   INT NOT NULL,
        [FormId]               INT NOT NULL,
        [SectionName]          VARCHAR(50) NULL,
        [Sort]                 INT NULL,
        [IsAllowedInWorkFlow]  BIT NULL,
        [TableName]            VARCHAR(50) NULL,
        [IsAllowedInTemplate]  BIT NULL,
        [IsAllowedInReports]   BIT NULL,
        CONSTRAINT [PK_stFormSections] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
END

IF OBJECT_ID(N'[dbo].[stMetaTable]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[stMetaTable] (
        [Id]                   INT IDENTITY(1,1) NOT NULL,
        [SectionId]            INT NULL,
        [Field]                VARCHAR(50) NULL,
        [FieldType]            VARCHAR(50) NULL,
        [FieldMaxLength]       INT NULL,
        [IsValid]              BIT NULL,
        [IsRequired]           BIT NULL,
        [IsAllowed]            BIT NULL,
        [IsAllowedInWorkFlow]  BIT NULL,
        [HTablesCode]          CHAR(10) NULL,
        [DataTextField]        VARCHAR(50) NULL,
        [DataValueField]       VARCHAR(50) NULL,
        [DisplayName]          VARCHAR(MAX) NULL,
        [PanelName]            VARCHAR(50) NULL,
        [IsImportableField]    BIT NULL,
        [ImportDisplayName]    VARCHAR(50) NULL,
        [ImportType]           CHAR(1) NULL,
        [IsMassChangeable]     BIT NULL,
        [MassType]             VARCHAR(10) NULL,
        [SampleData]           VARCHAR(50) NULL,
        [IsAllowedinReports]   BIT NULL,
        [IsAllowedInTemplate]  BIT NULL,
        [IsVisible]            BIT NULL,
        [IsReadOnly]           BIT NULL,
        [IsSearchable]         BIT NULL,
        [IsSortable]           BIT NULL,
        [DefaultValue]         VARCHAR(500) NULL,
        [ValidationExpression] VARCHAR(1000) NULL,
        [Placeholder]          VARCHAR(255) NULL,
        [ToolTip]              VARCHAR(1000) NULL,
        CONSTRAINT [PK_stMetatable] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
END
ELSE
BEGIN
    IF COL_LENGTH('dbo.stMetaTable', 'IsVisible') IS NULL ALTER TABLE dbo.stMetaTable ADD [IsVisible] BIT NULL;
    IF COL_LENGTH('dbo.stMetaTable', 'IsReadOnly') IS NULL ALTER TABLE dbo.stMetaTable ADD [IsReadOnly] BIT NULL;
    IF COL_LENGTH('dbo.stMetaTable', 'IsSearchable') IS NULL ALTER TABLE dbo.stMetaTable ADD [IsSearchable] BIT NULL;
    IF COL_LENGTH('dbo.stMetaTable', 'IsSortable') IS NULL ALTER TABLE dbo.stMetaTable ADD [IsSortable] BIT NULL;
    IF COL_LENGTH('dbo.stMetaTable', 'DefaultValue') IS NULL ALTER TABLE dbo.stMetaTable ADD [DefaultValue] VARCHAR(500) NULL;
    IF COL_LENGTH('dbo.stMetaTable', 'ValidationExpression') IS NULL ALTER TABLE dbo.stMetaTable ADD [ValidationExpression] VARCHAR(1000) NULL;
    IF COL_LENGTH('dbo.stMetaTable', 'Placeholder') IS NULL ALTER TABLE dbo.stMetaTable ADD [Placeholder] VARCHAR(255) NULL;
    IF COL_LENGTH('dbo.stMetaTable', 'ToolTip') IS NULL ALTER TABLE dbo.stMetaTable ADD [ToolTip] VARCHAR(1000) NULL;
END

IF OBJECT_ID(N'[dbo].[stMetaData]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[stMetaData] (
        [Id]          INT IDENTITY(1,1) NOT NULL,
        [FieldId]     INT NOT NULL,
        [CompanyId]   INT NOT NULL,
        [DisplayName] VARCHAR(MAX) NULL,
        CONSTRAINT [PK_stMetaData] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
END

IF OBJECT_ID(N'[dbo].[stModuleSecurity]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[stModuleSecurity] (
        [Id]       INT IDENTITY(1,1) NOT NULL,
        [ModuleId] INT NOT NULL,
        [RoleName] NVARCHAR(256) NOT NULL,
        [CanView]  BIT NOT NULL CONSTRAINT [DF_stModSec_View] DEFAULT (0),
        [CanAdd]   BIT NOT NULL CONSTRAINT [DF_stModSec_Add] DEFAULT (0),
        [CanEdit]  BIT NOT NULL CONSTRAINT [DF_stModSec_Edit] DEFAULT (0),
        [CanDelete] BIT NOT NULL CONSTRAINT [DF_stModSec_Del] DEFAULT (0),
        [CanExport] BIT NOT NULL CONSTRAINT [DF_stModSec_Exp] DEFAULT (0),
        [IsActive] BIT NOT NULL CONSTRAINT [DF_stModSec_Act] DEFAULT (1),
        CONSTRAINT [PK_stModuleSecurity] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
END

IF OBJECT_ID(N'[dbo].[stFormSecurity]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[stFormSecurity] (
        [Id]       INT IDENTITY(1,1) NOT NULL,
        [FormId]   INT NOT NULL,
        [RoleName] NVARCHAR(256) NOT NULL,
        [CanView]  BIT NOT NULL CONSTRAINT [DF_stFormSec_View] DEFAULT (0),
        [CanAdd]   BIT NOT NULL CONSTRAINT [DF_stFormSec_Add] DEFAULT (0),
        [CanEdit]  BIT NOT NULL CONSTRAINT [DF_stFormSec_Edit] DEFAULT (0),
        [CanDelete] BIT NOT NULL CONSTRAINT [DF_stFormSec_Del] DEFAULT (0),
        [CanExport] BIT NOT NULL CONSTRAINT [DF_stFormSec_Exp] DEFAULT (0),
        [IsActive] BIT NOT NULL CONSTRAINT [DF_stFormSec_Act] DEFAULT (1),
        CONSTRAINT [PK_stFormSecurity] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
END

IF OBJECT_ID(N'[dbo].[stSectionSecurity]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[stSectionSecurity] (
        [Id]        INT IDENTITY(1,1) NOT NULL,
        [SectionId] INT NOT NULL,
        [RoleName]  NVARCHAR(256) NOT NULL,
        [CanView]   BIT NOT NULL CONSTRAINT [DF_stSecSec_View] DEFAULT (0),
        [CanAdd]    BIT NOT NULL CONSTRAINT [DF_stSecSec_Add] DEFAULT (0),
        [CanEdit]   BIT NOT NULL CONSTRAINT [DF_stSecSec_Edit] DEFAULT (0),
        [IsActive]  BIT NOT NULL CONSTRAINT [DF_stSecSec_Act] DEFAULT (1),
        CONSTRAINT [PK_stSectionSecurity] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
END

IF OBJECT_ID(N'[dbo].[stFieldSecurity]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[stFieldSecurity] (
        [Id]         INT IDENTITY(1,1) NOT NULL,
        [FieldId]    INT NOT NULL,
        [RoleName]   NVARCHAR(256) NOT NULL,
        [CanView]    BIT NOT NULL CONSTRAINT [DF_stFldSec_View] DEFAULT (1),
        [CanAdd]     BIT NOT NULL CONSTRAINT [DF_stFldSec_Add] DEFAULT (1),
        [CanEdit]    BIT NOT NULL CONSTRAINT [DF_stFldSec_Edit] DEFAULT (1),
        [IsRequired] BIT NULL,
        [IsActive]   BIT NOT NULL CONSTRAINT [DF_stFldSec_Act] DEFAULT (1),
        CONSTRAINT [PK_stFieldSecurity] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
END
");
            SeedDefaults(db);
        }

        private static void SeedDefaults(ClientDbContext db)
        {
            if (!db.StModules.Any())
            {
                db.StModules.Add(new StModule { ModuleName = "Human Resources", ControlId = "iEmployee" });
                db.StModules.Add(new StModule { ModuleName = "Time And Attendance", ControlId = "iTimeAndAttendance" });
                db.StModules.Add(new StModule { ModuleName = "Payroll", ControlId = "iPayroll" });
                db.StModules.Add(new StModule { ModuleName = "Administration", ControlId = "iAdministration" });
                db.StModules.Add(new StModule { ModuleName = "Reporting", ControlId = "iReporting" });
                db.StModules.Add(new StModule { ModuleName = "Setup", ControlId = "iSetup" });
                db.SaveChanges();
            }

            var hr = db.StModules.FirstOrDefault(m => m.ControlId == "iEmployee")
                     ?? db.StModules.OrderBy(m => m.Id).FirstOrDefault();
            if (hr == null) return;
            int hrId = hr.Id;

            if (!db.StForms.Any())
            {
                db.StForms.Add(new StForm { Id = 1, FormName = "Personal", ModuleId = hrId, ControlId = "iPersonal", IsUsed = true, TableName = "Person" });
                db.StForms.Add(new StForm { Id = 2, FormName = "Employment", ModuleId = hrId, ControlId = "iEmployment", IsUsed = true, TableName = "Employee" });
                db.StForms.Add(new StForm { Id = 5, FormName = "Performance", ModuleId = hrId, ControlId = "iPerformance", IsUsed = true, TableName = "PR_Review" });
                db.StForms.Add(new StForm { Id = 6, FormName = "Talent Management", ModuleId = hrId, ControlId = "iEducation", IsUsed = true, TableName = null });
                db.SaveChanges();
            }
            else
            {
                var perf = db.StForms.FirstOrDefault(f => f.Id == 5 || f.ControlId == "iPerformance");
                if (perf == null)
                {
                    int nextId = db.StForms.Max(f => f.Id) + 1;
                    db.StForms.Add(new StForm { Id = nextId < 5 ? 5 : nextId, FormName = "Performance", ModuleId = hrId, ControlId = "iPerformance", IsUsed = true, TableName = "PR_Review" });
                    db.SaveChanges();
                }
                else if (perf.IsUsed != true)
                {
                    perf.IsUsed = true;
                    if (string.IsNullOrEmpty(perf.TableName)) perf.TableName = "PR_Review";
                    db.SaveChanges();
                }
            }

            if (!db.StFormSections.Any())
            {
                db.StFormSections.Add(new StFormSection { Id = 1, FormId = 1, SectionName = "PersonalInfo", Sort = 1, IsAllowedInWorkFlow = true, TableName = "Person", IsAllowedInTemplate = true, IsAllowedInReports = true });
                db.StFormSections.Add(new StFormSection { Id = 2, FormId = 1, SectionName = "EmergencyContact", Sort = 2, IsAllowedInWorkFlow = true, TableName = "Person", IsAllowedInTemplate = true, IsAllowedInReports = true });
                db.StFormSections.Add(new StFormSection { Id = 5, FormId = 2, SectionName = "Employment", Sort = 1, IsAllowedInWorkFlow = true, TableName = "Employee", IsAllowedInTemplate = true, IsAllowedInReports = true });
                db.StFormSections.Add(new StFormSection { Id = 50, FormId = 5, SectionName = "PerformanceReviews", Sort = 1, IsAllowedInWorkFlow = true, TableName = "PR_ReviewEmployee", IsAllowedInTemplate = false, IsAllowedInReports = true });
                db.StFormSections.Add(new StFormSection { Id = 51, FormId = 5, SectionName = "ApproverScores", Sort = 2, IsAllowedInWorkFlow = true, TableName = "PR_ReviewReviewerEmployee", IsAllowedInTemplate = false, IsAllowedInReports = true });
                db.SaveChanges();
            }

            EnsureEmploymentSectionAndFields(db);

            if (!db.StMetaTables.Any(m => m.SectionId == 1))
            {
                SeedField(db, 1, "FNAME", "varchar", "First Name", "pnlPerFName", "Jack");
                SeedField(db, 1, "LNAME", "varchar", "Last Name", "pnlPerLName", "Smith");
                SeedField(db, 1, "SSN", "varchar", "SSN", "pnlPerSSN", "123-45-6789");
                SeedField(db, 1, "BIRTH", "datetime", "Birth Date", "pnlPerBirth", "1/1/1980");
                SeedField(db, 1, "GENDER", "list", "Gender", "pnlPerGender", null, "GE");
            }
            if (!db.StMetaTables.Any(m => m.SectionId == 50))
            {
                SeedField(db, 50, "ReviewName", "varchar", "Review Name", "pnlPerfName", "Annual Review");
                SeedField(db, 50, "ReviewType", "varchar", "Review Type", "pnlPerfType", "Annual");
                SeedField(db, 50, "InitiatedDate", "datetime", "Initiated Date", "pnlPerfInit", null);
                SeedField(db, 50, "CompletionDate", "datetime", "Completion Date", "pnlPerfComplete", null);
                SeedField(db, 50, "Status", "varchar", "Status", "pnlPerfStatus", "Complete");
                SeedField(db, 50, "FinalScore", "numeric", "Final Score", "pnlPerfFinalScore", null);
            }
            if (!db.StMetaTables.Any(m => m.SectionId == 51))
            {
                SeedField(db, 51, "Approver1Score", "numeric", "Approver 1 Score", "pnlAppr1Score", null);
                SeedField(db, 51, "Approver2Score", "numeric", "Approver 2 Score", "pnlAppr2Score", null);
                SeedField(db, 51, "Approver3Score", "numeric", "Approver 3 Score", "pnlAppr3Score", null);
                SeedField(db, 51, "HrComments", "varchar", "HR Comments", "pnlHrComments", null);
            }
            db.SaveChanges();
        }

        private static void EnsureEmploymentSectionAndFields(ClientDbContext db)
        {
            var empForm = db.StForms.FirstOrDefault(f => f.ControlId == "iEmployment" || f.FormName == "Employment");
            if (empForm == null) return;

            var section = db.StFormSections.FirstOrDefault(s => s.FormId == empForm.Id && s.SectionName == "Employment");
            if (section == null)
            {
                int nextId = db.StFormSections.Any() ? db.StFormSections.Max(s => s.Id) + 1 : 5;
                section = new StFormSection
                {
                    Id = nextId,
                    FormId = empForm.Id,
                    SectionName = "Employment",
                    Sort = 1,
                    IsAllowedInWorkFlow = true,
                    TableName = "Employee",
                    IsAllowedInTemplate = true,
                    IsAllowedInReports = true
                };
                db.StFormSections.Add(section);
                db.SaveChanges();
            }

            if (db.StMetaTables.Any(m => m.SectionId == section.Id)) return;

            int sid = section.Id;
            SeedField(db, sid, "PersonId", "list", "Employee", "pnlEmpPerson", null, null, true);
            SeedField(db, sid, "CompanyCodeId", "list", "Company Code", "pnlEmpCompany", "ELL", null, true);
            SeedField(db, sid, "BusinessLevelNbr", "list", "BusinessLevel Code", "pnlEmpBusinessLevel", null, null, true);
            SeedField(db, sid, "FileNumber", "varchar", "File Number", "pnlEmpFileNumber", "112524", null, true);
            SeedField(db, sid, "Rate", "numeric", "Rate", "pnlEmpRate", "8.85");
            SeedField(db, sid, "EmploymentNumber", "varchar", "Employment Number", "pnlEmpEmploymentNumber", "1", null, true);
            SeedField(db, sid, "EmploymentStatusId", "list", "Employee Status", "pnlEmpStatus", "Active", null, true);
            SeedField(db, sid, "EmployeeTypeID", "list", "Employee Type", "pnlEmpType", null);
            SeedField(db, sid, "HireDate", "datetime", "Hire Date", "pnlEmpHireDate", "10/18/2019", null, true);
            SeedField(db, sid, "TerminationDate", "datetime", "Termination Date", "pnlEmpTermDate", null);
            SeedField(db, sid, "PayFrequencyId", "list", "Pay Frequency", "pnlEmpPayFreq", "BiWeekly", null, true);
            SeedField(db, sid, "FedExemptions", "varchar", "Fed Exemptions", "pnlEmpFedEx", null);
            SeedField(db, sid, "MaritalStatusID", "list", "Marital Status", "pnlEmpMarital", null);
            SeedField(db, sid, "WorkedStateTaxCodeId", "list", "Worked State Tax Code", "pnlEmpWorkedState", null);
            SeedField(db, sid, "Hours", "numeric", "Hours", "pnlEmpHours", null);
            SeedField(db, sid, "ActualServiceStartDate", "datetime", "Actual Service Start Date", "pnlEmpActualStart", null);
            SeedField(db, sid, "PlannedServiceStartDate", "datetime", "Planned Service Start Date", "pnlEmpPlanStart", null);
            SeedField(db, sid, "ProbationEndDate", "datetime", "Probation End Date", "pnlEmpProbation", null);
            SeedField(db, sid, "TrainingEndDate", "datetime", "Training End Date", "pnlEmpTraining", null);
            SeedField(db, sid, "SeniorityDate", "datetime", "Seniority Date", "pnlEmpSeniority", null);
            SeedField(db, sid, "TimeCardTypeId", "list", "Time Card Type", "pnlEmpTimeCard", "DH-DailyHoursTC", null, true);
            SeedField(db, sid, "EarningsCodeId", "list", "Earning Code", "pnlEmpEarnCode", "13");
            SeedField(db, sid, "Amount", "numeric", "Remaining Amount", "pnlEmpAmount", null, null, false, true);
            SeedField(db, sid, "TreatyLimit", "numeric", "Treaty Limit", "pnlEmpTreaty", null);
            SeedField(db, sid, "UsedAmount", "numeric", "Used Amount", "pnlEmpUsedAmount", null, null, false, true);
            SeedField(db, sid, "IsStudent", "bit", "IsStudent", "pnlEmpIsStudent", null);
            db.SaveChanges();
        }

        private static void SeedField(ClientDbContext db, int sectionId, string field, string type, string display,
            string panel, string sample, string hTablesCode = null, bool isRequired = false, bool isReadOnly = false)
        {
            db.StMetaTables.Add(new StMetaTable
            {
                SectionId = sectionId,
                Field = field,
                FieldType = type,
                IsValid = true,
                IsRequired = isRequired,
                IsAllowed = true,
                IsAllowedInWorkFlow = true,
                HTablesCode = hTablesCode,
                DataTextField = hTablesCode != null ? "Description" : null,
                DataValueField = hTablesCode != null ? "Id" : null,
                DisplayName = display,
                PanelName = panel,
                SampleData = sample,
                IsVisible = true,
                IsReadOnly = isReadOnly,
                IsAllowedinReports = true,
                IsAllowedInTemplate = true
            });
        }
    }
}
