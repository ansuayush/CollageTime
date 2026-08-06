-- Self Onboarding schema (profiles, hire notice, wizard data)
-- Also created at runtime by SelfOnboardingSchemaHelper.EnsureSchema

IF OBJECT_ID(N'[dbo].[OnboardingLookups]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[OnboardingLookups] (
        [LookupId]    INT            IDENTITY (1, 1) NOT NULL,
        [LookupType]  NVARCHAR (50)  NOT NULL,
        [Code]        NVARCHAR (50)  NULL,
        [Description] NVARCHAR (200) NOT NULL,
        [SortOrder]   INT            NOT NULL CONSTRAINT [DF_OnboardingLookups_Sort] DEFAULT (0),
        [IsActive]    BIT            NOT NULL CONSTRAINT [DF_OnboardingLookups_Active] DEFAULT (1),
        CONSTRAINT [PK_OnboardingLookups] PRIMARY KEY CLUSTERED ([LookupId] ASC)
    );
END
GO

IF OBJECT_ID(N'[dbo].[OnboardingProfiles]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[OnboardingProfiles] (
        [ProfileId]   INT            IDENTITY (1, 1) NOT NULL,
        [ProfileName] NVARCHAR (200) NOT NULL,
        [Description] NVARCHAR (500) NULL,
        [IsActive]    BIT            NOT NULL CONSTRAINT [DF_OnboardingProfiles_Active] DEFAULT (1),
        [CreatedBy]   NVARCHAR (100) NULL,
        [CreatedDate] DATETIME       NOT NULL,
        [ModifiedBy]  NVARCHAR (100) NULL,
        [ModifiedDate] DATETIME      NULL,
        CONSTRAINT [PK_OnboardingProfiles] PRIMARY KEY CLUSTERED ([ProfileId] ASC)
    );
END
GO

IF OBJECT_ID(N'[dbo].[OnboardingProfileDocuments]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[OnboardingProfileDocuments] (
        [ProfileDocumentId] INT            IDENTITY (1, 1) NOT NULL,
        [ProfileId]         INT            NOT NULL,
        [DocumentName]      NVARCHAR (200) NOT NULL,
        [DocumentTypeId]    INT            NULL,
        [FileName]          NVARCHAR (260) NULL,
        [FilePath]          NVARCHAR (500) NULL,
        [RequiresSignature] BIT            NOT NULL CONSTRAINT [DF_OnbProfDocs_ReqSig] DEFAULT (1),
        [EnableUpload]      BIT            NOT NULL CONSTRAINT [DF_OnbProfDocs_Upload] DEFAULT (0),
        [SortOrder]         INT            NOT NULL CONSTRAINT [DF_OnbProfDocs_Sort] DEFAULT (0),
        [IsActive]          BIT            NOT NULL CONSTRAINT [DF_OnbProfDocs_Active] DEFAULT (1),
        CONSTRAINT [PK_OnboardingProfileDocuments] PRIMARY KEY CLUSTERED ([ProfileDocumentId] ASC),
        CONSTRAINT [FK_OnboardingProfileDocuments_Profiles] FOREIGN KEY ([ProfileId])
            REFERENCES [dbo].[OnboardingProfiles] ([ProfileId])
    );
END
GO

IF OBJECT_ID(N'[dbo].[SelfOnboardingHires]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[SelfOnboardingHires] (
        [HireId]            INT            IDENTITY (1, 1) NOT NULL,
        [PositionId]        INT            NULL,
        [PositionTitle]     NVARCHAR (200) NULL,
        [ProfileId]         INT            NULL,
        [ApplicationId]     INT            NULL,
        [ApplicantId]       INT            NULL,
        [FirstName]         NVARCHAR (100) NOT NULL,
        [LastName]          NVARCHAR (100) NOT NULL,
        [HomeEmail]         NVARCHAR (200) NOT NULL,
        [WorkEmail]         NVARCHAR (200) NULL,
        [FileNumber]        NVARCHAR (50)  NULL,
        [OfferLetterId]     INT            NULL,
        [GeneratedUserName] NVARCHAR (100) NULL,
        [AspNetUserId]      NVARCHAR (128) NULL,
        [Status]            NVARCHAR (30)  NOT NULL CONSTRAINT [DF_SelfOnboardingHires_Status] DEFAULT (N'Invited'),
        [CurrentStep]       INT            NOT NULL CONSTRAINT [DF_SelfOnboardingHires_Step] DEFAULT (1),
        [TransactionId]     NVARCHAR (50)  NULL,
        [NoticeSentDate]    DATETIME       NULL,
        [SubmittedDate]     DATETIME       NULL,
        [ConfirmationDate]  DATETIME       NULL,
        [ApprovedBy]        NVARCHAR (100) NULL,
        [ApprovedDate]      DATETIME       NULL,
        [EmployeeId]        INT            NULL,
        [RejectionReason]   NVARCHAR (1000) NULL,
        [RejectedFormName]  NVARCHAR (200) NULL,
        [RejectedBy]        NVARCHAR (100) NULL,
        [RejectedDate]      DATETIME       NULL,
        [CreatedBy]         NVARCHAR (100) NULL,
        [CreatedDate]       DATETIME       NOT NULL,
        [ModifiedBy]        NVARCHAR (100) NULL,
        [ModifiedDate]      DATETIME       NULL,
        CONSTRAINT [PK_SelfOnboardingHires] PRIMARY KEY CLUSTERED ([HireId] ASC)
    );
