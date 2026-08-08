-- Benefits Administration & Open Enrollment (SRS)
-- Safe to re-run

IF OBJECT_ID(N'[dbo].[BenCategories]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[BenCategories] (
        [CategoryId]   INT            IDENTITY (1, 1) NOT NULL,
        [CategoryName] NVARCHAR (100) NOT NULL,
        [Description]  NVARCHAR (500) NULL,
        [IsActive]     BIT            NOT NULL CONSTRAINT [DF_BenCategories_Active] DEFAULT (1),
        [DisplayOrder] INT            NOT NULL CONSTRAINT [DF_BenCategories_Order] DEFAULT (0),
        [CreatedBy]    NVARCHAR (100) NULL,
        [CreatedDate]  DATETIME       NOT NULL,
        [ModifiedBy]   NVARCHAR (100) NULL,
        [ModifiedDate] DATETIME       NULL,
        CONSTRAINT [PK_BenCategories] PRIMARY KEY CLUSTERED ([CategoryId] ASC)
    );
END
GO

IF OBJECT_ID(N'[dbo].[BenWaitingPeriods]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[BenWaitingPeriods] (
        [WaitingPeriodId] INT            IDENTITY (1, 1) NOT NULL,
        [Name]            NVARCHAR (100) NOT NULL,
        [Days]            INT            NOT NULL CONSTRAINT [DF_BenWait_Days] DEFAULT (0),
        [CalculationType] NVARCHAR (50)  NOT NULL CONSTRAINT [DF_BenWait_Calc] DEFAULT (N'Days'),
        [Description]     NVARCHAR (500) NULL,
        [IsActive]        BIT            NOT NULL CONSTRAINT [DF_BenWait_Active] DEFAULT (1),
        CONSTRAINT [PK_BenWaitingPeriods] PRIMARY KEY CLUSTERED ([WaitingPeriodId] ASC)
    );
END
GO

IF OBJECT_ID(N'[dbo].[BenEligibilityRules]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[BenEligibilityRules] (
        [EligibilityRuleId]   INT            IDENTITY (1, 1) NOT NULL,
        [RuleName]            NVARCHAR (150) NOT NULL,
        [Description]         NVARCHAR (500) NULL,
        [EmploymentStatusIds] NVARCHAR (200) NULL,
        [EmployeeTypeIds]     NVARCHAR (200) NULL,
        [MinHours]            FLOAT          NULL,
        [MinServiceDays]      INT            NULL,
        [MinAge]              INT            NULL,
        [RuleExpression]      NVARCHAR (1000) NULL,
        [IsActive]            BIT            NOT NULL CONSTRAINT [DF_BenElig_Active] DEFAULT (1),
        CONSTRAINT [PK_BenEligibilityRules] PRIMARY KEY CLUSTERED ([EligibilityRuleId] ASC)
    );
END
GO

IF OBJECT_ID(N'[dbo].[BenPlans]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[BenPlans] (
        [PlanId]              INT             IDENTITY (1, 1) NOT NULL,
        [PlanName]            NVARCHAR (200)  NOT NULL,
        [PlanCode]            NVARCHAR (50)   NULL,
        [CategoryId]          INT             NOT NULL,
        [Carrier]             NVARCHAR (150)  NULL,
        [Description]         NVARCHAR (1000) NULL,
        [EffectiveDate]       DATETIME        NULL,
        [ExpirationDate]      DATETIME        NULL,
        [EmployeeCost]        FLOAT           NOT NULL CONSTRAINT [DF_BenPlans_EmpCost] DEFAULT (0),
        [EmployerCost]        FLOAT           NOT NULL CONSTRAINT [DF_BenPlans_ErCost] DEFAULT (0),
        [RequireDependents]   BIT             NOT NULL CONSTRAINT [DF_BenPlans_ReqDep] DEFAULT (0),
        [RequireBeneficiary]  BIT             NOT NULL CONSTRAINT [DF_BenPlans_ReqBen] DEFAULT (0),
        [WaiveAllowed]        BIT             NOT NULL CONSTRAINT [DF_BenPlans_Waive] DEFAULT (1),
        [IsActive]            BIT             NOT NULL CONSTRAINT [DF_BenPlans_Active] DEFAULT (1),
        [CreatedBy]           NVARCHAR (100)  NULL,
        [CreatedDate]         DATETIME        NOT NULL,
        [ModifiedBy]          NVARCHAR (100)  NULL,
        [ModifiedDate]        DATETIME        NULL,
        CONSTRAINT [PK_BenPlans] PRIMARY KEY CLUSTERED ([PlanId] ASC)
    );
END
GO

