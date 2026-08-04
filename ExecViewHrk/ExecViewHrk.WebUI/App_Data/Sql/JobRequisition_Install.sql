-- Run against the client HR database to enable Job Requisition & Apply Portal.
IF OBJECT_ID(N'[dbo].[JobRequisitions]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[JobRequisitions] (
        [RequisitionId]     INT            IDENTITY (1, 1) NOT NULL,
        [RequisitionNumber] NVARCHAR (50)  NOT NULL,
        [PositionTitle]     NVARCHAR (200) NOT NULL,
        [Division]          NVARCHAR (100) NULL,
        [Department]        NVARCHAR (100) NULL,
        [PositionId]        INT            NULL,
        [Description]       NVARCHAR (MAX) NULL,
        [RequisitionDate]   DATETIME       NOT NULL,
        [OpenDate]          DATETIME       NULL,
        [ClosedDate]        DATETIME       NULL,
        [Status]            NVARCHAR (30)  NOT NULL CONSTRAINT [DF_JobRequisitions_Status] DEFAULT (N'Open'),
        [ApplicantCount]    INT            NOT NULL CONSTRAINT [DF_JobRequisitions_ApplicantCount] DEFAULT (0),
        [IsPublished]       BIT            NOT NULL CONSTRAINT [DF_JobRequisitions_IsPublished] DEFAULT (1),
        [CreatedBy]         NVARCHAR (100) NULL,
        [CreatedDate]       DATETIME       NOT NULL,
        [ModifiedBy]        NVARCHAR (100) NULL,
        [ModifiedDate]      DATETIME       NULL,
        CONSTRAINT [PK_JobRequisitions] PRIMARY KEY CLUSTERED ([RequisitionId] ASC)
    );
    CREATE NONCLUSTERED INDEX [IX_JobRequisitions_Status] ON [dbo].[JobRequisitions]([Status] ASC);
END
GO

IF OBJECT_ID(N'[dbo].[RecruitingQuestions]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[RecruitingQuestions] (
        [QuestionId]   INT            IDENTITY (1, 1) NOT NULL,
        [QuestionText] NVARCHAR (500) NOT NULL,
        [QuestionType] NVARCHAR (30)  NOT NULL CONSTRAINT [DF_RecruitingQuestions_Type] DEFAULT (N'Text'),
        [Choices]      NVARCHAR (1000) NULL,
        [WizardPage]   INT            NOT NULL CONSTRAINT [DF_RecruitingQuestions_WizardPage] DEFAULT (2),
        [SortOrder]    INT            NOT NULL CONSTRAINT [DF_RecruitingQuestions_SortOrder] DEFAULT (0),
        [IsRequired]   BIT            NOT NULL CONSTRAINT [DF_RecruitingQuestions_IsRequired] DEFAULT (1),
        [IsActive]     BIT            NOT NULL CONSTRAINT [DF_RecruitingQuestions_IsActive] DEFAULT (1),
        CONSTRAINT [PK_RecruitingQuestions] PRIMARY KEY CLUSTERED ([QuestionId] ASC)
    );
END
GO

IF OBJECT_ID(N'[dbo].[RecruitingDocuments]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[RecruitingDocuments] (
        [DocumentSetupId] INT            IDENTITY (1, 1) NOT NULL,
        [DocumentName]    NVARCHAR (200) NOT NULL,
        [Instructions]    NVARCHAR (500) NULL,
        [IsRequired]      BIT            NOT NULL CONSTRAINT [DF_RecruitingDocuments_IsRequired] DEFAULT (1),
        [RequiresSignature] BIT          NOT NULL CONSTRAINT [DF_RecruitingDocuments_RequiresSignature] DEFAULT (0),
        [SortOrder]       INT            NOT NULL CONSTRAINT [DF_RecruitingDocuments_SortOrder] DEFAULT (0),
        [IsActive]        BIT            NOT NULL CONSTRAINT [DF_RecruitingDocuments_IsActive] DEFAULT (1),
        CONSTRAINT [PK_RecruitingDocuments] PRIMARY KEY CLUSTERED ([DocumentSetupId] ASC)
    );
END
GO