END
GO

IF OBJECT_ID(N'[dbo].[SelfOnboardingHires]', N'U') IS NOT NULL AND COL_LENGTH(N'dbo.SelfOnboardingHires', N'RejectionReason') IS NULL
    ALTER TABLE [dbo].[SelfOnboardingHires] ADD [RejectionReason] NVARCHAR (1000) NULL;
GO
IF OBJECT_ID(N'[dbo].[SelfOnboardingHires]', N'U') IS NOT NULL AND COL_LENGTH(N'dbo.SelfOnboardingHires', N'RejectedFormName') IS NULL
    ALTER TABLE [dbo].[SelfOnboardingHires] ADD [RejectedFormName] NVARCHAR (200) NULL;
GO
IF OBJECT_ID(N'[dbo].[SelfOnboardingHires]', N'U') IS NOT NULL AND COL_LENGTH(N'dbo.SelfOnboardingHires', N'RejectedBy') IS NULL
    ALTER TABLE [dbo].[SelfOnboardingHires] ADD [RejectedBy] NVARCHAR (100) NULL;
GO
IF OBJECT_ID(N'[dbo].[SelfOnboardingHires]', N'U') IS NOT NULL AND COL_LENGTH(N'dbo.SelfOnboardingHires', N'RejectedDate') IS NULL
    ALTER TABLE [dbo].[SelfOnboardingHires] ADD [RejectedDate] DATETIME NULL;
GO

IF OBJECT_ID(N'[dbo].[SelfOnboardingPersonal]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[SelfOnboardingPersonal] (
        [HireId]              INT            NOT NULL,
        [PrefixId]            INT            NULL,
        [SuffixId]            INT            NULL,
        [FirstName]           NVARCHAR (100) NULL,
        [MiddleName]          NVARCHAR (100) NULL,
        [LastName]            NVARCHAR (100) NULL,
        [PreferredName]       NVARCHAR (100) NULL,
        [WorkEmail]           NVARCHAR (200) NULL,
        [HomeEmail]           NVARCHAR (200) NULL,
        [Phone]               NVARCHAR (50)  NULL,
        [DateOfBirth]         DATETIME       NULL,
        [SSN]                 NVARCHAR (20)  NULL,
        [GenderId]            INT            NULL,
        [MaritalStatusId]     INT            NULL,
        [EthnicityId]         INT            NULL,
        [Address1]            NVARCHAR (200) NULL,
        [Address2]            NVARCHAR (200) NULL,
        [City]                NVARCHAR (100) NULL,
        [StateId]             INT            NULL,
        [Zip]                 NVARCHAR (20)  NULL,
        [CountryId]           INT            NULL,
        [LicenseCountryId]    INT            NULL,
        [EmergencyName]       NVARCHAR (150) NULL,
        [EmergencyPhone]      NVARCHAR (50)  NULL,
        [RelationshipTypeId]  INT            NULL,
        [FilingStatusId]      INT            NULL,
        [WorkingCountryId]    INT            NULL,
        [WorkingStateId]      INT            NULL,
        [StateTaxStatusId]    INT            NULL,
        [ModifiedDate]        DATETIME       NULL,
        CONSTRAINT [PK_SelfOnboardingPersonal] PRIMARY KEY CLUSTERED ([HireId] ASC),
        CONSTRAINT [FK_SelfOnboardingPersonal_Hires] FOREIGN KEY ([HireId])
            REFERENCES [dbo].[SelfOnboardingHires] ([HireId])
    );
END
GO

