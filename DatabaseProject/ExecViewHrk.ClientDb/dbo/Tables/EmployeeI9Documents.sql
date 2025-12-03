CREATE TABLE [dbo].[EmployeeI9Documents] (
    [EmployeeI9DocumentId] INT           IDENTITY (1, 1) NOT NULL,
    [EmployeeId]           INT           NOT NULL,
    [I9DocumentTypeId]     TINYINT       NULL,
    [PresentedDate]        SMALLDATETIME NULL,
    [ExpirationDate]       SMALLDATETIME NULL,
    [Notes]                TEXT          NULL,
    [EnteredBy]            VARCHAR (50)  NOT NULL,
    [EnteredDate]          SMALLDATETIME NOT NULL,
    CONSTRAINT [PK_eI9Documents] PRIMARY KEY CLUSTERED ([EmployeeI9DocumentId] ASC),
    CONSTRAINT [FK_EmployeeI9Documents_DdlI9DocumentTypes] FOREIGN KEY ([I9DocumentTypeId]) REFERENCES [dbo].[DdlI9DocumentTypes] ([I9DocumentTypeId]),
    CONSTRAINT [FK_EmployeeI9Documents_Employees] FOREIGN KEY ([EmployeeId]) REFERENCES [dbo].[Employees] ([EmployeeId])
);