IF OBJECT_ID(N'[dbo].[RecruitingConfig]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[RecruitingConfig] (
        [ConfigId]           INT            IDENTITY (1, 1) NOT NULL,
        [HomePageHtml]       NVARCHAR (MAX) NULL,
        [IntroductionHtml]   NVARCHAR (MAX) NULL,
        [ReviewSubmitHtml]   NVARCHAR (MAX) NULL,
        [AttestationHtml]    NVARCHAR (MAX) NULL,
        [ModifiedBy]         NVARCHAR (100) NULL,
        [ModifiedDate]       DATETIME       NULL,
        CONSTRAINT [PK_RecruitingConfig] PRIMARY KEY CLUSTERED ([ConfigId] ASC)
    );
END
GO

IF OBJECT_ID(N'[dbo].[JobApplicants]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[JobApplicants] (
        [ApplicantId]   INT            IDENTITY (1, 1) NOT NULL,
        [UserName]      NVARCHAR (100) NOT NULL,
        [PasswordHash]  NVARCHAR (200) NOT NULL,
        [PasswordSalt]  NVARCHAR (100) NOT NULL,
        [Email]         NVARCHAR (200) NOT NULL,
        [FirstName]     NVARCHAR (100) NOT NULL,
        [LastName]      NVARCHAR (100) NOT NULL,
        [Phone]         NVARCHAR (50)  NULL,
        [CreatedDate]   DATETIME       NOT NULL,
        [LastLoginDate] DATETIME       NULL,
        CONSTRAINT [PK_JobApplicants] PRIMARY KEY CLUSTERED ([ApplicantId] ASC),
        CONSTRAINT [UQ_JobApplicants_UserName] UNIQUE ([UserName])
    );
END
GO

IF OBJECT_ID(N'[dbo].[JobApplications]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[JobApplications] (
        [ApplicationId]  INT            IDENTITY (1, 1) NOT NULL,
        [RequisitionId]  INT            NOT NULL,
        [ApplicantId]    INT            NULL,
        [EmployeeId]     INT            NULL,
        [Status]         NVARCHAR (30)  NOT NULL CONSTRAINT [DF_JobApplications_Status] DEFAULT (N'Draft'),
        [CurrentStep]    INT            NOT NULL CONSTRAINT [DF_JobApplications_CurrentStep] DEFAULT (1),
        [SubmittedDate]  DATETIME       NULL,
        [CreatedDate]    DATETIME       NOT NULL,
        [ModifiedDate]   DATETIME       NULL,
        CONSTRAINT [PK_JobApplications] PRIMARY KEY CLUSTERED ([ApplicationId] ASC),
        CONSTRAINT [FK_JobApplications_JobRequisitions] FOREIGN KEY ([RequisitionId]) REFERENCES [dbo].[JobRequisitions] ([RequisitionId]),
        CONSTRAINT [FK_JobApplications_JobApplicants] FOREIGN KEY ([ApplicantId]) REFERENCES [dbo].[JobApplicants] ([ApplicantId]),
        CONSTRAINT [FK_JobApplications_Employees] FOREIGN KEY ([EmployeeId]) REFERENCES [dbo].[Employees] ([EmployeeId])
    );
    CREATE NONCLUSTERED INDEX [IX_JobApplications_RequisitionId] ON [dbo].[JobApplications]([RequisitionId] ASC);
END
GO

IF OBJECT_ID(N'[dbo].[JobApplicationAnswers]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[JobApplicationAnswers] (
        [AnswerId]      INT            IDENTITY (1, 1) NOT NULL,
        [ApplicationId] INT            NOT NULL,
        [QuestionId]    INT            NOT NULL,
        [AnswerText]    NVARCHAR (MAX) NULL,
        CONSTRAINT [PK_JobApplicationAnswers] PRIMARY KEY CLUSTERED ([AnswerId] ASC),
        CONSTRAINT [FK_JobApplicationAnswers_Applications] FOREIGN KEY ([ApplicationId]) REFERENCES [dbo].[JobApplications] ([ApplicationId]) ON DELETE CASCADE
    );
END
GO