IF OBJECT_ID(N'[dbo].[SelfOnboardingI9]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[SelfOnboardingI9] (
        [HireId]                   INT            NOT NULL,
        [CitizenStatus]            INT            NOT NULL CONSTRAINT [DF_SelfOnbI9_Citizen] DEFAULT (0),
        [AlienNumber]              NVARCHAR (50)  NULL,
        [PermanentResidentExpire]  DATETIME       NULL,
        [LawCitizenOfId]           INT            NULL,
        [LawCitizenOfText]         NVARCHAR (100) NULL,
        [AlienAuthorizedUntil]     DATETIME       NULL,
        [AlienCitizenOfId]         INT            NULL,
        [AlienCitizenOfText]       NVARCHAR (100) NULL,
        [AlienRegistrationNumber]  NVARCHAR (50)  NULL,
        [AdmissionNumber]          NVARCHAR (50)  NULL,
        [PassportNumber]           NVARCHAR (50)  NULL,
        [CountryOfIssuanceId]      INT            NULL,
        [CountryOfIssuanceText]    NVARCHAR (100) NULL,
        [TranslatorNotUsed]        BIT            NOT NULL CONSTRAINT [DF_SelfOnbI9_TransNo] DEFAULT (0),
        [TranslatorUsed]           BIT            NOT NULL CONSTRAINT [DF_SelfOnbI9_TransYes] DEFAULT (0),
        [FederalLawAcknowledged]   BIT            NOT NULL CONSTRAINT [DF_SelfOnbI9_Federal] DEFAULT (0),
        [HideSsnOnForm]            BIT            NOT NULL CONSTRAINT [DF_SelfOnbI9_HideSsn] DEFAULT (0),
        [EmployeeDocumentId]       INT            NULL,
        [ModifiedDate]             DATETIME       NULL,
        CONSTRAINT [PK_SelfOnboardingI9] PRIMARY KEY CLUSTERED ([HireId] ASC),
        CONSTRAINT [FK_SelfOnboardingI9_Hires] FOREIGN KEY ([HireId])
            REFERENCES [dbo].[SelfOnboardingHires] ([HireId])
    );
END
GO

IF OBJECT_ID(N'[dbo].[SelfOnboardingTax]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[SelfOnboardingTax] (
        [HireId]                              INT             NOT NULL,
        [FilingStatusId]                      INT             NULL,
        [OtherIncomeAmount]                   DECIMAL (18, 2) NULL,
        [DeductionsAmount]                    DECIMAL (18, 2) NULL,
        [ExtraWithholdingAmount]              DECIMAL (18, 2) NULL,
        [ExtraWithholdingPercent]             DECIMAL (18, 4) NULL,
        [FederalExempt]                       BIT             NOT NULL CONSTRAINT [DF_SelfOnbTax_FedExempt] DEFAULT (0),
        [CopyFromFederal]                     BIT             NOT NULL CONSTRAINT [DF_SelfOnbTax_CopyFed] DEFAULT (0),
        [WorkingCountryId]                    INT             NULL,
        [WorkingStateId]                      INT             NULL,
        [StateTaxStatusId]                    INT             NULL,
        [StateExemptions]                     NVARCHAR (50)   NULL,
        [StateAdditionalWithholdingAmount]    DECIMAL (18, 2) NULL,
        [StateAdditionalWithholdingPercent]   DECIMAL (18, 4) NULL,
        [StateExempt]                         BIT             NOT NULL CONSTRAINT [DF_SelfOnbTax_StateExempt] DEFAULT (0),
        [EmployeeDocumentId]                  INT             NULL,
        [ModifiedDate]                        DATETIME        NULL,
        CONSTRAINT [PK_SelfOnboardingTax] PRIMARY KEY CLUSTERED ([HireId] ASC),
        CONSTRAINT [FK_SelfOnboardingTax_Hires] FOREIGN KEY ([HireId])
            REFERENCES [dbo].[SelfOnboardingHires] ([HireId])
    );
END
GO

IF OBJECT_ID(N'[dbo].[SelfOnboardingSignatures]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[SelfOnboardingSignatures] (
        [SignatureId]         INT            IDENTITY (1, 1) NOT NULL,
        [HireId]              INT            NOT NULL,
        [DocumentKey]         NVARCHAR (100) NOT NULL,
        [ProfileDocumentId]   INT            NULL,
        [IsSigned]            BIT            NOT NULL CONSTRAINT [DF_SelfOnbSig_Signed] DEFAULT (0),
        [SignedName]          NVARCHAR (150) NULL,
        [SignedDate]          DATETIME       NULL,
        [SignedIp]            NVARCHAR (50)  NULL,
        [TransactionId]       NVARCHAR (50)  NULL,
        [FilePath]            NVARCHAR (500) NULL,
        [EmployeeDocumentId]  INT            NULL,
        CONSTRAINT [PK_SelfOnboardingSignatures] PRIMARY KEY CLUSTERED ([SignatureId] ASC),
        CONSTRAINT [FK_SelfOnboardingSignatures_Hires] FOREIGN KEY ([HireId])
            REFERENCES [dbo].[SelfOnboardingHires] ([HireId])
    );
