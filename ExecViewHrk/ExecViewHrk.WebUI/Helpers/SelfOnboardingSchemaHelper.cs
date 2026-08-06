using ExecViewHrk.EfClient;
using System;
using System.Linq;

namespace ExecViewHrk.WebUI.Helpers
{
    public static class SelfOnboardingSchemaHelper
    {
        public const string StorageFolderName = "SelfOnboarding";

        public static void EnsureSchema(ClientDbContext db)
        {
            db.Database.ExecuteSqlCommand(@"
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

IF OBJECT_ID(N'[dbo].[OnboardingProfiles]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[OnboardingProfiles] (
        [ProfileId]    INT            IDENTITY (1, 1) NOT NULL,
        [ProfileName]  NVARCHAR (200) NOT NULL,
        [Description]  NVARCHAR (500) NULL,
        [IsActive]     BIT            NOT NULL CONSTRAINT [DF_OnboardingProfiles_Active] DEFAULT (1),
        [CreatedBy]    NVARCHAR (100) NULL,
        [CreatedDate]  DATETIME       NOT NULL,
        [ModifiedBy]   NVARCHAR (100) NULL,
        [ModifiedDate] DATETIME       NULL,
        CONSTRAINT [PK_OnboardingProfiles] PRIMARY KEY CLUSTERED ([ProfileId] ASC)
    );
END

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
        CONSTRAINT [PK_OnboardingProfileDocuments] PRIMARY KEY CLUSTERED ([ProfileDocumentId] ASC)
    );
END

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

IF OBJECT_ID(N'[dbo].[SelfOnboardingHires]', N'U') IS NOT NULL AND COL_LENGTH(N'dbo.SelfOnboardingHires', N'RejectionReason') IS NULL
    ALTER TABLE [dbo].[SelfOnboardingHires] ADD [RejectionReason] NVARCHAR (1000) NULL;
IF OBJECT_ID(N'[dbo].[SelfOnboardingHires]', N'U') IS NOT NULL AND COL_LENGTH(N'dbo.SelfOnboardingHires', N'RejectedFormName') IS NULL
    ALTER TABLE [dbo].[SelfOnboardingHires] ADD [RejectedFormName] NVARCHAR (200) NULL;
IF OBJECT_ID(N'[dbo].[SelfOnboardingHires]', N'U') IS NOT NULL AND COL_LENGTH(N'dbo.SelfOnboardingHires', N'RejectedBy') IS NULL
    ALTER TABLE [dbo].[SelfOnboardingHires] ADD [RejectedBy] NVARCHAR (100) NULL;
IF OBJECT_ID(N'[dbo].[SelfOnboardingHires]', N'U') IS NOT NULL AND COL_LENGTH(N'dbo.SelfOnboardingHires', N'RejectedDate') IS NULL
    ALTER TABLE [dbo].[SelfOnboardingHires] ADD [RejectedDate] DATETIME NULL;

IF OBJECT_ID(N'[dbo].[SelfOnboardingPersonal]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[SelfOnboardingPersonal] (
        [HireId]             INT            NOT NULL,
        [PrefixId]           INT            NULL,
        [SuffixId]           INT            NULL,
        [FirstName]          NVARCHAR (100) NULL,
        [MiddleName]         NVARCHAR (100) NULL,
        [LastName]           NVARCHAR (100) NULL,
        [PreferredName]      NVARCHAR (100) NULL,
        [WorkEmail]          NVARCHAR (200) NULL,
        [HomeEmail]          NVARCHAR (200) NULL,
        [Phone]              NVARCHAR (50)  NULL,
        [DateOfBirth]        DATETIME       NULL,
        [SSN]                NVARCHAR (20)  NULL,
        [GenderId]           INT            NULL,
        [MaritalStatusId]    INT            NULL,
        [EthnicityId]        INT            NULL,
        [Address1]           NVARCHAR (200) NULL,
        [Address2]           NVARCHAR (200) NULL,
        [City]               NVARCHAR (100) NULL,
        [StateId]            INT            NULL,
        [Zip]                NVARCHAR (20)  NULL,
        [CountryId]          INT            NULL,
        [LicenseCountryId]   INT            NULL,
        [EmergencyName]      NVARCHAR (150) NULL,
        [EmergencyPhone]     NVARCHAR (50)  NULL,
        [RelationshipTypeId] INT            NULL,
        [FilingStatusId]     INT            NULL,
        [WorkingCountryId]   INT            NULL,
        [WorkingStateId]     INT            NULL,
        [StateTaxStatusId]   INT            NULL,
        [ModifiedDate]       DATETIME       NULL,
        CONSTRAINT [PK_SelfOnboardingPersonal] PRIMARY KEY CLUSTERED ([HireId] ASC)
    );
END

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
        CONSTRAINT [PK_SelfOnboardingI9] PRIMARY KEY CLUSTERED ([HireId] ASC)
    );
END

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
        CONSTRAINT [PK_SelfOnboardingTax] PRIMARY KEY CLUSTERED ([HireId] ASC)
    );
END

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
        CONSTRAINT [PK_SelfOnboardingSignatures] PRIMARY KEY CLUSTERED ([SignatureId] ASC)
    );
END

IF COL_LENGTH(N'dbo.SelfOnboardingSignatures', N'EmployeeDocumentId') IS NULL
BEGIN
    ALTER TABLE [dbo].[SelfOnboardingSignatures] ADD [EmployeeDocumentId] INT NULL;
END

