CREATE TABLE [dbo].[Managers] (
    [ManagerId] INT IDENTITY (1, 1) NOT NULL,
    [PersonId]  INT NOT NULL,
    CONSTRAINT [PK_Managers] PRIMARY KEY CLUSTERED ([ManagerId] ASC),
    CONSTRAINT [FK_Managers_Persons] FOREIGN KEY ([PersonId]) REFERENCES [dbo].[Persons] ([PersonId])
);

