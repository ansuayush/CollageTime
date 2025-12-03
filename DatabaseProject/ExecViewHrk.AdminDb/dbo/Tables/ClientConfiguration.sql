CREATE TABLE [dbo].[ClientConfiguration] (
    [ClientConfigId]     INT           IDENTITY (1, 1) NOT NULL,
    [EmployerId]         INT           NOT NULL,
    [ConfigurationName]  VARCHAR (100) NULL,
    [ConfigurationValue] INT           NULL,
    CONSTRAINT [PK_ClientConfiguration] PRIMARY KEY CLUSTERED ([ClientConfigId] ASC),
    CONSTRAINT [FK_ClientConfiguration_Employers] FOREIGN KEY ([EmployerId]) REFERENCES [dbo].[Employers] ([EmployerId])
);