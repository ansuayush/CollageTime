CREATE TABLE [dbo].[PersonPhoneNumbers] (
    [PersonPhoneNumberId] INT           IDENTITY (1, 1) NOT NULL,
    [PersonId]            INT           NOT NULL,
    [PhoneTypeId]         INT      NOT NULL,
    [PhoneNumber]         VARCHAR (15)  NOT NULL,
    [Extension]           VARCHAR (5)   NULL,
    [EnteredBy]           VARCHAR (50)  NOT NULL,
    [EnteredDate]         DATETIME NOT NULL,
    [ModifiedBy]          VARCHAR (50)  NULL,
    [ModifiedDate]        DATETIME NULL,
    [IsPrimaryPhone]      BIT           DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_pPhoneNumbers] PRIMARY KEY CLUSTERED ([PersonPhoneNumberId] ASC),
    CONSTRAINT [FK_PersonPhoneNumbers_DdlPhoneTypes] FOREIGN KEY ([PhoneTypeId]) REFERENCES [dbo].[DdlPhoneTypes] ([PhoneTypeId]),
    CONSTRAINT [FK_PersonPhoneNumbers_Persons] FOREIGN KEY ([PersonId]) REFERENCES [dbo].[Persons] ([PersonId])
);