END
GO

IF COL_LENGTH(N'dbo.SelfOnboardingSignatures', N'EmployeeDocumentId') IS NULL
BEGIN
    ALTER TABLE [dbo].[SelfOnboardingSignatures] ADD [EmployeeDocumentId] INT NULL;
END
GO

IF OBJECT_ID(N'[dbo].[SelfOnboardingBankAccounts]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[SelfOnboardingBankAccounts] (
        [BankAccountId]   INT            IDENTITY (1, 1) NOT NULL,
        [HireId]          INT            NOT NULL,
        [AccountTypeId]   INT            NULL,
        [BankName]        NVARCHAR (150) NULL,
        [RoutingNumber]   NVARCHAR (50)  NULL,
        [AccountNumber]   NVARCHAR (50)  NULL,
        [IsPrimary]       BIT            NOT NULL CONSTRAINT [DF_SelfOnbBank_Primary] DEFAULT (0),
        CONSTRAINT [PK_SelfOnboardingBankAccounts] PRIMARY KEY CLUSTERED ([BankAccountId] ASC),
        CONSTRAINT [FK_SelfOnboardingBankAccounts_Hires] FOREIGN KEY ([HireId])
            REFERENCES [dbo].[SelfOnboardingHires] ([HireId])
    );
END
GO

IF OBJECT_ID(N'[dbo].[SelfOnboardingUploads]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[SelfOnboardingUploads] (
        [UploadId]          INT            IDENTITY (1, 1) NOT NULL,
        [HireId]            INT            NOT NULL,
        [ProfileDocumentId] INT            NULL,
        [FileName]          NVARCHAR (260) NOT NULL,
        [FilePath]          NVARCHAR (500) NOT NULL,
        [UploadedDate]      DATETIME       NOT NULL,
        [IsSigned]          BIT            NOT NULL CONSTRAINT [DF_SelfOnbUpload_Signed] DEFAULT (0),
        CONSTRAINT [PK_SelfOnboardingUploads] PRIMARY KEY CLUSTERED ([UploadId] ASC),
        CONSTRAINT [FK_SelfOnboardingUploads_Hires] FOREIGN KEY ([HireId])
            REFERENCES [dbo].[SelfOnboardingHires] ([HireId])
    );
END
GO

-- Seed common lookup types if empty
IF NOT EXISTS (SELECT 1 FROM [dbo].[OnboardingLookups] WHERE LookupType = N'DocumentType')
BEGIN
    INSERT INTO [dbo].[OnboardingLookups] (LookupType, Code, Description, SortOrder) VALUES
    (N'DocumentType', N'POLICY', N'Policy', 1),
    (N'DocumentType', N'HANDBOOK', N'Handbook', 2),
    (N'DocumentType', N'FORM', N'Form', 3),
    (N'DocumentType', N'OTHER', N'Other', 4);
END
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[OnboardingLookups] WHERE LookupType = N'OfferLetter')
BEGIN
    INSERT INTO [dbo].[OnboardingLookups] (LookupType, Code, Description, SortOrder) VALUES
    (N'OfferLetter', N'STD', N'Standard Offer Letter', 1),
    (N'OfferLetter', N'EXEMPT', N'Exempt Offer Letter', 2),
    (N'OfferLetter', N'INTERN', N'Intern Offer Letter', 3);
END
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[OnboardingLookups] WHERE LookupType = N'AccountType')
BEGIN
    INSERT INTO [dbo].[OnboardingLookups] (LookupType, Code, Description, SortOrder) VALUES
    (N'AccountType', N'SAVING', N'Saving', 1),
    (N'AccountType', N'CURRENT', N'Current / Checking', 2);
END
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[OnboardingLookups] WHERE LookupType = N'FilingStatus')
BEGIN
    INSERT INTO [dbo].[OnboardingLookups] (LookupType, Code, Description, SortOrder) VALUES
    (N'FilingStatus', N'Single', N'Single', 1),
    (N'FilingStatus', N'Married', N'Married filing jointly', 2),
    (N'FilingStatus', N'MFS', N'Married filing separately', 3),
    (N'FilingStatus', N'HOH', N'Head of household', 4);
END
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[OnboardingLookups] WHERE LookupType = N'StateTaxStatus')
BEGIN
    INSERT INTO [dbo].[OnboardingLookups] (LookupType, Code, Description, SortOrder) VALUES
    (N'StateTaxStatus', N'Single', N'Single', 1),
    (N'StateTaxStatus', N'Married', N'Married', 2),
    (N'StateTaxStatus', N'Exempt', N'Exempt', 3);
END
GO