IF OBJECT_ID(N'[dbo].[BenCoverageOptions]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[BenCoverageOptions] (
        [CoverageOptionId] INT            IDENTITY (1, 1) NOT NULL,
        [PlanId]           INT            NOT NULL,
        [OptionCode]       NVARCHAR (50)  NOT NULL,
        [OptionName]       NVARCHAR (150) NOT NULL,
        [EmployeeCost]     FLOAT          NOT NULL CONSTRAINT [DF_BenCov_EmpCost] DEFAULT (0),
        [EmployerCost]     FLOAT          NOT NULL CONSTRAINT [DF_BenCov_ErCost] DEFAULT (0),
        [RequiresDependent] BIT           NOT NULL CONSTRAINT [DF_BenCov_ReqDep] DEFAULT (0),
        [SortOrder]        INT            NOT NULL CONSTRAINT [DF_BenCov_Sort] DEFAULT (0),
        [IsActive]         BIT            NOT NULL CONSTRAINT [DF_BenCov_Active] DEFAULT (1),
        CONSTRAINT [PK_BenCoverageOptions] PRIMARY KEY CLUSTERED ([CoverageOptionId] ASC)
    );
END
GO

IF OBJECT_ID(N'[dbo].[BenClasses]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[BenClasses] (
        [BenefitClassId]    INT            IDENTITY (1, 1) NOT NULL,
        [ClassName]         NVARCHAR (150) NOT NULL,
        [Description]       NVARCHAR (500) NULL,
        [WaitingPeriodId]   INT            NULL,
        [EligibilityRuleId] INT            NULL,
        [IsActive]          BIT            NOT NULL CONSTRAINT [DF_BenClasses_Active] DEFAULT (1),
        [CreatedBy]         NVARCHAR (100) NULL,
        [CreatedDate]       DATETIME       NOT NULL,
        [ModifiedBy]        NVARCHAR (100) NULL,
        [ModifiedDate]      DATETIME       NULL,
        CONSTRAINT [PK_BenClasses] PRIMARY KEY CLUSTERED ([BenefitClassId] ASC)
    );
END
GO

IF OBJECT_ID(N'[dbo].[BenClassPlans]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[BenClassPlans] (
        [BenefitClassPlanId] INT IDENTITY (1, 1) NOT NULL,
        [BenefitClassId]     INT NOT NULL,
        [PlanId]             INT NOT NULL,
        [SortOrder]          INT NOT NULL CONSTRAINT [DF_BenClassPlans_Sort] DEFAULT (0),
        CONSTRAINT [PK_BenClassPlans] PRIMARY KEY CLUSTERED ([BenefitClassPlanId] ASC)
    );
END
GO

IF OBJECT_ID(N'[dbo].[BenEnrollmentPeriods]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[BenEnrollmentPeriods] (
        [EnrollmentPeriodId]   INT            IDENTITY (1, 1) NOT NULL,
        [EnrollmentName]       NVARCHAR (200) NOT NULL,
        [StartDate]            DATETIME       NOT NULL,
        [EndDate]              DATETIME       NOT NULL,
        [CoverageEffectiveDate] DATETIME      NULL,
        [AllowChangesUntil]    DATETIME       NULL,
        [Status]               NVARCHAR (30)  NOT NULL CONSTRAINT [DF_BenOE_Status] DEFAULT (N'Draft'),
        [EnrollmentMessage]    NVARCHAR (1000) NULL,
        [ReminderEmails]       BIT            NOT NULL CONSTRAINT [DF_BenOE_Remind] DEFAULT (1),
        [CreatedBy]            NVARCHAR (100) NULL,
        [CreatedDate]          DATETIME       NOT NULL,
        [ModifiedBy]           NVARCHAR (100) NULL,
        [ModifiedDate]         DATETIME       NULL,
        CONSTRAINT [PK_BenEnrollmentPeriods] PRIMARY KEY CLUSTERED ([EnrollmentPeriodId] ASC)
    );
END
GO

IF OBJECT_ID(N'[dbo].[BenEmployeeClasses]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[BenEmployeeClasses] (
        [EmployeeBenefitClassId] INT            IDENTITY (1, 1) NOT NULL,
        [EmployeeId]             INT            NOT NULL,
        [BenefitClassId]         INT            NOT NULL,
        [EffectiveDate]          DATETIME       NULL,
        [AssignedBy]             NVARCHAR (100) NULL,
        [AssignedDate]           DATETIME       NOT NULL,
        CONSTRAINT [PK_BenEmployeeClasses] PRIMARY KEY CLUSTERED ([EmployeeBenefitClassId] ASC)
    );
END
GO

