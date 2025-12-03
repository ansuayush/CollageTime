CREATE TABLE [dbo].[Persons] (
    [PersonId]              INT           IDENTITY (1, 1) NOT NULL,
    [SSN]                   VARCHAR (9)   NULL,
    [ShowSSN]               BIT           NULL,
    [Lastname]              VARCHAR (50)  NOT NULL,
    [Firstname]             VARCHAR (50)  NOT NULL,
    [MiddleName]            VARCHAR (50)  NULL,
    [PrefixId]              INT           CONSTRAINT [DF_pPerson_prefixID] DEFAULT ((0)) NULL,
    [SuffixId]              INT           CONSTRAINT [DF_pPerson_suffixID] DEFAULT ((0)) NULL,
    [PreferredName]         VARCHAR (50)  NULL,
    [eMail]                 VARCHAR (100) NULL,
    [AlternateEMail]        VARCHAR (100) NULL,
    [DOB]                   DATE          NULL,
    [GenderId]              INT           NULL,
    [ActualMaritalStatusId] INT           NULL,
    [MaidenName]            VARCHAR (50)  NULL,
    [IsDependent]           BIT           CONSTRAINT [DF_pMain_dependent] DEFAULT ((0)) NULL,
    [EnteredBy]             VARCHAR (50)  NULL,
    [EnteredDate]           DATETIME      NULL,
    [ModifiedBy]            VARCHAR (50)  NULL,
    [ModifiedDate]          DATETIME      NULL,
    [IsStudent]             BIT           CONSTRAINT [DF_Persons_IsStudent] DEFAULT ((0)) NULL,
    [IsTrainer]             BIT           CONSTRAINT [DF_Persons_IsTrainer] DEFAULT ((0)) NULL,
    [IsApplicant]           BIT           CONSTRAINT [DF_Persons_IsApplicant] DEFAULT ((0)) NULL,
    CONSTRAINT [PK_pPerson] PRIMARY KEY CLUSTERED ([PersonId] ASC),
    CONSTRAINT [IX_Persons_2] UNIQUE NONCLUSTERED ([SSN] ASC, [eMail] ASC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Persons_1]
    ON [dbo].[Persons]([eMail] ASC);

