-- St Form Metadata / Security tables (from Stformtables.docx architecture)
-- Module → Form → Section → Field + company labels + role security
-- Safe to re-run; schema helper also ensures these on first Metadata Management open.

IF OBJECT_ID(N'[dbo].[stModules]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[stModules] (
        [Id]         INT IDENTITY(1,1) NOT NULL,
        [ModuleName] VARCHAR(50) NULL,
        [ControlId]  VARCHAR(50) NULL,
        CONSTRAINT [PK_stModules] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
END
GO

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
GO

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
GO

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
GO

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
GO

IF OBJECT_ID(N'[dbo].[stModuleSecurity]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[stModuleSecurity] (
        [Id]        INT IDENTITY(1,1) NOT NULL,
        [ModuleId]  INT NOT NULL,
        [RoleName]  NVARCHAR(256) NOT NULL,
        [CanView]   BIT NOT NULL DEFAULT (0),
        [CanAdd]    BIT NOT NULL DEFAULT (0),
        [CanEdit]   BIT NOT NULL DEFAULT (0),
        [CanDelete] BIT NOT NULL DEFAULT (0),
        [CanExport] BIT NOT NULL DEFAULT (0),
        [IsActive]  BIT NOT NULL DEFAULT (1),
        CONSTRAINT [PK_stModuleSecurity] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
END
GO

IF OBJECT_ID(N'[dbo].[stFormSecurity]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[stFormSecurity] (
        [Id]        INT IDENTITY(1,1) NOT NULL,
        [FormId]    INT NOT NULL,
        [RoleName]  NVARCHAR(256) NOT NULL,
        [CanView]   BIT NOT NULL DEFAULT (0),
        [CanAdd]    BIT NOT NULL DEFAULT (0),
        [CanEdit]   BIT NOT NULL DEFAULT (0),
        [CanDelete] BIT NOT NULL DEFAULT (0),
        [CanExport] BIT NOT NULL DEFAULT (0),
        [IsActive]  BIT NOT NULL DEFAULT (1),
        CONSTRAINT [PK_stFormSecurity] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
END
GO

IF OBJECT_ID(N'[dbo].[stSectionSecurity]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[stSectionSecurity] (
        [Id]        INT IDENTITY(1,1) NOT NULL,
        [SectionId] INT NOT NULL,
        [RoleName]  NVARCHAR(256) NOT NULL,
        [CanView]   BIT NOT NULL DEFAULT (0),
        [CanAdd]    BIT NOT NULL DEFAULT (0),
        [CanEdit]   BIT NOT NULL DEFAULT (0),
        [IsActive]  BIT NOT NULL DEFAULT (1),
        CONSTRAINT [PK_stSectionSecurity] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
END
GO

IF OBJECT_ID(N'[dbo].[stFieldSecurity]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[stFieldSecurity] (
        [Id]         INT IDENTITY(1,1) NOT NULL,
        [FieldId]    INT NOT NULL,
        [RoleName]   NVARCHAR(256) NOT NULL,
        [CanView]    BIT NOT NULL DEFAULT (1),
        [CanAdd]     BIT NOT NULL DEFAULT (1),
        [CanEdit]    BIT NOT NULL DEFAULT (1),
        [IsRequired] BIT NULL,
        [IsActive]   BIT NOT NULL DEFAULT (1),
        CONSTRAINT [PK_stFieldSecurity] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
END
GO
