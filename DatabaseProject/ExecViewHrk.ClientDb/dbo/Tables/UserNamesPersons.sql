CREATE TABLE [dbo].[UserNamesPersons] (
    [UserName]     NVARCHAR (256) NOT NULL,
    [PersonID]     INT            NOT NULL,
    [CreationDate] DATETIME       NULL,
    [ModifiedDate] DATETIME       NULL,
    [EnteredBy]    VARCHAR (50)   NULL,
    CONSTRAINT [PK_UserNamesPersons] PRIMARY KEY CLUSTERED ([UserName] ASC, [PersonID] ASC),
    CONSTRAINT [FK_UserNamesPersons_Persons] FOREIGN KEY ([PersonID]) REFERENCES [dbo].[Persons] ([PersonId])
);