IF OBJECT_ID(N'[dbo].[SelfOnboardingBankAccounts]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[SelfOnboardingBankAccounts] (
        [BankAccountId] INT            IDENTITY (1, 1) NOT NULL,
        [HireId]        INT            NOT NULL,
        [AccountTypeId] INT            NULL,
        [BankName]      NVARCHAR (150) NULL,
        [RoutingNumber] NVARCHAR (50)  NULL,
        [AccountNumber] NVARCHAR (50)  NULL,
        [IsPrimary]     BIT            NOT NULL CONSTRAINT [DF_SelfOnbBank_Primary] DEFAULT (0),
        CONSTRAINT [PK_SelfOnboardingBankAccounts] PRIMARY KEY CLUSTERED ([BankAccountId] ASC)
    );
END

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
        CONSTRAINT [PK_SelfOnboardingUploads] PRIMARY KEY CLUSTERED ([UploadId] ASC)
    );
END
");

            SeedLookups(db);
        }

        private static void SeedLookups(ClientDbContext db)
        {
            if (!db.OnboardingLookups.Any(x => x.LookupType == "DocumentType"))
            {
                db.OnboardingLookups.Add(new OnboardingLookup { LookupType = "DocumentType", Code = "POLICY", Description = "Policy", SortOrder = 1, IsActive = true });
                db.OnboardingLookups.Add(new OnboardingLookup { LookupType = "DocumentType", Code = "HANDBOOK", Description = "Handbook", SortOrder = 2, IsActive = true });
                db.OnboardingLookups.Add(new OnboardingLookup { LookupType = "DocumentType", Code = "FORM", Description = "Form", SortOrder = 3, IsActive = true });
                db.OnboardingLookups.Add(new OnboardingLookup { LookupType = "DocumentType", Code = "OTHER", Description = "Other", SortOrder = 4, IsActive = true });
            }
            if (!db.OnboardingLookups.Any(x => x.LookupType == "OfferLetter"))
            {
                db.OnboardingLookups.Add(new OnboardingLookup { LookupType = "OfferLetter", Code = "STD", Description = "Standard Offer Letter", SortOrder = 1, IsActive = true });
                db.OnboardingLookups.Add(new OnboardingLookup { LookupType = "OfferLetter", Code = "EXEMPT", Description = "Exempt Offer Letter", SortOrder = 2, IsActive = true });
                db.OnboardingLookups.Add(new OnboardingLookup { LookupType = "OfferLetter", Code = "INTERN", Description = "Intern Offer Letter", SortOrder = 3, IsActive = true });
            }
            if (!db.OnboardingLookups.Any(x => x.LookupType == "AccountType"))
            {
                db.OnboardingLookups.Add(new OnboardingLookup { LookupType = "AccountType", Code = "SAVING", Description = "Saving", SortOrder = 1, IsActive = true });
                db.OnboardingLookups.Add(new OnboardingLookup { LookupType = "AccountType", Code = "CURRENT", Description = "Current / Checking", SortOrder = 2, IsActive = true });
            }
            if (!db.OnboardingLookups.Any(x => x.LookupType == "FilingStatus"))
            {
                db.OnboardingLookups.Add(new OnboardingLookup { LookupType = "FilingStatus", Code = "Single", Description = "Single or Married filing separately", SortOrder = 1, IsActive = true });
                db.OnboardingLookups.Add(new OnboardingLookup { LookupType = "FilingStatus", Code = "HOH", Description = "Head of household", SortOrder = 2, IsActive = true });
                db.OnboardingLookups.Add(new OnboardingLookup { LookupType = "FilingStatus", Code = "Married", Description = "Married Filing Jointly or Qualifying Widow(er)", SortOrder = 3, IsActive = true });
            }
            if (!db.OnboardingLookups.Any(x => x.LookupType == "StateTaxStatus"))
            {
                db.OnboardingLookups.Add(new OnboardingLookup { LookupType = "StateTaxStatus", Code = "Single", Description = "Single", SortOrder = 1, IsActive = true });
                db.OnboardingLookups.Add(new OnboardingLookup { LookupType = "StateTaxStatus", Code = "Married", Description = "Married", SortOrder = 2, IsActive = true });
                db.OnboardingLookups.Add(new OnboardingLookup { LookupType = "StateTaxStatus", Code = "Exempt", Description = "Exempt", SortOrder = 3, IsActive = true });
            }
            db.SaveChanges();
        }

        public static string NextFileNumber(ClientDbContext db)
        {
            var max = db.Employees
                .Where(e => e.FileNumber != null && e.FileNumber != "")
                .Select(e => e.FileNumber)
                .ToList()
                .Select(fn =>
                {
                    int n;
                    return int.TryParse(fn.TrimStart('0'), out n) ? n : 0;
                })
                .DefaultIfEmpty(0)
                .Max();

            var hireMax = db.SelfOnboardingHires
                .Where(h => h.FileNumber != null && h.FileNumber != "")
                .Select(h => h.FileNumber)
                .ToList()
                .Select(fn =>
                {
                    int n;
                    return int.TryParse(fn.TrimStart('0'), out n) ? n : 0;
                })
                .DefaultIfEmpty(0)
                .Max();

            int next = Math.Max(max, hireMax) + 1;
            return next.ToString().PadLeft(6, '0');
        }

        public static string BuildUserName(string firstName, string lastName, string fileNumber)
        {
            string f = (firstName ?? "").Trim().Replace(" ", "");
            string l = (lastName ?? "").Trim().Replace(" ", "");
            string digits = (fileNumber ?? "").Trim();
            if (digits.Length > 4) digits = digits.Substring(digits.Length - 4);
            return (f + l + digits).ToLowerInvariant();
        }

        public static string NewTransactionId()
        {
            return "TXN-" + DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + Guid.NewGuid().ToString("N").Substring(0, 6).ToUpperInvariant();
        }
    }
}
