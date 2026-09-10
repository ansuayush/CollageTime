-- Training Module install (from training-doc.docx)
-- Tables are also auto-created by TrainingModuleSchemaHelper on first admin open.

IF OBJECT_ID(N'[dbo].[TrainingCourseCode]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[TrainingCourseCode] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [Code] NVARCHAR(50) NULL,
        [Description] NVARCHAR(200) NULL,
        [IsActive] BIT NOT NULL DEFAULT (1),
        CONSTRAINT [PK_TrainingCourseCode] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
END
GO

IF OBJECT_ID(N'[dbo].[TrainingCourseCategory]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[TrainingCourseCategory] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [Name] NVARCHAR(100) NULL,
        [Description] NVARCHAR(500) NULL,
        [IsActive] BIT NOT NULL DEFAULT (1),
        CONSTRAINT [PK_TrainingCourseCategory] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
END
GO

IF OBJECT_ID(N'[dbo].[TrainingType]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[TrainingType] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [Name] NVARCHAR(100) NULL,
        [IsActive] BIT NOT NULL DEFAULT (1),
        CONSTRAINT [PK_TrainingType] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
END
GO

IF OBJECT_ID(N'[dbo].[TrainingStatus]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[TrainingStatus] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [Name] NVARCHAR(100) NULL,
        [IsActive] BIT NOT NULL DEFAULT (1),
        [SortOrder] INT NOT NULL DEFAULT (0),
        CONSTRAINT [PK_TrainingStatus] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
END
GO

IF OBJECT_ID(N'[dbo].[TrainingClass]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[TrainingClass] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [Name] NVARCHAR(500) NULL,
        [Description] NVARCHAR(2000) NULL,
        [Image] VARBINARY(MAX) NULL,
        [Cost] DECIMAL(19,4) NULL,
        [Hours] DECIMAL(18,2) NULL,
        [Location] NVARCHAR(1000) NULL,
        [CourseCodeId] INT NULL,
        [CourseCategoryId] INT NULL,
        [IsActive] BIT NOT NULL DEFAULT (1),
        [ExpirationDate] DATE NULL,
        [EnrollmentStartDate] DATE NULL,
        [EnrollmentEndDate] DATE NULL,
        [CompanyId] INT NULL,
        [CreatedBy] NVARCHAR(100) NULL,
        [CreatedDate] DATETIME NOT NULL DEFAULT (GETDATE()),
        [ModifiedBy] NVARCHAR(100) NULL,
        [ModifiedDate] DATETIME NULL,
        CONSTRAINT [PK_TrainingClass] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
END
GO

IF OBJECT_ID(N'[dbo].[TrainingTrack]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[TrainingTrack] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [Name] NVARCHAR(500) NULL,
        [Description] NVARCHAR(2000) NULL,
        [ClassInfo] NVARCHAR(MAX) NULL,
        [TotalHours] DECIMAL(18,2) NOT NULL DEFAULT (0),
        [ExpirationDate] DATETIME NULL,
        [CompanyId] INT NULL,
        [CreatedBy] NVARCHAR(100) NULL,
        [CreatedDate] DATETIME NOT NULL DEFAULT (GETDATE()),
        [ModifiedBy] NVARCHAR(100) NULL,
        [ModifiedDate] DATETIME NULL,
        CONSTRAINT [PK_TrainingTrack] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
END
GO

IF OBJECT_ID(N'[dbo].[TrainingTrackClass]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[TrainingTrackClass] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [TrainingTrackId] INT NOT NULL,
        [TrainingClassId] INT NOT NULL,
        [SortOrder] INT NOT NULL DEFAULT (0),
        CONSTRAINT [PK_TrainingTrackClass] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
END
GO

IF OBJECT_ID(N'[dbo].[TrainingClassSchedule]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[TrainingClassSchedule] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [TrainingClassId] INT NOT NULL,
        [ScheduleName] NVARCHAR(200) NULL,
        [StartDate] DATETIME NOT NULL,
        [EndDate] DATETIME NOT NULL,
        [Location] NVARCHAR(1000) NULL,
        [IsActive] BIT NOT NULL DEFAULT (1),
        CONSTRAINT [PK_TrainingClassSchedule] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
END
GO

IF OBJECT_ID(N'[dbo].[TrainingEmployee]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[TrainingEmployee] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [EmployeeId] INT NOT NULL,
        [PersonId] INT NULL,
        [CompanyId] INT NULL,
        [TrainingTypeId] INT NULL,
        [TrainingClassId] INT NULL,
        [ClassName] NVARCHAR(500) NULL,
        [TrainingClassScheduleId] INT NULL,
        [TrainingTrackId] INT NULL,
        [TrainingTrackName] NVARCHAR(100) NULL,
        [EnrollmentDate] DATETIME NULL,
        [EnrollmentStartDate] DATETIME NULL,
        [StartDate] DATETIME NULL,
        [EndDate] DATETIME NULL,
        [Location] NVARCHAR(500) NULL,
        [Hours] DECIMAL(18,2) NULL,
        [Cost] DECIMAL(19,4) NULL,
        [StatusId] INT NULL,
        [CompletionDate] DATETIME NULL,
        [CompletionStatusId] INT NULL,
        [ExpirationDate] DATETIME NULL,
        [Notes] NVARCHAR(MAX) NULL,
        [Instructor] NVARCHAR(50) NULL,
        [CourseCodeId] INT NULL,
        [CourseCategoryId] INT NULL,
        [DocumentName] NVARCHAR(400) NULL,
        [ContentType] NVARCHAR(100) NULL,
        [Attachment] VARBINARY(MAX) NULL,
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedOn] DATETIME NULL,
        [ModifiedBy] NVARCHAR(50) NULL,
        [ModifiedOn] DATETIME NULL,
        CONSTRAINT [PK_TrainingEmployee] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
END
GO
