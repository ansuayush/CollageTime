-- Optional signature columns for EmployeeDocuments
IF COL_LENGTH('dbo.EmployeeDocuments', 'IsSigned') IS NULL
BEGIN
    ALTER TABLE [dbo].[EmployeeDocuments] ADD
        [IsSigned] BIT NOT NULL CONSTRAINT [DF_EmployeeDocuments_IsSigned] DEFAULT (0),
        [SignedBy] NVARCHAR(100) NULL,
        [SignedDate] DATETIME NULL,
        [SignerRole] NVARCHAR(20) NULL,
        [SignatureName] NVARCHAR(150) NULL,
        [SignatureImagePath] NVARCHAR(500) NULL;
END
GO
