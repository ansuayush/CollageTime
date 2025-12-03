CREATE TABLE [dbo].[PersonMemberships] (
    [PersonMembershipId] INT           IDENTITY (1, 1) NOT NULL,
    [PersonId]           INT           NOT NULL,
    [ProfessionalBodyId] INT      NOT NULL,
    [StartDate]          DATETIME NULL,
    [RenewalDate]        DATETIME NULL,
    [Number]             VARCHAR (50)  NULL,
    [Fee]                MONEY         NULL,
    [FeePaidDate]        DATETIME NULL,
    [ProfessionalTitle]  VARCHAR (50)  NULL,
    [Notes]              TEXT          NULL,
    [RegionalChapterId]  INT      NULL,
    [EnteredBy]          VARCHAR (50)  NULL,
    [EnteredDate]        DATETIME NULL,
    [RegionalChapter]    VARCHAR (100) NULL,
    [ModifiedBy]         VARCHAR (50)  NULL,
    [ModifiedDate]       DATETIME NULL,
    CONSTRAINT [PK_pMemberships] PRIMARY KEY CLUSTERED ([PersonMembershipId] ASC),
    CONSTRAINT [FK_PersonMemberships_DdlProfessionalBodies] FOREIGN KEY ([ProfessionalBodyId]) REFERENCES [dbo].[DdlProfessionalBodies] ([ProfessionalBodyId]),
    CONSTRAINT [FK_PersonMemberships_DdlRegionalChapters] FOREIGN KEY ([RegionalChapterId]) REFERENCES [dbo].[DdlRegionalChapters] ([RegionalChapterId]),
    CONSTRAINT [FK_PersonMemberships_Persons] FOREIGN KEY ([PersonId]) REFERENCES [dbo].[Persons] ([PersonId])
);