IF OBJECT_ID(N'[dbo].[BenEnrollments]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[BenEnrollments] (
        [EnrollmentId]         INT            IDENTITY (1, 1) NOT NULL,
        [EmployeeId]           INT            NOT NULL,
        [EnrollmentPeriodId]   INT            NOT NULL,
        [BenefitClassId]       INT            NULL,
        [Status]               NVARCHAR (30)  NOT NULL CONSTRAINT [DF_BenEnroll_Status] DEFAULT (N'InProgress'),
        [ConfirmationNumber]   NVARCHAR (50)  NULL,
        [SubmittedDate]        DATETIME       NULL,
        [ApprovedBy]           NVARCHAR (100) NULL,
        [ApprovedDate]         DATETIME       NULL,
        [SignedName]           NVARCHAR (150) NULL,
        [SignedDate]           DATETIME       NULL,
        [SignedIp]             NVARCHAR (50)  NULL,
        [TermsAccepted]        BIT            NOT NULL CONSTRAINT [DF_BenEnroll_Terms] DEFAULT (0),
        [CreatedDate]          DATETIME       NOT NULL,
        [ModifiedDate]         DATETIME       NULL,
        CONSTRAINT [PK_BenEnrollments] PRIMARY KEY CLUSTERED ([EnrollmentId] ASC)
    );
END
GO

IF OBJECT_ID(N'[dbo].[BenElections]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[BenElections] (
        [ElectionId]        INT            IDENTITY (1, 1) NOT NULL,
        [EnrollmentId]      INT            NOT NULL,
        [PlanId]            INT            NOT NULL,
        [CoverageOptionId]  INT            NULL,
        [IsWaived]          BIT            NOT NULL CONSTRAINT [DF_BenElect_Waive] DEFAULT (0),
        [EmployeeCost]      FLOAT          NOT NULL CONSTRAINT [DF_BenElect_Emp] DEFAULT (0),
        [EmployerCost]      FLOAT          NOT NULL CONSTRAINT [DF_BenElect_Er] DEFAULT (0),
        [EffectiveDate]     DATETIME       NULL,
        CONSTRAINT [PK_BenElections] PRIMARY KEY CLUSTERED ([ElectionId] ASC)
    );
END
GO

IF OBJECT_ID(N'[dbo].[BenDependents]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[BenDependents] (
        [DependentId]   INT            IDENTITY (1, 1) NOT NULL,
        [EnrollmentId]  INT            NOT NULL,
        [ElectionId]    INT            NULL,
        [FirstName]     NVARCHAR (100) NOT NULL,
        [LastName]      NVARCHAR (100) NOT NULL,
        [Relationship]  NVARCHAR (50)  NOT NULL,
        [DateOfBirth]   DATETIME       NULL,
        [Gender]        NVARCHAR (20)  NULL,
        [SSN]           NVARCHAR (20)  NULL,
        CONSTRAINT [PK_BenDependents] PRIMARY KEY CLUSTERED ([DependentId] ASC)
    );
END
GO

IF OBJECT_ID(N'[dbo].[BenBeneficiaries]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[BenBeneficiaries] (
        [BeneficiaryId] INT            IDENTITY (1, 1) NOT NULL,
        [EnrollmentId]  INT            NOT NULL,
        [ElectionId]    INT            NULL,
        [Name]          NVARCHAR (150) NOT NULL,
        [Relationship]  NVARCHAR (50)  NOT NULL,
        [Percentage]    FLOAT          NOT NULL CONSTRAINT [DF_BenBene_Pct] DEFAULT (0),
        CONSTRAINT [PK_BenBeneficiaries] PRIMARY KEY CLUSTERED ([BeneficiaryId] ASC)
    );
END
GO

IF OBJECT_ID(N'[dbo].[BenDocuments]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[BenDocuments] (
        [DocumentId]   INT            IDENTITY (1, 1) NOT NULL,
        [PlanId]       INT            NULL,
        [DocumentName] NVARCHAR (200) NOT NULL,
        [DocumentType] NVARCHAR (50)  NULL,
        [FileName]     NVARCHAR (260) NULL,
        [FilePath]     NVARCHAR (500) NULL,
        [IsActive]     BIT            NOT NULL CONSTRAINT [DF_BenDocs_Active] DEFAULT (1),
        CONSTRAINT [PK_BenDocuments] PRIMARY KEY CLUSTERED ([DocumentId] ASC)
    );
END
GO

IF OBJECT_ID(N'[dbo].[BenAudit]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[BenAudit] (
        [AuditId]      INT            IDENTITY (1, 1) NOT NULL,
        [EnrollmentId] INT            NULL,
        [EmployeeId]   INT            NULL,
        [Action]       NVARCHAR (100) NOT NULL,
        [Details]      NVARCHAR (1000) NULL,
        [PerformedBy]  NVARCHAR (100) NULL,
        [PerformedDate] DATETIME      NOT NULL,
        [IpAddress]    NVARCHAR (50)  NULL,
        CONSTRAINT [PK_BenAudit] PRIMARY KEY CLUSTERED ([AuditId] ASC)
    );
END
GO
