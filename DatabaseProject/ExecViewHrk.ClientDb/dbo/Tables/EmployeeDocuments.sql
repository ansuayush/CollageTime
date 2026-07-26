CREATE TABLE [dbo].[EmployeeDocuments] (
    [DocumentId]         INT            IDENTITY (1, 1) NOT NULL,
    [EmployeeId]         INT            NOT NULL,
    [FileName]           NVARCHAR (260) NOT NULL,
    [FilePath]           NVARCHAR (500) NOT NULL,
    [UploadedBy]         NVARCHAR (100) NOT NULL,
    [UploadedDate]       DATETIME       NOT NULL,
    [IsSigned]           BIT            CONSTRAINT [DF_EmployeeDocuments_IsSigned] DEFAULT ((0)) NOT NULL,
    [SignedBy]           NVARCHAR (100) NULL,
    [SignedDate]         DATETIME       NULL,
    [SignerRole]         NVARCHAR (20)  NULL,
    [SignatureName]     NVARCHAR (150) NULL,
    [SignatureImagePath] NVARCHAR (500) NULL,
    CONSTRAINT [PK_EmployeeDocuments] PRIMARY KEY CLUSTERED ([DocumentId] ASC),
    CONSTRAINT [FK_EmployeeDocuments_Employees] FOREIGN KEY ([EmployeeId]) REFERENCES [dbo].[Employees] ([EmployeeId])
);

GO
CREATE NONCLUSTERED INDEX [IX_EmployeeDocuments_EmployeeId]
    ON [dbo].[EmployeeDocuments]([EmployeeId] ASC);
