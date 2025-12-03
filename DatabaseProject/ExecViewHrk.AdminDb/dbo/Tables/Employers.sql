CREATE TABLE [dbo].[Employers] (
    [EmployerId]   INT           IDENTITY (1, 1) NOT NULL,
    [EmployerName] VARCHAR (100) NOT NULL,
    [DatabaseName] VARCHAR (100) NOT NULL,
    [IsClient]     BIT           NOT NULL,
    [CreatedDate]  DATE          NULL,
    [CreatedBy]    VARCHAR (50)  NULL,
    [ModifiedDate] DATETIME      NULL,
    [ModifiedBy]   VARCHAR (50)  NULL,
    CONSTRAINT [PK_Employers] PRIMARY KEY CLUSTERED ([EmployerId] ASC),
    CONSTRAINT [IX_ConnString] UNIQUE NONCLUSTERED ([DatabaseName] ASC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Employers]
    ON [dbo].[Employers]([EmployerName] ASC);