IF OBJECT_ID(N'[dbo].[JobApplicationFiles]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[JobApplicationFiles] (
        [FileId]          INT            IDENTITY (1, 1) NOT NULL,
        [ApplicationId]   INT            NOT NULL,
        [DocumentSetupId] INT            NULL,
        [FileCategory]    NVARCHAR (50)  NOT NULL,
        [FileName]        NVARCHAR (260) NOT NULL,
        [FilePath]        NVARCHAR (500) NOT NULL,
        [UploadedDate]    DATETIME       NOT NULL,
        CONSTRAINT [PK_JobApplicationFiles] PRIMARY KEY CLUSTERED ([FileId] ASC),
        CONSTRAINT [FK_JobApplicationFiles_Applications] FOREIGN KEY ([ApplicationId]) REFERENCES [dbo].[JobApplications] ([ApplicationId]) ON DELETE CASCADE
    );
END
GO

IF OBJECT_ID(N'[dbo].[JobApplicationReferences]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[JobApplicationReferences] (
        [ReferenceId]   INT            IDENTITY (1, 1) NOT NULL,
        [ApplicationId] INT            NOT NULL,
        [FullName]      NVARCHAR (150) NOT NULL,
        [Relationship]  NVARCHAR (100) NULL,
        [Company]       NVARCHAR (150) NULL,
        [Phone]         NVARCHAR (50)  NULL,
        [Email]         NVARCHAR (200) NULL,
        [YearsKnown]    NVARCHAR (50)  NULL,
        CONSTRAINT [PK_JobApplicationReferences] PRIMARY KEY CLUSTERED ([ReferenceId] ASC),
        CONSTRAINT [FK_JobApplicationReferences_Applications] FOREIGN KEY ([ApplicationId]) REFERENCES [dbo].[JobApplications] ([ApplicationId]) ON DELETE CASCADE
    );
END
GO

IF OBJECT_ID(N'[dbo].[JobApplicationEmployment]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[JobApplicationEmployment] (
        [EmploymentId]  INT            IDENTITY (1, 1) NOT NULL,
        [ApplicationId] INT            NOT NULL,
        [EmployerName]  NVARCHAR (200) NOT NULL,
        [JobTitle]      NVARCHAR (150) NULL,
        [StartDate]     DATETIME       NULL,
        [EndDate]       DATETIME       NULL,
        [Duties]        NVARCHAR (MAX) NULL,
        [ReasonLeft]    NVARCHAR (300) NULL,
        CONSTRAINT [PK_JobApplicationEmployment] PRIMARY KEY CLUSTERED ([EmploymentId] ASC),
        CONSTRAINT [FK_JobApplicationEmployment_Applications] FOREIGN KEY ([ApplicationId]) REFERENCES [dbo].[JobApplications] ([ApplicationId]) ON DELETE CASCADE
    );
END
GO

IF OBJECT_ID(N'[dbo].[JobApplicationEducation]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[JobApplicationEducation] (
        [EducationId]   INT            IDENTITY (1, 1) NOT NULL,
        [ApplicationId] INT            NOT NULL,
        [SchoolName]    NVARCHAR (200) NOT NULL,
        [Degree]        NVARCHAR (150) NULL,
        [FieldOfStudy]  NVARCHAR (150) NULL,
        [GraduationYear] NVARCHAR (20) NULL,
        CONSTRAINT [PK_JobApplicationEducation] PRIMARY KEY CLUSTERED ([EducationId] ASC),
        CONSTRAINT [FK_JobApplicationEducation_Applications] FOREIGN KEY ([ApplicationId]) REFERENCES [dbo].[JobApplications] ([ApplicationId]) ON DELETE CASCADE
    );
END
GO

IF OBJECT_ID(N'[dbo].[JobApplicationSignatures]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[JobApplicationSignatures] (
        [SignatureId]     INT            IDENTITY (1, 1) NOT NULL,
        [ApplicationId]   INT            NOT NULL,
        [SignatureType]   NVARCHAR (50)  NOT NULL,
        [DocumentSetupId] INT            NULL,
        [SignerName]      NVARCHAR (150) NOT NULL,
        [SignedDate]      DATETIME       NOT NULL,
        [SignatureImagePath] NVARCHAR (500) NULL,
        CONSTRAINT [PK_JobApplicationSignatures] PRIMARY KEY CLUSTERED ([SignatureId] ASC),
        CONSTRAINT [FK_JobApplicationSignatures_Applications] FOREIGN KEY ([ApplicationId]) REFERENCES [dbo].[JobApplications] ([ApplicationId]) ON DELETE CASCADE
    );
END
GO
